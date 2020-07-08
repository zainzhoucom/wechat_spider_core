using CefSharp;
using System;
using System.Linq;
using System.Collections.Generic;

namespace wechat_spider_core
{
    public class SpiderTask
    {
        public long SpiderId { get; set; }
        public string NickName { get; set; }

        public string Alias { get; set; }

        public List<string> Roles { get; set; }

        public int CurrentPage { get; set; } = 1;

        public SearchBiz AppModel { get; set; }

        public DateTime? LastUpdateDate { get; set; }

        public SpiderTask(string name)
        {
            Roles = new List<string>();
            NickName = name;
        }

        public bool Ready(string role, string runDate)
        {
            return Roles.Any(c => c == role) && (!LastUpdateDate.HasValue || LastUpdateDate.Value.ToString("yyyyMMddHH") != runDate);
        }

        public void Run()
        {
            SearchQueryModel searchQuery = new SearchQueryModel
            {
                token = Config.Token,
                query = NickName
            };
            ChromeWebBrowser.chromeBrowser.ExecuteScriptAsync($"httpGet('https://mp.weixin.qq.com/cgi-bin/searchbiz{searchQuery}','queryList')");
        }

        public void ToQueryArticleList(int page)
        {
            ArticleReuqestModel articleReuqestModel = new ArticleReuqestModel
            {
                token = Config.Token,
                begin = page.ToString(),
                fakeid = AppModel.fakeid
            };
            ChromeWebBrowser.chromeBrowser.ExecuteScriptAsync($"httpGet('https://mp.weixin.qq.com/cgi-bin/appmsg{articleReuqestModel}','article')");
        }
    }
}
