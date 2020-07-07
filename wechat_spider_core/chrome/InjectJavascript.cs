using CefSharp;
using CefSharp.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;
using wechat_spider_core.spider;

namespace wechat_spider_core.chrome
{
    public static class InjectJavascript
    {
        /// <summary>
        /// 注入get请求方法
        /// </summary>
        public static void InjectHttpGet()
        {
            ChromeWebBrowser.chromeBrowser.ExecuteScriptAsync(@"
                async function httpGet(url,type){
                    debugger;
                    await CefSharp.BindObjectAsync('httpHandler');
                    try{
                        var xhr = new XMLHttpRequest();
                        xhr.open('GET', url, true);
                        xhr.onreadystatechange = function() {
                            if (xhr.readyState == 4 && xhr.status == 200 || xhr.status == 304) {
                                httpHandler.OnWriteLogHandler(xhr.responseText,'info');
                                httpHandler.OnHttpGetHandler(xhr.responseText,type);
                            }else{
                                httpHandler.OnWriteLogHandler(`${xhr.readyState}-${xhr.status}-${xhr.responseText}`,'info')
                            }
                        };
                        xhr.send();
                    }catch(error){
                        httpHandler.OnWriteLogHandler(error)
                    }
                }
            ");
        }

        public static void InjectNetCollback()
        {
            //ChromeWebBrowser.chromeBrowser.JavascriptObjectRepository.Register("httpHandler", new JavascriptCallback(), true);
            ChromeWebBrowser.chromeBrowser.JavascriptObjectRepository.ResolveObject += (send, e) =>
            {
                var repo = e.ObjectRepository;
                if (e.ObjectName == "httpHandler")
                {
                    BindingOptions bindingOptions = BindingOptions.DefaultBinder;
                    bindingOptions.CamelCaseJavascriptNames = false;
                    bindingOptions = new BindingOptions { CamelCaseJavascriptNames = false, Binder = new MyCustomBinder() };
                    repo.Register("httpHandler", new JavascriptCallback(), isAsync: true, options: bindingOptions);
                }
            };
            
        }
    }

        public class MyCustomBinder : IBinder
        {
            public object Bind(object obj, Type targetParamType)
            {
                LogService.Info($"{ obj}");
                return obj;
            }
        }
}
