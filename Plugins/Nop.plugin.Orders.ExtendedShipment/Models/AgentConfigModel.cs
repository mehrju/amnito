using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class AgentConfigResultModel
    {
        public int Id { get; set; }
        public int AgentCustomerId { get; set; }
        public int NearStateId { get; set; }
        public int RepresentativeCustomerId { get; set; }
    }

    public class AgentConfigInputModel
    {
        public int AgentCustomerId { get; set; }
        public int RepresentativeCustomerId { get; set; }
        public int NearCountryId { get; set; }
        public int NearStateId { get; set; }
    }

    public class AgentConfigGridModel
    {
        public int Id { get; set; }
        public int AgentCustomerId { get; set; }
        public int NearSateId { get; set; }
        public int NearCountryId { get; set; }
        public int RepresentativeCustomerId { get; set; }
        public string AgentCustomerName { get; set; }
        public string RepresentativeCustomerName { get; set; }
        public string NearStateName { get; set; }
        public string NearCountryName { get; set; }
    }

    public class ServiceDiscountModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ServiceId { get; set; }
        public int DiscountPercent { get; set; }
        public int DiscountPrice { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DeactiveDate { get; set; }
        public int? DeactiveCustomerId { get; set; }
        public DateTime? ActiveDate { get; set; }
        public int ActiveCustomerId { get; set; }
    }

    public class ServiceDiscountGridModel
    {
        public string ServiceName { get; set; }
        public int Discount { get; set; }
    }
}
