using CefSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace wechat_spider_core
{
    public class ResourceRequestHandlerFactory : IResourceRequestHandlerFactory
    {
        public bool HasHandlers => true;

        public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            return new ResponseHandler();
        }
    }
}
