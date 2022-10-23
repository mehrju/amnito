using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Enums;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.PostTracking;
using Nop.plugin.Product.ExtendedShipment;
using Nop.Plugin.Misc.ShippingSolutions.Services;
using Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Shipping;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class ShipmentTrackingService : IShipmentTrackingService
    {
        #region Field
        private readonly IRepository<Order> _orderRepository;

        private readonly IRepository<ShipmentTrackingModel> _repositoryShipmentTracking;
        private readonly IRepository<ShipmentEventModel> _repositoryShipmentEvent;
        private readonly IRepository<CODShipmentEventModel> _repositoryCodShipmentEvent;
        private readonly IRepository<Shipment> _repositoryShipment;
        private readonly IRepository<OrderItemAttributeValueModel> _OrderItemAttributeValueRepository;
        private readonly ICodService _codService;
        private readonly IShipmentService _shipmentService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IRepository<Shipment> _shipmentRepository;
        private readonly IDbContext _dbContext;
        private readonly ICategoryService _CategoryService;
        private readonly Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces.ISafiran_Service _safiran_Service;
        private readonly Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces.IChapar_Service _chapar_Service;
        private readonly IPDE_Service _pDE_Service;
        //private readonly Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces.IPersian_Service _Persian_Service;
        private readonly ISnappbox_Service _Snappbox_Service;
        private readonly ITaroff_Service _Taroff_Service;
        //private readonly ITinex_Service _Tinex_Service;
        private readonly ITPG_Service _TPG_Service;
        private readonly IYarBox_Service _YarBox_Service;
        private readonly IUbaar_Service _ubaar_Service;
        private readonly ISkyBlue_Service _skyBlue_service;
        private readonly IWorkContext _workContext;
        #endregion

        #region ctor
        public ShipmentTrackingService(
          IRepository<OrderItemAttributeValueModel> OrderItemAttributeValueRepository,
          ICategoryService CategoryService,
          IRepository<Shipment> repositoryShipment,
          ISafiran_Service safiran_Service,
          IRepository<ShipmentTrackingModel> repositoryShipmentTracking,
          IRepository<ShipmentEventModel> repositoryShipmentEvent,
          IRepository<CODShipmentEventModel> repositoryCodShipmentEvent,
          ICodService codService,
          IShipmentService shipmentService,
          ISettingService settingService,
          IStoreContext storeContext,
          IRepository<Order> orderRepository,
          IRepository<Shipment> shipmentRepository,
          IDbContext dbContext,
          IChapar_Service chapar_Service,
          IPDE_Service pDE_Service,
          //IPersian_Service Persian_Service,
          ISnappbox_Service Snappbox_Service,
          ITaroff_Service Taroff_Service,
          //ITinex_Service Tinex_Service,
          ITPG_Service TPG_Service,
          IYarBox_Service YarBox_Service,
          IUbaar_Service ubaar_Service,
          ISkyBlue_Service skyBlue_service,
          IWorkContext workContext
          )
        {
            _OrderItemAttributeValueRepository = OrderItemAttributeValueRepository;
            _CategoryService = CategoryService;
            _pDE_Service = pDE_Service;
            _chapar_Service = chapar_Service;
            _repositoryShipment = repositoryShipment;
            _safiran_Service = safiran_Service;
            _dbContext = dbContext;
            _repositoryShipmentTracking = repositoryShipmentTracking;
            _repositoryShipmentEvent = repositoryShipmentEvent;
            _repositoryCodShipmentEvent = repositoryCodShipmentEvent;
            _codService = codService;
            _shipmentService = shipmentService;
            _settingService = settingService;
            _storeContext = storeContext;
            _orderRepository = orderRepository;
            _shipmentRepository = shipmentRepository;
            //_Persian_Service = Persian_Service;
            _Snappbox_Service = Snappbox_Service;
            _Taroff_Service = Taroff_Service;
            //_Tinex_Service = Tinex_Service;
            _TPG_Service = TPG_Service;
            _YarBox_Service = YarBox_Service;
            _ubaar_Service = ubaar_Service;
            _skyBlue_service = skyBlue_service;
            _workContext = workContext;
        }
        #endregion

        #region Methods racking services
        public Plugin.Misc.ShippingSolutions.Models.Results.Safiran.Result_Safiran_Tracking TrackDtsShipment(int shipmentId, string TrackingNumber)
        {
            Shipment shipment = null;
            if (!string.IsNullOrEmpty(TrackingNumber))
                shipment = getShipmentByTrackingNUmber(TrackingNumber);
            else if (shipmentId != 0)
                shipment = _shipmentService.GetShipmentById(shipmentId);
            if (shipment != null)
            {
                var result = _safiran_Service.Tracking(new Plugin.Misc.ShippingSolutions.Models.Params.Safiran.Params_Safiran_Tracking()
                {
                    order = new Plugin.Misc.ShippingSolutions.Models.Params.Safiran.OrderTracking()
                    {
                        lang = "fa",
                        reference = shipment.TrackingNumber
                    }
                });
                if (result.Status)
                {
                    if (shipment != null)
                    {
                        if (result.objects.order.history.Any() && result.objects.order.history.Count() > 1)
                        {
                            var historyItem = result.objects.order.history.OrderBy(p => p.timestamp_date).ElementAt(1);
                            if (historyItem != null && historyItem.status.StartsWith("IR "))
                            {
                                shipment.ShippedDateUtc = GetDtsDate(historyItem.date);
                                _shipmentService.UpdateShipment(shipment);
                            }
                        }
                        if (!shipment.DeliveryDateUtc.HasValue && !string.IsNullOrEmpty(result.objects.order.delivered_to))
                        {
                            shipment.DeliveryDateUtc = GetDtsDate(result.objects.order.delivery_time);
                            _shipmentService.UpdateShipment(shipment);
                        }
                    }
                    return result;
                }
            }
            else
            {
                var result = _safiran_Service.Tracking(new Plugin.Misc.ShippingSolutions.Models.Params.Safiran.Params_Safiran_Tracking()
                {
                    order = new Plugin.Misc.ShippingSolutions.Models.Params.Safiran.OrderTracking()
                    {
                        lang = "fa",
                        reference = TrackingNumber
                    }
                });
                return result;
            }

            return null;
        }

        private static DateTime GetDtsDate(string date)
        {
            string[] DatePart = date.Split(' ');
            var dateItem = DatePart[0].Split('/');
            var dd = Convert.ToDateTime(dateItem[2] + "/" + dateItem[1] + "/" + dateItem[0] + " " + DatePart[1]).ToUniversalTime();
            return dd;
        }

        public Plugin.Misc.ShippingSolutions.Models.Results.Chapar.Result_Chapar_Tracking TrackChaparShipment(int shipmentId, string TrackingNumber)
        {
            Shipment shipment = null;
            if (!string.IsNullOrEmpty(TrackingNumber))
                shipment = getShipmentByTrackingNUmber(TrackingNumber);
            else if (shipmentId != 0)
                shipment = _shipmentService.GetShipmentById(shipmentId);

            if (shipment != null)
            {
                var result = _chapar_Service.Tracking(new Plugin.Misc.ShippingSolutions.Models.Params.Chapar.Params_Chapar_Tracking
                {
                    order = new Plugin.Misc.ShippingSolutions.Models.Params.Chapar.OrderTracking()
                    {
                        lang = "fa",
                        reference = shipment.TrackingNumber
                    }
                });
                if (result.Status)
                {
                    if (shipment != null)
                    {
                        if (result.objects.order.history.Any())
                        {
                            if (result.objects.order.history.Count() > 1)
                            {
                                var historyItem = result.objects.order.history.Where(p => p.status.StartsWith("IR ") || p.status.StartsWith("OS ")).OrderBy(p => p.timestamp_date).FirstOrDefault();

                                if (historyItem != null)
                                {
                                    shipment.ShippedDateUtc = UnixTimeStampToDateTime(historyItem.timestamp_date).ToUniversalTime();
                                    _shipmentService.UpdateShipment(shipment);
                                }
                            }
                        }
                        if (!shipment.DeliveryDateUtc.HasValue && !string.IsNullOrEmpty(result.objects.order.delivered_to))
                        {
                            shipment.DeliveryDateUtc = GetDtsDate(result.objects.order.delivery_time);
                            _shipmentService.UpdateShipment(shipment);
                        }
                    }
                    return result;
                }
            }
            else
            {
                var result = _chapar_Service.Tracking(new Plugin.Misc.ShippingSolutions.Models.Params.Chapar.Params_Chapar_Tracking
                {
                    order = new Plugin.Misc.ShippingSolutions.Models.Params.Chapar.OrderTracking()
                    {
                        lang = "fa",
                        reference = TrackingNumber
                    }
                });
                return result;
            }

            return null;
        }


        public Plugin.Misc.ShippingSolutions.Models.Results.PDE.Result_PDE_TrackingParcels TrackPDEShipment(int shipmentId, string TrackingNumber)
        {
            Shipment shipment = null;
            if (!string.IsNullOrEmpty(TrackingNumber))
                shipment = getShipmentByTrackingNUmber(TrackingNumber);
            else if (shipmentId != 0)
                shipment = _shipmentService.GetShipmentById(shipmentId);

            if (shipment != null)
            {
                var result = _pDE_Service.TrackingParcels(new Plugin.Misc.ShippingSolutions.Models.Params.PDE.Params_PDE_TrackingParcels
                {
                    IdOrder = Convert.ToInt64(shipment.TrackingNumber)
                });

                if (result.Status)
                {
                    if (shipment != null)
                    {
                        //if (result.DetailOrder != null)
                        //{
                        //    if (result.objects.order.history.Count() > 1)
                        //    {
                        //        var historyItem = result.objects.order.history.OrderBy(p => p.timestamp_date).Skip(1).Take(1).FirstOrDefault();
                        //        if (historyItem != null && historyItem.status.StartsWith("IR "))
                        //        {
                        //            string[] DatePart = historyItem.date.Split(' ');
                        //            var dateItem = DatePart[0].Split('/');
                        //            shipment.ShippedDateUtc = Convert.ToDateTime(dateItem[2] + "/" + dateItem[1] + "/" + dateItem[0] + " " + DatePart[1]).ToUniversalTime();
                        //            _shipmentService.UpdateShipment(shipment);
                        //        }
                        //    }
                        //}
                        //if (!shipment.DeliveryDateUtc.HasValue && !string.IsNullOrEmpty(result.objects.order.delivered_to))
                        //{
                        //    string[] DatePart = result.objects.order.delivery_time.Split(' ');
                        //    var dateItem = DatePart[0].Split('/');
                        //    shipment.DeliveryDateUtc = Convert.ToDateTime(dateItem[2] + "/" + dateItem[1] + "/" + dateItem[0] + " " + DatePart[1]).ToUniversalTime();
                        //    _shipmentService.UpdateShipment(shipment);
                        //}
                    }
                    return result;
                }
            }
            else
            {
                var result = _pDE_Service.TrackingParcels(new Plugin.Misc.ShippingSolutions.Models.Params.PDE.Params_PDE_TrackingParcels
                {
                    IdOrder = Convert.ToInt64(shipment.TrackingNumber)
                });
                return result;
            }

            return null;
        }


        public Plugin.Misc.ShippingSolutions.Models.Results.YarBox.Result_YarBox_Tracking TrackYarBoxShipment(int shipmentId, string TrackingNumber)
        {
            Shipment shipment = null;
            if (!string.IsNullOrEmpty(TrackingNumber))
                shipment = getShipmentByTrackingNUmber(TrackingNumber);
            else if (shipmentId != 0)
                shipment = _shipmentService.GetShipmentById(shipmentId);

            if (shipment != null)
            {
                var result = _YarBox_Service.Tracking(new Plugin.Misc.ShippingSolutions.Models.Params.YarBox.Params_YarBox_Tracking
                {
                    id = Convert.ToInt64(shipment.TrackingNumber)
                });

                if (result.Status)
                {
                    //if (shipment != null)
                    //{
                    //if (result.Detail_Tracking.status != null)
                    //{
                    //if (result.objects.order.history.Count() > 1)
                    //{
                    //    var historyItem = result.objects.order.history.OrderBy(p => p.timestamp_date).Skip(1).Take(1).FirstOrDefault();
                    //    if (historyItem != null && historyItem.status.StartsWith("IR "))
                    //    {
                    //        string[] DatePart = historyItem.date.Split(' ');
                    //        var dateItem = DatePart[0].Split('/');
                    //        shipment.ShippedDateUtc = Convert.ToDateTime(dateItem[2] + "/" + dateItem[1] + "/" + dateItem[0] + " " + DatePart[1]).ToUniversalTime();
                    //        _shipmentService.UpdateShipment(shipment);
                    //    }
                    //}
                    //}
                    //if (!shipment.DeliveryDateUtc.HasValue && !string.IsNullOrEmpty(result.objects.order.delivered_to))
                    //{
                    //    string[] DatePart = result.objects.order.delivery_time.Split(' ');
                    //    var dateItem = DatePart[0].Split('/');
                    //    shipment.DeliveryDateUtc = Convert.ToDateTime(dateItem[2] + "/" + dateItem[1] + "/" + dateItem[0] + " " + DatePart[1]).ToUniversalTime();
                    //    _shipmentService.UpdateShipment(shipment);
                    //}
                    //}
                    return result;
                }
            }
            else
            {
                var result = _YarBox_Service.Tracking(new Plugin.Misc.ShippingSolutions.Models.Params.YarBox.Params_YarBox_Tracking
                {
                    id = Convert.ToInt64(TrackingNumber)
                });
                return result;
            }

            return null;
        }


        public Plugin.Misc.ShippingSolutions.Models.Results.TGP.Result_TPG_Tracking TrackTGPShipment(int shipmentId, string TrackingNumber)
        {
            Shipment shipment = null;
            if (!string.IsNullOrEmpty(TrackingNumber))
                shipment = getShipmentByTrackingNUmber(TrackingNumber);
            else if (shipmentId != 0)
                shipment = _shipmentService.GetShipmentById(shipmentId);

            if (shipment != null)
            {
                var result = _TPG_Service.Tracking(new Plugin.Misc.ShippingSolutions.Models.Params.TPG.Params_TPG_Tracking
                {
                    CN = shipment.TrackingNumber
                });

                if (result.Status)
                {
                    if (shipment != null)
                    {
                        if (result.RootObject != null)
                        {
                            if (result.RootObject.History.Count() > 1)
                            {

                                var send = result.RootObject.History.Where(p => p.Status == 300).FirstOrDefault();
                                if (send != null)
                                {

                                    shipment.ShippedDateUtc = Convert.ToDateTime(send.StatusDate).ToUniversalTime();
                                    _shipmentService.UpdateShipment(shipment);
                                }
                                var recive = result.RootObject.History.Where(p => p.Status == 3).FirstOrDefault();
                                if (recive != null)
                                {

                                    shipment.DeliveryDateUtc = Convert.ToDateTime(recive.StatusDate).ToUniversalTime();
                                    _shipmentService.UpdateShipment(shipment);
                                }
                            }
                        }

                    }
                    return result;
                }
            }
            else
            {
                var result = _TPG_Service.Tracking(new Plugin.Misc.ShippingSolutions.Models.Params.TPG.Params_TPG_Tracking
                {
                    CN = TrackingNumber
                });
                return result;
            }

            return null;
        }


        public Plugin.Misc.ShippingSolutions.Models.Results.Ubbar.Result_Ubbar_Tracking TrackUbaarShipment(int shipmentId, string TrackingNumber)
        {
            Shipment shipment = null;
            if (!string.IsNullOrEmpty(TrackingNumber))
                shipment = getShipmentByTrackingNUmber(TrackingNumber);
            else if (shipmentId != 0)
                shipment = _shipmentService.GetShipmentById(shipmentId);

            if (shipment != null)
            {
                var result = _ubaar_Service.Tracking(new Plugin.Misc.ShippingSolutions.Models.Params.Ubbar.Params_Ubaar_Tracking
                {
                    order_code = shipment.TrackingNumber
                });

                if (result.Status)
                {
                    if (shipment != null)
                    {
                        if (result.DetailResiltTracking != null)
                        {
                            if (result.DetailResiltTracking.order_details != null)
                            {
                                if (result.DetailResiltTracking.order_details.status == "picked up")
                                {
                                    //send
                                    if (result.DetailResiltTracking.order_details.pickup_date != "")
                                    {
                                        string[] dateItem = result.DetailResiltTracking.order_details.pickup_date.Split('-');
                                        shipment.ShippedDateUtc = Convert.ToDateTime(dateItem[0] + "/" + dateItem[1] + "/" + dateItem[2]).ToUniversalTime();
                                        _shipmentService.UpdateShipment(shipment);
                                    }
                                }
                                if (result.DetailResiltTracking.order_details.status == "delivered ")
                                {
                                    //زمانی در خروجی پست من وجود مدارد
                                    //shipment.DeliveryDateUtc = Convert.ToDateTime(DateTime.Now).ToUniversalTime();
                                    //_shipmentService.UpdateShipment(shipment);
                                }
                            }
                        }
                    }
                    return result;
                }
            }
            else
            {
                var result = _ubaar_Service.Tracking(new Plugin.Misc.ShippingSolutions.Models.Params.Ubbar.Params_Ubaar_Tracking
                {
                    order_code = TrackingNumber
                });
                return result;
            }

            return null;
        }


        public Plugin.Misc.ShippingSolutions.Models.Results.Taroff.Result_Taroff_GetStateOrder TrackTaroffShipment(int shipmentId, string TrackingNumber)
        {
            Shipment shipment = null;
            if (!string.IsNullOrEmpty(TrackingNumber))
                shipment = getShipmentByTrackingNUmber(TrackingNumber);
            else if (shipmentId != 0)
                shipment = _shipmentService.GetShipmentById(shipmentId);

            if (shipment != null)
            {
                var result = _Taroff_Service.GetStateOrder(new Plugin.Misc.ShippingSolutions.Models.Params.Taroff.Params_Taroff_GetStateOrder
                {
                    OrderId = Convert.ToInt32(shipment.TrackingNumber)
                });

                if (result.Status)
                {
                    if (shipment != null)
                    {
                        if (result.DetailResult_Taroff_GetStateOrder != null)
                        {

                            //if (result.RootObject.History.Count() > 1)
                            //{

                            //    var send = result.RootObject.History.Where(p => p.Status == 300).FirstOrDefault();
                            //    if (send != null)
                            //    {

                            //        shipment.ShippedDateUtc = Convert.ToDateTime(send.StatusDate).ToUniversalTime();
                            //        _shipmentService.UpdateShipment(shipment);
                            //    }
                            //    var recive = result.RootObject.History.Where(p => p.Status == 3).FirstOrDefault();
                            //    if (recive != null)
                            //    {

                            //        shipment.DeliveryDateUtc = Convert.ToDateTime(recive.StatusDate).ToUniversalTime();
                            //        _shipmentService.UpdateShipment(shipment);
                            //    }
                            //}
                        }

                    }
                    return result;
                }
            }
            else
            {
                var result = _Taroff_Service.GetStateOrder(new Plugin.Misc.ShippingSolutions.Models.Params.Taroff.Params_Taroff_GetStateOrder
                {
                    OrderId = Convert.ToInt32(TrackingNumber)
                });
                return result;
            }

            return null;
        }



        public Plugin.Misc.ShippingSolutions.Models.Results.Snappbox.Result_Snappbox_Get_Order_Detail TrackSnappShipment(int shipmentId, string TrackingNumber)
        {
            Shipment shipment = null;
            if (!string.IsNullOrEmpty(TrackingNumber))
                shipment = getShipmentByTrackingNUmber(TrackingNumber);
            else if (shipmentId != 0)
                shipment = _shipmentService.GetShipmentById(shipmentId);

            if (shipment != null)
            {
                var result = _Snappbox_Service.Get_Order_Detail(new Plugin.Misc.ShippingSolutions.Models.Params.Snappbox.Params_Snappbox_Get_Order_Details
                {
                    orderId = shipment.TrackingNumber
                });

                //if (result.Status)
                //{
                //if (shipment != null)
                //{
                //if (result.orderDetails != null)
                //{

                //    //if (result.RootObject.History.Count() > 1)
                //    //{

                //    //    var send = result.RootObject.History.Where(p => p.Status == 300).FirstOrDefault();
                //    //    if (send != null)
                //    //    {

                //    //        shipment.ShippedDateUtc = Convert.ToDateTime(send.StatusDate).ToUniversalTime();
                //    //        _shipmentService.UpdateShipment(shipment);
                //    //    }
                //    //    var recive = result.RootObject.History.Where(p => p.Status == 3).FirstOrDefault();
                //    //    if (recive != null)
                //    //    {

                //    //        shipment.DeliveryDateUtc = Convert.ToDateTime(recive.StatusDate).ToUniversalTime();
                //    //        _shipmentService.UpdateShipment(shipment);
                //    //    }
                //    //}
                //}

                //}
                return result;
                //}
            }
            else
            {
                var result = _Snappbox_Service.Get_Order_Detail(new Plugin.Misc.ShippingSolutions.Models.Params.Snappbox.Params_Snappbox_Get_Order_Details
                {
                    orderId = TrackingNumber
                });
                return result;
            }

        }





        public Plugin.Misc.ShippingSolutions.Models.Results.SkyBlue.Result_SkyBlue_Tracking TrackSkyBlueShipment(int shipmentId, string TrackingNumber)
        {
            Shipment shipment = null;
            if (!string.IsNullOrEmpty(TrackingNumber))
                shipment = getShipmentByTrackingNUmber(TrackingNumber);
            else if (shipmentId != 0)
                shipment = _shipmentService.GetShipmentById(shipmentId);

            if (shipment != null)
            {
                var result = _skyBlue_service.Tracking(new Plugin.Misc.ShippingSolutions.Models.Params.SkyBlue.Params_SkyBlue_Cancel_Tracking
                {
                    OrderNumber = shipment.TrackingNumber
                });

                if (result.Status)
                {
                    //if (shipment != null)
                    //{

                    //}
                    return result;
                }
            }
            else
            {
                var result = _skyBlue_service.Tracking(new Plugin.Misc.ShippingSolutions.Models.Params.SkyBlue.Params_SkyBlue_Cancel_Tracking
                {
                    OrderNumber = TrackingNumber
                });
                return result;
            }

            return null;
        }

        #endregion

        public bool RegisterLastShipmentStatus(int shipmentID, bool isCod, out string result)
        {
            var shipment = _shipmentService.GetShipmentById(shipmentID);
            //if (shipment.DeliveryDateUtc.HasValue)
            //{
            //    result = "محموله مورد نظر در مقصد تحویل داده شده است";
            //    return false;
            //}

            string Str_result = "";
            ShipmentTrackingModel model = null;
            if (isCod)
            {
                model = _codService.GetStatus(shipment.TrackingNumber, out Str_result);
                if (model == null)
                {
                    result = Str_result;
                    return false;
                }

                if (model.CODShipmentEventId == 5) // ارسال شده
                {
                    shipment.ShippedDateUtc = model.LastShipmentEventDate.ToUniversalTime();
                    _shipmentRepository.Update(shipment);
                }
                if (!shipment.DeliveryDateUtc.HasValue)
                    if (model.CODShipmentEventId == 7 //توزیع شده
                        || model.CODShipmentEventId == 15 // تسویه شده
                    )
                    {
                        shipment.DeliveryDateUtc = model.LastShipmentEventDate.ToUniversalTime();
                        _shipmentRepository.Update(shipment);
                    }

                var ST = _repositoryShipmentTracking.Table.SingleOrDefault(p => p.ShipmentId == shipmentID);
                if (ST == null)
                {
                    ST = new ShipmentTrackingModel()
                    {
                        CODShipmentEventId = model.CODShipmentEventId,
                        LastShipmentEventDate = model.LastShipmentEventDate,
                        ShipmentId = model.ShipmentId
                    };
                    _repositoryShipmentTracking.Insert(ST);
                    result = "اطلاعات رهگیری مرسوله به روز رسانی شد";
                    return true;
                }

                if (ST.CODShipmentEventId == model.CODShipmentEventId)
                {
                    result = "اطلاعات رهگیری مرسوله تغییر نکرده است";
                    return true;
                }

                ST.CODShipmentEventId = model.CODShipmentEventId;
                ST.LastShipmentEventDate = model.LastShipmentEventDate;
                _repositoryShipmentTracking.Update(ST);
                result = "اطلاعات رهگیری مرسوله به روز رسانی شد";
                return true;
            }
            else
            {
                var table = getNoramlShipmentTracking(shipment);
                if (table == null)
                {
                    result = "اطلاعات رهیگری مرسوله یافت نشد";
                    return false;
                }

                var sended = table.AsEnumerable().Where(r => r.Field<byte>("Type") == Convert.ToByte(1)
                                                             || r.Field<byte>("Type") == Convert.ToByte(2))
                    .OrderBy(p => p.Field<DateTime>("TDate")).FirstOrDefault();

                var Delivery = table.AsEnumerable()
                    .OrderByDescending(p => p.Field<DateTime>("TDate")).FirstOrDefault(r => r.Field<int?>("DType") == 3 ||
                                                                                            r.Field<int?>("DType") == 51);
                if (sended != null)
                    shipment.ShippedDateUtc = Convert.ToDateTime(sended.ItemArray[1]).ToUniversalTime();

                if (Delivery != null)
                {
                    shipment.DeliveryDateUtc = Convert.ToDateTime(Delivery.ItemArray[1]).ToUniversalTime();
                    var TbOrder = _orderRepository.Table.SingleOrDefault(p => p.Id == shipment.Order.Id);
                    TbOrder.ShippingStatus = ShippingStatus.Delivered;
                    _orderRepository.Update(TbOrder);
                }
                _shipmentRepository.Update(shipment);

                var lastEvent = table.AsEnumerable().OrderByDescending(p => p.Field<DateTime>("TDate")).FirstOrDefault();
                if (lastEvent == null)
                {
                    result = "آخرین رویداد رهگیری مرسوله یافت نشد";
                    return false;
                }

                int? type = lastEvent.Field<int?>("DType");
                int masterType = Convert.ToInt32(lastEvent.Field<byte>("Type"));
                DateTime LasteEventDate = Convert.ToDateTime(lastEvent.Field<string>("TFDATe"));

                var ST = _repositoryShipmentTracking.Table.SingleOrDefault(p => p.ShipmentId == shipmentID);
                if (ST == null)
                {
                    ST = new ShipmentTrackingModel()
                    {
                        LastShipmentEventDate = LasteEventDate,
                        ShipmenEventDesc = lastEvent.Field<string>("Describe"),
                        ShipmenMasterEventCode = masterType.ToString(),
                        ShipmentEventId = type ?? 0,
                        ShipmentId = shipmentID,
                        CODShipmentEventId = 0,
                    };
                    _repositoryShipmentTracking.Insert(ST);
                    result = "اطلاعات رهگیری مرسوله به روز رسانی شد";
                    return true;

                }
                //if (ST.ShipmenMasterEventCode == masterType.ToString() && ST.LastShipmentEventDate == LasteEventDate)
                //{
                //    result = "اطلاعات رهگیری مرسوله تغییر نکرده است";
                //    return false;
                //}
                ST.LastShipmentEventDate = LasteEventDate;
                ST.ShipmenEventDesc = lastEvent.Field<string>("Describe");
                ST.ShipmenMasterEventCode = masterType.ToString();
                ST.ShipmentEventId = type ?? 0;
                ST.CODShipmentEventId = 0;
                _repositoryShipmentTracking.Update(ST);
                result = "اطلاعات رهگیری مرسوله به روز رسانی شد";
                return true;
            }
        }

        public DataTable getNoramlShipmentTracking(Shipment shipment)
        {
            var setting = _settingService.LoadSetting<ExtendedShipmentSetting>(_storeContext.CurrentStore.Id);

            string strOutError = "";
            var dset = ExtendedShipmentPlugin.GetPostTrackingResult(setting, shipment.TrackingNumber, out strOutError);

            if (!string.IsNullOrEmpty(strOutError))
            {
                Log(" دریافت اطلاعات رهگیری انجام نشد", strOutError);
                return null;
            }
            var NotExisttRahgiry = "اطلاعاتی برای سفارش" + "{0}" + "رهگیری یافت نشد";
            NotExisttRahgiry = string.Format(NotExisttRahgiry, shipment.OrderId);
            if (dset == null)
            {
                Log(NotExisttRahgiry, "");
                return null;
            }
            if (dset.Tables.Count == 0)
            {
                Log(NotExisttRahgiry, "");
                return null;
            }
            var rows = dset.Tables[0].Rows;
            if (rows.Count == 0)
            {
                Log(NotExisttRahgiry, "");
                return null;
            }
            return dset.Tables[0];
        }
        public DataTable getNoramlShipmentTracking(string trackingNumber, out string result)
        {
            result = "";
            var setting = _settingService.LoadSetting<ExtendedShipmentSetting>(_storeContext.CurrentStore.Id);

            string strOutError = "";
            var dset = ExtendedShipmentPlugin.GetPostTrackingResult(setting, trackingNumber, out strOutError);

            if (!string.IsNullOrEmpty(strOutError))
            {
                result = " دریافت اطلاعات رهگیری انجام نشد";
                return null;
            }
            var NotExisttRahgiry = "اطلاعاتی برای مرسوله" + "{0}" + "رهگیری یافت نشد";
            NotExisttRahgiry = string.Format(NotExisttRahgiry, trackingNumber);

            if (dset == null)
            {
                result = NotExisttRahgiry;
                return null;
            }
            if (dset.Tables.Count == 0)
            {
                result = NotExisttRahgiry;
                return null;
            }
            var rows = dset.Tables[0].Rows;
            if (rows.Count == 0)
            {
                result = NotExisttRahgiry;
                return null;
            }
            return dset.Tables[0];
        }
        public DataTable getNoramlShipmentTracking(string trackingNumber, ExtendedShipmentSetting setting, out string error)
        {
            error = "";
            if (setting == null)
                setting = _settingService.LoadSetting<ExtendedShipmentSetting>(_storeContext.CurrentStore.Id);
            string strOutError = "";
            var dset = ExtendedShipmentPlugin.GetPostTrackingResult(setting, trackingNumber, out strOutError);

            if (!string.IsNullOrEmpty(strOutError))
            {
                error = " دریافت اطلاعات رهگیری انجام نشد" + "\r\n" + strOutError;
                return null;
            }
            error = "اطلاعاتی برای کد رهگیری" + "{0}" + "رهگیری یافت نشد";
            error = string.Format(error, trackingNumber);
            if (dset == null)
                return null;
            if (dset.Tables.Count == 0)
                return null;
            if (dset.Tables[0].Rows.Count == 0)
                return null;

            error = "";
            return dset.Tables[0];
        }
        public void Log(string header, string Message)
        {
            var logger =
                (DefaultLogger)EngineContext.Current
                    .Resolve<ILogger>();
            logger.InsertLog(LogLevel.Information, header, Message, null);
        }

        public List<QualityControlModel> getQualityControl(int orderId, string trackingNumber, int countryId, int stateId
        , int orderState, DateTime? DateFrom, DateTime? DateTo, int PageSize, int PageIndex, out int count)
        {
            count = 0;
            SqlParameter P_trackingNumber = new SqlParameter()
            {
                ParameterName = "trackingNumber",
                SqlDbType = SqlDbType.VarChar,
                Value = string.IsNullOrEmpty(trackingNumber) ? (object)DBNull.Value : (object)trackingNumber
            };
            SqlParameter P_DateFrom = new SqlParameter()
            {
                ParameterName = "DateFrom",
                SqlDbType = SqlDbType.DateTime,
                Value = !DateFrom.HasValue ? (object)DBNull.Value : (object)DateFrom
            };
            SqlParameter P_DateTo = new SqlParameter()
            {
                ParameterName = "DateTo",
                SqlDbType = SqlDbType.DateTime,
                Value = !DateTo.HasValue ? (object)DBNull.Value : (object)DateTo
            };
            SqlParameter P_count = new SqlParameter() { ParameterName = "Count", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

            SqlParameter[] prms = new SqlParameter[]{
                new SqlParameter() { ParameterName = "orderId", SqlDbType = SqlDbType.Int, Value = orderId },
                P_trackingNumber,
                new SqlParameter() { ParameterName = "countryId", SqlDbType = SqlDbType.Int, Value = countryId },
                new SqlParameter() { ParameterName = "stateId", SqlDbType = SqlDbType.Int, Value = stateId },
                new SqlParameter() { ParameterName = "orderState", SqlDbType = SqlDbType.Int, Value = orderState },
                P_DateFrom,
                P_DateTo,
                new SqlParameter() { ParameterName = "PageSize", SqlDbType = SqlDbType.Int, Value = PageSize },
                new SqlParameter() { ParameterName = "PageIndex", SqlDbType = SqlDbType.Int, Value = PageIndex },
                P_count
            };
            var allShipment = _dbContext.SqlQuery<QualityControlModel>(@"EXECUTE [dbo].[Sp_QualityControl] @orderId,@trackingNumber,@countryId,@stateId,@orderState
                    ,@DateFrom,@DateTo,@PageSize,@PageIndex,@count OUTPUT", prms).ToList<QualityControlModel>();

            count = (int)P_count.Value;
            return allShipment.ToList();
        }

        public List<TrackingInfo> getLastShipmentTracking(string trackingNumber, int orderId, string mobileNo, int CustomerId, int IdServiceProvider, out string strError)
        {
            List<TrackingInfo> lstTrackingInfo = new List<TrackingInfo>();//,
            if (string.IsNullOrEmpty(trackingNumber) && orderId == 0 && string.IsNullOrEmpty(mobileNo))
            {
                strError = "لطفا یکی از فیلد های کد رهگیری پستی،شماره سفارش و یا شماره موبایل فرستنده را جهت رهگیری وارد نمایید";
                return null;
            }
            var data = getInfoFromTrackingData(trackingNumber, orderId, mobileNo, CustomerId);
            if (!data.Any() && !string.IsNullOrEmpty(trackingNumber))
            {
                if (IdServiceProvider > 0 || trackingNumber.Length >= 20)
                {
                    TrackingInfo temp = new TrackingInfo();

                    if (trackingNumber.Length >= 20 && IdServiceProvider == 0)
                    {
                        if (trackingNumber.Length == 20)
                        {
                            IdServiceProvider = 667;
                        }
                        else
                        {
                            IdServiceProvider = 654;
                        }
                    }
                    temp.CategoryId = IdServiceProvider;
                    temp.TrackingNumber = trackingNumber;

                    var lastStateInfo = GetInfoFromTrackingService(temp, trackingNumber);
                    if (lastStateInfo != null)
                    {
                        lstTrackingInfo.Add(lastStateInfo);
                    }
                }
                else
                {
                    strError = "404";
                    return lstTrackingInfo;
                }
                strError = "200";
                return lstTrackingInfo;

            }
            else if (data != null && data.Count() > 0)
            {
                var ordertemp = data.FirstOrDefault();
                var item = data.Where(p => !string.IsNullOrEmpty(p.TrackingNumber)).FirstOrDefault();
                if (item != null)
                {
                    var serviceData = GetInfoFromTrackingService(item, item.TrackingNumber);
                    serviceData.NameServiceProvider += "-" + (_CategoryService.GetCategoryById(item.CategoryId.GetValueOrDefault()).Name);
                    serviceData.Description = "محتویات مرسوله";
                    serviceData.TrackingNumber = item.TrackingNumber != "" ? item.TrackingNumber : item.OrderId > 0 ? item.OrderId.ToString() : mobileNo;
                    lstTrackingInfo.Add(serviceData);
                }
                else
                {

                    TrackingInfo info = new TrackingInfo();
                    info = ordertemp;
                    if (ordertemp != null)
                    {
                        if (ordertemp.OrderStatusId == 40)
                        {
                            info.Description = "";
                            info.LastSate = "سفارش کنسل شده";
                            info.TrackingNumber = ordertemp.OrderId > 0 ? ordertemp.OrderId.ToString() : mobileNo;
                        }
                        if (ordertemp.OrderStatusId == 10)
                        {
                            info.Description = "";
                            info.LastSate = "سفارش معلق شده";
                            info.TrackingNumber = ordertemp.OrderId > 0 ? ordertemp.OrderId.ToString() : mobileNo;
                        }
                        if (ordertemp.OrderStatusId == 20)
                        {
                            info.Description = "";
                            info.LastSate = "سفارش در حال پردازش می باشد";
                            info.TrackingNumber = ordertemp.OrderId > 0 ? ordertemp.OrderId.ToString() : mobileNo;
                        }
                    }

                    lstTrackingInfo.Add(info);
                }
                //foreach (var item in data)
                //{
                //    if ()
                //    {
                //        var serviceData = GetInfoFromTrackingService(item, item.TrackingNumber);
                //        serviceData.NameServiceProvider += "-"+ (_CategoryService.GetCategoryById(item.CategoryId.GetValueOrDefault()).Name);

                //        serviceData.Description = "محتویات مرسوله";
                //        lstTrackingInfo.Add(serviceData);
                //    }
                //    else
                //    {
                //        item.LastSate = "نامشخص";
                //        item.TrackingNumber = "بدون بارکد";
                //        lstTrackingInfo.Add(item);
                //    }
                //}
                strError = "200";
                return lstTrackingInfo;
            }
            strError = "اطلاعاتی یافت نشد";
            return null;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        public TrackingInfo GetInfoFromTrackingService(TrackingInfo info, string trackingNumber)
        {
            if (info == null)
                info = new TrackingInfo();
            if (info.CategoryId.HasValue && info.CategoryId.Value > 0)
            {
                #region Post
                if (new int[] { 654, 655, 660, 661, 662, 667, 670, 690, 691, 692, 693, 694, 695, 696, 697, 698, 725, 726, 727, 722, 723 }.Contains(info.CategoryId.Value))
                {
                    bool isCod = false;//!(trackingNumber.Length > 20);
                    info.NameServiceProvider = "پست";
                    string Str_result = "";
                    ShipmentTrackingModel model = null;
                    if (isCod)
                    {
                        model = _codService.GetStatus(trackingNumber, out Str_result);
                        if (model == null)
                        {
                            info.LastSate = Str_result;
                            return info;
                        }

                        if (model.CODShipmentEventId == 5) // ارسال شده
                        {
                            info.ShippedDate = ConvertToJalali(model.LastShipmentEventDate);
                        }
                        if (model.CODShipmentEventId == 7 //توزیع شده
                            || model.CODShipmentEventId == 15 // تسویه شده
                        )
                        {
                            info.DeliveryDate = ConvertToJalali(model.LastShipmentEventDate);
                        }

                        if (!string.IsNullOrEmpty(info.DeliveryDate))
                        {
                            info.LastSate = "مرسوله تحویل گیرنده شده";
                        }
                        else if (!string.IsNullOrEmpty(info.ShippedDate))
                        {
                            info.LastSate = "مرسوله شما در حال ارسال به مقصد می باشد";
                        }
                        else
                            info.LastSate = model.EventName;
                        return info;
                    }
                    else
                    {
                        var table = getNoramlShipmentTracking(trackingNumber, out Str_result);
                        if (table == null)
                        {
                            info.LastSate = Str_result;
                            return info;
                        }
                        ////////////////////////////////////////////////////////////////////////////
                        string CodCost = $@"SELECT
                                            [CodCost]
                                        FROM [dbo].[ShipmentAppointment] AS	sa
                                        INNER JOIN dbo.Shipment ON Shipment.Id = sa.ShipmentId
                                        where dbo.Shipment.TrackingNumber LIKE '{trackingNumber}' ";
                        info.CodCost = _dbContext.SqlQuery<int>(CodCost).FirstOrDefault();

                        string CodBmValue = $@"SELECT
                                            [CodBmValue]
                                        FROM [dbo].[ShipmentAppointment] AS	sa
                                        INNER JOIN dbo.Shipment ON Shipment.Id = sa.ShipmentId
                                        where dbo.Shipment.TrackingNumber LIKE '{trackingNumber}' ";
                        info.CodBmValue = _dbContext.SqlQuery<int>(CodBmValue).FirstOrDefault();
                        ////////////////////////////////////////////////////////////////////////////

                        var sended = table.AsEnumerable().Where(r => r.Field<string>("Type") == "1"
                                                                     || r.Field<string>("Type") == "2")
                            .OrderBy(p => p.Field<string>("TFDate")).FirstOrDefault();
                        DataRow Delivery = null;
                        if (table.Columns.Contains("DType"))
                        {
                            Delivery = table.AsEnumerable()
                                .OrderByDescending(p => p.Field<string>("TFDate")).FirstOrDefault(r => r.Field<string>("DType") == "3" ||
                                                                                                        r.Field<string>("DType") == "51");
                        }
                        if (sended != null)
                            info.ShippedDate = ConvertToJalali(Convert.ToDateTime(sended.ItemArray[1]));

                        if (Delivery != null)
                        {
                            info.DeliveryDate = ConvertToJalali(Convert.ToDateTime(Delivery.ItemArray[1]));
                        }

                        var lastEvent = table.AsEnumerable().OrderByDescending(p => p.Field<string>("TFDate")).FirstOrDefault();
                        if (lastEvent == null)
                        {
                            info.LastSate = "آخرین رویداد رهگیری مرسوله یافت نشد";
                            return info;
                        }
                        DateTime LasteEventDate = Convert.ToDateTime(lastEvent.Field<string>("TFDATe"));
                        info.LastSate = lastEvent.Field<string>("Describe");
                        info.LastEventData = ConvertToJalali(LasteEventDate);

                        var data = table.AsEnumerable().OrderBy(p => p.Field<string>("TFDate")).Select(n => new PostTrackResultItems()
                        {
                            desc = n.Field<string>("Describe"),
                            date = n.Field<string>("TFDATe"),
                            state = n.Field<string>("State"),
                        }
                         ).ToList();
                        info.AllState = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    }
                }
                #endregion
                #region DTS
                if (new int[] { 699, 703, 705, 706 }.Contains(info.CategoryId.Value))//DTS
                {
                    var Result = TrackDtsShipment(0, trackingNumber);
                    if (Result == null)
                        return info;
                    info.TrackingNumber = trackingNumber;
                    info.NameServiceProvider = "DTS";
                    info.Description = "-";
                    if (string.IsNullOrEmpty(info.SenderAddress))
                        info.SenderAddress = Result.objects.order.origin.ToString();
                    if (string.IsNullOrEmpty(info.ReceiverAddress))
                        info.ReceiverAddress = Result.objects.order.dest.ToString();

                    if (Result.objects.order.history.Any())
                    {
                        if (Result.objects.order.history.Count() > 1)
                        {
                            var historyItem = Result.objects.order.history.Where(p => p.status.StartsWith("IR ") || p.status.StartsWith("OS ")).OrderBy(p => p.timestamp_date).FirstOrDefault();
                            if (historyItem != null)
                            {
                                info.ShippedDate = ConvertToJalali(UnixTimeStampToDateTime(historyItem.timestamp_date)); //ConvertToJalali(Convert.ToDateTime(dateItem[2] + "/" + dateItem[1] + "/" + dateItem[0] + " " + DatePart[1]).ToLocalTime());
                            }
                            var LastState = Result.objects.order.history.OrderByDescending(p => p.timestamp_date).First();
                            info.LastSate = LastState.status;
                            string[] _DatePart = historyItem.date.Split(' ');
                            var _dateItem = _DatePart[0].Split('/');
                            info.LastEventData = ConvertToJalali(UnixTimeStampToDateTime(LastState.timestamp_date)); //ConvertToJalali(Convert.ToDateTime(_dateItem[2] + "/" + _dateItem[1] + "/" + _dateItem[0] + " " + _DatePart[1]));
                            if (string.IsNullOrEmpty(info.SenderAddress))
                                info.SenderAddress = Result.objects.order.origin;
                            if (string.IsNullOrEmpty(info.ReceiverAddress))
                                info.SenderAddress = Result.objects.order.dest;
                            var data = Result.objects.order.history.OrderBy(p => p.timestamp_date).Select(n => new PostTrackResultItems()
                            {
                                desc = n.status + ":",
                                date = n.date,
                                state = ".",
                            }
                           ).ToList();
                            info.AllState = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                        }
                    }
                    if (!string.IsNullOrEmpty(Result.objects.order.delivered_to))
                    {
                        info.DeliveryDate = ConvertToJalali(GetDtsDate(Result.objects.order.delivery_time).ToLocalTime());
                        //info.DateDelivery = DateTime.TryParse(Result.objects.order.delivery_time, DateTime.UtcNow);
                    }
                    return info;
                }
                #endregion
                #region Chapar
                else if (new int[] { 712, 713, 714, 715 }.Contains(info.CategoryId.Value))//Chapar
                {
                    var Result = TrackChaparShipment(0, trackingNumber);
                    if (Result == null)
                        return info;
                    info.TrackingNumber = trackingNumber;
                    info.NameServiceProvider = "چاپار";
                    info.Description = "-";
                    if (string.IsNullOrEmpty(info.SenderAddress))
                        info.SenderAddress = Result.objects.order.origin.ToString();
                    if (string.IsNullOrEmpty(info.ReceiverAddress))
                        info.ReceiverAddress = Result.objects.order.dest.ToString();


                    if (Result.objects.order.history.Any())
                    {
                        if (Result.objects.order.history.Count() > 1)
                        {
                            var historyItem = Result.objects.order.history.Where(p => p.status.StartsWith("IR ") || p.status.StartsWith("OS ")).OrderBy(p => p.timestamp_date).FirstOrDefault();
                            if (historyItem != null)
                            {
                                info.ShippedDate = ConvertToJalali(UnixTimeStampToDateTime(historyItem.timestamp_date));
                            }
                            var LastState = Result.objects.order.history.OrderByDescending(p => p.timestamp_date).First();
                            info.LastSate = LastState.status;

                            info.LastEventData = LastState.date;
                            if (string.IsNullOrEmpty(info.SenderAddress))
                                info.SenderAddress = Result.objects.order.origin;
                            if (string.IsNullOrEmpty(info.ReceiverAddress))
                                info.SenderAddress = Result.objects.order.dest;

                            var data = Result.objects.order.history.OrderBy(p => p.timestamp_date).Select(n => new PostTrackResultItems()
                            {
                                desc = n.status + ":",
                                date = n.date,
                                state = ".",
                            }
                            ).ToList();
                            info.AllState = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                        }
                    }
                    if (!string.IsNullOrEmpty(Result.objects.order.delivered_to))
                    {
                        info.DeliveryDate = ConvertToJalali(GetDtsDate(Result.objects.order.delivery_time).ToLocalTime());
                        //info.DateDelivery = DateTime.TryParse(Result.objects.order.delivery_time, DateTime.UtcNow);
                    }
                    return info;
                }
                #endregion
                #region PDE
                if (new int[] { 707, 708 }.Contains(info.CategoryId.Value))
                {
                    var Result = TrackPDEShipment(0, trackingNumber);
                    if (Result == null)
                        return info;
                    info.TrackingNumber = trackingNumber;
                    info.NameServiceProvider = "PDE";
                    info.Description = "-";

                    if (Result != null)
                    {
                        if (Result.Status)
                        {
                            if (Result.Detail != null)
                            {
                                info.LastSate = Result.Detail.FaStatus.ToString();
                                info.LastEventData = Result.Detail.LocTime != null ? ConvertToJalali(Convert.ToDateTime(Result.Detail.LocTime)) : "-";
                                //info.DateDelivery = DateTime.TryParse(Result.Detail.LocTime, DateTime.UtcNow);
                                return info;
                            }


                        }
                        else
                        {
                            info.LastSate = "مشکل در رهگیری محموله:کاربر گرامی شماره رهگیری یا سرویس دهنده پستی دیگیری را انتخاب کنید ";
                            return info;
                        }
                    }
                }
                #endregion
                #region persian
                //if (new int[] { 704, 700 }.Contains(info.CategoryId.Value))
                //{

                //}
                #endregion
                #region sanpbox
                if (new int[] { 717 }.Contains(info.CategoryId.Value))
                {
                    var Result = TrackSnappShipment(0, trackingNumber);
                    if (Result == null)
                        return info;
                    info.TrackingNumber = trackingNumber;
                    info.NameServiceProvider = "اسنپ باکس";
                    info.Description = "-";

                    if (Result != null)
                    {
                        if (Result.Status)
                        {
                            if (Result.orderDetails != null)
                            {
                                info.LastSate = Result.orderDetails.orderDetails.status.ToString();
                                info.SenderAddress = Result.orderDetails.orderDetails.city.ToString() + Result.orderDetails.orderDetails.terminals[0].address.ToString();
                                info.ReceiverAddress = Result.orderDetails.orderDetails.city.ToString() + Result.orderDetails.orderDetails.terminals[1].address.ToString(); ;
                                info.SenderFullName = Result.orderDetails.orderDetails.terminals[0].contactName.ToString();
                                info.ReceiverFullName = Result.orderDetails.orderDetails.terminals[1].contactName.ToString();

                                //تاریخ ارسال
                                info.CreatedOn = Result.orderDetails.orderDetails.createdAt != null ? ConvertToJalali(Convert.ToDateTime(Result.orderDetails.orderDetails.createdAt.ToString())) : "-";
                                //تاریخ جمع اوری
                                info.DataCollect = Result.orderDetails.orderDetails.terminals[0].arrivedPickupAt != null ? ConvertToJalali(Convert.ToDateTime(Result.orderDetails.orderDetails.terminals[0].arrivedPickupAt)) : "-";
                                //info.LastEventData= Result.orderDetails.orderDetails.
                                //info.DateDelivery = DateTime.TryParse(Result.orderDetails.orderDetails.terminals[1].updatedAt.ToString(), DateTime.UtcNow);

                                return info;
                            }
                        }
                        else
                        {
                            info.LastSate = "مشکل در رهگیری محموله:کاربر گرامی شماره رهگیری یا سرویس دهنده پستی دیگیری را انتخاب کنید ";
                            return info;
                        }
                    }
                }
                #endregion
                #region taroff
                if (new int[] { 716 }.Contains(info.CategoryId.Value))
                {
                    var Result = TrackTaroffShipment(0, trackingNumber);
                    if (Result == null)
                        return info;
                    info.TrackingNumber = trackingNumber;
                    info.NameServiceProvider = "تعارف";
                    info.Description = "-";

                    if (Result != null)
                    {
                        if (Result.Status)
                        {
                            info.LastSate = Result.DetailResult_Taroff_GetStateOrder.statetitle.ToString();
                            return info;

                        }
                        else
                        {
                            info.LastSate = "مشکل در رهگیری محموله:کاربر گرامی شماره رهگیری یا سرویس دهنده پستی دیگیری را انتخاب کنید ";
                            return info;
                        }
                    }
                }
                #endregion
                #region tinex
                if (new int[] { 0 }.Contains(info.CategoryId.Value))
                {

                }
                #endregion
                #region tpg
                if (new int[] { 711, 710, 709 }.Contains(info.CategoryId.Value))
                {
                    var Result = TrackTGPShipment(0, trackingNumber);
                    if (Result == null)
                        return info;
                    info.TrackingNumber = trackingNumber;
                    info.NameServiceProvider = "TPG";
                    info.Description = "محتویات مرسوله";

                    if (Result != null)
                    {
                        if (Result.Status)
                        {
                            if (Result.RootObject != null)
                            {
                                info.SenderFullName = Result.RootObject.Sender.Name.ToString();
                                info.SenderAddress = Result.RootObject.ReceiptInfo.SourceTownNameFa.ToString() + "-" + Result.RootObject.Sender.Address.ToString();
                                info.ReceiverAddress = Result.RootObject.ReceiptInfo.DestinationTownNameFa.ToString() + "-" + Result.RootObject.Receiver.Address.ToString();
                                info.ReceiverFullName = Result.RootObject.Receiver.Name.ToString();
                                var t = Result.RootObject.History.OrderByDescending(p => p.StatusLogId).FirstOrDefault();
                                if (t != null)
                                {
                                    info.LastSate = t.StatusDescription.ToString();
                                    info.LastEventData = ConvertToJalali(t.RecordDate);

                                }
                                //تاریخ ارسال
                                info.CreatedOn = Result.RootObject.ReceiptInfo.DataSentDate != null ? ConvertToJalali(Result.RootObject.ReceiptInfo.DataSentDate) : "-";
                                //تاریخ ارسال
                                info.ShippedDate = Result.RootObject.ReceiptInfo.DataSentDate != null ? ConvertToJalali(Result.RootObject.ReceiptInfo.DataSentDate) : "-";
                                //تاریخ تحویل
                                info.DeliveryDate = Result.RootObject.ReceiptInfo.DataReceiveDate != null ? ConvertToJalali(Result.RootObject.ReceiptInfo.DataReceiveDate) : "-";
                                //وزن محموله
                                info.ShipmentWeight = Result.RootObject.Details[0].RealWeight.ToString();
                                return info;
                            }

                        }
                        else
                        {
                            info.LastSate = "مشکل در رهگیری محموله:کاربر گرامی شماره رهگیری یا سرویس دهنده پستی دیگیری را انتخاب کنید ";
                            return info;
                        }
                    }
                }
                #endregion
                #region ubbar
                if (new int[] { 701 }.Contains(info.CategoryId.Value))
                {
                    var Result = TrackUbaarShipment(0, trackingNumber);
                    if (Result == null)
                        return info;
                    info.TrackingNumber = trackingNumber;
                    info.NameServiceProvider = "اووبار";
                    info.Description = Result.DetailResiltTracking.order_details.load_type;

                    if (Result != null)
                    {
                        if (Result.Status)
                        {
                            info.SenderFullName = Result.DetailResiltTracking.order_details.sender_name.ToString();
                            info.SenderAddress = Result.DetailResiltTracking.order_details.source_city.ToString() + "-" + Result.DetailResiltTracking.order_details.source_state.ToString() + "-" + Result.DetailResiltTracking.order_details.source_address.ToString();
                            info.ReceiverAddress = Result.DetailResiltTracking.order_details.destination_city.ToString() + "-" + Result.DetailResiltTracking.order_details.destination_state.ToString() + "-" + Result.DetailResiltTracking.order_details.destination_address.ToString();
                            info.ReceiverFullName = Result.DetailResiltTracking.order_details.receiver_name.ToString();
                            if (Result.DetailResiltTracking.order_details.status == "picked up")
                            {
                                info.LastSate = "بارگیری شده";
                                info.LastEventData = Result.DetailResiltTracking.order_details.pickup_date;
                            }
                            if (Result.DetailResiltTracking.order_details.status == "delivered")
                            {
                                info.LastSate = "تحویل شده";
                                //info.DateDelivery=

                            }
                            if (Result.DetailResiltTracking.order_details.status == "Registered")
                            {
                                info.LastSate = "ثبت شده/منتظر پیدا شدن راننده";

                            }
                            return info;

                        }
                        else
                        {
                            info.LastSate = "مشکل در رهگیری محموله:کاربر گرامی شماره رهگیری یا سرویس دهنده پستی دیگیری را انتخاب کنید ";
                            return info;
                        }
                    }
                }
                #endregion
                #region yarbox
                if (new int[] { 702 }.Contains(info.CategoryId.Value))
                {
                    var Result = TrackYarBoxShipment(0, trackingNumber);
                    if (Result == null)
                        return info;
                    info.TrackingNumber = trackingNumber;
                    info.NameServiceProvider = "یارباکس";
                    info.Description = "-";

                    if (Result != null)
                    {
                        if (Result.Status)
                        {
                            info.LastSate = Result.Detail_Tracking.status.ToString();
                            return info;

                        }
                        else
                        {
                            info.LastSate = "مشکل در رهگیری محموله:کاربر گرامی شماره رهگیری یا سرویس دهنده پستی دیگیری را انتخاب کنید ";
                            return info;
                        }
                    }
                }
                #endregion
                #region blue sky
                if (new int[] { 719 }.Contains(info.CategoryId.Value))
                {
                    var Result = TrackSkyBlueShipment(0, trackingNumber);
                    if (Result == null)
                        return info;
                    info.TrackingNumber = trackingNumber;
                    info.NameServiceProvider = "راه آبی آسمان";
                    info.Description = "-";

                    if (Result != null && Result.Detail_sk_Tracking.Any())
                    {
                        if (Result.Status)
                        {
                            var lasthistory = Result.Detail_sk_Tracking[0];
                            info.LastSate = lasthistory.Status;
                            info.LastEventData = lasthistory.DateTime;
                            var data = Result.Detail_sk_Tracking.Select(n => new PostTrackResultItems()
                            {
                                desc = n.Location + ":" + n.Status + (string.IsNullOrEmpty(n.Details) ? "" : ("-->" + n.Details.Replace(":", ";"))),
                                date = n.DateTime,
                                state = n.Status
                            }
                            ).ToList();
                            info.AllState = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                            return info;

                        }
                        else
                        {
                            info.LastSate = "مشکل در رهگیری محموله:کاربر گرامی شماره رهگیری یا سرویس دهنده پستی دیگیری را انتخاب کنید ";
                            return info;
                        }
                    }
                    else
                    {
                        info.LastSate = "اطلاعاتی جهت رهگیری مرسوله شما یافت نشد";
                        return info;
                    }
                }
                #endregion


            }
            return info;
        }
        public string getOrderItemContent(OrderItem orderitem)
        {
            return _OrderItemAttributeValueRepository.Table.Single(p => p.OrderItemId == orderitem.Id && p.PropertyAttrName.Contains("نوع و وزن")).PropertyAttrValueText;
        }
        public void UpdateFromPost(TrackingInfo info, string trackingNumber)
        {
            try
            {
                var shipment = _shipmentRepository.Table.SingleOrDefault(p => p.TrackingNumber == trackingNumber);
                if (shipment != null)
                {
                    var Table = getNoramlShipmentTracking(shipment);
                    var sended = Table.AsEnumerable().Where(r => r.Field<byte>("Type") == Convert.ToByte(1)
                                                                 || r.Field<byte>("Type") == Convert.ToByte(2))
                        .OrderBy(p => p.Field<DateTime>("TDate")).FirstOrDefault();

                    var Delivery = Table.AsEnumerable()
                        .OrderByDescending(p => p.Field<DateTime>("TDate")).FirstOrDefault(r =>
                            r.Field<int?>("DType") == 3 ||
                            r.Field<int?>("DType") == 51);

                    if (sended != null)
                        shipment.ShippedDateUtc = Convert.ToDateTime(sended.ItemArray[1]).ToUniversalTime();

                    if (Delivery != null)
                    {
                        shipment.DeliveryDateUtc = Convert.ToDateTime(Delivery.ItemArray[1]).ToUniversalTime();
                        var TbOrder = _orderRepository.Table.SingleOrDefault(p => p.Id == shipment.Order.Id);
                        TbOrder.ShippingStatus = ShippingStatus.Delivered;
                        _orderRepository.Update(TbOrder);
                    }

                    _shipmentRepository.Update(shipment);
                    Table.Dispose();
                }
                else
                {
                    string error = "";
                    getNoramlShipmentTracking(trackingNumber, null, out error);
                }
            }
            catch (Exception ex)
            {
                Log("خطا در زمان دریافت اطلاعات رهگیری", ex.Message +
                                                         (ex.InnerException != null ? ex.InnerException.Message : ""));
            }
        }
        public List<TrackingInfo> getInfoFromTrackingData(string trackingNumber, int orderId, string phoneNumber, int CustomerId)
        {
            if (string.IsNullOrEmpty(trackingNumber))
                trackingNumber = null;
            if (string.IsNullOrEmpty(phoneNumber))
                phoneNumber = null;
            SqlParameter P_TrackingNumber = new SqlParameter()
            {
                ParameterName = "TrackingNumber",
                SqlDbType = SqlDbType.NVarChar,
                Value = trackingNumber == null ? (object)DBNull.Value : (object)trackingNumber
            };
            SqlParameter P_OrderId = new SqlParameter()
            {
                ParameterName = "OrderId",
                SqlDbType = SqlDbType.Int,
                Value = (object)orderId
            };
            SqlParameter P_MobileNumber = new SqlParameter()
            {
                ParameterName = "MobileNumber",
                SqlDbType = SqlDbType.NVarChar,
                Value = phoneNumber == null ? (object)DBNull.Value : (object)phoneNumber
            };
            SqlParameter P_CustomerId = new SqlParameter()
            {
                ParameterName = "CustomerId",
                SqlDbType = SqlDbType.Int,
                Value = (object)CustomerId
            };
            SqlParameter[] prms = new SqlParameter[]
            {
                P_TrackingNumber,
                P_OrderId,
                P_MobileNumber,
                P_CustomerId
            };
            string Query = @"EXECUTE [dbo].[Sp_TrackingInfo] @TrackingNumber, @OrderId, @MobileNumber, @CustomerId";

            return _dbContext.SqlQuery<TrackingInfo>(Query, prms).ToList();
        }

        public int GetShipmentStatistic(DateTime startDate, DateTime endDate)
        {
            SqlParameter P_startDate = new SqlParameter()
            {
                ParameterName = "StartDate",
                SqlDbType = SqlDbType.DateTime,
                Value = startDate
            };
            SqlParameter P_endDate = new SqlParameter()
            {
                ParameterName = "EndDate",
                SqlDbType = SqlDbType.DateTime,
                Value = endDate
            };
            SqlParameter[] prms = new SqlParameter[]
                {P_startDate, P_endDate};
            string Query = @"EXECUTE [dbo].[Sp_SHipmentStatistic] @StartDate,@EndDate";

            return _dbContext.SqlQuery<int>(Query, prms).FirstOrDefault();
        }

        public bool HasTrackingRecourd(int orderId)
        {
            var data = _shipmentRepository.Table.Where(p => p.OrderId == orderId).Select(p => p.Id).ToList();
            return _repositoryShipmentTracking.Table.Any(p => data.Contains(p.ShipmentId));
        }
        private string ConvertToJalali(DateTime dt)
        {
            PersianCalendar pr = new PersianCalendar();
            return pr.GetYear(dt) + "/" + pr.GetMonth(dt).ToString("00") + "/" +
                   pr.GetDayOfMonth(dt).ToString("00") + " " + pr.GetHour(dt).ToString("00") + ":" + pr.GetMinute(dt).ToString("00")
                   + ":" + pr.GetSecond(dt).ToString("00");
        }
        public Shipment getShipmentByTrackingNUmber(string trackingNumber)
        {
            var result = _shipmentRepository.Table.Where(p => p.TrackingNumber == trackingNumber).ToList();
            if (result.Any())
                if (result.Count() > 1)
                    return null;
                else
                    return result.First();
            return null;
        }

        public List<OrderShipmentInfo> getAllShipmentByOrderId(int orderId)
        {
            SqlParameter P_orderId = new SqlParameter()
            {
                ParameterName = "orderId",
                SqlDbType = SqlDbType.Int,
                Value = (object)orderId
            };
            SqlParameter[] prms = new SqlParameter[]
            {
                P_orderId
            };
            string Query = @"EXECUTE [dbo].[Sp_getOrderShipmentsForTracking] @orderId";

            return _dbContext.SqlQuery<OrderShipmentInfo>(Query, prms).ToList();
        }
        public OrderShipmentInfoDetails getShipmentDetails(string TrackingNumber, int OrderId, int ShipmentId, out string error)
        {
            error = "";
            #region getDataFrom database
            SqlParameter P_TrackingNumber = new SqlParameter()
            {
                ParameterName = "TrackingNumber",
                SqlDbType = SqlDbType.NVarChar,
                Value = TrackingNumber == null ? (object)DBNull.Value : (object)TrackingNumber
            };
            SqlParameter P_OrderId = new SqlParameter()
            {
                ParameterName = "OrderId",
                SqlDbType = SqlDbType.Int,
                Value = (object)OrderId
            };

            SqlParameter P_ShipmentId = new SqlParameter()
            {
                ParameterName = "ShipmentId",
                SqlDbType = SqlDbType.Int,
                Value = (object)ShipmentId
            };
            SqlParameter[] prms = new SqlParameter[]
            {
                P_TrackingNumber,
                P_OrderId,
                P_ShipmentId,
            };
            string Query = @"EXECUTE [dbo].[Sp_ShipmentTrackingDetailes] @TrackingNumber, @OrderId, @ShipmentId";

            var result = _dbContext.SqlQuery<OrderShipmentInfoDetails>(Query, prms).FirstOrDefault();
            if (result == null)
            {
                error = "اطلاعاتی یافت نشد";
                return null;
            }
            GetInfoFromTrackingService1(result);
            return result;
            #endregion
        }
        public void GetInfoFromTrackingService1(OrderShipmentInfoDetails _OrderShipmentInfoDetails)
        {
            #region Chapar
            if (new int[] { 712, 713, 714, 715 }.Contains(_OrderShipmentInfoDetails.CategoryId))//Chapar
            {
                var Result = TrackChaparShipment(0, _OrderShipmentInfoDetails.TrackingNumber);
                if (Result == null)
                    return;
                if (string.IsNullOrEmpty(_OrderShipmentInfoDetails.SenderAddress1))
                    _OrderShipmentInfoDetails.SenderAddress1 = Result.objects.order.origin.ToString();
                if (string.IsNullOrEmpty(_OrderShipmentInfoDetails.ReceiverAddress1))
                    _OrderShipmentInfoDetails.ReceiverAddress1 = Result.objects.order.dest.ToString();

                if (Result.objects.order.history.Any())
                {
                    if (Result.objects.order.history.Count() > 1)
                    {
                        var historyItem = Result.objects.order.history.Where(p => p.status.StartsWith("IR ") || p.status.StartsWith("OS ")).OrderBy(p => p.timestamp_date).FirstOrDefault();

                        if (historyItem != null)
                        {
                            _OrderShipmentInfoDetails.ShippedDate = historyItem.date;
                        }
                        var LastState = Result.objects.order.history.OrderByDescending(p => p.timestamp_date).First();
                        _OrderShipmentInfoDetails.LastSate = LastState.status;

                        _OrderShipmentInfoDetails.LastEventData = ConvertToJalali(UnixTimeStampToDateTime(LastState.timestamp_date));// ConvertToJalali(Convert.ToDateTime(_dateItem[2] + "/" + _dateItem[1] + "/" + _dateItem[0] + " " + _DatePart[1]).ToLocalTime());
                        if (string.IsNullOrEmpty(_OrderShipmentInfoDetails.SenderAddress1))
                            _OrderShipmentInfoDetails.SenderAddress1 = Result.objects.order.origin;
                        if (string.IsNullOrEmpty(_OrderShipmentInfoDetails.ReceiverAddress1))
                            _OrderShipmentInfoDetails.SenderAddress1 = Result.objects.order.dest;

                        var data = Result.objects.order.history.OrderBy(p => p.timestamp_date).Select(n => new PostTrackResultItems()
                        {
                            desc = n.status + ":",
                            date = n.date,
                            state = ".",
                        }
                           ).ToList();
                        _OrderShipmentInfoDetails.AllState = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                    }
                }
                if (!string.IsNullOrEmpty(Result.objects.order.delivered_to))
                {
                    string[] DatePart = Result.objects.order.delivery_time.Split(' ');
                    var dateItem = DatePart[0].Split('/');
                    _OrderShipmentInfoDetails.DeliveryDate = ConvertToJalali(Convert.ToDateTime(Result.objects.order.delivered_to));
                }
                return;
            }
            #endregion
            #region PDE
            if (new int[] { 707, 708 }.Contains(_OrderShipmentInfoDetails.CategoryId))
            {
                var Result = TrackPDEShipment(0, _OrderShipmentInfoDetails.TrackingNumber);
                if (Result == null)
                    return;

                if (Result != null)
                {
                    if (Result.Status)
                    {
                        if (Result.Detail != null)
                        {
                            _OrderShipmentInfoDetails.LastSate = Result.Detail.FaStatus.ToString();
                            _OrderShipmentInfoDetails.LastEventData = Result.Detail.LocTime != null ? ConvertToJalali(Convert.ToDateTime(Result.Detail.LocTime)) : "-";
                            return;
                        }


                    }
                    return;
                }
            }
            #endregion
            #region persian
            if (new int[] { 704, 700 }.Contains(_OrderShipmentInfoDetails.CategoryId))
            {

            }
            #endregion
            #region sanpbox
            if (new int[] { 717 }.Contains(_OrderShipmentInfoDetails.CategoryId))
            {
                var Result = TrackSnappShipment(0, _OrderShipmentInfoDetails.TrackingNumber);
                if (Result == null)
                    return;


                if (Result != null)
                {
                    if (Result.Status)
                    {
                        if (Result.orderDetails != null)
                        {
                            _OrderShipmentInfoDetails.LastSate = Result.orderDetails.orderDetails.status.ToString();
                            //تاریخ ارسال
                            _OrderShipmentInfoDetails.ShippedDate = Result.orderDetails.orderDetails.createdAt != null ? ConvertToJalali(Convert.ToDateTime(Result.orderDetails.orderDetails.createdAt.ToString())) : "-";
                            //تاریخ جمع اوری
                            _OrderShipmentInfoDetails.DataCollect = Result.orderDetails.orderDetails.terminals[0].arrivedPickupAt != null ? ConvertToJalali(Convert.ToDateTime(Result.orderDetails.orderDetails.terminals[0].arrivedPickupAt)) : "-";
                            //info.LastEventData= Result.orderDetails.orderDetails.
                            return;
                        }
                    }
                    return;
                }
            }
            #endregion
            #region taroff
            if (new int[] { 716 }.Contains(_OrderShipmentInfoDetails.CategoryId))
            {
                var Result = TrackTaroffShipment(0, _OrderShipmentInfoDetails.TrackingNumber);
                if (Result == null)
                    return;

                if (Result != null)
                {
                    if (Result.Status)
                    {
                        _OrderShipmentInfoDetails.LastSate = Result.DetailResult_Taroff_GetStateOrder.statetitle.ToString();
                        return;

                    }
                    return;
                }
            }
            #endregion
            #region tinex
            if (new int[] { 0 }.Contains(_OrderShipmentInfoDetails.CategoryId))
            {

            }
            #endregion
            #region tpg
            if (new int[] { 711, 710, 709 }.Contains(_OrderShipmentInfoDetails.CategoryId))
            {
                var Result = TrackTGPShipment(0, _OrderShipmentInfoDetails.TrackingNumber);
                if (Result == null)
                    return;
                if (Result != null)
                {
                    if (Result.Status)
                    {
                        if (Result.RootObject != null)
                        {
                            var t = Result.RootObject.History.OrderByDescending(p => p.StatusLogId).FirstOrDefault();
                            if (t != null)
                            {
                                _OrderShipmentInfoDetails.LastSate = t.StatusDescription.ToString();
                                _OrderShipmentInfoDetails.LastEventData = ConvertToJalali(t.RecordDate);

                            }
                            //تاریخ ارسال
                            _OrderShipmentInfoDetails.ShippedDate = Result.RootObject.ReceiptInfo.DataSentDate != null ? ConvertToJalali(Result.RootObject.ReceiptInfo.DataSentDate) : "-";
                            //تاریخ تحویل
                            _OrderShipmentInfoDetails.DeliveryDate = Result.RootObject.ReceiptInfo.DataReceiveDate != null ? ConvertToJalali(Result.RootObject.ReceiptInfo.DataReceiveDate) : "-";
                            return;
                        }
                        return;

                    }
                    return;
                }
            }
            #endregion
            #region ubbar
            if (new int[] { 701 }.Contains(_OrderShipmentInfoDetails.CategoryId))
            {
                var Result = TrackUbaarShipment(0, _OrderShipmentInfoDetails.TrackingNumber);
                if (Result == null)
                    return;

                if (Result != null)
                {
                    if (Result.Status)
                    {
                        if (Result.DetailResiltTracking.order_details.status == "picked up")
                        {
                            _OrderShipmentInfoDetails.LastSate = "بارگیری شده";
                            _OrderShipmentInfoDetails.LastEventData = Result.DetailResiltTracking.order_details.pickup_date;
                        }
                        else if (Result.DetailResiltTracking.order_details.status == "delivered")
                        {
                            _OrderShipmentInfoDetails.LastSate = "تحویل شده";
                            _OrderShipmentInfoDetails.LastEventData = Result.DetailResiltTracking.order_details.delivery_date;

                        }
                        else if (Result.DetailResiltTracking.order_details.status == "Registered")
                        {
                            _OrderShipmentInfoDetails.LastSate = "ثبت شده/منتظر پیدا شدن راننده";
                        }
                        else if (Result.DetailResiltTracking.order_details.status == "waiting_price")
                        {
                            _OrderShipmentInfoDetails.LastSate = "منتظر تعیین قیمت";
                        }
                        else if (Result.DetailResiltTracking.order_details.status == "waiting_price_approved")
                        {
                            _OrderShipmentInfoDetails.LastSate = "منتظر تایید قیمت";
                        }
                        else if (Result.DetailResiltTracking.order_details.status == "driver_approved")
                        {
                            _OrderShipmentInfoDetails.LastSate = "منتظر تایید مشتری";
                        }
                        else if (Result.DetailResiltTracking.order_details.status == "applied_for")
                        {
                            _OrderShipmentInfoDetails.LastSate = "منتظر تایید راننده";
                        }
                        else if (Result.DetailResiltTracking.order_details.status == "waiting_complete_info")
                        {
                            _OrderShipmentInfoDetails.LastSate = "منتظر تکمیل اطلاعات";
                        }
                        else if (Result.DetailResiltTracking.order_details.status == "client_approved")
                        {
                            _OrderShipmentInfoDetails.LastSate = "منتظر دریافت بارنامه";
                        }
                        else if (Result.DetailResiltTracking.order_details.status == "assigned")
                        {
                            _OrderShipmentInfoDetails.LastSate = "در مسیر بارگیری";
                        }
                        else if (Result.DetailResiltTracking.order_details.status == "assigned")
                        {
                            _OrderShipmentInfoDetails.LastSate = "در مسیر بارگیری";
                        }
                        else if (Result.DetailResiltTracking.order_details.status == "notfound")
                        {
                            _OrderShipmentInfoDetails.LastSate = "پیدا نشد";
                        }
                        else if (Result.DetailResiltTracking.order_details.status == "cancelled")
                        {
                            _OrderShipmentInfoDetails.LastSate = "لغو شد";
                        }
                        return;

                    }
                    return;
                }
            }
            #endregion
            #region yarbox
            if (new int[] { 702 }.Contains(_OrderShipmentInfoDetails.CategoryId))
            {
                var Result = TrackYarBoxShipment(0, _OrderShipmentInfoDetails.TrackingNumber);
                if (Result == null)
                    return;
                if (Result != null)
                {
                    if (Result.Status)
                    {
                        _OrderShipmentInfoDetails.LastSate = Result.Detail_Tracking.status.ToString();


                    }
                    return;
                }
            }
            #endregion
            #region blue sky
            if (new int[] { 719 }.Contains(_OrderShipmentInfoDetails.CategoryId))
            {
                var Result = TrackSkyBlueShipment(0, _OrderShipmentInfoDetails.TrackingNumber);
                if (Result == null)
                    return;
                if (Result != null)
                {
                    if (Result.Status)
                    {
                        var lasthistory = Result.Detail_sk_Tracking[0];
                        _OrderShipmentInfoDetails.LastSate = lasthistory.Status;
                        _OrderShipmentInfoDetails.LastEventData = lasthistory.DateTime;
                        return;

                    }
                    return;
                }
            }
            #endregion
            #region Post
            if (new int[] { 654, 655, 660, 661, 662, 667, 670, 690, 691, 692, 693, 694, 695, 696, 697, 698, 725, 726, 727 }.Contains(_OrderShipmentInfoDetails.CategoryId))
            {
                bool isCod = !(_OrderShipmentInfoDetails.TrackingNumber.Length > 20);

                string Str_result = "";
                ShipmentTrackingModel model = null;
                if (isCod)
                {
                    model = _codService.GetStatus(_OrderShipmentInfoDetails.TrackingNumber, out Str_result);
                    if (model == null)
                    {
                        _OrderShipmentInfoDetails.LastSate = Str_result;

                    }

                    if (model.CODShipmentEventId == 5) // ارسال شده
                    {
                        _OrderShipmentInfoDetails.ShippedDate = ConvertToJalali(model.LastShipmentEventDate);
                    }
                    if (model.CODShipmentEventId == 7 //توزیع شده
                        || model.CODShipmentEventId == 15 // تسویه شده
                    )
                    {
                        _OrderShipmentInfoDetails.DeliveryDate = ConvertToJalali(model.LastShipmentEventDate);
                    }

                    if (!string.IsNullOrEmpty(_OrderShipmentInfoDetails.DeliveryDate))
                    {
                        _OrderShipmentInfoDetails.LastSate = "مرسوله تحویل گیرنده شده";
                    }
                    else if (!string.IsNullOrEmpty(_OrderShipmentInfoDetails.ShippedDate))
                    {
                        _OrderShipmentInfoDetails.LastSate = "مرسوله شما در حال ارسال به مقصد می باشد";
                    }
                    else
                        _OrderShipmentInfoDetails.LastSate = model.EventName;
                    return;
                }
                else
                {
                    var table = getNoramlShipmentTracking(_OrderShipmentInfoDetails.TrackingNumber, out Str_result);
                    if (table == null)
                    {
                        _OrderShipmentInfoDetails.LastSate = Str_result;
                        return;
                    }

                    var sended = table.AsEnumerable().Where(r => r.Field<byte>("Type") == Convert.ToByte(1)
                                                                 || r.Field<byte>("Type") == Convert.ToByte(2))
                        .OrderBy(p => p.Field<DateTime>("TDate")).FirstOrDefault();

                    var Delivery = table.AsEnumerable()
                        .OrderByDescending(p => p.Field<DateTime>("TDate")).FirstOrDefault(r => r.Field<int?>("DType") == 3 ||
                                                                                                r.Field<int?>("DType") == 51);
                    if (sended != null)
                        _OrderShipmentInfoDetails.ShippedDate = ConvertToJalali(Convert.ToDateTime(sended.ItemArray[1]));

                    if (Delivery != null)
                    {
                        _OrderShipmentInfoDetails.DeliveryDate = ConvertToJalali(Convert.ToDateTime(Delivery.ItemArray[1]));
                    }

                    var lastEvent = table.AsEnumerable().OrderByDescending(p => p.Field<DateTime>("TDate")).FirstOrDefault();
                    if (lastEvent == null)
                    {
                        _OrderShipmentInfoDetails.LastSate = "آخرین رویداد رهگیری مرسوله یافت نشد";
                        return;
                    }
                    DateTime LasteEventDate = Convert.ToDateTime(lastEvent.Field<string>("TFDATe"));
                    _OrderShipmentInfoDetails.LastSate = lastEvent.Field<string>("Describe");
                    _OrderShipmentInfoDetails.LastEventData = ConvertToJalali(LasteEventDate);

                    var data = table.AsEnumerable().OrderBy(p => p.Field<DateTime>("TDate")).Select(n => new PostTrackResultItems()
                    {
                        desc = n.Field<string>("Describe"),
                        date = n.Field<string>("TFDATe"),
                        state = n.Field<string>("State"),
                    }
                        ).ToList();
                    _OrderShipmentInfoDetails.AllState = Newtonsoft.Json.JsonConvert.SerializeObject(data);

                }
            }
            #endregion
            return;
        }

        public List<WeightCategoryModel> GetWightCategories()
        {
            var result = _dbContext.SqlQuery<WeightCategoryModel>(@"SELECT DISTINCT
                                PAV.Name as [Text] ,
	                            CAST(CAST(PAV.WeightAdjustment as  int)*1000 AS VARCHAR(10))  as [Value]
                            FROM
	                            dbo.Category AS C
	                            INNER JOIN dbo.Product_Category_Mapping AS PCM ON PCM.CategoryId = C.Id
	                            INNER JOIN dbo.StoreMapping AS SM ON sm.EntityId = C.Id AND sm.EntityName ='Category'
	                            INNER JOIN dbo.CateguryPostType AS CPT ON CPT.CateguryId = C.Id
	                            INNER JOIN dbo.Product AS P ON P.Id = PCM.ProductId
	                            INNER JOIN dbo.Product_ProductAttribute_Mapping AS PPAM ON PPAM.ProductId = P.Id
	                            INNER JOIN dbo.ProductAttribute AS PA ON PA.Id = PPAM.ProductAttributeId
	                            INNER JOIN dbo.ProductAttributeValue AS PAV ON PAV.ProductAttributeMappingId = PPAM.Id
                            WHERE	
	                            pa.Name LIKE N'%وزن بسته%'
								AND C.ParentCategoryId <> 0
                                AND C.Deleted = 0
	                            AND C.Published = 1
	                            AND SM.StoreId = 5
	                            AND p.Published = 1
	                            AND p.Deleted = 0").ToList();
            return result;
        }

        public void InsertTracking(int shipmentId, OrderShipmentStatusEnum status, string des = "")
        {
            if (string.IsNullOrEmpty(des))
            {
                des = status.GetDisplayName();
            }

            InsertTracking(shipmentId, (int)status, des);
        }

        public void InsertTracking(int shipmentId, int status, string des)
        {
            _repositoryShipmentTracking.Insert(new ShipmentTrackingModel()
            {
                LastShipmentEventDate = DateTime.Now,
                ShipmenEventDesc = des,
                ShipmentEventId = status,
                ShipmentId = shipmentId,
                CreateCustomerId = _workContext.CurrentCustomer.Id
            });
        }

    }
    public class PostTrackResultItems
    {
        public string desc { get; set; }
        public string date { get; set; }
        public string state { get; set; }
    }
    public class TrackingInfo
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        //نام سرویس دهنده پستی
        public string NameServiceProvider { get; set; }
        //نوع سفارش یا ترکنیگ نامبر
        public string TrackingNumber { get; set; }
        //محتویات مرسوله
        public string Description { get; set; }
        //تاریخ ارسال
        public string CreatedOn { get; set; }
        //تاریخ جمع اوری
        public string DataCollect { get; set; }
        //تاریخ ارسال
        public string ShippedDate { get; set; }
        //تاریخ تحویل
        public string DeliveryDate { get; set; }
        //اخرین وضعیت مرسوله
        public string LastSate { get; set; }
        public int OrderStatusId { get; set; }
        //نام فرستنده
        public string SenderFullName { get; set; }
        //ادرس فرستنده
        public string SenderAddress { get; set; }
        //نام گیرنده
        public string ReceiverFullName { get; set; }
        //ادرس گیرنده
        public string ReceiverAddress { get; set; }
        public string LastEventData { get; set; }
        private string _title = "";
        public string Title
        {
            get =>
                (this.ShipmentWeight ?? "") + " " + (("تاریخ ثبت : " + this.CreatedOn) ?? "")
                + " " + ("کد رهگیری : " + (this.TrackingNumber) ?? "");
            set => _title = value;
        }
        //وزن محموله
        public string ShipmentWeight { get; set; }
        public int? CategoryId { get; set; }

        public string AllState { get; set; }
        //public DateTime DateDelivery { get; set; }
        public int CodCost { get; set; }
        public int CodBmValue { get; set; }
    }

    public class OrderShipmentInfo
    {
        public int OrderId { get; set; }
        public int ShipmentId { get; set; }
        public string TrackingNumber { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CreatedOn { get; set; }

    }
    public class OrderShipmentInfoDetails
    {
        public int OrderId { get; set; }
        public string TrackingNumber { get; set; }
        public string CategoryName { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderAddress1 { get; set; }
        public string SebderCountryName { get; set; }
        public string SenderStateName { get; set; }
        public string SenderPhoneNumber { get; set; }
        public string ReceiverFirstName { get; set; }
        public string ReceiverLastName { get; set; }
        public string ReceiverCountryName { get; set; }
        public string ReceiverStateName { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public string ReceiverAddress1 { get; set; }
        public string CreatedOn { get; set; }
        public string ShippedDate { get; set; }
        public string DataCollect { get; set; }
        public string DeliveryDate { get; set; }
        public string _Weight { get; set; }
        public string _Content { get; set; }
        public int OrderStatusId { get; set; }
        public int CategoryId { get; set; }
        public string LastSate { get; set; }
        public string LastEventData { get; set; }
        public string AllState { get; set; }
    }
}
