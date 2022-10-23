using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Orders;
using Nop.plugin.Orders.ExtendedShipment.Models;
using static Nop.plugin.Orders.ExtendedShipment.Services.AgentAmountRuleService;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface IAgentAmountRuleService
    {
        int GetPrivatePostDiscountForCustomer(int CustomerId, int ServiceId);
        InlineAgentSaleAMount getInlineDsicountByCustomer(OrderItem item, int CustomerId, out PrivatePostDiscount privatePostDiscount);
        InlineAgentSaleAMount getInlineCustomerDiscountForShipment(OrderItem item, out PrivatePostDiscount privatePostDiscount);
        void DeActiveAssignAgentAmountRule(int AssignAgentAmountRuleId, out string error);
        bool SaveAssignAgentAmountRule(AssignAgentAmountRuleModel model, out string error);
        List<AssignAgentAmountRuleModel> getAssignAgentAmountList(AssignAgentAmountRuleModel model);
        bool SaveAgentAmountRule(AgentAmountRuleModel model, out string error);
        bool DdeleteAgentAmountRule(int agentAmountRuleId);
        List<AgentAmountRuleModel> getList();
        List<SelectListItem> getAvailableAgentList(string userName);
        List<SelectListItem> getAvailableAgentAmountRule();
        bool updateAgentAmountRule(AgentAmountRuleModel model, out string error);
        bool getAgentAmountRuleEnable(string name);
        bool IsCustmoerInAgentRole(int customerId);
        bool IsCustomerInPrivcatePostDiscount(int customerId);
        PrivatePostDiscount GetPrivatePostDiscount(int CostomerId, int ServiceId);
        InlineAgentSaleAMount getInlineAgentSaleAmount(OrderItem item, out PrivatePostDiscount privatePostDiscount);
        void CalcOutLineAgentSaleAmount(Order order);

    }
}
