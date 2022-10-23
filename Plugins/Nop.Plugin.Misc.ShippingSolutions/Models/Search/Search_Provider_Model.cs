using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Search
{
    public class Search_Provider_Model
    {
        public Search_Provider_Model()
        {
            ListCategory = new List<SelectListItem>();
            ListOffice_City = new List<SelectListItem>();
            ListOffice_State = new List<SelectListItem>();
            ListServiceTypes = new List<SelectListItem>();
            //ListTransportationTypes = new List<SelectListItem>();
            ListStores = new List<SelectListItem>();
        }
        public bool ActiveSearch { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchServicesProviderName")]
        public string SearchServicesProviderName { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchServicesProviderAgentName")]
        public string SearchServicesProviderAgentName { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchCategoryId")]
        public int SearchCategoryId { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchIsActive")]
        public bool SearchIsActive { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchMaxOrder")]
        public int SearchMaxOrder { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchMaxWeight")]
        public int SearchMaxWeight { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchMinWeight")]
        public int SearchMinWeight { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchMaxTimeDeliver")]
        public int SearchMaxTimeDeliver { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Searchadvancefreight")]
        public bool Searchadvancefreight { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Searchfreightforward")]
        public bool Searchfreightforward { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.Searchcod")]
        public bool Searchcod { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchOfficeId")]
        public int SearchOfficeId { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchServiceTypeId")]
        public int SearchServiceTypeId { get; set; }
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchTransportationId")]
        //public int SearchTransportationId { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchStartTime")]
        public TimeSpan SearchStartTime { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchEndTime")]
        public TimeSpan SearchEndTime { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchStoreId")]
        public int SearchStoreId { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchIsPishtaz")]
        public bool SearchIsPishtaz { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchIsSefareshi")]
        public bool SearchIsSefareshi { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchIsVIje")]
        public bool SearchIsVIje { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchIsNromal")]
        public bool SearchIsNromal { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchIsDroonOstani")]
        public bool SearchIsDroonOstani { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchIsAdjoining")]
        public bool SearchIsAdjoining { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchIsNotAdjacent")]
        public bool SearchIsNotAdjacent { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchIsHeavyTransport")]
        public bool SearchIsHeavyTransport { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchIsForeign")]
        public bool SearchIsForeign { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchIsInCity")]
        public bool SearchIsInCity { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchIsAmanat")]
        public bool SearchIsAmanat { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchIsTwoStep")]
        public bool SearchIsTwoStep { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchHasHagheMaghar")]
        public bool SearchHasHagheMaghar { get; set; }








        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchMaxlength")]
        public int SearchMaxlength { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchMaxwidth")]
        public int SearchMaxwidth { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchMaxheight")]
        public int SearchMaxheight { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchMaxbillingamountCOD")]
        public double SearchMaxbillingamountCOD { get; set; }



















        public IList<SelectListItem> ListCategory { get; set; }
        public IList<SelectListItem> ListOffice_State { get; set; }
        public IList<SelectListItem> ListOffice_City { get; set; }
        public IList<SelectListItem> ListServiceTypes { get; set; }
        //public IList<SelectListItem> ListTransportationTypes { get; set; }
        public IList<SelectListItem> ListStores { get; set; }



    }
}
