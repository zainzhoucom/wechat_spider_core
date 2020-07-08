using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace wechat_spider_core.ef
{
    [Table("task_start_sign")]
    public class TaskStartSign
    {
        [Key,Column("id"),Required]
        public long Id { get; set; }
        [Column("client_id"),Required]
        public long ClientId { get; set; }
        [Column("start_date"),Required,MaxLength]
        public DateTime StartDate { get; set; }
        [Column("run_status"),Required]
        public bool RunStatus { get; set; }
    }
}
