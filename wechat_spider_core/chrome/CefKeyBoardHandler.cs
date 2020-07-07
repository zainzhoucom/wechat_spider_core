using CefSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using wechat_spider_core.spider;

namespace wechat_spider_core
{
    public class CefKeyBoardHandler : IKeyboardHandler
    {
        public bool OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            if (type == KeyType.KeyUp && Enum.IsDefined(typeof(Keys), windowsKeyCode))
            {
                var key = (Keys)windowsKeyCode;
                if(key == Keys.F5)
                {
                    browser.Reload();
                }
                else if(key == Keys.F12)
                {
                    browser.ShowDevTools();
                }
                else if (key == Keys.P)
                {
                    if(ChromeWebBrowser.spiderManager.taskStatus.Visible)
                    {
                        ChromeWebBrowser.spiderManager.taskStatus.Activate();
                    }
                    else
                    {
                        if(ChromeWebBrowser.spiderManager.taskStatus.IsDisposed)
                        {
                            ChromeWebBrowser.spiderManager.taskStatus = new TaskStatusMain();
                        }
                        ChromeWebBrowser.spiderManager.taskStatus.Show();
                    }
                }
            }
            return false;
        }

        public bool OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {
            return false;
        }
    }
}
