using CefSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace wechat_spider_core
{
    public class ResponseHandler : IResourceRequestHandler
    {
        public void Dispose()
        {
            
        }

        public ICookieAccessFilter GetCookieAccessFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            return null;
        }

        public IResourceHandler GetResourceHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            return null;
        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            if (Config.FilterUrls.Contains(request.Url))
            {
                var filter = FilterManager.CreateFilter(request.Identifier.ToString());
                return filter;
            }
            return null;
        }

        public CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            return CefReturnValue.Continue;
        }

        public bool OnProtocolExecution(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            return false;
        }

        public void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            if (Config.FilterUrls.Contains(request.Url))
            {
                var filter = FilterManager.GetFileter(request.Identifier.ToString()) as TestFilter;
                string data = Encoding.UTF8.GetString(filter.dataAll.ToArray());
                if (data.Contains("token"))
                {
                    LoginModel loginModel = JsonConvert.DeserializeObject<LoginModel>(data);
                    Config.SetToken(loginModel.redirect_url);
                }
                LogService.Info($"{data}");
            }
        }

        public void OnResourceRedirect(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
        {
            
        }

        public bool OnResourceResponse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return false;
        }
    }
}
