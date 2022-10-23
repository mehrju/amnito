using Nop.Web.Models.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class vm_SaleCartonWrapper
    {
        public Listitem[] Listitem { get; set; }
        public CheckoutPaymentMethodModel PaymentMethods { get; set; }
    }
}
