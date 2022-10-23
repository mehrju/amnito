using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Shipping;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Models;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Checkout
{
    public partial class CheckoutShippingMethodResponseModel : BaseResponse
    {
        public CheckoutShippingMethodResponseModel()
        {
            ShippingMethods = new List<ShippingMethodModel>();
            Warnings = new List<string>();
        }

        public IList<ShippingMethodModel> ShippingMethods { get; set; }

        public bool NotifyCustomerAboutShippingFromMultipleLocations { get; set; }

        public IList<string> Warnings { get; set; }

        #region Nested classes

        public partial class ShippingMethodModel : BaseNopModel
        {
            public string ShippingRateComputationMethodSystemName { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Fee { get; set; }
            public bool Selected { get; set; }

            public ShippingOptionModel ShippingOption { get; set; }
        }

        public partial class ShippingOptionModel 
        {
            /// <summary>
            /// Gets or sets the system name of shipping rate computation method
            /// </summary>
            public string ShippingRateComputationMethodSystemName { get; set; }

            /// <summary>
            /// Gets or sets a shipping rate (without discounts, additional shipping charges, etc)
            /// </summary>
            public decimal Rate { get; set; }

            /// <summary>
            /// Gets or sets a shipping option name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets a shipping option description
            /// </summary>
            public string Description { get; set; }
        }
        #endregion
    }
}
