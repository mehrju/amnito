using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NopFarsi.Payments.SepShaparak.Models
{
    public class PayModel
    {
        public string Amount { get; set; }

        public string MerchantId { get; set; }

        public string ResNum { get; set; }

        public string RedirectUrl { get; set; }
        public string Token { get; set; }
    }
}
