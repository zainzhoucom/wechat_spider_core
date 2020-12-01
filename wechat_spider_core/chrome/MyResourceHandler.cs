using CefSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace wechat_spider_core.chrome
{
    public class MyResourceHandler : ResourceHandler
    {
        public override CefReturnValue ProcessRequestAsync(IRequest request, ICallback callback)
        {
            var resType = request.ResourceType;
            callback.Continue();
            return CefReturnValue.Continue;
        }
    }
}
