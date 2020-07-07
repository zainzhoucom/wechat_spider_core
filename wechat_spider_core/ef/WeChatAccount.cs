using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace wechat_spider_core.ef
{
    [Table("wechat_account")]
    public class WeChatAccount
    {
        [Column("id"),Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
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
        [Column("hometownid"), MaxLength(50)]
        public string Homeownid { get; set; }
        [Column("spider_role"), MaxLength(20)]
        public string SpiderRole { get; set; }
        [Column("spider_role1"), MaxLength(20)]
        public string SpiderRole1 { get; set; }
        [Column("spider_role2"), MaxLength(20)]
        public string SpiderRole2 { get; set; }
        [Column("spider_role3"), MaxLength(20)]
        public string SpiderRole3 { get; set; }
        [Column("spider_role4"), MaxLength(20)]
        public string SpiderRole4 { get; set; }
        [Column("spider_role5"), MaxLength(20)]
        public string SpiderRole5 { get; set; }

    }
}
