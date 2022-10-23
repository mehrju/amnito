using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nop.Plugin.Misc.ShippingSolutions.Domain
{

    public class Tbl_ServicesProviders : BaseEntity
    {
        public Tbl_ServicesProviders()
        {
            //ServiceTypesProvider = new List<Tbl_ServiceTypesProvider>();
            //TransportationTypesProvider = new List<Tbl_TransportationTypesProvider>();
            //Offices = new List<Tbl_Offices>();
            ProviderStores = new List<Tbl_ProviderStores>();
            CollectorsServiceProvider = new List<Tbl_CollectorsServiceProvider>();
            ///
            ListCategory = new List<SelectListItem>();
            // ListOffice_City = new List<SelectListItem>();
            //ListOffice_State = new List<SelectListItem>();
            ListServiceTypes = new List<SelectListItem>();
            //ListTransportationTypes = new List<SelectListItem>();
            ListStores = new List<SelectListItem>();
            ListStateProvince = new List<SelectListItem>();
        }
        //[ScaffoldColumn(false)]
        //public int Id { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderName")]
        [DataType(DataType.Text)]
        public string ServicesProviderName { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderAgentName")]
        [DataType(DataType.Text)]
        public string AgentName { get; set; }


        [Required]
        [ScaffoldColumn(false)]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderServiceTypeId")]
        public int ServiceTypeId { get; set; }

        [Required]
        [ScaffoldColumn(false)]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderCategoryId")]
        public int CategoryId { get; set; }

        [ScaffoldColumn(false)]
        public bool IsActive { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderMaxOrder")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int MaxOrder { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderMaxWeight")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int MaxWeight { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderMinWeight")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int MinWeight { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderMaxTimeDeliver")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int MaxTimeDeliver { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProvideradvancefreight")]
        //[Range(typeof(bool), "false", "true")]
        public bool advancefreight { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderfreightforward")]

        public bool freightforward { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProvidercod")]

        public bool cod { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsPishtaz")]
        public bool IsPishtaz { get; set; }
        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsSefareshi")]
        public bool IsSefareshi { get; set; }
        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsVIje")]
        public bool IsVIje { get; set; }
        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsNromal")]
        public bool IsNromal { get; set; }
        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsDroonOstani")]
        public bool IsDroonOstani { get; set; }
        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsAdjoining")]
        public bool IsAdjoining { get; set; }
        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsNotAdjacent")]
        public bool IsNotAdjacent { get; set; }
        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsHeavyTransport")]
        public bool IsHeavyTransport { get; set; }
        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsForeign")]
        public bool IsForeign { get; set; }
        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsInCity")]
        public bool IsInCity { get; set; }

        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsAmanat")]
        //public bool IsAmanat { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewIsTwoStep")]
        public bool IsTwoStep { get; set; }
        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewHasHagheMaghar")]
        public bool HasHagheMaghar { get; set; }




        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderMaxMaxlength")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int Maxlength { get; set; }
        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderMaxMaxwidth")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int Maxwidth { get; set; }
        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderMaxheight")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int Maxheight { get; set; }
        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewProviderMaxbillingamountCOD")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter valid double Number")]
        public double MaxbillingamountCOD { get; set; }





        [ScaffoldColumn(false)]
        public DateTime DateInsert { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? DateUpdate { get; set; }
        [ScaffoldColumn(false)]
        public int IdUserInsert { get; set; }
        [ScaffoldColumn(false)]
        public int? IdUserUpdate { get; set; }

        public List<Tbl_ServiceTypesProvider> ServiceTypesProvider { get; set; }
        //public List<Tbl_TransportationTypesProvider> TransportationTypesProvider { get; set; }
        //public List<Tbl_Offices> Offices { get; set; }
        public List<Tbl_ProviderStores> ProviderStores { get; set; }
        public List<Tbl_CollectorsServiceProvider> CollectorsServiceProvider { get; set; }
        public List<Tbl_Dealer_Customer_ServiceProvider> Dealer_Customer_ServiceProvider { get; set; }
        //public List<Tbl_PricingPolicy> PricingPolicyServiceProvider { get; set; }

        //page crate or update
        [NotMapped]
        public IList<SelectListItem> ListCategory { get; set; }
        //[NotMapped]
        //public IList<SelectListItem> ListOffice_State { get; set; }
        //[NotMapped]
        //public IList<SelectListItem> ListOffice_City { get; set; }



        [NotMapped]
        public IList<SelectListItem> ListServiceTypes { get; set; }

        //[NotMapped]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewTransportationTypeId")]
        //public IList<int> TransportationTypeId { get; set; }
        //[NotMapped]
        //public IList<SelectListItem> ListTransportationTypes { get; set; }

        [NotMapped]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewStoreId")]
        public IList<int> StoreId { get; set; }
        [NotMapped]
        public IList<SelectListItem> ListStores { get; set; }


        [NotMapped]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorStateProvinceId")]
        public IList<int> StateProvinceId { get; set; }
        [NotMapped]
        public IList<SelectListItem> ListStateProvince { get; set; }
    }
}
