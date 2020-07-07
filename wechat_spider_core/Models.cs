using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using wechat_spider_core.ef;

namespace wechat_spider_core
{
    public class RequestStatus
    {
        public string err_msg { get; set; }
        public int ret { get; set; }
    }
    public class BaseModel
    {
        public RequestStatus base_resp { get; set; }
    }
    public class LoginModel : BaseModel
    {
        //{"base_resp":{"err_msg":"ok","ret":0},"redirect_url":"/cgi-bin/home?t=home/index&lang=zh_CN&token=1892570050"}
        public string redirect_url { get; set; }
    }
    /// <summary>
    /// 公众号搜索实体
    /// </summary>
    public class SearchBiz
    {
        public string fakeid { get; set; }

        public string nickname { get; set; }

        public string alias { get; set; }

        public string round_head_img { get; set; }

        public string service_type { get; set; }
    }

    public class SearchModel : BaseModel
    {
        public List<SearchBiz> list { get; set; }

        public int total { get; set; }
    }

    public class SearchQueryModel
    {
        public string action { get; set; } = "search_biz";
        public string begin { get; set; } = "0";
        public string count { get; set; } = "5";
        public string query { get; set; }
        public string token { get; set; }
        public string lang { get; set; } = "zh_CN";
        public string f { get; set; } = "json";
        public string ajax { get; set; } = "1";

        public override string ToString()
        {
            return $"?action={action}&begin={begin}&count={count}&query={query}&token={token}&lang={lang}&f={f}&ajax={ajax}";
        }
    }

    public class ArticleReuqestModel
    {
        public string action { get; set; } = "list_ex";
        public string begin { get; set; } = "0";
        public string count { get; set; } = "5";

        public string fakeid { get; set; }

        public string type { get; set; } = "9";

        public string query { get; set; }

        public string token { get; set; }
        public string lang { get; set; } = "zh_CN";
        public string f { get; set; } = "json";
        public string ajax { get; set; } = "1";

        public override string ToString()
        {
            StringBuilder queryString = new StringBuilder();
            foreach (var item in typeof(ArticleReuqestModel).GetProperties())
            {
                queryString.Append($"{item.Name}={item.GetValue(this)}&");
            }
            return $"?{queryString}";
        }
    }

    public class ArticleModel
    {
        public string aid { get; set; }
        public string album_id { get; set; }
        public long appmsgid { get; set; }
        public int checking { get; set; }
        public int copyright_type { get; set; }
        public string cover { get; set; }
        public int create_time { get; set; }
        public string digest { get; set; }
        public int has_red_packet_cover { get; set; }
        public int is_original { get; set; }
        public int is_pay_subscribe { get; set; }
        public int item_show_type { get; set; }
        public int itemidx { get; set; }
        public string link { get; set; }
        public string media_duration { get; set; }
        public int mediaapi_publish_status { get; set; }
        public object[] tagid { get; set; }
        public string title { get; set; }
        public int update_time { get; set; }

        public WeChatArticle ConvertModel()
        {
            WeChatArticle result = new WeChatArticle();
            Type type = result.GetType();
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            foreach (var item in typeof(ArticleModel).GetProperties())
            {
                foreach (var dbmodel in typeof(WeChatArticle).GetProperties())
                {
                    if(item.Name == dbmodel.Name && item.Name != "tagid")
                    {
                        if(item.Name == "update_time" || item.Name == "create_time")
                        {
                            int.TryParse(item.GetValue(this).ToString(),out int time);
                            TimeSpan toNow = new TimeSpan(long.Parse(time.ToString() + "0000000"));
                            DateTime targetDt = dtStart.Add(toNow);
                            type.GetProperty(item.Name).SetValue(result, targetDt);
                            continue;
                        }
                        type.GetProperty(item.Name).SetValue(result, item.GetValue(this));
                    }
                }
            }
            return result;
        }

    }

    public class ArticleResponseModel : BaseModel
    {
        public int app_msg_cnt { get; set; }

        public List<ArticleModel> app_msg_list { get; set; }
    }
}
