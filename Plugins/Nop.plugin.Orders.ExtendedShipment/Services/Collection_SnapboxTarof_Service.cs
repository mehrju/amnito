using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.Plugin.Misc.ShippingSolutions;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Snappbox;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Snappbox;
using Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Shipping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Terminal = Nop.Plugin.Misc.ShippingSolutions.Models.Params.Snappbox.Terminal;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class Collection_SnapboxTarof_Service: ICollection_SnapboxTarof_Service
    {
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly Plugin.Misc.ShippingSolutions.ShippingSettings _ShippingSettings;
        private readonly ISettingService _settingService;
        public readonly ISnappbox_Service _snappbox_service;
        public readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IShipmentService _shipmentService;
        private readonly IRepository<Tbl_Collection_Snappbaox_Tarof> _repositoryTbl_Collection_Snappbaox_Tarof;

        public Collection_SnapboxTarof_Service
            (ILogger logger, IWorkContext workContext, Plugin.Misc.ShippingSolutions.ShippingSettings ShippingSettings,
            ISettingService settingService,
            ISnappbox_Service snappbox_service,
            IShipmentService shipmentService,
            IRepository<Tbl_Collection_Snappbaox_Tarof> repositoryTbl_Collection_Snappbaox_Tarof

            )
        {
            _logger = logger;
            _workContext = workContext;
            this._settingService = settingService;
            this._ShippingSettings = Configuration();
            _snappbox_service = snappbox_service;
            _shipmentService = shipmentService;
            _repositoryTbl_Collection_Snappbaox_Tarof = repositoryTbl_Collection_Snappbaox_Tarof;
        }
        /// <summary>
        /// خواندن تنظیمات فروشگاه دیفالت
        /// </summary>
        /// <returns></returns>
        private Plugin.Misc.ShippingSolutions.ShippingSettings Configuration()
        {
            var _storeContext = EngineContext.Current.Resolve<IStoreContext>();
            var Settings = _settingService.LoadSetting<Plugin.Misc.ShippingSolutions.ShippingSettings>(_storeContext.CurrentStore.Id);
            return Settings;
        }


        public async void Collection(Param_Collection_SnapboxTarof_Service model)
        {
            if (model.Items.Count > 0)
            {
                foreach (var item in model.Items)
                {
                    //CheckParams ///item.Origin.latitude>0 && item.Origin.longitude > 0 &&
                    if (item.Type >= 0 && item.shipmentid > 0 && item.Destination.longitude > 0 && item.Destination.latitude > 0)
                    {
                        //List<Tbl_Collection_Snappbaox_Tarof> listtbl = new List<Tbl_Collection_Snappbaox_Tarof>();
                        Tbl_Collection_Snappbaox_Tarof listtbl = new Tbl_Collection_Snappbaox_Tarof();

                        #region insert into tabl جهت پیگیری
                        Random rnd = new Random();
                        int ran2 = rnd.Next(1, 999999999);
                        Tbl_Collection_Snappbaox_Tarof temp = new Tbl_Collection_Snappbaox_Tarof();
                        temp.DateInsert = DateTime.Now;
                        temp.Date_Statuse = DateTime.Now;
                        temp.Id_Request = ran2;
                        temp.ShipmentId = item.shipmentid;//item foreach
                        temp.Status = 1;//ثبت موقت
                        temp.TypeRequest = item.Type;
                        temp.orderId = 0;
                        temp.UserIdInsert = _workContext.CurrentCustomer.Id;
                        _repositoryTbl_Collection_Snappbaox_Tarof.Insert(temp);
                        listtbl = temp;


                        //foreach (var itemtbl in item.shipmentid)
                        //{

                        //}



                        #endregion

                        #region  Get Info Shipment and Calculate Weight
                        decimal TotalWeight = 0;
                        int maxDimensions = 45;
                        bool State_maxDimensions = true;
                        var shipment = _shipmentService.GetShipmentById(item.shipmentid);
                        var _oderItem = shipment.Order.OrderItems.FirstOrDefault(p => p.Id == shipment.ShipmentItems.First().OrderItemId);
                        if (shipment == null)
                        {
                            continue;
                        }
                        TotalWeight = shipment.TotalWeight != null ? shipment.TotalWeight.GetValueOrDefault() : 0;
                        //weight
                        if (20 > maxDimensions)
                        {
                            State_maxDimensions = false;
                        }

                        #endregion

                        switch (item.Type)
                        {

                            case 0:
                                #region Inquiry
                                #endregion
                                break;
                            case 1:
                                #region Snapbox


                                #region استعلام قیمت اسنپ باکس و ذخیره ان در جدول
                                Tbl_Collection_Snappbaox_Tarof tempinquery = new Tbl_Collection_Snappbaox_Tarof();
                                tempinquery.orderId = 0;
                                tempinquery.DateInsert = DateTime.Now;
                                tempinquery.Date_Statuse = DateTime.Now;
                                tempinquery.Id_Request = listtbl.Id_Request;
                                tempinquery.ShipmentId = listtbl.ShipmentId;
                                tempinquery.Status = 2;//استعلام قیمت



                                #region استعلام قیمت اسنپ باکس

                                #region prepar param

                                Params_Snappbox_Get_Price paraminquery = new Params_Snappbox_Get_Price();
                                paraminquery.id = null;
                                paraminquery.customerId = _ShippingSettings.Snappbox_CustomerId;
                                paraminquery.city = _shipmentService.GetShipmentById(listtbl.ShipmentId).Order.BillingAddress.City;
                                //if (shipment.TotalWeight < 25)
                                //{
                                    paraminquery.deliveryCategory = "bike-without-box";
                                //}
                                paraminquery.deliveryFarePaymentType = "prepaid";
                                paraminquery.isReturn = false;
                                paraminquery.pricingId = null;
                                paraminquery.sequenceNumberDeliveryCollection = 1;
                                paraminquery.totalFare = null;
                                paraminquery.voucherCode = null;
                                paraminquery.waitingTime = 0;
                                paraminquery.customerWalletType = null;
                                paraminquery.terminals = new List<Terminal>();

                                Terminal m = new Terminal();
                                m.id = null;
                                m.contactName = shipment.Order.BillingAddress.FirstName ?? "" + " " + shipment.Order.BillingAddress.LastName ?? "";
                                m.address = shipment.Order.BillingAddress.Address1;
                                m.contactPhoneNumber = shipment.Order.BillingAddress.PhoneNumber;
                                m.plate = "";
                                m.sequenceNumber = 1;
                                m.unit = "";
                                m.comment = "";
                                m.latitude = 0;
                                m.longitude = 0;
                                m.type = "pickup";
                                m.collectCash = "no";
                                m.paymentType = "prepaid";
                                m.cashOnPickup = 0;
                                m.cashOnDelivery = 0;
                                m.isHub = null;
                                m.vendorId = null;

                                paraminquery.terminals.Add(m);
                                ////************************************
                                Terminal g = new Terminal();
                                g.id = null;
                                g.contactName = item.Destination.contactName;
                                g.address = item.Destination.address;
                                g.contactPhoneNumber = item.Destination.contactPhoneNumber;
                                g.plate = "";
                                g.sequenceNumber = 2;
                                g.unit = "";
                                g.comment = item.Destination.comment;
                                g.latitude = item.Destination.latitude;
                                g.longitude = item.Destination.longitude;
                                g.type = "drop";
                                g.collectCash = "no";
                                g.paymentType = "prepaid";
                                g.cashOnPickup = 0;
                                g.cashOnDelivery = 0;
                                g.isHub = null;
                                g.vendorId = null;

                                paraminquery.terminals.Add(g);




                                #endregion

                                Result_Snappbox_GetPrice resultserviceinquery = new Result_Snappbox_GetPrice();
                                resultserviceinquery =await _snappbox_service.GetPrice(paraminquery);
                                if (resultserviceinquery.Status)
                                {
                                    tempinquery.Mablagh_Induery = resultserviceinquery.Detail_Result_Snappbox_GetPrice.totalFare;
                                }


                                #endregion
                                tempinquery.TypeRequest = listtbl.TypeRequest;
                                tempinquery.UserIdInsert = _workContext.CurrentCustomer.Id;
                                _repositoryTbl_Collection_Snappbaox_Tarof.Insert(tempinquery);
                                //foreach (var iteminquery in listtbl)
                                //{


                                //}
                                #endregion

                                #region Create Order and Prepar model 

                                if (TotalWeight <= 25)
                                {
                                    Params_Snappbox_create_order param = new Params_Snappbox_create_order();
                                    param.customerId = _ShippingSettings.Snappbox_CustomerId;
                                    #region prepar model creat order
                                    #region order detail
                                    param.data.orderDetails.packageSize = 1;
                                    param.data.orderDetails.city = _shipmentService.GetShipmentById(listtbl.ShipmentId).Order.BillingAddress.City;//*****
                                    param.data.orderDetails.deliveryFarePaymentType = "prepaid";
                                    param.data.orderDetails.isReturn = false;
                                    param.data.orderDetails.pricingId = "";
                                    param.data.orderDetails.sequenceNumberDeliveryCollection = 1;
                                    param.data.orderDetails.totalFare = 0;
                                    param.data.orderDetails.customerRefId = listtbl.Id_Request.ToString();
                                    param.data.orderDetails.voucherCode = null;
                                    param.data.orderDetails.waitingTime = 0;

                                    #endregion
                                    #region orgin
                                    //foreach (var item3 in listtbl)
                                    //{

                                    //}

                                    ItemDetail id = new ItemDetail();
                                    id.pickedUpSequenceNumber = 1;
                                    id.dropOffSequenceNumber = 1;
                                    id.name = _extendedShipmentService.getOrderItemContent(_oderItem);
                                    id.quantity = 1;
                                    id.quantityMeasuringUnit = "unit";
                                    id.packageValue = _extendedShipmentService.getApproximateValue(_oderItem.Id);
                                    id.createdAt = "";
                                    id.updatedAt = "";
                                    param.data.itemDetails.Add(id);


                                    PickUpDetail pd = new PickUpDetail();

                                    pd.id = null;
                                    pd.contactName = shipment.Order.BillingAddress.FirstName ?? "" + " " + shipment.Order.BillingAddress.LastName ?? "";
                                    pd.address = shipment.Order.BillingAddress.Address1;
                                    pd.contactPhoneNumber = shipment.Order.BillingAddress.PhoneNumber;
                                    pd.plate = "";
                                    pd.sequenceNumber = 1;
                                    pd.unit = "";
                                    pd.comment = "";
                                    pd.latitude = 0;
                                    pd.longitude = 0;
                                    pd.type = "pickup";
                                    pd.collectCash = "no";
                                    pd.paymentType = "prepaid";
                                    pd.cashOnPickup = 0;
                                    pd.cashOnDelivery = 0;
                                    pd.isHub = null;
                                    pd.vendorId = null;

                                    param.data.pickUpDetails.Add(pd);
                                    #endregion
                                    #region destination

                                    DropOffDetail dd = new DropOffDetail();
                                    dd.id = null;
                                    dd.contactName = item.Destination.contactName;
                                    dd.address = item.Destination.address;
                                    dd.contactPhoneNumber = item.Destination.contactPhoneNumber;
                                    dd.plate = "";
                                    dd.sequenceNumber = 1;
                                    dd.unit = "";
                                    dd.comment = item.Destination.comment;
                                    dd.latitude = item.Destination.latitude;
                                    dd.longitude = item.Destination.longitude;
                                    dd.type = "drop";
                                    dd.collectCash = "no";
                                    dd.paymentType = "prepaid";
                                    dd.cashOnPickup = 0;
                                    dd.cashOnDelivery = 0;
                                    dd.isHub = null;
                                    dd.vendorId = null;

                                    param.data.dropOffDetails.Add(dd);
                                    #endregion
                                    #endregion
                                    //if (State_maxDimensions)
                                    //{
                                    //    //bicker  
                                    //    param.data.orderDetails.deliveryCategory = "bike";

                                    //}
                                    //else
                                    //{
                                        //bike-without-box
                                        param.data.orderDetails.deliveryCategory = "bike-without-box";

                                    ///}
                                    #region CreateOrder and save result;
                                    Result_Snappbox_CreateOrder resultservice = new Result_Snappbox_CreateOrder();
                                    resultservice =await  _snappbox_service.CreateOrder(param);
                                    if (resultservice.Status)
                                    {
                                        Tbl_Collection_Snappbaox_Tarof tempsuccess = new Tbl_Collection_Snappbaox_Tarof();
                                        tempsuccess.orderId = resultservice.DetailResult_Snappbox_CreateOrder.data.orderId;
                                        tempsuccess.DateInsert = DateTime.Now;
                                        tempsuccess.Date_Statuse = DateTime.Now;
                                        tempsuccess.Id_Request = listtbl.Id_Request;
                                        tempsuccess.ShipmentId = listtbl.ShipmentId;
                                        tempsuccess.Status = 3;//ارسال شده
                                        tempsuccess.TypeRequest = listtbl.TypeRequest;
                                        tempsuccess.UserIdInsert = _workContext.CurrentCustomer.Id;
                                        _repositoryTbl_Collection_Snappbaox_Tarof.Insert(tempsuccess);
                                        //foreach (var itemupdate in listtbl)
                                        //{
                                        //}
                                    }
                                    else
                                    {
                                        Tbl_Collection_Snappbaox_Tarof temperror = new Tbl_Collection_Snappbaox_Tarof();
                                        temperror.DateInsert = DateTime.Now;
                                        temperror.Date_Statuse = DateTime.Now;
                                        temperror.Id_Request = listtbl.Id_Request;
                                        temperror.ShipmentId = listtbl.ShipmentId;
                                        temperror.Status = 0;//خطا
                                        temperror.TypeRequest = listtbl.TypeRequest;
                                        temperror.UserIdInsert = _workContext.CurrentCustomer.Id;
                                        temperror.Description_log = resultservice.Message;
                                        _repositoryTbl_Collection_Snappbaox_Tarof.Insert(temperror);
                                        //foreach (var itemupdate in listtbl)
                                        //{
                                        //}
                                    }
                                    #endregion
                                }
                                else
                                {
                                    //divide
                                    //List<Params_Snappbox_create_order> params = new List<Params_Snappbox_create_order>();
                                    // param.customerId = _ShippingSettings.Snappbox_CustomerId;
                                    #region CreateOrder and save result;
                                    #endregion
                                }

                                #endregion

                                #endregion
                                break;
                            case 2:
                                #region Tarof
                                #endregion
                                break;
                            default:
                                break;
                        }

                    }

                }
            }
        }


    }
}
