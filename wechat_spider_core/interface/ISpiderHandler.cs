using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using wechat_spider_core.ef;

namespace wechat_spider_core
{
    public interface ISpiderHandler
    {
        /// <summary>
        /// 查询所有公众号
        /// </summary>
        /// <returns></returns>
        Task<List<WeChatAccount>> ListWeChatAccountAsync();
        /// <summary>
        /// 设置客户端上线
        /// </summary>
        /// <returns></returns>
        Task InsertClientSgin(long clientId);
        /// <summary>
        /// 更新客户端存活时间
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        Task UpdateClientSgin(long clientId);
        /// <summary>
        /// 查询该公众还是否有任务在执行
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<bool> QuerySpaderStatus(long accountId);
        /// <summary>
        /// 查询公众号实体
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<WeChatAccount> QueryWeChatAccountAsync(long accountId);

        Task<int> UpdateWeChatAccountAsync(WeChatAccount model);
        /// <summary>
        /// 批量保存文章
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<int> InsertArticleByList(List<WeChatArticle> list);

        Task<List<WeChatArticle>> QueryArticleByAid(string aid);

        Task<int> SetClientSignOut(long clientId);
    }
}
