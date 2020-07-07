using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Text;

namespace wechat_spider_core
{
    public static class Config
    {
        public readonly static List<string> FilterUrls = new List<string>
        {
            "https://mp.weixin.qq.com/cgi-bin/bizlogin?action=login"
        };
        private readonly static Dictionary<string, string> Cookies = new Dictionary<string, string>();

        public readonly static string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.97 Safari/537.36";

        public static string Token { get; set; }

        public static void SetToken(string url)
        {
            string[] param = url.Split('&');
            foreach (var item in param)
            {
                string[] query = item.Split('=');
                if (query.Length == 2 && query[0] == "token")
                {
                    Token = query[1];
                }
            }
        }
        /// <summary>
        /// cookie
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetCookie(string key, string value, string domain = "")
        {
            if (Cookies.ContainsKey(key))
            {
                Cookies.Remove(key);
            }
            Cookies.Add(key, value);
        }

        public static string StringCookie()
        {
            SetCookie("rewardsn", "");
            SetCookie("wxtokenkey", "777");
            SetCookie("mm_lang", "zh_CN");
            SetCookie("wxuin", "91927860877577");

            List<string> stringBuilder = new List<string>();
            foreach (var item in Cookies)
            {
                stringBuilder.Add($"{item.Key}={item.Value}");
            }
            string result = string.Join(";", stringBuilder);
            LogService.Info($"拼接cookie:{result}");
            return result;
        }
    }
}
