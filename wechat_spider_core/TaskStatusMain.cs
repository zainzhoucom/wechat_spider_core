using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using wechat_spider_core.chrome;
using wechat_spider_core.spider;

namespace wechat_spider_core
{
    public partial class TaskStatusMain : Form
    {
        private ChromiumWebBrowser bowser;
        public TaskStatusMain()
        {
            Initialization();

            InitializeComponent();
        }

        private void Initialization()
        {
            bowser = new ChromiumWebBrowser(AppDomain.CurrentDomain.BaseDirectory + "taskStatus.html");
            bowser.KeyboardHandler = new CefKeyBoardHandler();
            bowser.MenuHandler = new MenuHandler();
            bowser.FrameLoadEnd += ChromeBrowser_FrameLoadEnd;
            this.Controls.Add(bowser);
            bowser.Dock = DockStyle.Fill;
        }

        private void ChromeBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            bowser.JavascriptObjectRepository.ResolveObject += (send, e) =>
            {
                var repo = e.ObjectRepository;
                if (e.ObjectName == "taskStatus")
                {
                    BindingOptions bindingOptions = BindingOptions.DefaultBinder;
                    bindingOptions.CamelCaseJavascriptNames = false;
                    repo.Register(e.ObjectName, new TaskStatus(), true, bindingOptions);
                }
            };
        }
    }
}
