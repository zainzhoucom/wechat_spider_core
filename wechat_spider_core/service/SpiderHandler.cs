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
        public async Task<int> InsertArticleByList(List<WeChatArticle> list, long accountId)
        {
            using var db = new SpiderContext();
            var account = await db.WeChatAccounts.FirstOrDefaultAsync(c => c.Id == accountId);
            if(null == account)
            {
                throw new NullReferenceException("未找到公众号实体");
            }
            list.ForEach(item =>
            {
                item.WeChatAccount = account;
                db.WeChatArticles.Add(item);
            });
            return await db.SaveChangesAsync();

        }

        public async Task InsertClientSgin(long clientId)
        {
            using var db = new SpiderContext();
            db.TaskStartSigns.Add(new TaskStartSign
            {
                Id = IdWorkContext.ID_WORKER.NextId(),
                ClientId = clientId,
                StartDate = DateTime.Now,
                RunStatus = true
            });
            await db.SaveChangesAsync();

        }

        public async Task<List<WeChatAccount>> ListWeChatAccountAsync()
        {
            using var db = new SpiderContext();
            return await db.WeChatAccounts
            .Include(c => c.TaskStartSign)
            .Include(role => role.SpiderRoles)
            .OrderBy(c => c.Id)
            .ToListAsync();
        }

        public async Task<List<WeChatArticle>> QueryArticleByAid(string aid)
        {
            using var db = new SpiderContext();
            return await db.WeChatArticles.Where(c => c.aid == aid).ToListAsync();
        }

        public async Task<bool> QuerySpaderStatus(long accountId)
        {
            using var spiderContext = new SpiderContext();
            var account = await spiderContext.WeChatAccounts.Where(a => a.Id == accountId)
            .Include(t => t.TaskStartSign).FirstOrDefaultAsync();
            if (null == account)
            {
                throw new NullReferenceException("未找到公众号实体");
            }
            if(account.TaskStartSign == null)
            {
                return false;
            }
            var date = account.TaskStartSign.StartDate;
            if (date.AddMinutes(10) > DateTime.Now)
            {
                return true;
            }
            return account.TaskStartSign.RunStatus;
        }

        public async Task<WeChatAccount> QueryWeChatAccountAsync(long accountId)
        {
            using var spiderContext = new SpiderContext();
            return await spiderContext.WeChatAccounts.FirstOrDefaultAsync(c => c.Id == accountId);
        }

        public async Task<int> SetAccountSpiderStart(long accountId, long clientId)
        {
            using var spiderContext = new SpiderContext();
            var account = await spiderContext.WeChatAccounts.FirstOrDefaultAsync(c => c.Id == accountId);
            if (null == account)
            {
                throw new NullReferenceException("未找到公众号实体");
            }
            var client = await spiderContext.TaskStartSigns.FirstOrDefaultAsync(c => c.ClientId == clientId);
            if (null == client)
            {
                throw new NullReferenceException("未找到客户端实体");
            }
            account.TaskStartSign = client;
            return await spiderContext.SaveChangesAsync();
        }

        public async Task<int> SetAccountSpiderStop(long accountId)
        {
            using var spiderContext = new SpiderContext();
            var account = await spiderContext.WeChatAccounts.FirstOrDefaultAsync(c => c.Id == accountId);
            if (null == account)
            {
                throw new NullReferenceException("未找到公众号实体");
            }
            account.TaskStartSign = null;
            return await spiderContext.SaveChangesAsync();
        }

        public async Task<int> SetClientSignOut(long clientId)
        {
            using var spiderContext = new SpiderContext();
            var sign = await spiderContext.TaskStartSigns.FirstOrDefaultAsync(c => c.ClientId == clientId);
            if (null == sign)
            {
                throw new NullReferenceException("未找到实体");
            }
            sign.RunStatus = false;
            return await spiderContext.SaveChangesAsync();
        }

        public async Task UpdateClientSgin(long clientId)
        {
            using var spiderContext = new SpiderContext();
            var sign = await spiderContext.TaskStartSigns.FirstOrDefaultAsync(c => c.ClientId == clientId);
            if (null != sign)
            {
                sign.StartDate = DateTime.Now;
                await spiderContext.SaveChangesAsync();
            }
            else
            {
                throw new NullReferenceException("未找到实体");
            }
        }

        public async Task<int> UpdateWeChatAccountAsync(WeChatAccount model)
        {
            using var spiderContext = new SpiderContext();
            spiderContext.WeChatAccounts.Update(model);
            return await spiderContext.SaveChangesAsync();
        }
    }
}
