using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using wechat_spider_core.ef;
using wechat_spider_core.ioc;
using wechat_spider_core.service;

namespace wechat_spider_core.spider
{
    public class SpiderManager
    {
        public TaskStatusMain taskStatus = new TaskStatusMain();

        private readonly List<SpiderTask> SpiderTaskList = new List<SpiderTask>();

        private readonly ISpiderHandler spiderHandler = InitIocModule.GetFromFac<ISpiderHandler>();

        private SpiderTask CurrentTask = null;

        private bool InitStatus = false;

        private bool TaskRuning = false;

        private static readonly System.Windows.Forms.Timer TaskTimer = new System.Windows.Forms.Timer();

        private int LastStartTimeSpan = 0;

        public int ErrorRetryCount = 0;

        private const int MaxRetry = 10;

        public List<SpiderTask> GetSpiderTasks()
        {
            return SpiderTaskList;
        }

        public SpiderTask GetCurrentTask()
        {
            return CurrentTask;
        }

        public async Task OnRequestError()
        {
            if(ErrorRetryCount >= MaxRetry)
            {
                LogService.Info($"{CurrentTask.NickName}重试到最大次数，跳过");
                await NextTask();
            }
            else
            {
                ErrorRetryCount++;
                LogService.Info($"请求出错，尝试重试，第{ErrorRetryCount}次");
                Thread.Sleep(1000 * 30);
                TaskRuning = false;
                TaskTimer_Tick(null, null);
            }
            
        }

        public SpiderManager()
        {
            if(!InitStatus)
            {
                spiderHandler.InsertClientSgin(IdWorkContext.CLIENT_ID);
            }
        }

        public async Task Init()
        {
            if (InitStatus)
                return;

            var list = await spiderHandler.ListWeChatAccountAsync();
            if (list.Count <= 0)
            {
                LogService.Error("当前没有取到公众号名称，请在数据库中写入数据");
                return;
            }
            SpiderTaskList.Clear();
            list.ForEach(item =>
            {
                var spider = new SpiderTask(item.NickName)
                {
                    SpiderId = item.Id,
                    Alias = item.Alias,
                    LastUpdateDate = item.LastUpdate
                };
                item.SpiderRoles.ForEach(role =>
                {
                    spider.Roles.Add(role.Role);
                });
                SpiderTaskList.Add(spider);
            });
            JavascriptCallback.OnSearchHandler += SearchHandlerCallback;
            JavascriptCallback.OnArticleResponseHandler += ArticleResponseHandlerCallback;
            TaskTimer.Interval = 10000;
            TaskTimer.Tick += TaskTimer_Tick;
            TaskTimer.Start();
            InitStatus = true;
        }

