using CefSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace wechat_spider_core.chrome
{
    public class ResourceSchemeHandlerFactory : ISchemeHandlerFactory
    {
        public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
        {
            return new MyResourceHandler();
        }
    }
}
