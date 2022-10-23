using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Checkout
{
    public class CheckoutPickupPointResponceModel : BaseResponse
    {
        public CheckoutPickupPointResponceModel()
        {
            Warnings = new List<string>();
            PickupPoints = new List<CheckoutPickupPointModelApi>();
        }
        public IList<string> Warnings { get; set; }
        public IList<CheckoutPickupPointModelApi> PickupPoints { get; set; }
        public bool AllowPickUpInStore { get; set; }
    }
    public class CheckoutPickupPointModelApi
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ProviderSystemName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string CountryName { get; set; }

        public string ZipPostalCode { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public string PickupFee { get; set; }

        public string OpeningHours { get; set; }
    }
}
