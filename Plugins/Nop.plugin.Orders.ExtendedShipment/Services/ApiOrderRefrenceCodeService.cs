using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class ApiOrderRefrenceCodeService : IApiOrderRefrenceCodeService
    {
        private readonly IRepository<Tbl_ApiOrderRefrenceCode> _repository_TblApiOrderRefrenceCode;
        private readonly IRepository<Order> _repository_Order;

        public ApiOrderRefrenceCodeService(IRepository<Tbl_ApiOrderRefrenceCode> repository_TblApiOrderRefrenceCode,
            IRepository<Order> repository_Order)
        {
            _repository_TblApiOrderRefrenceCode = repository_TblApiOrderRefrenceCode;
            _repository_Order = repository_Order;
        }


        public bool CheckAndInsertApiOrderRefrenceCode(int customerId, string refrenceNo,out Tbl_ApiOrderRefrenceCode obj)
        {
            obj = _repository_TblApiOrderRefrenceCode.Table.FirstOrDefault(p => p.CustomerId == customerId && p.RefrenceCode == refrenceNo);
            if (obj != null)
            {
                if (obj.OrderId.HasValue)
                {
                    var orderId = obj.OrderId.Value;
                    var order = _repository_Order.Table.FirstOrDefault(p => p.Id == orderId);
                    if(order != null && order.OrderStatus != OrderStatus.Cancelled)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return true;
            }
            obj = new Tbl_ApiOrderRefrenceCode
            {
                CustomerId = customerId,
                RefrenceCode = refrenceNo
            };
            _repository_TblApiOrderRefrenceCode.Insert(obj);
            return true;
        }


        public void SetOrderId(int customerId, string refrenceNo, int orderId)
        {
            var model = _repository_TblApiOrderRefrenceCode.Table.FirstOrDefault(p => p.CustomerId == customerId && p.RefrenceCode == refrenceNo);
            if(model != null)
            {
                model.OrderId = orderId;
                _repository_TblApiOrderRefrenceCode.Update(model);
            }
        }
        public string getRefrenceCode(int orderId)
        {
           return _repository_TblApiOrderRefrenceCode.Table.Where(p => p.OrderId == orderId).OrderByDescending(p => p.Id).Select(p=> p.RefrenceCode).FirstOrDefault();
        }
        public int getOrderId(string RefrenceCode)
        {
           return _repository_TblApiOrderRefrenceCode.Table.Where(p => p.RefrenceCode == RefrenceCode).OrderByDescending(p => p.Id).Select(p=> p.OrderId).FirstOrDefault()
                .GetValueOrDefault(0);
        }
        
    }
}
