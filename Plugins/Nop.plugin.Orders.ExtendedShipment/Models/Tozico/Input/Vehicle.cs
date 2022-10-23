using FrotelServiceLibrary.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.Tozico.Input
{
    public partial class Vehicle : BaseInput
    {
        [JsonProperty("branch")]
        public long Branch { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("phone_alt")]
        public string PhoneAlt { get; set; }

        [JsonProperty("is_active")]
        public bool IsActive { get; set; }

        [JsonProperty("is_deleted")]
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public VehicleTypeEnum VehicleTypeEnum { get; set; }

        [JsonProperty("vehicle_type")]
        public string VehicleType
        {
            get
            {
                return Enum.GetName(typeof(VehicleTypeEnum), VehicleTypeEnum);
            }
        }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("capacity_volume")]
        public long CapacityVolume { get; set; }

        [JsonProperty("capacity_weight")]
        public long CapacityWeight { get; set; }

        [JsonProperty("capacity_count")]
        public long CapacityCount { get; set; }

        [JsonProperty("capacity_percent")]
        public long CapacityPercent { get; set; }
    }
}