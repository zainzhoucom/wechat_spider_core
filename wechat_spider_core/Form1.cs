using CefSharp;
using System.Windows.Forms;

namespace wechat_spider_core
{
    public partial class Form1 : Form
    {
        private readonly static ChromeWebBrowser chromeWebBrowser = new ChromeWebBrowser();
        
        public Form1()
        {
            chromeWebBrowser.InitializeChromium(this);
            InitializeComponent();

            //TaskStatus taskStatus = new TaskStatus();
            //taskStatus.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }
    }
}
