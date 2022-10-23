using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models
{
    public class AgentNavyModel
    {
        public string CollectoName { get; set; }
        public string CollectorPhoneNumber { get; set; }
        public string VehicleType { get; set; }
    }
    public class AgentModel
    {
        public int CollectorCustomerId { get; set; }
        public int RepresentativeCustomerId { get; set; }
        public string CollectorUsername { get; set; }
        public string CollectorFullName { get; set; }
        public string RepresentativeUsername { get; set; }
        public string RepresentativeFullName { get; set; }
        public int PostexStateId { get; set; }
        public string PostStateCode { get; set; }
    }
}
