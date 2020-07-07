using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace wechat_spider_core.ef
{
    [Table("task_start_sign")]
    public class TaskStartSign
    {
        [Key,Column("id"),Required,MaxLength(128)]
        public string Id { get; set; }
        [Column("start_date"),Required,MaxLength]
        public string StartDate { get; set; }
    }
}
