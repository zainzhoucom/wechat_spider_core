using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace wechat_spider_core.ef
{
    public class SpiderContext : DbContext
    {
        public DbSet<WeChatAccount> WeChatAccounts { get; set; }

        public DbSet<WeChatArticle> WeChatArticles { get; set; }

        public DbSet<TaskStartSign> TaskStartSigns { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //"User ID=root;Password=123456;Host=127.0.0.1;Port=3306;Database=wechat_spider_record;Pooling=true;"
            optionsBuilder.UseMySQL(ConfigurationManager.ConnectionStrings["spiderConnection"].ConnectionString);
        }
    }
}
