using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using wechat_spider_core.ef;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using wechat_spider_core.ioc;

namespace wechat_spider_core.service
{
    public class SpiderHandler : ISpiderHandler
    {
        private readonly SpiderContext spiderContext = InitIocModule.GetContainer().Resolve<SpiderContext>();
        public async Task<int> InsertArticleByList(List<WeChatArticle> list)
        {
            list.ForEach(item =>
            {
                spiderContext.WeChatArticles.Add(item);
            });
            return await spiderContext.SaveChangesAsync();
        }

        public async Task InsertClientSgin(long clientId)
        {
            using (var db = new SpiderContext())
            {
                db.TaskStartSigns.Add(new TaskStartSign
                {
                    Id = IdWorkContext.ID_WORKER.NextId(),
                    ClientId = clientId,
                    StartDate = DateTime.Now,
                    RunStatus = true
                });
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<WeChatAccount>> ListWeChatAccountAsync()
        {
            using (var db = new SpiderContext())
            {
                return await db.WeChatAccounts
                    .Include(c => c.TaskStartSign)
                    .Include(role => role.SpiderRoles.OrderByDescending(role => role.Role))
                    .OrderBy(c => c.LastUpdate)
                    .ToListAsync();
            }            
        }

        public async Task<List<WeChatArticle>> QueryArticleByAid(string aid)
        {
            return await spiderContext.WeChatArticles.Where(c => c.aid == aid).ToListAsync();
        }

        public async Task<bool> QuerySpaderStatus(long accountId)
        {
            using (var db = new SpiderContext())
            {
                var account = await db.WeChatAccounts.Where(a => a.Id == accountId)
                    .Include(t => t.TaskStartSign).FirstOrDefaultAsync();
                if(null == account)
                {
                    throw new NullReferenceException("未找到公众号实体");
                }
                return account.TaskStartSign.RunStatus;
            }
        }

        public async Task<WeChatAccount> QueryWeChatAccountAsync(long accountId)
        {
            using (var db = new SpiderContext())
            {
                return await db.WeChatAccounts.FirstOrDefaultAsync(c => c.Id == accountId);
            }
        }

        public async Task<int> SetClientSignOut(long clientId)
        {
            var sign = await spiderContext.TaskStartSigns.FirstOrDefaultAsync(c => c.ClientId == clientId);
            if(null == sign)
            {
                throw new NullReferenceException("未找到实体");
            }
            sign.RunStatus = false;
            return await spiderContext.SaveChangesAsync();
        }

        public async Task UpdateClientSgin(long clientId)
        {
            using (var db = new SpiderContext())
            {
                var sign = await db.TaskStartSigns.FirstOrDefaultAsync(c => c.Id == clientId);
                if(null != sign)
                {
                    sign.StartDate = DateTime.Now;
                    await db.SaveChangesAsync();
                }
                else
                {
                    throw new NullReferenceException("未找到实体");
                }
            }
        }

        public async Task<int> UpdateWeChatAccountAsync(WeChatAccount model)
        {
            using (var db = new SpiderContext())
            {
                db.WeChatAccounts.Update(model);
                return await db.SaveChangesAsync();
            }
        }
    }
}
