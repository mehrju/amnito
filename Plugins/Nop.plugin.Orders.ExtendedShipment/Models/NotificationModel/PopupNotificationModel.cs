using Newtonsoft.Json;
using Nop.plugin.Orders.ExtendedShipment.JsonConvertors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.NotificationModel
{
    public class PopupNotificationModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? FromDate { get; set; }
        public string FromDatePersian
        {
            get
            {
                return FromDate.HasValue ? FromDate.Value.ToPersianDate() : null;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    FromDate = null;
                }
                else
                {
                    FromDate = value.FromPersianToGregorianDate();
                }
                //FromDate = (DateTime)(string.IsNullOrEmpty(value) ? null : (object)value.FromPersianToGregorianDate());
            }
        }
        public DateTime? ToDate { get; set; }
        public string ToDatePersian
        {
            get
            {
                return ToDate.HasValue ? ToDate.Value.ToPersianDate() : null;
            }
            set
            {
                if(string.IsNullOrEmpty(value))
                {
                    ToDate = null;
                }
                else
                {
                    ToDate = value.FromPersianToGregorianDate();
                }
                //ToDate = (DateTime)(string.IsNullOrEmpty(value) ? null : (object)value.FromPersianToGregorianDate());
            }
        }
        public bool IsActive { get; set; }
        public string IsActiveString
        {
            get
            {
                return IsActive ? "فعال" : "غیر فعال";
            }
        }

        public string Content { get; set; }
        public string[] StoreIds { get; set; }
    }
}
