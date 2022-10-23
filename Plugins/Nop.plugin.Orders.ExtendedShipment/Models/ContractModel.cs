using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class ContractDetaill
    {
        public int ContractId { get; set; }
        public int CustomerId { get; set; }
        public int ContractItemsId { get; set; }
        public int ContractItemsTypeId { get; set; }
        public int? ContractorCustomerId { get; set; }
        public string ContractItemsTypeName { get; set; }
        public int? ContractItemDetailesId_Pk { get; set; }
        public int? ContractItemDetailesCost { get; set; }
        public int? ContractItemDetailesPrice { get; set; }
        public int? ContractItemPercent { get; set; }
        public int? ContractItemNumber { get; set; }
    }
    public class CustomerContractPakcingItems
    {
        public int ContractItemDetailesId_Pk { get; set; }
        public int ContractItemDetailesCost { get; set; }
        public int ContractItemDetailesPrice { get; set; }
        public string ContractItemDetailesName { get; set; }
    }
    public class ContractItems
    {
        public int ContractId { get; set; }
        public int CustomerId { get; set; }

        public int? RegistereContrctor { get; set; }
        public int? RegistereContrctorCost { get; set; }
        public int? RegistereContrctorPrice { get; set; }

        public int? CollectorContrctor { get; set; }
        public int? CollectorContrctorCost { get; set; }
        public int? CollectorContrctorPrice { get; set; }

        public int? PriningContrctor { get; set; }
        public int? PriningContrctorCost { get; set; }
        public int? PriningContrctorPrice { get; set; }

        public int? AffiliateContrctor { get; set; }
        public int? MarketingCommissionValue { get; set; }
        public int? MarketingCommissionLeasing { get; set; }

        public int? PackingContractor { get; set; }
        public List<PackingItemForContract> PackingContractorItems { get; set; }

        public int? LeasingPercent { get; set; }
        public int? ReturnRoofValue { get; set; }
        public int? DailyCrediteRoofValue { get; set; }
        public int? ShippingAcceptancePercentageValue { get; set; }
        public int? CODPercent { get; set; }
    }
    public class PackingItemForContract
    {
        public string CartoonItemsName { get; set; }
        public int? CartoonItemsCost { get; set; }
        public int? CartoonItemsPrice { get; set; }
    }

}
