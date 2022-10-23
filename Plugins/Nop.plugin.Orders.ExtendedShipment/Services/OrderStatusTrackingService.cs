using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Models;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class OrderStatusTrackingService: IOrderStatusTrackingService
    {
        private readonly IRepository<OrderStatusTrackingModel> _orderStatusTrackinRepository;
        private readonly IDbContext _dbContext;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;
        public OrderStatusTrackingService(
            IWorkContext workContext,
            IRepository<OrderStatusTrackingModel> orderStatusTrackinRepository,
            IDbContext dbContext,
            IWebHelper webHelper
            )
        {
            _webHelper = webHelper;
            _workContext = workContext;
            _orderStatusTrackinRepository = orderStatusTrackinRepository;
            _dbContext = dbContext;
        }
        public void Insert(Order order)
        {
            Insert(order.Id, order.OrderStatusId);
        }


        public void Insert(int orderId, int orderStatusId)
        {
            var orderStatusTrackingModel = _orderStatusTrackinRepository.Table.OrderByDescending(p => p.Id).FirstOrDefault(p => p.orderId == orderId
                                                                                                                                && p.OrderstatusId == orderStatusId);
            if (orderStatusTrackingModel == null)
            {
                int customerId = _workContext.CurrentCustomer == null ? 0 : _workContext.CurrentCustomer.Id;
                orderStatusTrackingModel = new OrderStatusTrackingModel()
                {
                    OrderstatusId = orderStatusId,
                    chageDate = DateTime.Now,
                    orderId = orderId,
                    CustomerId = customerId,
                    ClientIp = _webHelper.GetCurrentIpAddress()

                };
                _orderStatusTrackinRepository.Insert(orderStatusTrackingModel);
            }
            else
            {
                orderStatusTrackingModel.chageDate = DateTime.Now;
                orderStatusTrackingModel.CustomerId = _workContext.CurrentCustomer == null ? 0 : _workContext.CurrentCustomer.Id;
                _orderStatusTrackinRepository.Update(orderStatusTrackingModel);
            }
        }

        public bool IsFirstOrder(int orderId)
        {
            string query = @"SELECT
	                        TOP(1)1
                        FROM
	                        FirstOrder
                        WHERE OrderId = "+ orderId;
            var result = _dbContext.SqlQuery<int>(query, new object[0]).FirstOrDefault();
            if (result == 0)
                return false;
            return true;
        }
    }
}
