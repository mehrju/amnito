using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.plugin.Orders.ExtendedShipment.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public static class OrderExtension
    {
        public static bool IsOrderCod(this Order order)
        {
            var _extendedShipmentService = EngineContext.Current.Resolve<IExtendedShipmentService>();
            var categories = order.OrderItems.SelectMany(p => p.Product.ProductCategories).Select(p => p.CategoryId).Distinct();
            foreach (var item in categories)
            {
                var data = _extendedShipmentService.GetCategoryInfo(item);
                if (data != null && data.IsCod)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool IsOrderForeign(this Order order)
        {
            var _extendedShipmentService = EngineContext.Current.Resolve<IExtendedShipmentService>();
            var categories = order.OrderItems.SelectMany(p => p.Product.ProductCategories).Select(p => p.CategoryId).Distinct();
            foreach (var item in categories)
            {
                var data = _extendedShipmentService.GetCategoryInfo(item);
                if (data != null && data.IsForeign)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool canPayForgin(this Order order)
        {

            var TrackingForeignOrder = EngineContext.Current.Resolve<IRepository<Nop.plugin.Orders.ExtendedShipment.Models.Tbl_TrackingForeignOrder>>();
            int status= (TrackingForeignOrder.Table.Where(p=> p.OrderId == order.Id).OrderByDescending(p=> p.Id).FirstOrDefault()?.Status).GetValueOrDefault(0);
            if(status==2 || status==3  || status==4)
                return true;
            return false;
        }
    }
}
