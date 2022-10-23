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
    public class CalcPriceOrderItem_Service : ICalcPriceOrderItem_Service
    {
        private readonly IRepository<Tb_CalcPriceOrderItem> _Tb_CalcPriceOrderItemRepository;
        private readonly IDbContext _dbContext;
        private readonly IExtendedShipmentService _extendedShipmentService;
        public CalcPriceOrderItem_Service(IRepository<Tb_CalcPriceOrderItem> Tb_CalcPriceOrderItemRepository, IDbContext dbContext, IExtendedShipmentService extendedShipmentService)
        {
            _Tb_CalcPriceOrderItemRepository = Tb_CalcPriceOrderItemRepository;
            _dbContext = dbContext;
            _extendedShipmentService = extendedShipmentService;
        }


        public int Get_IncomePrice_by_OrderItemId(int OrderItem)
        {
            return _Tb_CalcPriceOrderItemRepository.Table.Where(p=>p.OrderItemId==OrderItem).FirstOrDefault().IncomePrice;
        }

        public bool Update_IncomePrice_by_OrderItemId(int OrderItem,int price)
        {
            try
            {
                Tb_CalcPriceOrderItem tb_Calc = _Tb_CalcPriceOrderItemRepository.Table.Where(p => p.OrderItemId == OrderItem).FirstOrDefault();
                tb_Calc.IncomePrice = price;
                _Tb_CalcPriceOrderItemRepository.Update(tb_Calc);
                return true;
            }
            catch (Exception ex)
            {
                _extendedShipmentService.Log("Error in update table Tb_CalcPriceOrderItem", ex.Message);
                return false;
            }
        }


    }
}
