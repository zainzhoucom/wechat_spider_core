using CefSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace wechat_spider_core
{
    public class FilterManager
    {
        private static readonly Dictionary<string, IResponseFilter> FilterList = new Dictionary<string, IResponseFilter>();

        public static IResponseFilter CreateFilter(string guid)
        {
            lock (FilterList)
            {
                IResponseFilter filter;

                filter = new TestFilter();
                FilterList.Add(guid, filter);
                return filter;
            }
        }
        public static void RemoveFileter(string guid)
        {
            lock (FilterList)
            {
                FilterList.Remove(guid);
            }
        }

        public static IResponseFilter GetFileter(string guid)
        {
            lock (FilterList)
            {
                return FilterList[guid];
            }
        }
    }
}
