using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
   public class Tbl_TrackingUbaarOrder : BaseEntity
    {
        [ScaffoldColumn(false)]
        public int OrderId { get; set; }
        [ScaffoldColumn(false)]
        public int OrderItemId { get; set; }
        [ScaffoldColumn(false)]
        // 1 ویرایش قیمت
        //2 تایید مشتری
        //3 تایید راننده
        public int Status { get; set; }
        [ScaffoldColumn(false)]
        public string Description { get; set; }
        [ScaffoldColumn(false)]
        public string IP { get; set; }
        [ScaffoldColumn(false)]
        public int price { get; set; }
        [ScaffoldColumn(false)]
        public int newprice { get; set; }
        [ScaffoldColumn(false)]
        public bool IsPay { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateInsert { get; set; }
        [ScaffoldColumn(false)]
        public int IdUserInsert { get; set; }
    }
}
