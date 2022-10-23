using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.CODServiceRefrence;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Models.Orders;
using RestSharp;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class CodService : ICodService
    {
        #region field
        private const string ApiUserName = "postbar";
        private const string ApiPassword = "postbar12345";
        private const string ApiUrl = "";
        private readonly BasicHttpBinding _basicHttpbinding;
        private readonly EndpointAddress _endpointAddress;
        private readonly IRepository<Shipment> _repositoryShipment;
        private readonly IRepository<CODShipmentEventModel> _repositoryCODShipmentEvent;
        private readonly IRewardPointService _rewardPointService;
        private readonly IShipmentService _shipmentService;
        private readonly IStoreContext _storeContext;
        private readonly IRepository<PayForCodLogModel> _repositoryPayForCodLog;
        private readonly IAccountingService _accountingService;
        private readonly IWebHelper _webHelper;
        private readonly IChargeWalletFailService _chargeWalletFailService;
        private readonly IDbContext _dbcontext;
        private readonly string BasUri = "https://Services.postex.ir/api/Gateway";
        //private readonly string BasUri = "http://localhost:5000/api/Gateway";
        #endregion

        #region ctor
        public CodService(IRepository<Shipment> repositoryShipment
       , IRepository<CODShipmentEventModel> repositoryCODShipmentEvent
       , IRewardPointService rewardPointService
       , IShipmentService shipmentService
       , IStoreContext storeContext
       , IRepository<PayForCodLogModel> repositoryPayForCodLog,
           IAccountingService accountingService,
           IWebHelper webHelper,
           IChargeWalletFailService chargeWalletFailService, IDbContext dbcontext)
        {
            _repositoryPayForCodLog = repositoryPayForCodLog;
            _accountingService = accountingService;
            _webHelper = webHelper;
            _chargeWalletFailService = chargeWalletFailService;
            _repositoryShipment = repositoryShipment;
            _repositoryCODShipmentEvent = repositoryCODShipmentEvent;
            _rewardPointService = rewardPointService;
            _shipmentService = shipmentService;
            _storeContext = storeContext;
            _dbcontext = dbcontext;
        }

        #endregion

        public bool ChargeMoneyBagForCodGood(int shipmentId, int price, out string Msg)
        {
            try
            {
                if (_repositoryPayForCodLog.Table.Any(p => p.ShipmentId == shipmentId))
                {
                    Msg = "واریزی قبلا انجام شده";
                    return true;
                }

                var shipment = _shipmentService.GetShipmentById(shipmentId);
                _accountingService.InsertChargeWallethistory(new plugin.Orders.ExtendedShipment.Models.ChargeWalletHistoryModel()
                {
                    //rewaridPointHistoryId = rewardPointHistoryId,
                    orderId = shipment.OrderId,
                    CustomerId = shipment.Order.Customer.Id,
                    //orderItemId = item.Id,
                    shipmentId = shipment.Id,
                    ChargeWalletTypeId = 10,
                    Description = "واریز مبلغ کالا برای محموله پس کرایه شماره" + " " + shipment.Id + " از سفارش شماره " + " " + shipment.OrderId +
                      "_" + "AddCodPrice",
                    Point = price,
                    IpAddress = _webHelper.GetCurrentIpAddress(),
                    CreateDate = DateTime.Now
                });

                //int rewardPointHistoryId = _rewardPointService.AddRewardPointsHistoryEntry(shipment.Order.Customer, price,
                //      shipment.Order.StoreId,
                //      "واریز مبلغ کالا برای محموله پس کرایه شماره" + " " + shipment.Id + " از سفارش شماره " + " " + shipment.OrderId +
                //      "_" + "AddCodPrice", usedAmount: 0);
                //if (rewardPointHistoryId > 0)
                //{

                //    _repositoryPayForCodLog.Insert(new PayForCodLogModel()
                //    {
                //        RewardPointHistoryId = rewardPointHistoryId,
                //        ShipmentId = shipment.Id
                //    });
                //    Msg = "واریز به کیف پول با موفقیت انجام شد";
                //    return true;
                //}

                Msg = "واریز به کیف پول با موفقیت انجام شد";
                return false;
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                _chargeWalletFailService.InsertFailedLog(ex, new { shipmentId, price }, "CodService.ChargeMoneyBagForCodGood");
                throw;
            }

        }
        public bool ChangeStatus(int Status, string trackingNumber, out string result)
        {
            result = "در حال حاضر این امکان غیر فعال است";
            return true;
            result = "";
            var str_Result = ChangeStatus(trackingNumber, Status);
            if (str_Result == "0")
            {
                result = "تغییرات محموله سفارش پس کرایه با موفقیت انجام شد";
                return true;
            }
            result = GateWayError.GetErrorMsg(str_Result);
            return false;
        }
        public bool ShipmentChangeStatus(int Status, string trackingNumber, out string result)
        {
            result = "";
            var str_Result = ChangeStatus(trackingNumber, Status);
            if (str_Result == "0")
            {
                result = "تغییرات محموله سفارش پس کرایه با موفقیت انجام شد";
                return true;
            }
            result = GateWayError.GetErrorMsg(str_Result);
            return false;
        }
        public ShipmentTrackingModel GetStatus(string trackingNumber, out string result)
        {
            var shipment = _repositoryShipment.Table.FirstOrDefault(p => p.TrackingNumber == trackingNumber);
            if (shipment == null)
            {
                result = "محموله مورد نظر یافت نشد";
                return null;
            }

            var strResult = GetStatus(trackingNumber);
            if (strResult.Contains(";"))
            {
                var statusItem = strResult.Split(';');
                var datepart = statusItem[1].Substring(0, 4) + "/" + statusItem[1].Substring(4, 2) + "/" + statusItem[1].Substring(6, 2);
                result = "";
                return new ShipmentTrackingModel()
                {
                    ShipmentId = shipment.Id,
                    CODShipmentEventId = int.Parse(statusItem[0]),
                    EventName = getCodStatusName(statusItem[0]),
                    LastShipmentEventDate = Convert.ToDateTime(datepart)
                };
            }
            else
            {
                result = GateWayError.GetErrorMsg(strResult);
                return null;
            }
        }
        public string getCodStatusName(string statusCode)
        {
            var result = _repositoryCODShipmentEvent.Table.SingleOrDefault(p => p.EventCode == statusCode);
            return result == null ? "" : result.EventName;
        }
        public List<CodShipmentFinancialModel> getBilling(string startDate, string endDate)
        {
            string result = Billing(int.Parse(startDate.Replace("/", ""))
                 , int.Parse(endDate.Replace("/", "")));
            var data = result.Split(';');
            var listTrackingNUmbers = new List<CodShipmentFinancialModel>();

            foreach (var item in data)
            {
                var dataPart = item.Split('^');
                DateTime VarizDate = Convert.ToDateTime(dataPart[1].Substring(0, 4) + "/" + dataPart[1].Substring(4, 2) + "/" + dataPart[1].Substring(6, 2));
                var dataCoditem = getBillingDetailes(dataPart[0], VarizDate, null);
                if (dataCoditem != null)
                {
                    listTrackingNUmbers.AddRange(dataCoditem);
                }
            }
            return listTrackingNUmbers;
        }
        public List<CodShipmentFinancialModel> getBillingDetailes(string settleCode, DateTime VarizDate, WebService1SoapClient CODSoapClient = null)
        {

            var listTrackingNumber = new List<CodShipmentFinancialModel>();
            var result2 = Billing2(settleCode
                , 1
                , 1);
            var codItems = result2.Split(';');
            foreach (var codOrder in codItems)
            {
                if (codOrder == "") continue;
                var shipmentId = _repositoryShipment.Table.FirstOrDefault(p => p.TrackingNumber == codOrder.Split('^')[0])?.Id;
                if (!shipmentId.HasValue)
                    continue;
                listTrackingNumber.Add(new CodShipmentFinancialModel()
                {
                    shipmentId = shipmentId.Value,
                    EngAndKalaPrice = int.Parse(codOrder.Split('^')[3]),
                    SumFraction = int.Parse(codOrder.Split('^')[4]),
                    VairzCode = codOrder.Split('^')[2],
                    Tax = int.Parse(codOrder.Split('^')[5]),
                    VarizDate = VarizDate
                });
            }
            return listTrackingNumber;
        }
        public List<SelectListItem> getCODEventList()
        {
            return _repositoryCODShipmentEvent.Table.Select(p => new SelectListItem()
            {
                Text = p.EventName,
                Value = p.EventCode
            }).ToList();
        }

        public List<string> GetTrackingNumbersByUniqueReferenceNo(string UniqueReferenceNo)
        {
            string query = $@"SELECT 
                                TrackingNumber 
                              FROM dbo.Tbl_ApiOrderRefrenceCode
                                INNER JOIN dbo.[Order] ON [Order].Id = Tbl_ApiOrderRefrenceCode.OrderId
                                INNER JOIN dbo.Shipment ON Shipment.OrderId = [Order].Id
                              WHERE RefrenceCode = {UniqueReferenceNo} AND TrackingNumber IS NOT NULL";
            return _dbcontext.SqlQuery<string>(query, new object[0]).ToList();
        }

        public string GetPrice(int Weight, int Price, string Shcode, int State, int City, int Tip, int Cod, int Showtype)
        {
            string uri = BasUri + $"/GetPrice?Username={ApiUserName}&Password={ApiPassword}&Weight={Weight}&Price={Price}&Shcode={Shcode}&State={State}&City={City}&Tip={Tip}&Cod={Cod}&Showtype={Showtype}";
            return SendGetRequest(uri);
        }
        public string NewOrder2(string EncodedOrderDetailes, string Ordertip)
        {
            return SendPostRequest(BasUri, new
                NewOrderModel()
            {
                EncodedOrderDetailes = EncodedOrderDetailes,
                Ordertip = Ordertip,
                Password = ApiPassword,
                Username = ApiUserName
            });
        }
        public string ChangeStatus(string trackingNumber, int Status)
        {
            string uri = BasUri + $"/ChangeStatus?Username={ApiUserName}&Password={ApiPassword}&trackingNumber={trackingNumber}&Status={Status}";
            return SendGetRequest(uri);
        }
        public string GetStatus(string trackingNumber)
        {
            string uri = BasUri + $"/GetStatus?Username={ApiUserName}&Password={ApiPassword}&trackingNumber={trackingNumber}";
            return SendGetRequest(uri);
        }

        public string Billing(int Min, int Max)
        {
            string uri = BasUri + $"/GetStatus?Username={ApiUserName}&Password={ApiPassword}&Min={Min}&Max={Max}";
            return SendGetRequest(uri);
        }
        public string Billing2(string trackingNumber, int Tip, int Page)
        {
            string uri = BasUri + $"/GetStatus?Username={ApiUserName}&Password={ApiPassword}&trackingNumber={trackingNumber}&Tip={Tip}&Page={Page}";
            return SendGetRequest(uri);
        }

        public ShopResisterResult RegisterGatewayShop(Shop model)
        {
            model.ManagerBirthDate= model.ManagerBirthDate.Replace("." , "/"); 
            model.PostalCode = model.PostalCode.ToEnDigitString();
            model.ManagerNationalID = model.ManagerNationalID.ToEnDigitString();    
            model.Mobile=   model.Mobile.ToEnDigitString();   
            model.AccountNumber= model.AccountNumber.ToEnDigitString();  
            model.Phone= model.Phone.ToEnDigitString();

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new RestClient(BasUri);
            //var client = new RestClient("http://localhost:5000/api/Gateway");
            var request = new RestRequest("/RegisterShop/", Method.POST);
            string jsonToSend = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            ShopResisterResult reuslt = new ShopResisterResult();
            try
            {
                var reposnce = client.Execute(request);
                if (reposnce.StatusCode == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrEmpty(reposnce.Content))
                    {
                        reuslt = Newtonsoft.Json.JsonConvert.DeserializeObject<ShopResisterResult>(reposnce.Content);
                        return reuslt;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                return null;
            }
        }

        public List<ShopInfo> GetShopList()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new RestClient(BasUri);
            //var client = new RestClient("http://localhost:5000/api/Gateway");
            var request = new RestRequest("/GetShopList/", Method.POST);
            request.RequestFormat = DataFormat.Json;
            List<ShopInfo> reuslt = new List<ShopInfo>();
            try
            {
                var reposnce = client.Execute(request);
                if (reposnce.StatusCode == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrEmpty(reposnce.Content))
                    {
                        var _reuslt = Newtonsoft.Json.JsonConvert.DeserializeObject<ShopinfoResult>(reposnce.Content);
                        return _reuslt.ShopInfos;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                return null;
            }
        }
        private string SendGetRequest(string uri)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.Method = "GET";
            string result = "";
            using (var response = httpWebRequest.GetResponse() as HttpWebResponse)
            {
                if (httpWebRequest.HaveResponse && response != null)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }
                    if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(result))
                    {
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        private string SendPostRequest(string uri, NewOrderModel model)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new RestClient(BasUri);
            var request = new RestRequest("/NewOrder2/", Method.POST);
            string jsonToSend = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            try
            {
                var reposnce = client.Execute(request);
                if (reposnce.StatusCode == HttpStatusCode.OK)
                {
                    return reposnce.Content.Replace("\"", "");
                }
                else if (reposnce.StatusCode == HttpStatusCode.OK)
                {

                }
                return null;
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                return null;
            }
        }
        #region model
        public class ShopResisterResult
        {
            public int ShopId { get; set; }
            public string Message { get; set; }
            public bool UnhandledMessage { get; set; }
            public int ReposnceStatusCode { get; set; }
        }
        public class NewOrderModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string EncodedOrderDetailes { get; set; }
            public string Ordertip { get; set; }
        }
        public class Shop
        {
            public int? ShopID { get; set; }
            public string ShopUsername { get; set; }
            public string Shopname { get; set; }
            public string Phone { get; set; }
            public string PostalCode { get; set; }
            public string ManagerName { get; set; }
            public string ManagerFamily { get; set; }
            public string ManagerNationalID { get; set; }
            public string ManagerNationalIDSerial { get; set; }
            public string ManagerBirthDate { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
            public int? ReciptNumber { get; set; }
            public string ReciptDate { get; set; }
            public string StartDate { get; set; }
            public string EndDate { get; set; }
            public string WebSiteURL { get; set; }
            public int? CityID { get; set; }
            public string AccountNumber { get; set; }
            public string AccountIBAN { get; set; }
            public string AccountBranchName { get; set; }
            public int NeedToCollect { get; set; }
        }
        public class ShopinfoResult
        {
            public List<ShopInfo> ShopInfos { get; set; }
            public string Message { get; set; }
            public bool UnhandledMessage { get; set; }
            public int ReposnceStatusCode { get; set; }
        }
        public class ShopInfo
        {
            public int ShopID { get; set; }
            public int ShopStatusCode { get; set; }
            public string ShopStatus { get; set; }
            public string ShopName { get; set; }
            public string ShopUsername { get; set; }
        } 
        #endregion
    }
}
