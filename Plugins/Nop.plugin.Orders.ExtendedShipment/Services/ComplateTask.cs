using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.plugin.Orders.ExtendedShipment.Models;
using BS.Plugin.Orders.ExtendedShipment.Services;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class ComplateTask : IScheduleTask
    {
        private readonly IDbContext _dbContext;
        private readonly IOrderService _orderService;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IRepository<TaskHistoryModel> _repositoryTaskHistory;
        private readonly INotificationService _notificationService;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IOptimeApiService _optimeApiService;
        public ComplateTask(IDbContext dbContext
            , IRepository<TaskHistoryModel> repositoryTaskHistory
            , IOrderService orderService
            , IRepository<OrderNote> orderNoteRepository
            , INotificationService notificationService
            , IExtendedShipmentService extendedShipmentService
            , IOptimeApiService optimeApiService)
        {
            _optimeApiService = optimeApiService;
            _repositoryTaskHistory = repositoryTaskHistory;
            _orderNoteRepository = orderNoteRepository;
            _orderService = orderService;
            _dbContext = dbContext;
            _notificationService = notificationService;
            _extendedShipmentService = extendedShipmentService;
        }
        public void Execute()
        {
            try
            {
                if (!AllowToExec())
                    return;
                DateTime? dtStart, dtEnd, execDate;
                dtStart = dtEnd = execDate = DateTime.Now;
                string value = "";
                string AddtionalWhere = "";
                string AddtionalJoin = "";
                bool CanPostCoardination = true;
                if (DateTime.Now.Hour >= 8 && DateTime.Now.Hour <= 17)
                {
                    if (!IsExecqutedTask("AlarmToPostSupervisor", execDate.Value, DateTime.Now.Hour.ToString("00")))
                    {
                        int type = 0;
                        if (DateTime.Now.Hour >= 8 && DateTime.Now.Hour <= 13)
                            type = 1;
                        else
                            type = 2;
                        if (type != 0)
                        {
                            AlarmToPostSupervisor(type);
                            InsertHistory("AlarmToPostSupervisor", execDate.Value, DateTime.Now.Hour.ToString("00"));
                        }

                    }
                }
                int NumbersOfdayToComplate = -getNumbersOfdayToComplate();
                dtStart = (DateTime.Now.Date.AddHours(12)).AddDays(NumbersOfdayToComplate).ToUniversalTime();
                if (DateTime.Now.Hour == 8)
                {
                    value = "8";

                    dtEnd = (DateTime.Now.Date.AddHours(8)).ToUniversalTime();
                    AddtionalWhere = "";// " AND (O.PaymentMethodSystemName IS NULL OR O.PaymentMethodSystemName <>N'Payments.CashOnDelivery') ";
                    if (IsExecqutedTask("OrderComplateTask", execDate.Value, value))
                        return;
                }
                else if (DateTime.Now.Hour == 9)
                {
                    value = "9";
                    dtEnd = (DateTime.Now.Date).AddHours(9).ToUniversalTime();
                    AddtionalWhere = "";// " AND (O.PaymentMethodSystemName IS NULL OR O.PaymentMethodSystemName <>N'Payments.CashOnDelivery') ";
                    if (IsExecqutedTask("OrderComplateTask", execDate.Value, value))
                        return;
                }
                else if (DateTime.Now.Hour == 10)
                {
                    value = "10";
                    dtEnd = (DateTime.Now.Date.AddHours(10)).ToUniversalTime();
                    AddtionalWhere = "";// " AND (O.PaymentMethodSystemName IS NULL OR O.PaymentMethodSystemName <>N'Payments.CashOnDelivery') ";
                    if (IsExecqutedTask("OrderComplateTask", execDate.Value, value))
                        return;
                }
                else if (DateTime.Now.Hour == 11)
                {
                    value = "11";
                    dtEnd = (DateTime.Now.Date.AddHours(11)).ToUniversalTime();
                    AddtionalWhere = "";// " AND (O.PaymentMethodSystemName IS NULL OR O.PaymentMethodSystemName <>N'Payments.CashOnDelivery') ";
                    if (IsExecqutedTask("OrderComplateTask", execDate.Value, value))
                        return;
                }
                else if (DateTime.Now.Hour == 12)
                {
                    value = "12";
                    dtEnd = (DateTime.Now.Date.AddHours(12)).ToUniversalTime();
                    //AddtionalWhere = @" AND (O.PaymentMethodSystemName IS NULL OR O.PaymentMethodSystemName <> N'Payments.CashOnDelivery') 
                    //                AND (A.StateProvinceId IN (SELECT
                    //                     TCC.CenterOfCountry
                    //                    FROM
                    //                     dbo.Tb_CountryCenter AS TCC) OR A.CountryId = 1)";
                    AddtionalWhere = "";// " AND (O.PaymentMethodSystemName IS NULL OR O.PaymentMethodSystemName <>N'Payments.CashOnDelivery') ";
                    if (IsExecqutedTask("OrderComplateTask", execDate.Value, value))
                        return;
                }
                else if (DateTime.Now.Hour == 13)
                {
                    value = "13";
                    dtEnd = (DateTime.Now.Date.AddHours(13)).ToUniversalTime();
                    AddtionalWhere = "";// " AND (O.PaymentMethodSystemName IS NULL OR O.PaymentMethodSystemName <>N'Payments.CashOnDelivery') ";
                    if (IsExecqutedTask("OrderComplateTask", execDate.Value, value))
                        return;
                }
                else if (DateTime.Now.Hour == 14)
                {
                    value = "14";
                    dtEnd = (DateTime.Now.Date.AddHours(14)).ToUniversalTime();
                    AddtionalWhere = "";//" AND (O.PaymentMethodSystemName IS NULL OR O.PaymentMethodSystemName <>N'Payments.CashOnDelivery') ";
                    if (IsExecqutedTask("OrderComplateTask", execDate.Value, value))
                        return;
                }
                else if (DateTime.Now.Hour == 15)
                {
                    value = "15";
                    dtEnd = (DateTime.Now.Date.AddHours(14)).ToUniversalTime();
                    AddtionalWhere = "";//" AND (O.PaymentMethodSystemName IS NULL OR O.PaymentMethodSystemName <>N'Payments.CashOnDelivery') ";
                    if (IsExecqutedTask("OrderComplateTask", execDate.Value, value))
                        return;
                }
                //else if (DateTime.Now.Hour == 16)
                //{
                //    value = "16";
                //    dtStart = (DateTime.Now.Date.AddHours(15)).ToUniversalTime();
                //    dtEnd = (DateTime.Now.Date.AddHours(16)).ToUniversalTime();
                //    AddtionalWhere = "";//" AND (O.PaymentMethodSystemName IS NULL OR O.PaymentMethodSystemName <>N'Payments.CashOnDelivery') ";
                //    if (IsExecqutedTask("OrderComplateTask", execDate.Value, value))
                //        return;
                //}
                else
                {
                    dtStart = dtEnd = null;
                    return;
                }
                if (HasTodyHistory(value))
                    return;
                SqlParameter[] prms = new SqlParameter[]
                {
                    new SqlParameter() {ParameterName = "dt_Start", SqlDbType = SqlDbType.DateTime, Value = dtStart},
                    new SqlParameter() {ParameterName = "dt_End", SqlDbType = SqlDbType.DateTime, Value = dtEnd}
                };
                string query = $@"SELECT
	                                DISTINCT C.Id AS CustomerId
                                INTO #Tb1 
                                FROM
	                                dbo.Customer AS C
	                                INNER JOIN dbo.Customer_CustomerRole_Mapping AS CCRM ON CCRM.Customer_Id = C.Id
	                                INNER JOIN dbo.CustomerRole AS CR ON CR.Id = CCRM.CustomerRole_Id

                                WHERE
	                                CR.SystemName LIKE N'%Collector%' 


                                SELECT DISTINCT
	                                O.Id
                                FROM
	                                dbo.[Order] AS O
	                                INNER JOIN dbo.Shipment AS S ON S.OrderId = O.Id
	                                INNER JOIN dbo.ShipmentItem AS SI ON SI.ShipmentId = S.Id
	                                INNER JOIN dbo.OrderItem AS OI ON OI.Id = SI.OrderItemId
	                                INNER JOIN dbo.Product AS P ON P.Id = OI.ProductId
	                                INNER JOIN dbo.Product_Category_Mapping AS PCM ON PCM.ProductId = P.Id
                                    INNER JOIN dbo.Address AS A ON A.Id = O.BillingAddressId
                                    LEFT JOIN dbo.ShipmentAppointment AS SA 
		                                    INNER JOIN dbo.Tb_Collectors AS TC ON TC.ShipmentAppointmentId = sa.Id
	                                    ON SA.ShipmentId = S.Id
	                                LEFT JOIN #Tb1 AS T ON T.CustomerId = O.CustomerId
                                    " + AddtionalJoin + @"
                                WHERE
	                                (O.OrderStatusId in (20,30) or (O.PaymentMethodSystemName =N'Payments.CashOnDelivery' AND O.OrderStatusId IN (10,20)))
	                                AND TC.Id IS NULL 
	                                AND O.Deleted = 0
	                                AND T.CustomerId IS NULL 
	                                AND O.CreatedOnUtc BETWEEN @dt_Start AND @dt_End
	                                AND pcm.Id NOT IN (717,719,710,707) " + AddtionalWhere;

                var data = _dbContext.SqlQuery<int>(query, prms).ToList();
                string orderIds = "";
                List<int> withoutCollectorIds = new List<int>();
                foreach (var item in data)
                {
                    try
                    {
                        var order = _orderService.GetOrderById(item);
                        var mark = _extendedShipmentService.GetOrderRegistrationMethod(order);
                        if ((!order.Customer.IsInCustomerRole("Collector") && !order.Customer.IsInCustomerRole("TwoStepOrder")) ||
                            (mark == OrderRegistrationMethod.bidok && order.OrderStatus == OrderStatus.Complete))
                            withoutCollectorIds.Add(item);
                        if (!order.Customer.IsInCustomerRole("TwoStepOrder") && order.OrderStatus != OrderStatus.Complete)
                        {
                            order.OrderStatusId = 30;
                            _orderService.UpdateOrder(order);
                            InsertOrderNote("وضعیت سفارش ویرایش شد. وضعیت جدید: تکمیل", item);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log("خطا در زمان تکمیل کردن سفارش" + item, ex.Message + ex.InnerException != null ? "-->" + ex.InnerException.Message : "");
                    }
                }

                _extendedShipmentService.SavePostCoordination(withoutCollectorIds, "ارسال اتوماتیک");
                _optimeApiService.SendForOptimizeRout();
                foreach (var item in data)
                {
                    var order = _orderService.GetOrderById(item);
                    if (order.OrderStatus != OrderStatus.Complete)
                        continue;
                    int serviceId = order.OrderItems.First().Product.ProductCategories.First().CategoryId;
                    if (new int[] { 654, 655, 703, 699, 705, 706, 712, 713, 714, 715, 662, 725, 726, 727 }.Contains(serviceId))
                    {
                        if (!order.OrderNotes.Any(p => p.Note.Contains("SmsNewOrderSend")))
                        {
                            InsertOrderNote("SmsNewOrderSend", order.Id);
                            orderIds += orderIds == "" ? "" : "," + item;
                        }
                        if ((order.PaymentMethodSystemName ?? "") != "Payments.CashOnDelivery" && !order.Customer.IsInCustomerRole("mini-Administrators"))
                        {
                            if (!order.OrderNotes.Any(p => p.Note.Contains("SendSmsSupervisor")))
                            {
                                InsertOrderNote("SendSmsSupervisor", order.Id);
                                //_notificationService.NotifyPostSupervisor(order, serviceId, _extendedShipmentService);
                            }
                        }
                    }
                }
                if (orderIds != "")
                {
                    try
                    {
                        _notificationService.sendSmsPostAdminForNewOrder(orderIds, _extendedShipmentService);
                    }
                    catch (Exception ex)
                    {
                        _extendedShipmentService.Log("خطا در زمان ارسال پیامک سفارش جدید برای مدیر و ناظر پست ", ex.Message +
                                                                                                                 (ex.InnerException != null ? ex.InnerException.Message : ""));
                    }
                }
                InsertHistory("OrderComplateTask", execDate.Value, value);
            }
            catch (Exception ex)
            {
                _extendedShipmentService.LogException(ex);
            }

        }
        public void AlarmToPostSupervisor(int type)
        {
            string Message1 = "";
            if (type == 1)
            {
                Message1 = "ناظر محترم تعدادی از سفارشات مشتریان مربوط به شما جهت جمع آوری به پستچی هماهنگ نگردیده، لطفاهر چه سریعتر اقدام فرمایید";
            }
            else
            {
                Message1 = "ناظر محترم تعدادی از سفارشات مشتریان مربوط به شما جهت جمع آوری به پستچی ارجاع گردیده ولی تا این لحظه، تیک جمع آوری اعمال نگردیده لطفا هر چه سریعتر بارها را تعیین تکلیف فرمایید";
            }
            SqlParameter[] prms = new SqlParameter[]{
                new SqlParameter(){ParameterName="type",SqlDbType = SqlDbType.Int,Value= (object)type},
            };
            string query = "EXEC dbo.Sp_getAgentForNotCoordinatedShipment @type";
            var AgentForNotCoordinatedShipment1 = _dbContext.SqlQuery<string>(query, prms).ToList();
            foreach (var item in AgentForNotCoordinatedShipment1)
            {
                _notificationService._sendSms(item, Message1);
            }
        }
        public bool AllowToExec()
        {
            SqlParameter[] prms = new SqlParameter[]
           {
                    new SqlParameter() {ParameterName = "Today", SqlDbType = SqlDbType.DateTime, Value = DateTime.Now}
           };
            string query = @"IF EXISTS(
                SELECT TOP(1)
	                THD.HolidayDate
                FROM	
	                dbo.Tb_HoliDay AS THD
                WHERE
	                CAST(THD.HolidayDate AS DATE) = CAST(@Today AS date ))
	                SELECT 1
                ELSE 
	                SELECT 0";
            int result = _dbContext.SqlQuery<int>(query, prms).Single();
            if (result == 1)
                return false;
            return true;

        }
        public void InsertOrderNote(string note, int orderId)
        {
            OrderNote Onote = new OrderNote()
            {
                Note = note,
                CreatedOnUtc = DateTime.Now.ToUniversalTime(),
                DisplayToCustomer = false,
                OrderId = orderId
            };
            _orderNoteRepository.Insert(Onote);
        }

        public void Log(string header, string Message)
        {
            var logger =
                (DefaultLogger)EngineContext.Current
                    .Resolve<ILogger>();
            logger.InsertLog(LogLevel.Information, header, Message, null);
        }

        public void InsertHistory(string Name, DateTime execDate, string value)
        {
            TaskHistoryModel model = new TaskHistoryModel()
            {
                ExecDate = execDate.Date,
                Name = Name,
                value = value
            };
            _repositoryTaskHistory.Insert(model);
        }
        public bool IsExecqutedTask(string Name, DateTime execDate, string value)
        {
            var date = execDate.Date;
            return _repositoryTaskHistory.Table.Any(p => p.ExecDate.Year == date.Year && p.ExecDate.Month == date.Month && p.ExecDate.Day == date.Day
            && p.value == value && p.Name == Name);
        }
        public bool HasTodyHistory(string value)
        {
            DateTime dt = DateTime.Now.Date;
            return _repositoryTaskHistory.Table.Any(p => p.Name == "OrderComplateTask"
                                                         && p.ExecDate.Year == dt.Year && p.ExecDate.Month == dt.Month && p.ExecDate.Day == dt.Day
                                                         && p.value == value);
        }
        public int getNumbersOfdayToComplate()
        {
            var execDate = _repositoryTaskHistory.Table.Max(p => p.ExecDate);
            int days = (DateTime.Now - execDate).Days;
            if (days == 0)
                return 1;
            return days;
        }
    }
}
