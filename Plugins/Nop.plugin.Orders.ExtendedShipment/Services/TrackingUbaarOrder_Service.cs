using Nop.Core;
using Nop.Core.Data;
using Nop.plugin.Orders.ExtendedShipment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class TrackingUbaarOrder_Service : ITrackingUbaarOrder_Service
    {
        private readonly IRepository<Models.Tbl_TrackingUbaarOrder> _repositoryTbl_TrackingUbaarOrder;
        private readonly IWorkContext _workContext;
        private readonly IExtendedShipmentService _extendedShipmentService;
        //
        public TrackingUbaarOrder_Service
            (
              IRepository<Models.Tbl_TrackingUbaarOrder> repositoryTbl_TrackingUbaarOrder,
             IWorkContext workContext,
             IExtendedShipmentService extendedShipmentService
            )
        {
            _repositoryTbl_TrackingUbaarOrder = repositoryTbl_TrackingUbaarOrder;
            _workContext = workContext;
            _extendedShipmentService = extendedShipmentService;
        }
        public bool Insert(int _OrderId, int _OrderItemId, int _Status, string _Des, int _Price, int _NewPrice)
        {
            try
            {
                Tbl_TrackingUbaarOrder temp = new Tbl_TrackingUbaarOrder();
                temp.OrderId = _OrderId;
                temp.OrderItemId = _OrderItemId;
                temp.Status = _Status;
                temp.DateInsert = DateTime.Now;
                temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                temp.Description = _Des;
                temp.IP = _workContext.CurrentCustomer.LastIpAddress;
                temp.price = _Price;
                temp.newprice = _NewPrice;
                temp.IsPay = false;
                _repositoryTbl_TrackingUbaarOrder.Insert(temp);
                return true;
            }
            catch (Exception ex)
            {
                _extendedShipmentService.Log("Error in insert table TrackingUbaarOrder", ex.Message);
                return false;
            }
        }
    }
}
