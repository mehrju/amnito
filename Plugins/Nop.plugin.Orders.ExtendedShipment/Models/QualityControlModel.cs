using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class QualityControlModel
    {
        public string CreatedOnUtc { get; set; }
        public string CoordinationDate { get; set; }
        public string DataCollect { get; set; }
        public string delayDataCollect { get; set; }
        public string ShippedDate { get; set; }
        public string delayShippedDate { get; set; }
        public string DeliveryDate { get; set; }
        public string SalDate { get; set; }
        public string SlaDelay { get; set; }
        public int OrderId { get; set; }
        public string TrackingNumber { get; set; }
        public string LastEvent { get; set; }
        public string fullName { get; set; }
        public string PhoneNumber { get; set; }
        public string IndemnityBySla { get; set; }
        public string ItemWeight { get; set; }
        public string catgoryName { get; set; }
        public string PostSupervisorInfo { get; set; }
        public int ShipmentId { get; set; }

    }

    public class QualityControlInputModel
    {
        public QualityControlInputModel()
        {
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            AvailableOrderState =new List<SelectListItem>()
            {
                new SelectListItem(){Text = "انتخاب کنید",Value = "0"},
                new SelectListItem(){Text = "هماهنگ نشده",Value = "1"},
                new SelectListItem(){Text = "جمع آوری نشده",Value = "2"},
                new SelectListItem(){Text = "ارسال نشده",Value = "3"},
                new SelectListItem(){Text = "تحویل نشده",Value = "4"},
                new SelectListItem(){Text = "شامل غرامت",Value = "5"}
            };
        }
        [DisplayName("شماره سفارش")]
        public int orderId { get; set; }
        [DisplayName("کد رهگیری")]
        public string trackingNumber { get; set; }
        [DisplayName("استان فرستنده")]
        public int countryId { get; set; }
        [DisplayName("شهرستان فرستنه")]
        public int stateId { get; set; }
        [DisplayName("وضعیت محموله")]
        public int orderState { get; set; }
        [DisplayName("تاریخ سفارش از")]
        [UIHint("DateNullable")]
        public DateTime? dateFrom { get; set; }
        [DisplayName("تاریخ سفارش تا")]
        [UIHint("DateNullable")]
        public DateTime? dateTo { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }
        public IList<SelectListItem> AvailableOrderState { get; set; }
        


    }
}
