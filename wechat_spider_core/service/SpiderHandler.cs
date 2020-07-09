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
        private readonly SpiderContext spiderContext = InitIocModule.GetFromFac<SpiderContext>();
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
            spiderContext.TaskStartSigns.Add(new TaskStartSign
            {
                Id = IdWorkContext.ID_WORKER.NextId(),
                ClientId = clientId,
                StartDate = DateTime.Now,
                RunStatus = true
            });
            await spiderContext.SaveChangesAsync();
        }

        public async Task<List<WeChatAccount>> ListWeChatAccountAsync()
        {
            return await spiderContext.WeChatAccounts
                .Include(c => c.TaskStartSign)
                .Include(role => role.SpiderRoles.OrderByDescending(role => role.Role))
                .OrderBy(c => c.LastUpdate)
                .ToListAsync();
        }

        public async Task<List<WeChatArticle>> QueryArticleByAid(string aid)
        {
            return await spiderContext.WeChatArticles.Where(c => c.aid == aid).ToListAsync();
        }

        public async Task<bool> QuerySpaderStatus(long accountId)
        {
            var account = await spiderContext.WeChatAccounts.Where(a => a.Id == accountId)
                .Include(t => t.TaskStartSign).FirstOrDefaultAsync();
            if(null == account)
            {
                throw new NullReferenceException("未找到公众号实体");
            }
            return account.TaskStartSign != null || account.TaskStartSign.RunStatus;
        }

        public async Task<WeChatAccount> QueryWeChatAccountAsync(long accountId)
        {
            return await spiderContext.WeChatAccounts.FirstOrDefaultAsync(c => c.Id == accountId);
        }

        public async Task<int> SetAccountSpiderStart(long accountId, long clientId)
        {
            var account = await spiderContext.WeChatAccounts.FirstOrDefaultAsync(c => c.Id == accountId);
            if(null == account)
            {
                throw new NullReferenceException("未找到公众号实体");
            }
            var client = await spiderContext.TaskStartSigns.FirstOrDefaultAsync(c => c.Id == clientId);
            if(null == client)
            {
                throw new NullReferenceException("未找到客户端实体");
            }
            account.TaskStartSign = client;
            return await spiderContext.SaveChangesAsync();
        }

        public async Task<int> SetAccountSpiderStop(long accountId)
        {
            var account = await spiderContext.WeChatAccounts.FirstOrDefaultAsync(c => c.Id == accountId);
            if(null == account)
            {
                throw new NullReferenceException("未找到公众号实体");
            }
            account.TaskStartSign = null;
            return await spiderContext.SaveChangesAsync();
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
            var sign = await spiderContext.TaskStartSigns.FirstOrDefaultAsync(c => c.Id == clientId);
            if(null != sign)
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
            spiderContext.WeChatAccounts.Update(model);
            return await spiderContext.SaveChangesAsync();
        }
    }
}
