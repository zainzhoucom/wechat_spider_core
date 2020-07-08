using Autofac;
using CefSharp;
using System;
using System.Windows.Forms;
using wechat_spider_core.ioc;
using wechat_spider_core.service;

namespace wechat_spider_core
{
    public partial class Form1 : Form
    {
        private readonly static ChromeWebBrowser chromeWebBrowser = new ChromeWebBrowser();
        private readonly ISpiderHandler spiderHandler = InitIocModule.GetContainer().Resolve<ISpiderHandler>();

        public Form1()
        {
            chromeWebBrowser.InitializeChromium(this);
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                spiderHandler.SetClientSignOut(IdWorkContext.CLIENT_ID);
            }
            catch (Exception ex)
            {
                LogService.Error($"设置客户端退出异常:{ex.Message}", ex);
            }
            
            Cef.Shutdown();
        }
    }
}
