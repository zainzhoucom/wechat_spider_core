using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace wechat_spider_core.ef
{
    [Table("wechat_article")]
    public class WeChatArticle
    {
        [Column("id"), Key, Required, MaxLength(128)]
        public string Id { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }
        [Column("hometownid"), MaxLength(50)]
        public string Homeownid { get; set; }
        [Column("download"),DefaultValue(false)]
        public bool Download { get; set; }
        [Column("local_path"), MaxLength(255)]
        public string LocalPath { get; set; }
        public string aid { get; set; }
        public string album_id { get; set; }
        public long appmsgid { get; set; }
        public int checking { get; set; }
        public int copyright_type { get; set; }
        public string cover { get; set; }
        public DateTime create_time { get; set; }
        public string digest { get; set; }
        public int has_red_packet_cover { get; set; }
        public int is_original { get; set; }
        public int is_pay_subscribe { get; set; }
        public int item_show_type { get; set; }
        public int itemidx { get; set; }
        public string link { get; set; }
        public string media_duration { get; set; }
        public int mediaapi_publish_status { get; set; }
        public string tagid { get; set; }
        public string title { get; set; }
        public DateTime update_time { get; set; }
        [ForeignKey("account_id")]
        public WeChatAccount WeChatAccount { get; set; }
    }
}
