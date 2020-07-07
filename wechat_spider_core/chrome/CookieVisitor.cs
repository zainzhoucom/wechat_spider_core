using CefSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace wechat_spider_core
{
    public class CookieVisitor : ICookieVisitor
    {
        public event Action<Cookie> SendCookie;
        public void Dispose()
        {
            
        }

        public bool Visit(Cookie cookie, int count, int total, ref bool deleteCookie)
        {
            deleteCookie = false;
            SendCookie?.Invoke(cookie);
            return true;
        }
    }
}
