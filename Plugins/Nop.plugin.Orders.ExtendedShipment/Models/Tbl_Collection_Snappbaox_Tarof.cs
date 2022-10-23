using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class Tbl_Collection_Snappbaox_Tarof : BaseEntity
    {
        [ScaffoldColumn(false)]
        public int Id_Request { get; set; }

        //0 inquery  1 snap  2 tarof
        [ScaffoldColumn(false)]
        public int TypeRequest { get; set; }

        [ScaffoldColumn(false)]
        public int ShipmentId { get; set; }


        [ScaffoldColumn(false)]
        public DateTime DateInsert { get; set; }

        [ScaffoldColumn(false)]
        public int UserIdInsert { get; set; }

        //**********************
        public int orderId { get; set; }
        [ScaffoldColumn(false)]
        //0 خطا
        // 1 ثبت شده
        // 2 استعلام قیمت
        //3 درخواست شده
        //4 قبول شده
        //5 کنسل شده
        //6 برداشت شده
        //7 رسیده
        //8 تحویل شده
       
        public int Status { get; set; }


        [ScaffoldColumn(false)]
        public DateTime Date_Statuse { get; set; }

        [ScaffoldColumn(false)]
        public double Mablagh_Induery { get; set; }
        //**********  اسنپ حالا مقادیر برگشتی از سرویس

        [ScaffoldColumn(false)]
        public string Snapp_bikerName { get; set; }
        [ScaffoldColumn(false)]
        public string Snapp_bikerPhone { get; set; }
        [ScaffoldColumn(false)]
        public double Snapp_distance { get; set; }
        [ScaffoldColumn(false)]
        public int Snapp_eta { get; set; }
        [ScaffoldColumn(false)]
        public string Snapp_orderAcceptedAt { get; set; }


        public string Description_log { get; set; }

    }
}
