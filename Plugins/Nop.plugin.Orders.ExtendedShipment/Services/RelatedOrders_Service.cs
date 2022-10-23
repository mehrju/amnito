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
    public class RelatedOrders_Service : IRelatedOrders_Service
    {
        private readonly IRepository<Models.Tb_RelatedOrders> _repositoryTb_RelatedOrders;
        private readonly IWorkContext _workContext;
        private readonly IExtendedShipmentService _extendedShipmentService;

        public RelatedOrders_Service
            (
           IRepository<Models.Tb_RelatedOrders> repositoryTb_RelatedOrders,
             IWorkContext workContext,
             IExtendedShipmentService extendedShipmentService
            )
        {
            _repositoryTb_RelatedOrders = repositoryTb_RelatedOrders;
            _workContext = workContext;
            _extendedShipmentService = extendedShipmentService;
        }

        public bool Insert(int ParentOrderId,int ChildOrderId)
        {
            try
            {
                Tb_RelatedOrders temp = new Tb_RelatedOrders();
                temp.ChildOrderId = ChildOrderId;
                temp.ParentOrderId = ParentOrderId;
                _repositoryTb_RelatedOrders.Insert(temp);
                return true;
            }
            catch (Exception ex)
            {
                _extendedShipmentService.Log("Error in insert table RelatedOrders", ex.Message);
                return false;
            }
        }

        public Tb_RelatedOrders GetTb_RelatedOrders_By_ParentOrderId(int ParentOrderId)
        {
            return _repositoryTb_RelatedOrders.Table.Where(p => p.ParentOrderId == ParentOrderId).FirstOrDefault();
        }


    }
}
