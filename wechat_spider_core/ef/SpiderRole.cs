using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace wechat_spider_core.ef
{
    [Table("spider_role")]
    public class SpiderRole
    {
        [Required,Key, Column("id")]
        public long Id { get; set; }
        /// <summary>
        /// 爬虫规则，日:时 -- dd:hh (01:12 -- 1日:12时)
        /// </summary>
        [Required,Column("role"),MaxLength(20)]
        public string Role { get; set; }
        [ForeignKey("account_id")]
        public WeChatAccount WeChatAccount { get; set; }
    }
}
