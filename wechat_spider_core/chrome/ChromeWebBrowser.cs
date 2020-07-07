using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using wechat_spider_core.chrome;
using wechat_spider_core.spider;

namespace wechat_spider_core
{
    public class ChromeWebBrowser
    {
        public static ChromiumWebBrowser chromeBrowser;
        private readonly static Timer SpiderManagerTask = new Timer();
        public readonly static SpiderManager spiderManager = new SpiderManager();
        private static Timer ReloadPageTimer = new Timer();
        public void InitializeChromium(Form form)
        {
            CefSettings settings = new CefSettings();
            settings.CefCommandLineArgs.Add("--disable-web-security", "1");
            settings.CefCommandLineArgs.Add("--enable-system-flash", "1");
            settings.Locale = "zh-CN";
            settings.UserAgent = Config.UserAgent;
            Cef.Initialize(settings);

            chromeBrowser = new ChromiumWebBrowser("https://mp.weixin.qq.com/");

            chromeBrowser.FrameLoadEnd += ChromeBrowser_FrameLoadEnd;
            chromeBrowser.KeyboardHandler = new CefKeyBoardHandler();
            chromeBrowser.MenuHandler = new MenuHandler();
            chromeBrowser.ResourceRequestHandlerFactory = new ResourceRequestHandlerFactory();

            chromeBrowser.ConsoleMessage += ChromeBrowser_ConsoleMessage;
            InjectJavascript.InjectNetCollback();
            

            form.Controls.Add(chromeBrowser);

            chromeBrowser.Dock = DockStyle.Fill;

            ReloadPageTimer.Interval = 1000 * 60 * 20;//20分钟刷新一次，防止过期
            ReloadPageTimer.Tick += ReloadPageTimer_Tick;
            ReloadPageTimer.Start();

            SpiderManagerTask.Interval = 1000 * 60 * 10;
            SpiderManagerTask.Tick += SpiderManagerTask_Tick;
            SpiderManagerTask.Start();
            
        }

        private void ChromeBrowser_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            if (e.Message.Contains("Error"))
            {
                LogService.Error($"JS错误：{e.Message},Url:{e.Source},Line:{e.Line}");
            }
        }

        private void ChromeBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            var cookieManager = Cef.GetGlobalCookieManager();
            CookieVisitor cookie = new CookieVisitor();
            cookie.SendCookie += Cookie_SendCookie;
            cookieManager.VisitAllCookies(cookie);
            if (e.Url.StartsWith("https://mp.weixin.qq.com/cgi-bin/home"))
            {
                InjectJavascript.InjectHttpGet();
                //进入首页后开始任务
                SpiderManagerTask_Tick(sender, e);
            }

        }

        private void Cookie_SendCookie(Cookie obj)
        {
            Config.SetCookie(obj.Name, obj.Value, obj.Domain);
            LogService.Info($"add cookie:{obj.Name}={obj.Value}");
        }

        private void ReloadPageTimer_Tick(object sender, EventArgs e)
        {
            chromeBrowser.Reload();
        }

        private void SpiderManagerTask_Tick(object sender, EventArgs e)
        {
            spiderManager.Init();
        }
    }
}
