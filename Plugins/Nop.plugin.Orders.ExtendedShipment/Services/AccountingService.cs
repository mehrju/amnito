using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class AccountingService : IAccountingService
    {
        private readonly IRepository<ChargeWalletHistoryModel> _repositoryChargeWalletHistory;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IDbContext _dbContext;
        private readonly IRewardPointService _rewardPointService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;
        private readonly IChargeWalletFailService _chargeWalletFailService;
        private readonly IOrderService _orderService;

        public AccountingService(IRepository<ChargeWalletHistoryModel> repositoryChargeWalletHistory,
            IRepository<OrderNote> orderNoteRepository
            , IDbContext dbContext
            , IRewardPointService rewardPointService
            , ICustomerService customerService
            , IWorkContext workContext, IWebHelper webHelper
            , IOrderService orderService
            , IChargeWalletFailService chargeWalletFailService)
        {
            _workContext = workContext;
            _webHelper = webHelper;
            _chargeWalletFailService = chargeWalletFailService;
            _customerService = customerService;
            _rewardPointService = rewardPointService;
            _repositoryChargeWalletHistory = repositoryChargeWalletHistory;
            _orderNoteRepository = orderNoteRepository;
            _dbContext = dbContext;
            _orderService = orderService;
        }
        /// <summary>
        /// درج تاریخ چه شارژ کیف پول
        /// </summary>
        /// <param name="model"></param>
        public void InsertChargeWallethistory(ChargeWalletHistoryModel model)
        {
            try
            {
                string error = "";
                if (model.orderId == 0)
                {
                    error = "شماره سفارش وارد شده جهت ثبت تاریخچه شارژ کیف پول نامعتبر می باشد";
                    Log("مشکل در زمان ثبت تاریخچه کیف پول نماینده ", error);
                    return;
                }
                //if (model.rewaridPointHistoryId == 0)
                //{
                //    error = "شناسه تاریخچه امتیازات جهت ثبت تاریخچه شارژ کیف پول نامعتبر می باشد";
                //    InsertOrderNote(error, model.orderId);
                //    return;
                //}
                if (model.CustomerId == 0 || model.CustomerId == null)
                    model.CustomerId = _orderService.GetOrderById(model.orderId)?.CustomerId;
                _repositoryChargeWalletHistory.Insert(model);
            }
            catch (Exception ex)
            {
                if (model.orderId > 0)
                {
                    InsertOrderNote("خطا در زمان ثبت تاریخچه شارژ کیف پول نماینده" + "---" + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""), model.orderId);
                    return;
                }
                LogException(ex, "خطا در زمان ثبت تاریخچه شارژ کیف پول نماینده");
            }
        }
        public bool HasChargeWallethistory(int ChargeWalleTypeId, int orderId)
        {
            return _repositoryChargeWalletHistory.Table.Any(p => p.orderId == orderId && p.ChargeWalletTypeId == ChargeWalleTypeId);
        }
        public void RecoverAgentAmountMonyForShipment(Shipment shipment)
        {
            try
            {
                string msg = "";
                int orderItemId = shipment.ShipmentItems.First().OrderItemId;
                int orderId = shipment.OrderId;
                string query = @"DECLARE @price INT 
                                SELECT
	                                @price = ISNULL(SUM(RPH.Points),0)
                                FROM
	                                dbo.Tb_ChargeWalletHistory AS TCWH
	                                INNER JOIN dbo.RewardPointsHistory AS RPH ON RPH.Id = TCWH.rewaridPointHistoryId
	                                INNER JOIN dbo.[Order] AS O ON O.Id = TCWH.orderId
                                WHERE
	                                TCWH.orderId = " + orderId + @"
	                                AND O.Deleted = 0
                                IF @price > 0
                                BEGIN
	                                SELECT
	                                    (RPH.Points / OI.Quantity) AgentAmountPrice
                                    FROM
	                                    dbo.Tb_ChargeWalletHistory AS TCWH
	                                    INNER JOIN dbo.RewardPointsHistory AS RPH ON RPH.Id = TCWH.Id
	                                    INNER JOIN dbo.OrderItem AS OI ON TCWH.orderItemId = Oi.Id
                                    WHERE
	                                    TCWH.orderId = " + orderId + @"
	                                    AND TCWH.orderItemId = " + orderItemId + @"
	                                    AND RPH.Points > 0
                                END 
                                ELSE
                                BEGIN
	                                SELECT 0 AS AgentAmountPrice
                                END ";

                int? AgentAmountPrice = _dbContext.SqlQuery<int>(query, new object[0]).FirstOrDefault();
                if (!AgentAmountPrice.HasValue)
                {
                    msg = $"مبلغ تخفیف نمایندگی در زمان حذف برای محموله شماره {shipment.Id} یافت نشد";
                    InsertOrderNote(msg, shipment.OrderId);
                    return;
                }
                if (AgentAmountPrice.Value <= 0)
                {
                    msg = $"مبلغ تخفیف نمایندگی در زمان حذف برای محموله {shipment.Id} نامعتبر میباشد";
                    InsertOrderNote(msg, shipment.OrderId);
                    return;
                }
                string walletCharje = $"برگشت مبلغ تخفیف نمایندگی برای محموله شماره {shipment.OrderId} از سفارش شماره {shipment.Id}";

                int rewardPointHistoryId = ChargeWalletForAgentSaleAmount(shipment.Order, (AgentAmountPrice.Value * -1), walletCharje
                     , out msg);

                if (rewardPointHistoryId == 0)
                {
                    msg += " شماره محموله: " + shipment.Id;
                    InsertOrderNote(msg, shipment.OrderId);

                }
                else
                    InsertOrderNote(walletCharje, shipment.OrderId);

                InsertChargeWallethistory(new Models.ChargeWalletHistoryModel()
                {
                    orderId = shipment.Order.Id,
                    orderItemId = orderItemId,
                    rewaridPointHistoryId = rewardPointHistoryId,
                    AgentAmountRuleId = null,
                    shipmentId = shipment.Id,
                    ChargeWalletTypeId = 2,
                    Description = walletCharje,
                    Point = AgentAmountPrice.Value * -1,
                    IpAddress = _webHelper.GetCurrentIpAddress(),
                    CreateDate = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                _chargeWalletFailService.InsertFailedLog(ex, new { shipmentId = shipment.Id, orderId = shipment.OrderId }, "AccountingService.RecoverAgentAmountMonyForShipment");
                throw;
            }

        }
        public bool IsChargedWalletForAgentAMountRule(int orderItem, int ChargeWalletType)
        {
            return _repositoryChargeWalletHistory.Table.Any(p => p.orderItemId == orderItem && p.ChargeWalletTypeId == ChargeWalletType);
        }
        /// <summary>
        /// شارژ کردن کیف پول
        /// </summary>
        /// <param name="order"></param>
        /// <param name="price"></param>
        /// <param name="inputMsg"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public int ChargeWalletForAgentSaleAmount(Order order, int price, string inputMsg, out string Msg)
        {
            //TODO : Reward point
            int rewardPointHistoryId = _rewardPointService.AddRewardPointsHistoryEntry(order.Customer, price,
                order.StoreId, inputMsg, usedAmount: 0);
            if (rewardPointHistoryId > 0)
            {
                Msg = "شارژ کیف پول با موفقیت انجام شد";
                return rewardPointHistoryId;
            }
            Msg = "اشکال در شارژ کیف پول.لطفا با مدیر فنی تماس بگیرید";
            return 0;
        }
        public void calcAffilateCharge(Order order)
        {
            try
            {
                SqlParameter[] prms = new SqlParameter[] {
                    new SqlParameter() { ParameterName = "orderId", SqlDbType = SqlDbType.Int, Value = order.Id }
                };
                string query = "EXEC dbo.Sp_calcChargeForAffilateOrder @orderId";

                var result = _dbContext.SqlQuery<AffiliateCharge>(query, prms).FirstOrDefault();
                if (result == null)
                    return;
                if (result.CharjePrice > 0)
                {
                    var customer = _customerService.GetCustomerById(result.AffilatecustomerId);
                    if (customer == null)
                        return;
                    int rewardPointHistoryId = _rewardPointService.AddRewardPointsHistoryEntry(customer, result.CharjePrice,
                    order.StoreId, $"واریز مبلغ کمیسیون همکار فروش برای سفارش شماره {order.Id} از مشتریان شما", usedAmount: 0);
                    ChargeWalletHistoryModel Cwhm = new ChargeWalletHistoryModel()
                    {
                        AgentAmountRuleId = null,
                        ChargeWalletTypeId = 6,
                        orderId = order.Id,
                        rewaridPointHistoryId = rewardPointHistoryId,
                        CustomerId = result.AffilatecustomerId,
                        Description = $"واریز مبلغ کمیسیون همکار فروش برای سفارش شماره {order.Id} از مشتریان شما",
                        Point = result.CharjePrice,
                        IpAddress = _webHelper.GetCurrentIpAddress(),
                        CreateDate = DateTime.Now
                    };
                    _repositoryChargeWalletHistory.Insert(Cwhm);
                }
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                _chargeWalletFailService.InsertFailedLog(ex, new { orderId = order.Id }, "AccountingService.calcAffilateCharge");
                //throw;
            }
        }
        #region Utility
        private void LogException(Exception ex, string title)
        {
            Log(title, ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""));
        }
        private void Log(string header, string Message)
        {
            var logger =
                (DefaultLogger)EngineContext.Current
                    .Resolve<ILogger>();
            logger.InsertLog(LogLevel.Information, header, Message, null);
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

        #endregion
        public void refoundAffilateCharge(Order order)
        {
            try
            {
                string query = $@"SELECT top(1)
	                            TCWH.CustomerId,
	                            RPH.Points,
	                            TCWH.Id
                            FROM
	                            dbo.Tb_ChargeWalletHistory AS TCWH
	                            INNER JOIN dbo.[Order] AS O ON O.Id = TCWH.orderId
	                            INNER JOIN dbo.RewardPointsHistory AS RPH ON tcwh.rewaridPointHistoryId = RPH.Id
                            WHERE
	                            O.Id = {order.Id}
	                            AND tcwh.ChargeWalletTypeId = 6
                            ORDER BY TCWH.id DESC";
                var data = _dbContext.SqlQuery<refoundAffilate>(query, new object[0]).FirstOrDefault();
                if (data != null)
                {
                    string walletCharje = $"برگشت مبلغ کمیسیون همکار فروش برای سفارش شماره " + order.Id;
                    var _customer = _customerService.GetCustomerById(data.CustomerId);
                    int rewardPointHistoryId = _rewardPointService.AddRewardPointsHistoryEntry(_customer, (data.Points * -1),
                   order.StoreId, walletCharje, usedAmount: 0);


                    InsertChargeWallethistory(new Models.ChargeWalletHistoryModel()
                    {
                        orderId = order.Id,
                        orderItemId = null,
                        rewaridPointHistoryId = rewardPointHistoryId,
                        AgentAmountRuleId = null,
                        shipmentId = null,
                        ChargeWalletTypeId = 7,
                        CustomerId = data.CustomerId,
                        Description = walletCharje,
                        Point = data.Points * -1,
                        IpAddress = _webHelper.GetCurrentIpAddress(),
                        CreateDate = DateTime.Now
                    });
                }
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                _chargeWalletFailService.InsertFailedLog(ex, new { orderId = order.Id }, "AccountingService.refoundAffilateCharge");
            }
        }
        public List<SettlementListInfo> SettlementList(int FileId
            , string UserName
            , int OrderId
            , DateTime? DepositDateFrom 
            , DateTime? DepositDateTo
            , DateTime? SettlementDateFrom
            , DateTime? SettlementDateTo
            , int PageIndex
            , int PageSize
            , ref int PageCount
            )
        {
            string query = $@"EXEC dbo.Sp_SettlementInfo @FileId ,@UserName,@OrderId,@DepositDateFrom,@DepositDateTo,@SettlementDateFrom,@SettlementDateTo,@PageIndex,@PageSize,@TotalCount OUTPUT";
            SqlParameter ToatlCount = new SqlParameter() {ParameterName= "TotalCount", SqlDbType = SqlDbType.Int,Direction= ParameterDirection.Output, Value=0 };
            SqlParameter[] prms = new SqlParameter[] {
                    new SqlParameter() { ParameterName = "FileId", SqlDbType = SqlDbType.Int, Value = FileId }
                    ,new SqlParameter() { ParameterName = "UserName", SqlDbType = SqlDbType.NVarChar, Value = (string.IsNullOrEmpty(UserName)?(object)DBNull.Value:UserName)}
                    ,new SqlParameter() { ParameterName = "OrderId", SqlDbType = SqlDbType.Int, Value = OrderId }
                    ,new SqlParameter() { ParameterName = "DepositDateFrom", SqlDbType = SqlDbType.DateTime, Value = (DepositDateFrom.HasValue?(object)DepositDateFrom.Value:(object)DBNull.Value)}
                    ,new SqlParameter() { ParameterName = "DepositDateTo", SqlDbType = SqlDbType.DateTime, Value = (DepositDateTo.HasValue?(object)DepositDateTo.Value:(object)DBNull.Value)}
                    ,new SqlParameter() { ParameterName = "SettlementDateFrom", SqlDbType = SqlDbType.DateTime, Value = (SettlementDateFrom.HasValue?(object)SettlementDateFrom.Value:(object)DBNull.Value)}
                    ,new SqlParameter() { ParameterName = "SettlementDateTo", SqlDbType = SqlDbType.DateTime, Value = (SettlementDateTo.HasValue?(object)SettlementDateTo.Value:(object)DBNull.Value)}
                    ,new SqlParameter() { ParameterName = "PageIndex", SqlDbType = SqlDbType.Int, Value = PageIndex }
                    ,new SqlParameter() { ParameterName = "PageSize", SqlDbType = SqlDbType.Int, Value = PageSize }
                    ,ToatlCount

            };
            var data= _dbContext.SqlQuery<SettlementListInfo>(query, prms).ToList();
            PageCount = (int)ToatlCount.Value;
            return data;
        }
    }
    public class AffiliateCharge
    {
        public int CharjePrice { get; set; }
        public int AffilatecustomerId { get; set; }
    }
    public class refoundAffilate
    {
        public int CustomerId { get; set; }
        public int Points { get; set; }
        public int Id { get; set; }
    }
    public class SettlementListInfo
    {
        public int OrderId { get; set; }
        public string Message { get; set; }
        public int Value { get; set; }
        public int PaymentFileNo { get; set; }
        public DateTime? SettlementDate { get; set; }
        public DateTime? WalletOprationDate { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string RefrenceCode { get; set; }
    }
}
