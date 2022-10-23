using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Events
{
    public class OrderPlacedEventConsumer : IConsumer<OrderPlacedEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly ICartonService _cartonPriceService;

        public OrderPlacedEventConsumer(IDbContext dbContext,
            IExtendedShipmentService extendedShipmentService,
            ICartonService cartonPriceService)
        {
            _dbContext = dbContext;
            _extendedShipmentService = extendedShipmentService;
            _cartonPriceService = cartonPriceService;
        }
        public void HandleEvent(OrderPlacedEvent eventMessage)
        {
            AddSize9Prices(eventMessage);
        }

        private void AddSize9Prices(OrderPlacedEvent eventMessage)
        {
            //بدست آوردن مواردی که از بین آیتم ها سایز 9 دارند
            string query = $"SELECT OrderItemId FROM Tb_OrderItemAttributeValue WHERE OrderItemId IN ({string.Join(", ", eventMessage.Order.OrderItems.Select(p => p.Id).ToArray())}) AND PropertyAttrValueName = N'سایر(بزرگتر از سایز 9)'";
            var orderItemsWithSize9 = _dbContext.SqlQuery<int>(query).ToList();
            var newTotalPrice = 0;
            foreach (var item in eventMessage.Order.OrderItems.Where(p => orderItemsWithSize9.Contains(p.Id)))
            {
                var diminsions = _extendedShipmentService.getDimensions(item);
                var sizeCost = _cartonPriceService.CalculateSize9Price(diminsions.Item1, diminsions.Item2, diminsions.Item3);
                //30% به مبلغ cost اضافه میشود
                var sizePrice = (sizeCost * 30 / 100) + sizeCost;
                _dbContext.ExecuteSqlCommand($"UPDATE [Tb_OrderItemsRecord] SET [CartonCost] = '{sizeCost}', [CartonPrice] = '{sizePrice}' WHERE [OrderItemId] = {item.Id}");
                _dbContext.ExecuteSqlCommand($"UPDATE [Tb_OrderItemAttributeValue] SET [PropertyAttrValueCost] = {sizeCost}, [PropertyAttrValuePrice] = '{sizePrice}' WHERE [OrderItemId] = {item.Id} AND [PropertyAttrName] like N'%کارتن و لفاف بندی%'");
                var totalPrice = sizePrice * item.Quantity;
                // 9% مالیات
                //var finalItemPrice = (totalPrice * 9 / 100) + totalPrice;
                newTotalPrice += totalPrice;
            }
            if (newTotalPrice != 0)
            {
                eventMessage.Order.OrderTotal += newTotalPrice;
                _dbContext.ExecuteSqlCommand($"UPDATE [Order] SET [OrderTotal] = {eventMessage.Order.OrderTotal} WHERE Id = {eventMessage.Order.Id}");
            }
        }
    }
}
