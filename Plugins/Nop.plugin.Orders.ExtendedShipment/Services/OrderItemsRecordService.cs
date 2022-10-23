using Nop.Data;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class OrderItemsRecordService : IOrderItemsRecordService
    {
        private readonly IDbContext _dbContext;

        public OrderItemsRecordService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int ShipmentHasPacking(int orderItemId)
        {
            string query = $@"SELECT
	                           TOP(1) OIR.CartonPrice
                            FROM
	                            dbo.Tb_OrderItemsRecord AS OIR
                            WHERE
	                            OIR.OrderItemId = {orderItemId}
                            ORDER BY OIR.Id DESC";

            var result = _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);

            return result;
        }
    }
}
