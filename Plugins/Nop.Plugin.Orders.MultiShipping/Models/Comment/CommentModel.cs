using Nop.Plugin.Orders.MultiShipping.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models.Comment
{
    public class CommentModel
    {
        public string TrackingCode { get; set; }
        public string Description { get; set; }
        public CommentRateEnum Rate { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }

        public string IsValid()
        {
            if (string.IsNullOrEmpty(TrackingCode))
            {
                return "لطفا شماره رهگیری را وارد کنید";
            }
            if (string.IsNullOrEmpty(Description))
            {
                return "لطفا توضیحات را وارد کنید";
            }
            return string.Empty;
        }
    }
}
