using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace wechat_spider_core.spider
{
    public class JavascriptCallback
    {
        public static Action<SearchModel> OnSearchHandler { get; set; }
        public static Action<ArticleResponseModel> OnArticleResponseHandler { get; set; }

        public void OnWriteLogHandler(string log, string type = "error")
        {
            if (type == "error")
            {
                LogService.Error(log);
            }
            else
            {
                LogService.Info(log);
            }

        }
        /// <summary>
        /// javascript异步请求完成后的回调
        /// </summary>
        /// <param name="result"></param>
        /// <param name="origin"></param>
        public void OnHttpGetHandler(string result, string origin)
        {
            BaseModel baseModel = JsonConvert.DeserializeObject<BaseModel>(result);
            if (baseModel.base_resp.err_msg == "ok")
            {
                if (origin == "queryList")
                {
                    SearchModel searchModel = JsonConvert.DeserializeObject<SearchModel>(result);
                    OnSearchHandler?.Invoke(searchModel);
                }
                else if (origin == "article")
                {
                    ArticleResponseModel articleResponseModel = JsonConvert.DeserializeObject<ArticleResponseModel>(result);
                    OnArticleResponseHandler?.Invoke(articleResponseModel);
                }
            }
            else
            {
                LogService.Error($"请求{origin}出错：{result}");
                ChromeWebBrowser.spiderManager.OnRequestError();
            }
        }
    }
}
