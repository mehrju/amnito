using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces;
using Nop.Services.Events;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class OrderCanceledEventConsumer : IConsumer<OrderCancelledEvent>
    {
        private readonly IDbContext _dbContext;
        private readonly IRewardPointService _rewardPointService;
        private readonly IAccountingService _accountingService;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly ISnappbox_Service _snappbox_Service;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;
        private readonly IChargeWalletFailService _chargeWalletFailService;

        public OrderCanceledEventConsumer(IDbContext dbContext
            , IRewardPointService rewardPointService
            , IAccountingService accountingService
            , IRepository<OrderNote> orderNoteRepository
            , ISnappbox_Service _snappbox_Service
            , IExtendedShipmentService extendedShipmentService
            , IOrderService orderService
            , IWorkContext workContext
            , IWebHelper webHelper
            , IChargeWalletFailService chargeWalletFailService)
        {
            _workContext = workContext;
            _webHelper = webHelper;
            _chargeWalletFailService = chargeWalletFailService;
            _orderNoteRepository = orderNoteRepository;
            _dbContext = dbContext;
            _rewardPointService = rewardPointService;
            _accountingService = accountingService;
            _extendedShipmentService = extendedShipmentService;
            _orderService = orderService;
        }
        public void HandleEvent(OrderCancelledEvent eventMessage)
        {
            if (eventMessage.Order.OrderItems.First().Product.ProductCategories.First().CategoryId == 717)
            {
                if (!_orderNoteRepository.Table.Any(p => p.OrderId == eventMessage.Order.Id && p.Note.Contains("SnapBoxCanceled")))
                {
                    if (eventMessage.Order.Shipments.All(p => p.ShippedDateUtc == null))
                    {
                        InsertOrderNote("SnapBoxCanceled", eventMessage.Order.Id);
                        if (!_orderNoteRepository.Table.Any(p => p.OrderId == eventMessage.Order.Id && p.Note.Contains("CancelFromSnap")))
                        {
                            string error = "";
                            _extendedShipmentService.CancelSappbox_Order(eventMessage.Order, out error);
                            InsertOrderNote(error, eventMessage.Order.Id);
                        }
                    }
                    else
                    {
                        InsertOrderNote(" مرسوله جمع آوری شده و امکان کنسل شدن آن وجود ندارد", eventMessage.Order.Id);
                        eventMessage.Order.OrderStatus = OrderStatus.Complete;
                        _orderService.UpdateOrder(eventMessage.Order);
                        return;
                    }
                }
            }
            if (eventMessage.Order.Customer.IsInCustomerRole("mini-Administrators"))
            {
                RecoverOutLineAgentAmount(eventMessage.Order);
            }
            RecoverInLineAgentAmount(eventMessage.Order);
            _accountingService.refoundAffilateCharge(eventMessage.Order);
        }
        private void RecoverInLineAgentAmount(Order order)
        {
            try
            {
                if (!_accountingService.HasChargeWallethistory(1, order.Id))
                    return;
                string query = $@"SELECT
	                                SUM(ISNULL(Rph.Points, TCWH.Point)) AS AgentAmountTotal
                                FROM
	                                dbo.Tb_ChargeWalletHistory AS TCWH 
	                                INNER JOIN dbo.[Order] AS O ON O.Id = TCWH.orderId
	                                LEFT JOIN dbo.RewardPointsHistory AS RPH ON Tcwh.rewaridPointHistoryId = Rph.Id
                                WHERE
	                                O.Deleted = 0
	                                AND TCWH.ChargeWalletTypeId IN (1,2)
                                    AND O.Id = {order.Id}";
                int AgentAmountTotal = _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
                
                if (AgentAmountTotal <= 0)
                    return;
                string str_msg = "";
                int rewardPointHistoryId = ChargeWalletForAgentSaleAmount(order, (AgentAmountTotal * -1), "", out str_msg);
                _accountingService.InsertChargeWallethistory(new Models.ChargeWalletHistoryModel()
                {
                    orderId = order.Id,
                    orderItemId = null,
                    rewaridPointHistoryId = rewardPointHistoryId,
                    AgentAmountRuleId = null,
                    shipmentId = null,
                    ChargeWalletTypeId = 2,
                    Description = "کسر مبلغ تخفیف نمایندگی به جهت کنسل شدن سفارش شماره " + order.Id,
                    Point = AgentAmountTotal * -1,
                    IpAddress = _webHelper.GetCurrentIpAddress(),
                    CreateDate = DateTime.Now
                });
                if (!string.IsNullOrEmpty(str_msg))
                    InsertOrderNote(str_msg, order.Id);
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                _chargeWalletFailService.InsertFailedLog(ex, new { orderId = order.Id }, "OrderCanceledEventConsumer.RecoverAgentAmountMoney -> کسر مبلغ تخفیف نمایندگی به جهت کنسل شدن سفارش شماره");
                throw;
            }

        }
        private void RecoverOutLineAgentAmount(Order order)
        {
            try
            {
                if (!_accountingService.HasChargeWallethistory(1, order.Id))
                    return;
                string query = $@"SELECT
	                                SUM(ISNULL(Rph.Points, TCWH.Point)) AS AgentAmountTotal
                                FROM
	                                dbo.Tb_ChargeWalletHistory AS TCWH 
	                                INNER JOIN dbo.[Order] AS O ON O.Id = TCWH.orderId
	                                LEFT JOIN dbo.RewardPointsHistory AS RPH ON Tcwh.rewaridPointHistoryId = Rph.Id
                                WHERE
	                                O.Deleted = 0
	                                AND TCWH.ChargeWalletTypeId IN (3,4)
                                    AND O.Id = {order.Id}";
                int AgentAmountTotal = _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
               
                if (AgentAmountTotal <= 0)
                    return;
                string str_msg = "";
                int rewardPointHistoryId = ChargeWalletForAgentSaleAmount(order, (AgentAmountTotal * -1), "کسر مبلغ خدمات نمایندگی به جهت کنسل شدن سفارش شماره", out str_msg);
                _accountingService.InsertChargeWallethistory(new Models.ChargeWalletHistoryModel()
                {
                    orderId = order.Id,
                    orderItemId = null,
                    rewaridPointHistoryId = rewardPointHistoryId,
                    AgentAmountRuleId = null,
                    shipmentId = null,
                    ChargeWalletTypeId = 4,
                    Description = "کسر مبلغ خدمات نمایندگی به جهت کنسل شدن سفارش شماره " + order.Id,
                    Point = AgentAmountTotal * -1,
                    IpAddress = _webHelper.GetCurrentIpAddress(),
                    CreateDate = DateTime.Now
                });
                if (!string.IsNullOrEmpty(str_msg))
                    InsertOrderNote(str_msg, order.Id);
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                _chargeWalletFailService.InsertFailedLog(ex, new { orderId = order.Id }, "OrderCanceledEventConsumer.RecoverAgentAmountMoney -> کسر مبلغ خدمات نمایندگی به جهت کنسل شدن سفارش شماره");
                throw;
            }
        }
        private int ChargeWalletForAgentSaleAmount(Order order, int price, string inputMsg, out string Msg)
        {
            string Message = (string.IsNullOrEmpty(inputMsg) ? ("کسر مبلغ تخفیف نمایندگی به جهت کنسل شدن سفارش شماره" + " " + order.Id) : inputMsg);
            //TODO : Reward point
            int rewardPointHistoryId = _rewardPointService.AddRewardPointsHistoryEntry(order.Customer, price,
                order.StoreId, Message, usedAmount: 0);
            if (rewardPointHistoryId > 0)
            {
                Msg = "کسر مبلغ تخفیف نمایندگی از کیف پول با موفقیت انجام شد";
                return rewardPointHistoryId;
            }
            Msg = "اشکال در کسر مبلغ تخفیف نمایندگی از کیف پول.لطفا با مدیر فنی تماس بگیرید";
            return 0;
        }
        private void InsertOrderNote(string note, int orderId)
        {
            OrderNote Onote = new OrderNote()
            {
                Note = note + "-" + _workContext.CurrentCustomer.Id.ToString(),
                CreatedOnUtc = DateTime.Now.ToUniversalTime(),
                DisplayToCustomer = false,
                OrderId = orderId
            };
            _orderNoteRepository.Insert(Onote);
        }

    }
}
