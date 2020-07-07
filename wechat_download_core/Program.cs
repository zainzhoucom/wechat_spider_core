using Microsoft.CSharp.RuntimeBinder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using wechat_spider_core.ef;

namespace wechat_download_core
{
    class Program
    {
        private static List<DownloadModel> downloadModels = new List<DownloadModel>();
        private static readonly string BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"downloads");
        private static int SleepTime = 1000 * 3;
        static async Task Main(string[] args)
        {
            if(args.Length > 0 && int.TryParse(args[0], out SleepTime))
            {
                SleepTime *= 1000;
            }
            Console.WriteLine("程序启动!");
            while (true)
            {
                await BeginRequestImages();
                if(downloadModels.Count > 0)
                {
                    await DownloadImages();
                }
                else
                {
                    Console.WriteLine("没有待下载记录");
                }
                
                Console.WriteLine("一轮遍历完毕，休眠30秒");
                Thread.Sleep(1000 * 30);
            }
            
        }

        static async Task BeginRequestImages()
        {
            Console.WriteLine("开始获取文章");
            downloadModels.Clear();
            using (SpiderContext db = new SpiderContext())
            {
                var list = await db.WeChatArticles.Where(c => !c.Download)
                    .Include(c=> c.WeChatAccount)
                    .OrderByDescending(c => c.CreateDate).Take(100).ToListAsync();
                list.ForEach(c =>
                {
                    downloadModels.Add(
                        new DownloadModel
                        {
                            Id = c.Id,
                            alias = c.WeChatAccount.Alias,
                            Url = c.cover
                        }
                        );
                });
            }
            Console.WriteLine($"获取文章结束，共{downloadModels.Count}条");
        }

        static async Task DownloadImages()
        {
            Func<DownloadModel, Task<string>> asyncTask = async model =>
            {
                using (WebClient webClient = new WebClient())
                {
                    Console.WriteLine($"下载文件{model.Url}");
                    string fileName = Guid.NewGuid().ToString().Replace("-", "") + ".jpg";
                    string path = Path.Combine(model.alias, fileName);
                    try
                    {
                        string savePath = Path.Combine(BasePath, model.alias);
                        if(!Directory.Exists(savePath))
                        {
                            Directory.CreateDirectory(savePath);
                        }
                        await webClient.DownloadFileTaskAsync(model.Url, Path.Combine(savePath,fileName));
                        Console.WriteLine($"下载完成，输出路径{path}");
                        return path;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"下载出错:{e.Message}");
                        return "";
                    }
                    
                }
            };
            Func<DownloadModel, Task> saveRecordAsync = async model =>
                {
                    Console.WriteLine("准备保存地址到数据库");
                    using (SpiderContext db = new SpiderContext())
                    {
                        var article = await db.WeChatArticles.FirstOrDefaultAsync(c => c.Id == model.Id);
                        if (null != article)
                        {
                            article.Download = true;
                            article.LocalPath = model.Path;
                            await db.SaveChangesAsync();
                        }
                    }
                    Console.WriteLine("保存完成");
                };
            foreach (var item in downloadModels)
            {
                item.Path = await asyncTask(item);
                if (!string.IsNullOrWhiteSpace(item.Path))
                {
                    await saveRecordAsync(item);
                }

                Console.WriteLine($"在此休眠{SleepTime / 1000}秒");
                Thread.Sleep(SleepTime);
            }
        }

    }
}
