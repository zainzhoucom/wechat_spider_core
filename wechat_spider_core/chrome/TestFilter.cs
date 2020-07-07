using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace wechat_spider_core
{
    public class TestFilter : IResponseFilter
    {
        private int contentLength = 0;
        public List<byte> dataAll = new List<byte>();
        public void SetContentLength(int contentLength)
        {
            this.contentLength = contentLength;
        }

        public void Dispose()
        {
            ;
        }

        public FilterStatus Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten)
        {
            try
            {
                if (dataIn == null)
                {
                    dataInRead = 0;
                    dataOutWritten = 0;
                    return FilterStatus.Done;
                }

                dataInRead = dataIn.Length;
                byte[] bs = new byte[dataIn.Length];
                dataIn.Read(bs, 0, bs.Length);
                dataAll.AddRange(bs);

                dataInRead = dataIn.Length;
                dataOutWritten = Math.Min(dataInRead, dataOut.Length);

                dataOut.Write(bs, 0, (int)dataOutWritten);
                dataOut.Seek(0, SeekOrigin.Begin);

                if (dataAll.Count < contentLength)
                {
                    return FilterStatus.NeedMoreData;
                }
                else
                {
                    return FilterStatus.Done;
                }
            }
            catch (Exception ex)
            {
                dataOutWritten = 0;
                dataInRead = 0;
                return FilterStatus.Error;
            }
        }

        public bool InitFilter()
        {
            return true;
        }
    }
}
