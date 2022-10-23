using Nop.Core;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services.ForiegnOrder
{
    public class ForeignOrder : IForeignOrder
    {
        private readonly IDbContext _dbContext;
        private readonly IWorkContext _workContext;
        public ForeignOrder(IDbContext dbContext, IWorkContext workContext)
        {
            _dbContext = dbContext;
            _workContext = workContext;
        }
        public List<ForeinOrderModel> GetForeinOrdersList(int ServiceId, int ForeignOrderStatus, int OrderId,
            DateTime? OrderDateFrom, DateTime? OrderDateTo, out int Count, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            string query = $@"EXEC dbo.Sp_ForeinOrderList @CustomerId , @ServiceId , @ForeignOrderStatus , @OrderId, @OrderDateFrom , @OrderDateTo,@PageSize,@PageIndex, @Count OUTPUT ";
            Count = 0;
            SqlParameter P_CustomerId = new SqlParameter()
            {
                ParameterName = "CustomerId",
                SqlDbType = SqlDbType.Int,
                Value = (object)_workContext.CurrentCustomer.Id
            }; SqlParameter P_serviceId = new SqlParameter()
            {
                ParameterName = "ServiceId",
                SqlDbType = SqlDbType.Int,
                Value = (object)ServiceId
            }; SqlParameter P_ForeignOrderStatus = new SqlParameter()
            {
                ParameterName = "ForeignOrderStatus",
                SqlDbType = SqlDbType.Int,
                Value = (object)ForeignOrderStatus
            };
            SqlParameter P_OrderId = new SqlParameter()
            {
                ParameterName = "OrderId",
                SqlDbType = SqlDbType.Int,
                Value = (object)OrderId
            };
            SqlParameter P_OrderDateFrom = new SqlParameter()
            {
                ParameterName = "OrderDateFrom",
                SqlDbType = SqlDbType.DateTime,
                Value = (object)OrderDateFrom ?? DBNull.Value
            };
            SqlParameter P_OrderDateTo = new SqlParameter()
            {
                ParameterName = "OrderDateTo",
                SqlDbType = SqlDbType.DateTime,
                Value = (object)OrderDateTo ?? DBNull.Value
            };
            SqlParameter P_PageSize = new SqlParameter()
            {
                ParameterName = "PageSize",
                SqlDbType = SqlDbType.Int,
                Value = (object)pageSize
            };
            SqlParameter P_PageIndex = new SqlParameter()
            {
                ParameterName = "PageIndex",
                SqlDbType = SqlDbType.Int,
                Value = (object)pageIndex
            };
            SqlParameter P_Count = new SqlParameter()
            {
                ParameterName = "Count",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output,
                Value = (object)Count
            };

            SqlParameter[] prms = new SqlParameter[]{
                P_CustomerId,
                P_serviceId,
                P_ForeignOrderStatus,
                P_OrderId,
                P_OrderDateFrom,
                P_OrderDateTo,
                P_PageSize,
                P_PageIndex,
                P_Count
            };
            var reuslt= _dbContext.SqlQuery<ForeinOrderModel>(query, prms).ToList();
            Count = int.Parse(P_Count.Value.ToString());
            return reuslt;
        }
        public class ForeinOrderModel
        {
            public int OrderId { get; set; }
            public int ShipmentId { get; set; }
            public string ServiceName { get; set; }
            public string CreateDateTime_Sh { get; set; }
            public string ShippedDate { get; set; }
            public string DeliveryDate { get; set; }
            public int OrderTotal { get; set; }
            public string SenderName { get; set; }
            public string SenderCity { get; set; }
            public string SenderAddress { get; set; }
            public string SenderPhoneNumber { get; set; }
            public string ReceaiverAddress { get; set; }
            public string ReceaiverName { get; set; }
            public string ReceaiverPhoneNumber { get; set; }
            public string ReceaiverEmail { get; set; }
            public string GoodsType { get; set; }
            public string ExactWeight { get; set; }
            public string CategoryName { get; set; }
            public string img1 { get; set; }
            public string img2 { get; set; }
            public string img3 { get; set; }
        }

    }
}
