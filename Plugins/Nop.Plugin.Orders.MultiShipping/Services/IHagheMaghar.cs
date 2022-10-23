using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Services.Payments;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public interface IHagheMaghar
    {
        int getHagheMagharByCountryId(int countryId);
        void Insert(int orderItemId, int HagheMagharPrice, int ShipmentHagheMaghar);
        int getHaghMaghrInOrder(int customerId, OrderTotalsModel model);
        int getInsertedHagheMaghar(Order order);
    }
}
