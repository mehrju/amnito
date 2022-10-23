using BS.Plugin.NopStation.MobileWebApi.Domain;
using BS.Plugin.NopStation.MobileWebApi.Models;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Services.Sms
{
    public class SmsReceiverService : ISmsReceiverService
    {
        private readonly MobileWebApiSettings _mobileWebApiSettings;
        private readonly IRepository<Tbl_ReceivedSms> _repository_Tbl_ReceivedSms;
        private readonly ITicketService _ticketService;
        private readonly IStoreContext _storeContext;
        private readonly IChargeWalletFailService _chargeWalletFailService;
        private readonly INotificationService _notificationService;
        private readonly IWebHelper _webHelper;
        private readonly IAccountingService _accountingService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IRepository<Customer> _repository_Customer;
        private readonly IRepository<Tbl_CustomerDepositCode> _repository_CustomerDepositCode;

        public SmsReceiverService(MobileWebApiSettings mobileWebApiSettings,
            IRepository<Tbl_ReceivedSms> repository_Tbl_ReceivedSms,
            ITicketService ticketService,
            IStoreContext storeContext,
            IChargeWalletFailService chargeWalletFailService,
            INotificationService notificationService,
            IWebHelper webHelper,
            IAccountingService accountingService,
            IRewardPointService rewardPointService,
            IRepository<Customer> repository_Customer,
            IRepository<Tbl_CustomerDepositCode> repository_CustomerDepositCode)
        {
            _mobileWebApiSettings = mobileWebApiSettings;
            _repository_Tbl_ReceivedSms = repository_Tbl_ReceivedSms;
            _ticketService = ticketService;
            _storeContext = storeContext;
            _chargeWalletFailService = chargeWalletFailService;
            _notificationService = notificationService;
            _webHelper = webHelper;
            _accountingService = accountingService;
            _rewardPointService = rewardPointService;
            _repository_Customer = repository_Customer;
            _repository_CustomerDepositCode = repository_CustomerDepositCode;
        }

        public void SmsReceived(string from, string to, string message, string messageId)
        {
            if (from.Trim().EndsWith(_mobileWebApiSettings.SmsWebHookSender))
            {
                var maliPhoneNo = "09129427467";
                if (_repository_Tbl_ReceivedSms.Table.Any(p => p.MessageId == messageId))
                {
                    return;
                }
                Tbl_ReceivedSms receivedSms = new Tbl_ReceivedSms()
                {
                    From = from,
                    To = to,
                    Message = message,
                    MessageId = messageId
                };
                _repository_Tbl_ReceivedSms.Insert(receivedSms);
                int? ticketId = null;
                //100000 ریال
                //0912***9883
                //1399/05/07
                //ش.مرجع: 024079135024
                //کد نشان پرداخت: 98181209
                //واریز به کیف پول آپ
                if (IsValidDataForApSms(message, out ReceivedSmsModel model))
                {
                    var preReceived = _repository_Tbl_ReceivedSms.Table.FirstOrDefault(p => p.RefrenceNumber == model.RefrenceNumber);
                    if (preReceived != null)
                    {
                        receivedSms.RefrenceNumber = model.RefrenceNumber;
                        _repository_Tbl_ReceivedSms.Update(receivedSms);
                        common.Log("duplicate refrence number for charge wallet", "duplicate refrencenumber " + model.RefrenceNumber + Environment.NewLine + message);
                        _notificationService.sendSms($"این پیامک قبلا ارسال شده و با شماره پیگیری {preReceived.RewardPointHistoryId} شارژ کیف پول انجام شده است", maliPhoneNo);
                        return;
                    }
                    var customers = _repository_Customer.Table.Where(p => p.Username.StartsWith(model.UserNameFirstPart) && p.Username.EndsWith(model.UserNameSecondPart) && p.CustomerRoles.Any(q => q.Id == 7)).ToList();
                    if (customers.Count == 1)
                    {
                        var selectedCustomer = customers.FirstOrDefault();
                        var depositCustomer = _repository_CustomerDepositCode.Table.FirstOrDefault(p => p.CustomerId == selectedCustomer.Id);
                        if (depositCustomer != null)
                        {
                            try
                            {
                                AddRewardPoint(from, messageId, depositCustomer.DepositCode, receivedSms, model, selectedCustomer);
                            }
                            catch (Exception ex)
                            {
                                common.LogException(ex);
                                ticketId = InsertTicket(message);
                            }
                        }
                        else
                        {
                            //ثبت تیکت
                            ticketId = InsertTicket(message);
                        }

                    }
                    else
                    {
                        //ثبت تیکت
                        ticketId = InsertTicket(message);
                        _notificationService.sendSms(" نمایندگی یافت نشد", maliPhoneNo);
                    }
                }
                //بانك سامان
                //واريز مبلغ  3,000,000ريال
                //به ‪9561-777-2669518-1‬
                //مانده 8,000,000
                //1399/6/3
                //10:09
                else if (IsValidDataForSamanSms(message, out model))
                {
                    var preReceived = _repository_Tbl_ReceivedSms.Table.FirstOrDefault(p => p.RefrenceNumber == model.RefrenceNumber);
                    if (preReceived != null)
                    {
                        receivedSms.RefrenceNumber = model.RefrenceNumber;
                        _repository_Tbl_ReceivedSms.Update(receivedSms);
                        common.Log("duplicate refrence number for charge wallet", "duplicate refrencenumber " + model.RefrenceNumber + Environment.NewLine + message);
                        _notificationService.sendSms($"این پیامک قبلا ارسال شده و با شماره پیگیری {preReceived.RewardPointHistoryId} شارژ کیف پول انجام شده است", maliPhoneNo);
                        return;
                    }
                    var depositCustomer = _repository_CustomerDepositCode.GetById(model.CustomerDepositId);
                    if (depositCustomer != null)
                    {
                        var customer = _repository_Customer.GetById(depositCustomer.CustomerId);
                        if (customer != null)
                        {
                            try
                            {
                                AddRewardPoint(from, messageId, depositCustomer.DepositCode, receivedSms, model, customer);
                            }
                            catch (Exception ex)
                            {
                                common.LogException(ex);
                                ticketId = InsertTicket(message);
                            }
                        }
                        else
                        {
                            //ثبت تیکت
                            ticketId = InsertTicket(message);
                        }
                    }
                    else
                    {
                        //ثبت تیکت
                        ticketId = InsertTicket(message);
                        _notificationService.sendSms(" نمایندگی یافت نشد", maliPhoneNo);
                    }

                }
                else
                {
                    //ثبت تیکت
                    ticketId = InsertTicket(message);
                    _notificationService.sendSms("متن پیامک شناسایی نشد", maliPhoneNo);
                }
                if (ticketId.HasValue)
                {
                    receivedSms.TicketId = ticketId.Value;
                    _repository_Tbl_ReceivedSms.Update(receivedSms);
                }

            }
            else
            {
                try
                {
                    SendToCrm(from, to, message, messageId);
                }
                catch (Exception)
                {
                    try
                    {
                        SendToCrm(from, to, message, messageId);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            SendToCrm(from, to, message, messageId);
                        }
                        catch (Exception ex)
                        {
                            common.Log("crm sms NOT sent", ex.ToString());
                            common.LogException(ex);
                            throw;
                        }
                    }

                }
            }
        }

        private static void SendToCrm(string from, string to, string message, string messageId)
        {
            WebClient wc = new WebClient();
            wc.DownloadString($"http://crm.postbar.ir/services/receivesmsurl.ashx?from={from}&to={to}&msg={message}&msgid={messageId}");
        }

        private void AddRewardPoint(string from, string messageId, string depositCode, Tbl_ReceivedSms receivedSms, ReceivedSmsModel model, Customer selectedCustomer)
        {
            try
            {
                int rewardPointHistoryId = _rewardPointService.AddRewardPointsHistoryEntry(selectedCustomer,
                                                  model.Amount, _storeContext.CurrentStore.Id,
                                                  $"واریز جهت شارژ کیف پول با شناسه پیام {messageId} به نماینده {depositCode}");
                if (rewardPointHistoryId > 0)
                {
                    _accountingService.InsertChargeWallethistory(new ChargeWalletHistoryModel()
                    {
                        rewaridPointHistoryId = rewardPointHistoryId,
                        orderId = 0,
                        CustomerId = selectedCustomer.Id,
                        orderItemId = null,
                        shipmentId = null,
                        ChargeWalletTypeId = 9,
                        Description = $"واریز جهت شارژ کیف پول با شناسه پیام {messageId} به نماینده {depositCode}",
                        Point = model.Amount,
                        IpAddress = _webHelper.GetCurrentIpAddress(),
                        CreateDate = DateTime.Now
                    });
                    receivedSms.RewardPointHistoryId = rewardPointHistoryId;
                    receivedSms.RefrenceNumber = model.RefrenceNumber;
                    _repository_Tbl_ReceivedSms.Update(receivedSms);
                    _notificationService.sendSms($"کیف پول شما به مبلغ {model.Amount} با شناسه {rewardPointHistoryId} شارژ شد {Environment.NewLine}", selectedCustomer.Username);
                }
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                _chargeWalletFailService.InsertFailedLog(ex, new { customerId = selectedCustomer.Id, smsModel = model }, "ReceiveSmsWebHookController");
                throw;
            }

        }

        private bool IsValidDataForSamanSms(string message, out ReceivedSmsModel model)
        {
            var lines = message.Split(new string[] { "\n", Environment.NewLine }, StringSplitOptions.None);
            model = null;
            if (!lines.Any(p => p.Contains("سامان")))
            {
                return false;
            }
            else if (!lines.Any(p => p.Contains("مبلغ")))
            {
                return false;
            }
            else
            {
                int amount = Convert.ToInt32(Regex.Replace(lines.First(p => p.Contains("مبلغ")), "[^0-9]+", string.Empty));
                int customerDepositCode = Convert.ToInt32(amount.ToString().Right(3));
                if (customerDepositCode > 0)
                {
                    model = new ReceivedSmsModel()
                    {
                        Amount = amount,
                        CustomerDepositId = customerDepositCode,
                        RefrenceNumber = message.RemoveWhiteSpaces()
                    };
                    return true;
                }
                return false;
            }
        }

        private int InsertTicket(string message)
        {
            TicketModel newticket = new TicketModel();
            newticket.DateInsert = DateTime.Now;
            newticket.DepartmentId = 3;
            newticket.IdCategoryTicket = 2;
            newticket.ProrityId = 4;
            newticket.IdCustomer = 10437104;// سرویس شارژ پیامکی
            newticket.IsActive = true;
            newticket.Issue = "ثبت ناموفق شارژ کیف پول از طریق پیامک";
            newticket.OrderCode = 0;
            newticket.StoreId = _storeContext.CurrentStore.Id;
            newticket.TrackingCode = null;
            newticket.ShowCustomer = false;
            newticket.ticket_Details = new List<TicketDetailModel>()
                        {
                            new TicketDetailModel()
                            {
                                DateInsert = DateTime.Now,
                                Description = message.Replace(Environment.NewLine, "</br>"),
                                Type = false,
                                UrlFile1 = null,
                                UrlFile2 = null,
                                UrlFile3 = null
                            }
                        };
            _ticketService.Insert(newticket);
            return newticket.Id;
        }

        /// <summary>
        /// check message validation
        /// </summary>
        /// <param name="message"></param>
        /// <param name="model"></param>
        /// <returns>model based on message</returns>
        private bool IsValidDataForApSms(string message, out ReceivedSmsModel model)
        {
            var lines = message.Split(new string[] { "\n", Environment.NewLine }, StringSplitOptions.None);
            model = null;
            if (lines.Length < 4)
            {
                return false;
            }
            else if (!lines.Any(p => p.Contains("ریال")))
            {
                return false;
            }
            else if (!lines.Any(p => p.Contains("ش.مرجع:")))
            {
                return false;
            }
            else if (lines[1].Length != 11)
            {
                return false;
            }
            else
            {
                try
                {
                    string[] userNameParts = null;
                    if (lines[1].Contains("*"))
                    {
                        userNameParts = lines[1].Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries);
                        if (userNameParts.Length != 2 || userNameParts[0].Length != 4 || userNameParts[1].Length != 4)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        userNameParts = new string[] { lines[1].Substring(0, 4), lines[1].Substring(7, 4) };
                    }
                    model = new ReceivedSmsModel()
                    {
                        Amount = Convert.ToInt32(lines.FirstOrDefault(p => p.Contains("ریال")).Replace("ریال", "").Trim()),
                        UserNameFirstPart = userNameParts[0],
                        UserNameSecondPart = userNameParts[1],
                        RefrenceNumber = lines.FirstOrDefault(p => p.Contains("ش.مرجع:")).Replace("ش.مرجع:", "").Trim()
                    };
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }

            }
        }
    }
}
