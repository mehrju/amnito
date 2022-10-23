using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Orders.MultiShipping.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public class OrderCostService : IOrderCostService
    {
        private readonly IRepository<OrderItem> _repository_OrderItem;
        private readonly IRepository<ShipmentItem> _repository_ShipmentItem;
        private readonly IDbContext _dbcontext;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IRepository<ShipmentAppointmentModel> _repository_ShipmentAppointment;
        private readonly IRepository<Shipment> _repository_Shipment;

        public OrderCostService(IRepository<OrderItem> repository_OrderItem,
            IRepository<ShipmentItem> repository_ShipmentItem,
            IDbContext dbcontext,
            IExtendedShipmentService extendedShipmentService,
            IRepository<ShipmentAppointmentModel> repository_ShipmentAppointment,
            IRepository<Shipment> repository_Shipment)
        {
            _repository_OrderItem = repository_OrderItem;
            _repository_ShipmentItem = repository_ShipmentItem;
            _dbcontext = dbcontext;
            _extendedShipmentService = extendedShipmentService;
            _repository_ShipmentAppointment = repository_ShipmentAppointment;
            _repository_Shipment = repository_Shipment;
        }

        public IEnumerable<OrderItemCostInfoViewModel> GetOrderItemsCost(int orderId = 0, int shipmentId = 0)
        {
            string query = @"SELECT * FROM [dbo].[Tb_OrderItemAttributeValue] ";
            if (orderId != 0)
            {
                var orderItemIds = _repository_OrderItem.TableNoTracking.Where(p => p.OrderId == orderId).Select(p => p.Id).ToList();
                query += $"WHERE [OrderItemId] IN ({string.Join(",", orderItemIds)})";
            }
            else
            {
                var orderItemId = _repository_ShipmentItem.TableNoTracking.Where(p => p.Id == shipmentId).Select(p => p.OrderItemId).FirstOrDefault();
                query += $"WHERE [OrderItemId] = {orderItemId}";
            }

            var data = _dbcontext.SqlQuery<OrderItemAttributeValue>(query).ToList();
            var result = new List<OrderItemCostInfoViewModel>();

            foreach (var item in data.Where(p => p.PropertyAttrValueCost.HasValue &&
                    p.PropertyAttrValuePrice.HasValue && (p.PropertyAttrValueCost.Value > 0 || p.PropertyAttrValuePrice.Value > 0)).GroupBy(p => p.PropertyAttrId))
            {
                var quantity = _dbcontext.SqlQuery<int>($"select sum(quantity) from [OrderItem] where Id in ({string.Join(", ", item.Select(p => p.OrderItemId).ToArray())})").FirstOrDefault();
                result.Add(new OrderItemCostInfoViewModel()
                {
                    Cost = item.Sum(p => p.PropertyAttrValueCost.Value * quantity),
                    Name = item.First().PropertyAttrName,
                    Price = item.Sum(p => p.PropertyAttrValuePrice.Value * quantity),
                    AttrId = item.Key
                });
            }

            if (orderId != 0)
            {
                var product = _repository_OrderItem.TableNoTracking.FirstOrDefault(p => p.OrderId == orderId).Product;
                var catInfo = _extendedShipmentService.GetCategoryInfo(product);
                if (catInfo.CategoryId == 722 || catInfo.CategoryId == 723)
                {
                    OrderItemCostInfoViewModel itemCost = result.FirstOrDefault(p => p.AttrId == 115);
                    if (itemCost == null)
                    {
                        itemCost = new OrderItemCostInfoViewModel();
                        result.Add(itemCost);
                    }

                    var shipments = _repository_Shipment.TableNoTracking.Where(p => p.OrderId == orderId).ToList();
                    var shipmentIds = shipments.Select(p => p.Id).ToArray();
                    var appointments = _repository_ShipmentAppointment.TableNoTracking.Where(p => shipmentIds.Contains(p.ShipmentId)).ToList();
                    int totalPrice = 0, totalCost = 0;
                    foreach (var item in appointments)
                    {
                        if (item.CodCost.HasValue)
                        {
                            var tmp = item.CodCost.Value + (item.CodCost.Value * 9 / 100);
                            totalPrice += tmp + (tmp * 20 / 100);//33
                            totalCost += tmp; 
                        }
                    }
                    itemCost.Price = totalPrice;
                    itemCost.Cost = totalCost;
                }
            }


            //if (data.Any(p => p.WeightCategoryCost.HasValue && p.WeightCategoryCost.Value > 0 &&
            //    p.WeightCategoryPrice.HasValue && p.WeightCategoryPrice.Value > 0))
            //{
            //    result.Add(new OrderItemCostInfoViewModel()
            //    {
            //        Cost = data.Where(p => p.WeightCategoryCost.HasValue).Sum(p => p.WeightCategoryCost.Value),
            //        Name = "WeightCategory",
            //        Price = data.Where(p => p.WeightCategoryPrice.HasValue).Sum(p => p.WeightCategoryPrice.Value)
            //    });
            //}
            //if (data.Any(p => p.InsuranceCost.HasValue && p.InsuranceCost.Value > 0 &&
            //    p.InsurancePrice.HasValue && p.InsurancePrice.Value > 0))
            //{
            //    result.Add(new OrderItemCostInfoViewModel()
            //    {
            //        Cost = data.Where(p => p.InsuranceCost.HasValue).Sum(p => p.InsuranceCost.Value),
            //        Name = "بیمه",
            //        Price = data.Where(p => p.InsurancePrice.HasValue).Sum(p => p.InsurancePrice.Value)
            //    });
            //}
            //if (data.Any(p => p.AccessPrinterCost.HasValue && p.AccessPrinterCost.Value > 0 &&
            //    p.AccessPrinterPrice.HasValue && p.AccessPrinterPrice.Value > 0))
            //{
            //    result.Add(new OrderItemCostInfoViewModel()
            //    {
            //        Cost = data.Where(p => p.AccessPrinterCost.HasValue).Sum(p => p.AccessPrinterCost.Value),
            //        Name = "AccessPrinter",
            //        Price = data.Where(p => p.AccessPrinterPrice.HasValue).Sum(p => p.AccessPrinterPrice.Value)
            //    });
            //}
            //if (data.Any(p => p.CartonCost.HasValue && p.CartonCost.Value > 0 &&
            //    p.CartonPrice.HasValue && p.CartonPrice.Value > 0))
            //{
            //    result.Add(new OrderItemCostInfoViewModel()
            //    {
            //        Cost = data.Where(p => p.CartonCost.HasValue).Sum(p => p.CartonCost.Value),
            //        Name = "بسته بندی",
            //        Price = data.Where(p => p.CartonPrice.HasValue).Sum(p => p.CartonPrice.Value)
            //    });
            //}
            //if (data.Any(p => p.PrintLogoCost.HasValue && p.PrintLogoCost.Value > 0 &&
            //    p.PrintLogoPrice.HasValue && p.PrintLogoPrice.Value > 0))
            //{
            //    result.Add(new OrderItemCostInfoViewModel()
            //    {
            //        Cost = data.Where(p => p.PrintLogoCost.HasValue).Sum(p => p.PrintLogoCost.Value),
            //        Name = "پرینت لوگو",
            //        Price = data.Where(p => p.PrintLogoPrice.HasValue).Sum(p => p.PrintLogoPrice.Value)
            //    });
            //}
            //if (data.Any(p => p.SmsCost.HasValue && p.SmsCost.Value > 0 &&
            //   p.SmsPrice.HasValue && p.SmsPrice.Value > 0))
            //{
            //    result.Add(new OrderItemCostInfoViewModel()
            //    {
            //        Cost = data.Where(p => p.SmsCost.HasValue).Sum(p => p.SmsCost.Value),
            //        Name = "ارسال پیامک",
            //        Price = data.Where(p => p.SmsPrice.HasValue).Sum(p => p.SmsPrice.Value)
            //    });
            //}

            //if (data.Any(p => p.CompulsoryInsuranceCost.HasValue && p.CompulsoryInsuranceCost.Value > 0 &&
            //  p.CompulsoryInsurancePrice.HasValue && p.CompulsoryInsurancePrice.Value > 0))
            //{
            //    result.Add(new OrderItemCostInfoViewModel()
            //    {
            //        Cost = data.Where(p => p.CompulsoryInsuranceCost.HasValue).Sum(p => p.CompulsoryInsuranceCost.Value),
            //        Name = "CompulsoryInsurance",
            //        Price = data.Where(p => p.CompulsoryInsurancePrice.HasValue).Sum(p => p.CompulsoryInsurancePrice.Value)
            //    });
            //}

            return result;

        }
    }
}