        private async void TaskTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                await spiderHandler.UpdateClientSgin(IdWorkContext.CLIENT_ID);
            }
            catch (NullReferenceException)
            {
                LogService.Error("更新客户端时间时未找到实体，尝试重新写入");
                await spiderHandler.InsertClientSgin(IdWorkContext.CLIENT_ID);
            }
            if (TaskRuning)
                return;
            DateTime now = DateTime.Now;
            string role = $"{now.Day.ToString().PadLeft(2,'0')}:{now.Hour.ToString().PadLeft(2,'0')}";
            string runDate = now.ToString("yyyyMMddHH");
            foreach (var item in SpiderTaskList)
            {
                try
                {
                    if(!await spiderHandler.QuerySpaderStatus(item.SpiderId))
                    {
                        LogService.Info($"公众号{item.NickName}正在执行中");
                        break;
                    }
                }
                catch (NullReferenceException)
                {
                    LogService.Error($"未找到公众号{item.NickName}:({item.SpiderId})");
                    break;
                }
                 
                if(item.Ready(role, runDate))
                {
                    LogService.Info($"当前规则{role}");
                    CurrentTask = item;
                    CurrentTask.CurrentPage = 1;
                    await spiderHandler.SetAccountSpiderStart(item.SpiderId, IdWorkContext.CLIENT_ID);
                    item.Run();
                    TaskRuning = true;
                    break;
                }
            }
        }

        /// <summary>
        /// 搜索公众号完成后回调
        /// </summary>
        /// <param name="searchModel"></param>
        private async void SearchHandlerCallback(SearchModel searchModel)
        {
            if(null == searchModel.list || 0 >= searchModel.list.Count)
            {
                LogService.Error($"未查询到公众号:{CurrentTask.NickName}");
                return;
            }
            SearchBiz searchBiz = searchModel.list.FirstOrDefault(c => c.nickname == CurrentTask.NickName || c.alias == CurrentTask.Alias);
            if(null != searchBiz)
            {
                CurrentTask.AppModel = searchBiz;
                var accountModel = await spiderHandler.QueryWeChatAccountAsync(CurrentTask.SpiderId);
                if (null == accountModel)
                {
                    LogService.Error($"未找到账号实体:{CurrentTask.NickName}");
                    return;
                }
                if (accountModel.LastUpdate.HasValue)
                {
                    DateTime last = accountModel.LastUpdate.Value;
                    TimeSpan ts = last - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    LastStartTimeSpan = Convert.ToInt32(ts.TotalSeconds);
                }
                LogService.Info($"更新公众号信息实体{CurrentTask.NickName}");
                accountModel.Alias = searchBiz.alias;
                accountModel.FakeId = searchBiz.fakeid;
                accountModel.RoundHeadImg = searchBiz.round_head_img;
                int.TryParse(searchBiz.service_type, out int sType);
                accountModel.ServiceType = sType;
                accountModel.NickName = searchBiz.nickname;
                accountModel.LastUpdate = DateTime.Now;
                await spiderHandler.UpdateWeChatAccountAsync(accountModel);

                var task = SpiderTaskList.FirstOrDefault(c => c.SpiderId == CurrentTask.SpiderId);
                task.LastUpdateDate = DateTime.Now;
                task.Alias = accountModel.Alias;
                task.NickName = accountModel.NickName;
                CurrentTask.ToQueryArticleList(CurrentTask.CurrentPage);
            }
            else
            {
                LogService.Error($"未匹配到公众号名称:{CurrentTask.NickName}");
                await NextTask();
            }
        }
        /// <summary>
        /// 获取历史文章列表回调
        /// </summary>
        /// <param name="articleResponseModel"></param>
        private async void ArticleResponseHandlerCallback(ArticleResponseModel articleResponseModel)
        {
            LogService.Info($"当前条数：{articleResponseModel.app_msg_list.Count},准备写入到数据库");
            var accountModel = await spiderHandler.QueryWeChatAccountAsync(CurrentTask.SpiderId);
            if(null == accountModel)
            {
                LogService.Error($"未找到账号实体:{CurrentTask.NickName}");
                await NextTask();
            }
            else
            {
                int oldArticleCount = articleResponseModel.app_msg_list.Count(a => a.create_time < LastStartTimeSpan);      
                if (oldArticleCount < articleResponseModel.app_msg_list.Count)
                {
                    List<WeChatArticle> saveList = new List<WeChatArticle>();
                    articleResponseModel.app_msg_list.ForEach(async item =>
                    {
                        var article = await spiderHandler.QueryArticleByAid(item.aid);
                        if (article.Count >= 1 || item.create_time < LastStartTimeSpan)
                        {
                            LogService.Info($"文章({item.aid})已经存在，跳过保存");
                            return;
                        }
                        WeChatArticle model = item.ConvertModel();
                        model.Id = IdWorkContext.ID_WORKER.NextId();
                        model.CreateDate = DateTime.Now;
                        model.WeChatAccount = accountModel;
                        saveList.Add(model);
                    });
                    try
                    {
                        await spiderHandler.InsertArticleByList(saveList);
                    }
                    catch (Exception e)
                    {
                        LogService.Error($"将文章写入到数据库出错{e.Message}", e);
                    }

                    LogService.Info("准备翻页，休眠30秒");
                    Thread.Sleep(1000 * 30);
                    CurrentTask.ToQueryArticleList(++CurrentTask.CurrentPage);
                }
                else
                {
                    await NextTask();
                }
            }
        }

        private async Task NextTask()
        {
            LogService.Info("准备开始下一个任务");
            SpiderTaskList.Remove(CurrentTask);
            await spiderHandler.SetAccountSpiderStop(CurrentTask.SpiderId);
            CurrentTask = null;
            TaskRuning = false;
            if (SpiderTaskList.Count == 0)
            {
                InitStatus = false;
            }
        }
    }
}
