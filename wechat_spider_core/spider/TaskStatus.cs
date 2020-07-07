using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace wechat_spider_core.spider
{
    public class TaskStatus
    {
        public List<object> GetTaskList()
        {
            var list = ChromeWebBrowser.spiderManager.GetSpiderTasks();
            List<object> listName = new List<object>();
            list.ForEach(t =>
            {
                listName.Add(
                    new
                    {
                        taskName = t.NickName,
                        taskRole = string.Join(',', t.Roles),
                        updateDate = t.LastUpdateDate.HasValue ? t.LastUpdateDate?.ToString("yyyy-MM-dd HH:mm") : "暂无"
                    });
            });
            return listName;
        }

        public object GetCurrentTask()
        {
            var current = ChromeWebBrowser.spiderManager.GetCurrentTask();
            return new
            {
                taskName = current == null ? "等待中" : current.NickName,
                error = ChromeWebBrowser.spiderManager.ErrorRetryCount,
                page = current == null ? "" : current.CurrentPage.ToString()
            };
        }
    }
}
