using Newtonsoft.Json;

namespace FrotelServiceLibrary.Input
{
    public class TrackingInput : BaseInput
    {
        /// <summary>
        /// شماره سفارش	
        /// </summary>
        [JsonProperty("factor")]
        public string FactorId { get; set; }
    }
}