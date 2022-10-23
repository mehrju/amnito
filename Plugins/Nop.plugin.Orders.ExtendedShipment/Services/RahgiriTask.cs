using Nop.Services.Shipping;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Logging;
using Nop.Core.Infrastructure;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.Services.Configuration;
using Nop.Services.Logging;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class RahgiriTask : IScheduleTask
    {
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IShipmentService _shipmentService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IShipmentTrackingService _shipmentTrackingService;
        public RahgiriTask(IExtendedShipmentService extendedShipmentService
            , IShipmentService shipmentService
            , ISettingService settingService
            , IStoreContext storeContext
            , IShipmentTrackingService shipmentTrackingService)
        {
            _shipmentTrackingService = shipmentTrackingService;
            _extendedShipmentService = extendedShipmentService;
            _shipmentService = shipmentService;
            _settingService = settingService;
            _storeContext = storeContext;
        }
        public void Execute()
        {
            DateTime dt = DateTime.Now.AddDays(-15);
            int count = 0;
            var setting = _settingService.LoadSetting<ExtendedShipmentSetting>(_storeContext.CurrentStore.Id);
            var Shipments = _extendedShipmentService.GetAllShipments(out count, createdFromUtc: dt, ignorCod: 0);
            foreach (var item in Shipments)
            {
                try
                {
                    if (!string.IsNullOrEmpty(item.TrackingNumber) && string.IsNullOrEmpty(item.DeliveryDate))
                    {
                        var shipment = _shipmentService.GetShipmentById(item.Id);
                        string Str_Error = "";
                        _shipmentTrackingService.RegisterLastShipmentStatus(item.Id,
                            shipment.Order.PaymentMethodSystemName == "Payments.CashOnDelivery", out Str_Error);
                    }
                }
                catch (Exception ex)
                {
                    Log("خطا در زمان به روز رسانی زمان بندی شده از پست"+"===> "+ item.Id, ex.Message +
                                                                                                  (ex.InnerException != null ? ex.InnerException.Message : ""));
                }
            }
        }
        public void Log(string header, string Message)
        {
            var logger =
                (DefaultLogger)EngineContext.Current
                    .Resolve<ILogger>();
            logger.InsertLog(LogLevel.Information, header, Message, null);
        }
    }
}
