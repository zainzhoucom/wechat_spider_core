using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace wechat_spider_core.ef
{
    public class SpiderContext : DbContext
    {
        public DbSet<WeChatAccount> WeChatAccounts { get; set; }

        public DbSet<WeChatArticle> WeChatArticles { get; set; }

        public DbSet<TaskStartSign> TaskStartSigns { get; set; }

        public DbSet<SpiderRole> SpiderRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //"User ID=root;Password=123456;Host=127.0.0.1;Port=3306;Database=wechat_spider_record;Pooling=true;"
            //ConfigurationManager.ConnectionStrings["spiderConnection"].ConnectionString
            optionsBuilder.UseNpgsql("User ID=postgres;Password=123456;Host=127.0.0.1;Port=5432;Database=postgres;Pooling=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskStartSign>()
                .Property(t => t.RunStatus)
                .HasDefaultValue(false);
        }
    }
}
