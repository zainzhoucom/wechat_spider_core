using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using wechat_spider_core.chrome;
using wechat_spider_core.ef;

namespace wechat_spider_core.spider
{
    public class SpiderManager
    {
        public TaskStatusMain taskStatus = new TaskStatusMain();

        private readonly List<SpiderTask> SpiderTaskList = new List<SpiderTask>();

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

        public void OnRequestError()
        {
            if(ErrorRetryCount >= MaxRetry)
            {
                LogService.Info($"{CurrentTask.NickName}重试到最大次数，跳过");
                NextTask();
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
                using (var db = new SpiderContext())
                {
                    DateTime dateTime = DateTime.Now;
                    TimeSpan ts = dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    LastStartTimeSpan = Convert.ToInt32(ts.TotalSeconds);
                    db.TaskStartSigns.Add(new TaskStartSign
                    {
                        Id = Guid.NewGuid().ToString().Replace("-", ""),
                        StartDate = LastStartTimeSpan.ToString()
                    });
                    db.SaveChanges();
                }
            }
            
        }

        public void Init()
        {
            if (InitStatus)
                return;
            using (var db = new SpiderContext())
            {
                var list = db.WeChatAccounts.OrderBy(a => a.SpiderRole).ToList(); 
                if(list.Count <= 0)
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
                    if(!string.IsNullOrWhiteSpace(item.SpiderRole))
                        spider.Roles.Add(item.SpiderRole);
                    if (!string.IsNullOrWhiteSpace(item.SpiderRole1))
                        spider.Roles.Add(item.SpiderRole1);
                    if (!string.IsNullOrWhiteSpace(item.SpiderRole2))
                        spider.Roles.Add(item.SpiderRole2);
                    if (!string.IsNullOrWhiteSpace(item.SpiderRole3))
                        spider.Roles.Add(item.SpiderRole3);
                    if (!string.IsNullOrWhiteSpace(item.SpiderRole4))
                        spider.Roles.Add(item.SpiderRole4);
                    if (!string.IsNullOrWhiteSpace(item.SpiderRole5))
                        spider.Roles.Add(item.SpiderRole5);
                    SpiderTaskList.Add(spider);
                });
                JavascriptCallback.OnSearchHandler += SearchHandlerCallback;
                JavascriptCallback.OnArticleResponseHandler += ArticleResponseHandlerCallback;
                TaskTimer.Interval = 1000 * 4;
                TaskTimer.Tick += TaskTimer_Tick;
                TaskTimer.Start();
                InitStatus = true;
            }
        }

        private void TaskTimer_Tick(object sender, EventArgs e)
        {
            if (TaskRuning)
                return;
            DateTime now = DateTime.Now;
            string role = $"{now.Day.ToString().PadLeft(2,'0')}:{now.Hour.ToString().PadLeft(2,'0')}";
            string runDate = now.ToString("yyyyMMddHH");
            foreach (var item in SpiderTaskList)
            {
                if(item.Ready(role, runDate))
                {
                    LogService.Info($"当前规则{role}");
                    CurrentTask = item;
                    CurrentTask.CurrentPage = 1;
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
        private void SearchHandlerCallback(SearchModel searchModel)
        {
            SearchBiz searchBiz = searchModel.list.Where(c => c.nickname == CurrentTask.NickName || c.alias == CurrentTask.Alias).FirstOrDefault();
            if(null != searchBiz)
            {
                CurrentTask.AppModel = searchBiz;
                using (var db = new SpiderContext())
                {
                    var accountModel = db.WeChatAccounts.Where(c => c.Id == CurrentTask.SpiderId).FirstOrDefault();
                    if (null == accountModel)
                    {
                        LogService.Error("未找到账号实体");
                        return;
                    }
                    if (accountModel.LastUpdate.HasValue)
                    {
                        DateTime last = accountModel.LastUpdate.Value;
                        TimeSpan ts = last - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                        LastStartTimeSpan = Convert.ToInt32(ts.TotalSeconds);
                    }
                    LogService.Info("更新公众号信息实体");
                    accountModel.Alias = searchBiz.alias;
                    accountModel.FakeId = searchBiz.fakeid;
                    accountModel.RoundHeadImg = searchBiz.round_head_img;
                    int.TryParse(searchBiz.service_type, out int sType);
                    accountModel.ServiceType = sType;
                    accountModel.NickName = searchBiz.nickname;
                    accountModel.LastUpdate = DateTime.Now;
                    db.SaveChanges();

                    var task = SpiderTaskList.Where(c => c.SpiderId == CurrentTask.SpiderId).FirstOrDefault();
                    task.LastUpdateDate = DateTime.Now;
                    task.Alias = accountModel.Alias;
                    task.NickName = accountModel.NickName;

                }
                CurrentTask.ToQueryArticleList(CurrentTask.CurrentPage);
            }
            else
            {
                LogService.Error("未匹配到公众号名称");
                NextTask();
            }
        }
        /// <summary>
        /// 获取历史文章列表回调
        /// </summary>
        /// <param name="articleResponseModel"></param>
        private void ArticleResponseHandlerCallback(ArticleResponseModel articleResponseModel)
        {
            LogService.Info($"当前条数：{articleResponseModel.app_msg_list.Count},准备写入到数据库");
            using (var db = new SpiderContext())
            {
                var accountModel = db.WeChatAccounts.Where(c => c.Id == CurrentTask.SpiderId).FirstOrDefault();
                if(null == accountModel)
                {
                    LogService.Error("未找到账号实体");
                    NextTask();
                }
                else
                {
                    int oldArticleCount = articleResponseModel.app_msg_list.Where(a => a.create_time < LastStartTimeSpan).Count();
                    
                    if (oldArticleCount < articleResponseModel.app_msg_list.Count)
                    {
                        articleResponseModel.app_msg_list.ForEach(item =>
                        {
                            var article = db.WeChatArticles.Where(c => c.aid == item.aid).ToList();
                            if (article.Count >= 1 || item.create_time < LastStartTimeSpan)
                            {
                                return;
                            }
                            WeChatArticle model = item.ConvertModel();
                            model.Id = Guid.NewGuid().ToString().Replace("-", "");
                            model.CreateDate = DateTime.Now;
                            model.Homeownid = accountModel.Homeownid;
                            model.WeChatAccount = accountModel;
                            db.WeChatArticles.Add(model);
                        });
                        try
                        {
                            db.SaveChanges();
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
                        NextTask();
                    }
                }
            }
        }

        private void NextTask()
        {
            LogService.Info("准备开始下一个任务，休眠30秒");
            Thread.Sleep(1000 * 30);
            SpiderTaskList.Remove(CurrentTask);
            CurrentTask = null;
            TaskRuning = false;
            if (SpiderTaskList.Count == 0)
            {
                InitStatus = false;
            }
        }
    }
}
