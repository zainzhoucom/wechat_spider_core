using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace wechat_spider_core
{
    public static class LogService
    {
        private static readonly string LoggerAppenderTypeName = "Default";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly bool Isinit = false;
        private static bool _logDubugEnable = false;
        private static bool _logInfoEnable = false;
        private static bool _logErrorEnable = false;

        static LogService()
        {
            if (!Isinit)
            {
                Isinit = true;
                LoadConfig();
            }
        }
        public static void LoadConfig()
        {
            _logErrorEnable = Logger.IsErrorEnabled;
            _logDubugEnable = Logger.IsDebugEnabled;
            _logInfoEnable = Logger.IsInfoEnabled;
        }

        /// <summary>
        /// 写调试消息
        /// </summary>
        /// <param name="info"></param>
        public static void Debug(string info, Exception ex = null)
        {
            if (_logDubugEnable)
            {
                Logger.Debug(BuildMessage(info, ex));
            }
        }

        public static void Info(string info, Exception ex = null)
        {
            if (_logInfoEnable)
            {
                Logger.Info(BuildMessage(info, ex));
            }
        }

        /// <summary>
        /// 写错误消息
        /// </summary>
        /// <param name="info"></param>
        public static void Error(string info, Exception ex = null)
        {
            if (_logErrorEnable)
            {
                Logger.Error(BuildMessage(info, ex));
            }
        }

        private static string BuildMessage(string info, Exception ex = null)
        {
            if (ex == null)
            {
                return info;
            }

            return string.Format("{3}标题：{0} {3}详情：{1} {3}堆栈：{2}{3}", info, ex.Message, ex.StackTrace, Environment.NewLine);
        }
    }
}
