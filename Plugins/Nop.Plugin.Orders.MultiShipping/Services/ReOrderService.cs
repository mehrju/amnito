using Nop.Core.Data;
using Nop.Plugin.Orders.MultiShipping.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public class ReOrderService : IReOrderService
    {
        private readonly IRepository<Tbl_OrderJson> _repository_OrderJson;

        public ReOrderService(IRepository<Tbl_OrderJson> repository_OrderJson)
        {
            _repository_OrderJson = repository_OrderJson;
        }

        public void InsertOrderJson(int orderId, string orderJson,bool isWebApi = false)
        {
            _repository_OrderJson.Insert(new Tbl_OrderJson()
            {
                JsonData = orderJson,
                OrderId = orderId,
                IsWebApi = isWebApi
            });
        }
    }
}
