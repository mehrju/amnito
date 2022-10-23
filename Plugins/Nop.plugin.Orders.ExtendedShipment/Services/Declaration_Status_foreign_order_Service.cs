using Nop.Core;
using Nop.Core.Data;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class Declaration_Status_foreign_order_Service : IDeclaration_Status_foreign_order_Service
    {
        private readonly IRepository<Models.Tbl_TrackingForeignOrder> _repositoryTbl_TrackingForeignOrder;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IExtendedShipmentService _extendedShipmentService;
        public Declaration_Status_foreign_order_Service
            (
            IRepository<Models.Tbl_TrackingForeignOrder> repositoryTbl_TrackingForeignOrder,
            IWorkContext workContext,
             IExtendedShipmentService extendedShipmentService,
             IDbContext dbContext
            )
        {
            _dbContext= dbContext;
            _repositoryTbl_TrackingForeignOrder = repositoryTbl_TrackingForeignOrder;
            _workContext = workContext;
            _extendedShipmentService = extendedShipmentService;
        }

        public bool Insert_to_TrackingForeign(int _OrderId, int _Status, string _Des, int _Mablagh, int _ShipingId)
        {
            try
            {
                Tbl_TrackingForeignOrder temp = new Tbl_TrackingForeignOrder();
                temp.OrderId = _OrderId;
                temp.ShipingId = _ShipingId;
                temp.Status = _Status;
                temp.DateInsert = DateTime.Now;
                temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                temp.Description = _Des;
                temp.IP = _workContext.CurrentCustomer.LastIpAddress;
                temp.Mablagh = _Mablagh;
                temp.IsPay = false;
                _repositoryTbl_TrackingForeignOrder.Insert(temp);
                return true;
            }
            catch (Exception ex)
            {
                _extendedShipmentService.Log("error in TrackingForeignOrder", ex.Message);
                return false;
            }
        }

        public List<Tbl_TrackingForeignOrder> GetTbl_TrackingForeignOrders(int _OrderId, int _ShipingId)
        {
          return  _repositoryTbl_TrackingForeignOrder.Table.Where(p => p.OrderId == _OrderId && p.ShipingId == _ShipingId).ToList().OrderByDescending(p => p.Id).ToList();
        }
        public bool IsOrderConfirm(int orderId)
        {
            string query = $@"SELECT
	                            MAX(id) id
                            INTO #tb1 
                            FROM
	                            dbo.Tbl_TrackingForeignOrder AS TTFO
                            WHERE
	                            TTFO.OrderId = {orderId}
                            GROUP BY TTFO.ShipingId


                            IF NOT EXISTS(SELECT
          	                            TOP(1) 1
                                      FROM
			                            dbo.Tbl_TrackingForeignOrder AS TTFO
			                            INNER JOIN #tb1 AS T ON T.id = TTFO.Id
		                            WHERE
			                            TTFO.Status = 1)
                            BEGIN
                                SELECT CAST(1 AS bit) Confirmmed
                            END
                            ELSE
                            BEGIN
                                SELECT CAST(0 AS bit) Confirmmed
                            END";
            return _dbContext.SqlQuery<bool?>(query,new object[0]).FirstOrDefault().GetValueOrDefault(false);
        }

    }
}
