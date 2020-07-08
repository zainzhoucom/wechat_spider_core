using Snowflake.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace wechat_spider_core.service
{
    public static class IdWorkContext
    {
        public readonly static IdWorker ID_WORKER = new IdWorker(1, 1);

        public readonly static long CLIENT_ID = ID_WORKER.NextId();
    }
}
