using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.plugin.Orders.ExtendedShipment.Models;
using System.Collections.Generic;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface IAgentConfigService
    {
        List<SelectListItem> GetAgents();
        List<SelectListItem> GetCollectors();
        void SaveAgentConfig(AgentConfigResultModel model);
        List<AgentConfigGridModel> GridAgentConfig(AgentConfigInputModel model, int PageSize, int PageIndex, out int count);
        void UpdateAgentConfig(AgentConfigResultModel model);
        List<ServiceDiscountGridModel> GridServiceDiscount(int CustomerId);
        List<SelectListItem> GetServices();
        void SaveServiceDiscount(ServiceDiscountModel model);
    }
}