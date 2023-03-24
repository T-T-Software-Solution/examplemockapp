using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace App.Domain
{
    public class BaseEntity<T>: IBaseEntity<T>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public BaseEntity()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

        }

        [Column(Order = 1), Comment("คีร์ของข้อมูล")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public T id { get; set; }

        [Column(Order = 200), Comment("เวลาสร้าง")]
        public DateTime? created { get; set; }
        [Column(Order = 201), Comment("เวลาปรับปรุงล่าสุด")]
        public DateTime? updated { get; set; }
        [Column(Order = 202), Comment("ใช้งานได้หรือไม่")]
        public bool? isActive { get; set; }
    }
}
