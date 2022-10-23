using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models.Tbl_Extra_Status_Field_Shipment
{
    public class TblExtraFiledShipment : BaseEntity
    {
        public TblExtraFiledShipment()
        {

        }
        [Required]
        [ScaffoldColumn(false)]
        public int ShippingId { get; set; }

        //1 مفقود شده 
        //2 غرامت خسارت
        //3 غرامت مفقودی
        //4 غرامت تاخیر
        //5 شکایت
        [Required]
        [ScaffoldColumn(false)]
        public int Type { get; set; }


        //1 دارد
        //2 ندارد
        [Required]
        [ScaffoldColumn(false)]
        public int value { get; set; }

        [Required]
        [ScaffoldColumn(false)]
        public int OrderNoteId { get; set; }


        [ScaffoldColumn(false)]
        public DateTime DateInsert { get; set; }

        [ScaffoldColumn(false)]
        public int IdUserInsert { get; set; }
    }
}
