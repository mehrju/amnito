using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PrintedReports_Requirements.Models
{
    public class vm_SaleCartonWrapper1
    {
        public ProductDetailsModel ProductDetailsModel { get; set; }
        public Nop.Web.Models.Checkout.CheckoutPaymentMethodModel PaymentMethods { get; set; }
    }
}
