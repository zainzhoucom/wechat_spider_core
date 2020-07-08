using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wechat_spider_core.ef
{
    [Table("wechat_account")]
    public class WeChatAccount
    {
        [Column("id"),Key, Required]
        public long Id { get; set; }
        [Column("nick_name"), Required, MaxLength(128)]
        public string NickName { get; set; }
        [Column("alias"), MaxLength(128)]
        public string Alias { get; set; }
        [Column("fackid"), MaxLength(128)]
        public string FakeId { get; set; }
        [Column("round_head_img"), MaxLength(500)]
        public string RoundHeadImg { get; set; }
        [Column("service_type")]
        public int ServiceType { get; set; }
        [Column("last_update_time")]
        public DateTime? LastUpdate { get; set; }

        [ForeignKey("task_sign_id")]
        public TaskStartSign TaskStartSign { get; set; }

        public List<SpiderRole> SpiderRoles { get; set; }

    }
}
