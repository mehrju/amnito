using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class ShipmentController : BaseApiController
    {
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        public ShipmentController(IWorkContext workContext, IDbContext dbContext)
        {
            _workContext = workContext;
            _dbContext = dbContext;
        }
        [Route("api/Shipments/GetListForCollecting")]
        [HttpPost]
        public IActionResult getShipmentsForCollecting(getShipmentForCollectorModel model)
        {
            try
            {
                string query = $@"EXEC dbo.Sp_GetCollectorData @CustomerId , @DateFrom , @DateTo";

                SqlParameter P_CustomerId = new SqlParameter()
                {
                    ParameterName = "CustomerId",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)model.CollectorAgentId
                };
                SqlParameter P_DateFrom = new SqlParameter()
                {
                    ParameterName = "DateFrom",
                    SqlDbType = SqlDbType.DateTime,
                    Value = (object)model.From
                };
                SqlParameter P_DateTo = new SqlParameter()
                {
                    ParameterName = "DateTo",
                    SqlDbType = SqlDbType.DateTime,
                    Value = (object)model.To
                };
                SqlParameter[] prms = new SqlParameter[]{
                P_CustomerId,
                P_DateFrom,
                P_DateTo
                };
                var data = _dbContext.SqlQuery<ShipmentForCollecting>(query, prms).ToList();
                return Json(new { success = true, message = "", data = data });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { success = false, message = "اطلاعاتی یافت نشد", data = (List<ShipmentForCollecting>)null });
            }

        }
        [Route("api/Shipments/setShipmentStatus")]
        [HttpGet]
        public IActionResult setShipmentStatus(int shipmentId, int shipmentEventId)
        {
            return Json(new { success = true, message = "ثبت با موفقیت انجام شد", });
        }
    }
    public class getShipmentForCollectorModel
    {
        public int CollectorAgentId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
    public class ShipmentForCollecting
    {
        public int OrderId { get; set; }
        public string TrackingNumber { get; set; }
        public string CreateDateTime { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderPhoneNumber { get; set; }
        public string SenderAddress { get; set; }
        public double? SenderLat { get; set; }
        public double? SenderLong { get; set; }
        public string ReciverFirstName { get; set; }
        public string ReciverLastName { get; set; }
        public string ReciverPhoneNumber { get; set; }
        public string ReciverAddress { get; set; }
        public double? ReciverLat { get; set; }
        public double? ReciverLong { get; set; }
        public string ExactWeight { get; set; }
        public string Dimensions { get; set; }
        public string ServiceName { get; set; }
        public string SenderPostalCode { get; set; }
        public string ReciverPostalCode { get; set; }
        public int ServiceId { get; set; }
        public bool Needcarton { get; set; }
    }
}
