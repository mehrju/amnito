using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using System;
using System.Linq;
using static Nop.plugin.Orders.ExtendedShipment.Services.AgentAmountRuleService;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{

    public class OrderCompletedEventConsumer : IConsumer<EntityUpdated<Order>>
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IOrderStatusTrackingService _orderStatusTrackingService;
        private readonly INotificationService _notificationService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IOrderService _orderService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IDbContext _dbContext;
        private readonly IStoreContext _storeContext;


        public OrderCompletedEventConsumer(IRepository<Order> orderRepository,
            IExtendedShipmentService extendedShipmentService
            , IOrderStatusTrackingService orderStatusTrackingService
            , INotificationService notificationService
            , IProductAttributeParser productAttributeParser
            , IOrderService orderService
            , ILocalizationService localizationService
            , IWorkContext workContext
            , IDbContext dbContext
            , ICustomerActivityService customerActivityService
            , IStoreContext storeContext

            )
        {
            _storeContext = storeContext;
            _customerActivityService = customerActivityService;
            _dbContext = dbContext;
            _orderService = orderService;
            _localizationService = localizationService;
            _workContext = workContext;
            this._orderRepository = orderRepository;
            this._extendedShipmentService = extendedShipmentService;
            _orderStatusTrackingService = orderStatusTrackingService;
            _notificationService = notificationService;
            _productAttributeParser = productAttributeParser;

        }
        public void HandleEvent(EntityUpdated<Order> eventMessage)
        {

            var order = eventMessage?.Entity;
            if (order == null)
                return;
            if (order.OrderStatus != OrderStatus.Pending && order.PaymentStatus == Core.Domain.Payments.PaymentStatus.Paid && order.OrderNotes.Any(x => x.Note == "PaymentHandled"))
            {
                order.OrderStatus = OrderStatus.Pending;
                _orderService.UpdateOrder(order);
                return;
            }
            _orderStatusTrackingService.Insert(order);
            bool isWallet = order.OrderItems.Any(p => p.Id == 10277);
            if (isWallet)
                return;
            if (order.OrderStatus == OrderStatus.Cancelled &&
                !order.OrderNotes.Any(p => p.Note.Contains("SendSmsForCancel") && order.OrderStatus != OrderStatus.Pending))
            {
                _notificationService.NotifyForCancel(order.Id, _extendedShipmentService);
            }
            var isFirstOrder = false;// _orderStatusTrackingService.IsFirstOrder(order.Id);
            bool isBulkOrder = IsBulkOrder(order.Id);
            var mark = _extendedShipmentService.GetOrderRegistrationMethod(order);
            if ((!isFirstOrder && (order.OrderStatus == OrderStatus.Processing || order.OrderStatus == OrderStatus.Complete))
                || ((isFirstOrder) && order.OrderStatus == OrderStatus.Complete))
            {
                var cat = order.OrderItems.First().Product.ProductCategories.Where(p => p.Category.ParentCategoryId != 0).FirstOrDefault();

                if (cat == null)
                {
                    return;
                }

                int storeId = _storeContext.CurrentStore.Id;
                String ERROR = "";
                if (order.PaymentMethodSystemName == "Payments.CashOnDelivery")
                {
                    if (!order.OrderNotes.Any(p => p.Note.Contains("Send Data TO COD")))
                    {
                        //_extendedShipmentService.InsertOrderNote("Send Data TO COD", order.Id);
                        if (new int[] { 705, 706 }.Contains(cat.CategoryId))
                        {
                            ERROR = _extendedShipmentService.RegisterDTS_Order(order).Result;

                        }
                        else if (new int[] { 711 }.Contains(cat.CategoryId))
                        {
                            ERROR = _extendedShipmentService.registerTPGOrder(order).Result;

                        }
                        else if (new int[] { 713, 715 }.Contains(cat.CategoryId))
                        {
                            ERROR = _extendedShipmentService.RegisterChapar_Order(order).Result;

                        }
                        else if (new int[] { 667, 670 }.Contains(cat.CategoryId))
                        {
                            _extendedShipmentService.SendToCod(order, true);
                            if (CanComplete(order))
                                CompleteOrder(order);
                        }
                        else if (cat.CategoryId == 731)
                        {
                            ERROR = _extendedShipmentService.RegisterMahex_Order(order).Result;
                        }
                        _notificationService.SendSmsForPlaceOrder(order);
                    }
                }
                else
                {
                    if (!order.OrderNotes.Any(p => p.Note.Contains("SendDataToPost")))
                    {
                        //if ((!order.Customer.IsInCustomerRole("TwoStepOrder") || (order.Customer.IsInCustomerRole("TwoStepOrder") && order.CustomerId != _workContext.CurrentCustomer.Id))
                        //    && ((mark != OrderRegistrationMethod.NewUi) || (mark == OrderRegistrationMethod.NewUi) && order.CustomerId != _workContext.CurrentCustomer.Id))
                        {
                            if (new int[] { 703, 699 }.Contains(cat.CategoryId))
                            {
                                ERROR = _extendedShipmentService.RegisterDTS_Order(order).Result;

                            }
                            else if (cat.CategoryId == 730)
                            {
                                _extendedShipmentService.RegisterMahex_Order(order);
                            }
                            else if (_extendedShipmentService.IsPostService(cat.CategoryId) && mark != OrderRegistrationMethod.PhoneOrder
                                && ((mark == OrderRegistrationMethod.NewUi) || !order.Customer.IsInCustomerRole("TwoStepOrder") || (order.Customer.IsInCustomerRole("TwoStepOrder") && order.CustomerId != _workContext.CurrentCustomer.Id)))
                            {
                                //if (order.Customer.Id == 273739)
                                //    return;
                                if (new int[] { 722, 723 }.Contains(cat.CategoryId))
                                    _extendedShipmentService.SendToCod(order, false);
                                else
                                    _extendedShipmentService.ProcessOrderForPost(order);
                            }
                            else if (cat.CategoryId == 708)// PDE Domestic
                            {
                                string Error = "";
                                ERROR = _extendedShipmentService.RegisterDomesticPDEOrder(order).Result;

                            }
                            else if (cat.CategoryId == 707)// PDE International
                            {
                                string Error = "";
                                ERROR = _extendedShipmentService.RegisterInternationalPDEOrder(order).Result;

                            }
                            else if (cat.CategoryId == 702)// yarbox
                            {
                                string Error = "";
                                ERROR = _extendedShipmentService.registerYarboxOrder(order).Result;

                            }
                            else if (new int[] { 709, 710 }.Contains(cat.CategoryId))// TPG Domestic & International
                            {
                                string Error = "";
                                ERROR = _extendedShipmentService.registerTPGOrder(order).Result;

                            }
                            else if (new int[] { 701 }.Contains(cat.CategoryId))
                            {
                                string Error = "";
                                ERROR = _extendedShipmentService.RegisterUbbarOrder(order).Result;

                            }
                            else if (new int[] { 712, 714 }.Contains(cat.CategoryId))
                            {
                                string Error = "";
                                ERROR = _extendedShipmentService.RegisterChapar_Order(order).Result;

                            }
                            else if (cat.CategoryId == 719)// sky blue
                            {
                                string Error = "";
                                ERROR = _extendedShipmentService.RegisterBlueSky_Order(order).Result;

                            }
                            else if (cat.CategoryId == 717)//snapbox
                            {
                                string Error = "";
                                ERROR = _extendedShipmentService.RegisterSnappbox_Order(order).Result;

                            }

                            else if (cat.CategoryId == 718)//peykhub
                            {
                                string Error = "";
                                ERROR = _extendedShipmentService.Registerpeykhub_Order(order).Result;

                            }
                            else if (cat.CategoryId == 733)//kalaresan
                            {
                                string Error = "";
                                ERROR = _extendedShipmentService.RegisterKalaresan(order).Result;

                            }
                            //else if (new int[] { 722, 723 }.Contains(cat.CategoryId))
                            //{
                            //    _extendedShipmentService.SendToCod(order, false);
                            //}
                            else if (cat.CategoryId == 724)
                            {
                                _extendedShipmentService.ProcessPhoneOrder(order);
                            }
                            if (CanComplete(order))
                                CompleteOrder(order);
                        }
                        _notificationService.SendSmsForPlaceOrder(order);
                    }
                }
            }
        }
        private bool IsBulkOrder(int orderId)
        {
            string Query = $@"SELECT
	                            TOP(1) CAST(1 AS BIT)
                            FROM
	                            dbo.BulkOrder AS BO
                            WHERE
	                            Bo.OrderId = {orderId}
	                            OR Bo.OrderIds LIKE N'{orderId},%'
	                            OR Bo.OrderIds LIKE N'%,{orderId},%'
	                            OR Bo.OrderIds LIKE N'%,{orderId}'";
            return _dbContext.SqlQuery<bool?>(Query, new object[0]).FirstOrDefault().GetValueOrDefault(false);
        }
        private bool CanComplete(Order order)
        {
            if (order.OrderStatus == OrderStatus.Complete)
                return false;
            if (order.PaymentMethodSystemName == "Payments.CashOnDelivery")
            {
                if (order.Shipments.All(p => !string.IsNullOrEmpty(p.TrackingNumber)))
                    return true;
            }
            else
            {
                string query = $@"SELECT
	                                TOP(1) CAST(1 AS BIT)
                                FROM
	                                dbo.[Order] AS O
                                    INNER JOIN dbo.Shipment AS S ON S.OrderId = O.Id
	                                INNER JOIN dbo.Customer AS C ON C.Id = O.CustomerId
                                WHERE
	                                O.Deleted = 0
	                                AND o.OrderStatusId = 30
	                                AND C.Id = {order.CustomerId}
	                                AND O.Id <> {order.Id}
	                                AND CAST(O.CreatedOnUtc AS DATE) =  CAST('{order.CreatedOnUtc.Date}' as Date)
	                                AND DATEPART(HOUR,GETDATE()) > 12";
                bool canComplete = _dbContext.SqlQuery<bool?>(query, new object[0]).SingleOrDefault().GetValueOrDefault(false);
                return canComplete;
            }
            return false;
        }
        private void CompleteOrder(Order order)
        {
            try
            {
                order.OrderStatusId = 30;
                _orderService.UpdateOrder(order);
                //add a note
                order.OrderNotes.Add(new OrderNote
                {
                    Note = $"وضعیت سفارش ویرایش شد. وضعیت جدید : {OrderStatus.Complete.GetLocalizedEnum(_localizationService, _workContext)}",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _orderService.UpdateOrder(order);
                LogEditOrder(order.Id);
            }
            catch (Exception ex)
            {
                _extendedShipmentService.Log("خطا در زمان تکمیل کردن سفارش" + order.Id, ex.Message + ex.InnerException != null ? "-->" + ex.InnerException.Message : "");
            }
        }
        private void LogEditOrder(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);

            _customerActivityService.InsertActivity("EditOrder", _localizationService.GetResource("ActivityLog.EditOrder"), order.CustomOrderNumber);
        }
    }

    public class OrderPaidEventConsumer : IConsumer<OrderPaidEvent>
    {
        #region field
        private readonly IRewardPointService _rewardPointService;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IDbContext _dbContext;
        private readonly IAccountingService _accountingService;
        private readonly IAgentAmountRuleService _agentAmountRuleService;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly INotificationService _notificationService;
        private readonly IWorkContext _workContext;
        private readonly IChargeWalletFailService _chargeWalletFailService;
        private readonly IWebHelper _webHelper;
        private readonly IRewardPointCashoutService _rewardPointCashoutService;

        #endregion

        #region ctor
        public OrderPaidEventConsumer(IProductAttributeParser productAttributeParser,
      IRewardPointService rewardPointService, IRepository<OrderNote> orderNoteRepository,
      IGenericAttributeService genericAttributeService,
      IDbContext dbContext,
      IAccountingService accountingService,
      IAgentAmountRuleService agentAmountRuleService,
      IExtendedShipmentService extendedShipmentService,
      INotificationService notificationService,
      IWorkContext workContext,
      IChargeWalletFailService chargeWalletFailService,
      IRewardPointCashoutService rewardPointCashoutService,
      IWebHelper webHelper)
        {
            _workContext = workContext;
            _webHelper = webHelper;
            _chargeWalletFailService = chargeWalletFailService;
            _extendedShipmentService = extendedShipmentService;
            _agentAmountRuleService = agentAmountRuleService;
            _dbContext = dbContext;
            _genericAttributeService = genericAttributeService;
            _rewardPointService = rewardPointService;
            _productAttributeParser = productAttributeParser;
            _orderNoteRepository = orderNoteRepository;
            _accountingService = accountingService;
            _notificationService = notificationService;
            _rewardPointCashoutService = rewardPointCashoutService;
        }
        #endregion

        #region fn&Event
        public void HandleEvent(OrderPaidEvent eventMessage)
        {
            try
            {
                if (_extendedShipmentService.IsWallet(eventMessage.Order))
                    return;

                if (_extendedShipmentService.IsSafeBuy(eventMessage.Order.Id))
                {
                    var Address = _extendedShipmentService.getAddressFromShipment(eventMessage.Order.Shipments.First().Id);

                    var ApproximateValue = 0;
                    foreach (var item in eventMessage.Order.OrderItems)
                    {
                        ApproximateValue += _extendedShipmentService.getApproximateValue(item.Id) * item.Quantity;
                    }

                    _notificationService._sendSms(Address.PhoneNumber, $@"فروشنده/ فرستنده محترم
|
سفارش شماره
{eventMessage.Order.Id}
از سوی
{eventMessage.Order.ShippingAddress.FirstName + " " + eventMessage.Order.ShippingAddress.LastName}
برای کالایی به مبلغ
{Convert.ToInt32(ApproximateValue).ToString("N0") + " ریال "}
و خدمات پستی به مبلغ
{Convert.ToInt32(eventMessage.Order.OrderTotal - ApproximateValue).ToString("N0") + " ریال "}
توسط گیرنده کالا پرداخت شد.
|
با تشکر امنیتو".Replace(Environment.NewLine, " ").Replace("|", Environment.NewLine));
                }
                if (_extendedShipmentService.getSourceByOrder(eventMessage.Order.Id) == 15)
                {
                    //TODO: send order info to webhook
                }

                _accountingService.calcAffilateCharge(eventMessage.Order);
               // if (_agentAmountRuleService.IsCustmoerInAgentRole(eventMessage.Order.CustomerId))
                {
                    //int serviceId = eventMessage.Order.OrderItems.First().Product.ProductCategories.First().CategoryId;
                    //if (!_agentAmountRuleService.getAgentAmountRuleEnable("AgentAmountRuleEnable"))
                    //    return;
                    string Msg = "واریز مبلغ تخفیف نمایندگی بر اساس قوانین نمایندگی برای سفارش شماره" + " " + eventMessage.Order.Id + " بدون کسر مالیات";
                    string outMsg = "";
                    foreach (var item in eventMessage.Order.OrderItems)
                    {
                        //if (!_accountingService.IsChargedWalletForAgentAMountRule(item.Id, 1))
                        {
                            PrivatePostDiscount privatePostDiscount;
                            var inlineAgentSaleAMount = _agentAmountRuleService.getInlineAgentSaleAmount(item, out privatePostDiscount);
                            if (inlineAgentSaleAMount != null)
                            {
                                var msg = _rewardPointCashoutService.execSp<string>("Checkout_Sp_DiscountCustomerOrderItem", new { disCountAmount = privatePostDiscount.DisCountAmount, ispercent = privatePostDiscount.IsPercent, orderItemId = item.Id });
                                if (inlineAgentSaleAMount.Price > 0)
                                {
                                    int rewardPointHistoryId = ChargeWalletForAgentSaleAmount(eventMessage.Order, inlineAgentSaleAMount.Price, Msg, out outMsg);
                                    _accountingService.InsertChargeWallethistory(new Models.ChargeWalletHistoryModel()
                                    {
                                        orderId = eventMessage.Order.Id,
                                        orderItemId = item.Id,
                                        rewaridPointHistoryId = rewardPointHistoryId,
                                        AgentAmountRuleId = inlineAgentSaleAMount.AgentAmountRuleId,
                                        shipmentId = null,
                                        ChargeWalletTypeId = 1,
                                        Description = Msg,
                                        Point = inlineAgentSaleAMount.Price,
                                        IpAddress = _webHelper.GetCurrentIpAddress(),
                                        CreateDate = DateTime.Now
                                    });
                                    if (!string.IsNullOrEmpty(outMsg))
                                        InsertOrderNote(outMsg, eventMessage.Order.Id);
                                }
                            }
                        }
                    }
                }
                if (eventMessage.Order.Customer.IsInCustomerRole("mini-Administrators"))
                {
                    _agentAmountRuleService.CalcOutLineAgentSaleAmount(eventMessage.Order);
                }

            }
            catch (Exception ex)
            {
                common.LogException(ex);
                _chargeWalletFailService.InsertFailedLog(ex, new { orderId = eventMessage.Order.Id }, "OrderPaidEventConsumer -> واریز مبلغ تخفیف نمایندگی");
                throw;
            }


        }
        /// <summary>
        /// شارژ کردن کیف پول
        /// </summary>
        /// <param name="order"></param>
        /// <param name="price"></param>
        /// <param name="inputMsg"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        private int ChargeWalletForAgentSaleAmount(Order order, int price, string inputMsg, out string Msg)
        {
            string Message = (string.IsNullOrEmpty(inputMsg) ? ("واریز مبلغ خدمات نمایندگی برای سفارش شماره" + " " + order.Id + " با کسر مالیات") : inputMsg);
            //price = price; - ((price * 9) / 100);
            //TODO : Reward point
            int rewardPointHistoryId = _rewardPointService.AddRewardPointsHistoryEntry(order.Customer, price,
                order.StoreId, Message, usedAmount: 0);
            if (rewardPointHistoryId > 0)
            {
                Msg = "واریز به کیف پول با موفقیت انجام شد";
                return rewardPointHistoryId;
            }
            Msg = "اشکال در واریز به کیف پول.لطفا با مدیر فنی تماس بگیرید";
            return 0;
        }
        public void InsertOrderNote(string note, int orderId)
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


        #endregion
    }
    public class InlineAgentSaleAMount
    {
        public int Price { get; set; }
        public int? AgentAmountRuleId { get; set; }
    }

}
