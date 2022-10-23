using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.PostbarDashboard.Services;
using Nop.Plugin.Misc.Ticket.Domain;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Events
{
    /// <summary>
    /// پرداخت هزینه cod
    /// </summary>
    public class OrderPaidEventConsumer : IConsumer<OrderPaidEvent>
    {
        private readonly IRepository<Tbl_RequestCODCustomer> _repository_Tbl_RequestCODCustomer;
        private readonly ICODRequestService _cODRequestService;
        private readonly Nop.Services.Orders.IOrderService _orderServices;

        public OrderPaidEventConsumer(IRepository<Tbl_RequestCODCustomer> repository_Tbl_RequestCODCustomer,
            ICODRequestService cODRequestService,
            Nop.Services.Orders.IOrderService orderServices)
        {
            _repository_Tbl_RequestCODCustomer = repository_Tbl_RequestCODCustomer;
            _cODRequestService = cODRequestService;
            _orderServices = orderServices;
        }

        public void HandleEvent(OrderPaidEvent eventMessage)
        {
            var codRequest = _repository_Tbl_RequestCODCustomer.Table.FirstOrDefault(p=>p.OrderId == eventMessage.Order.Id);
            if(codRequest != null)
            {
                _cODRequestService.CODRequestPaid(codRequest);
                eventMessage.Order.OrderStatus = OrderStatus.Complete;
                _orderServices.UpdateOrder(eventMessage.Order);
            }
        }
    }
}
