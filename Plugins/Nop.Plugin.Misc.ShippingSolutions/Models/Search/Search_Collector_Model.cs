using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Search
{
    public class Search_Collector_Model
    {
        public Search_Collector_Model()
        {
            ListOffice_City = new List<SelectListItem>();
            ListOffice_State = new List<SelectListItem>();
            ListProvider = new List<SelectListItem>();
            ListStores = new List<SelectListItem>();
        }
        public bool ActiveSearch { get; set; }
        
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchName")]
        public string SearchName { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.SearchUserName")]
        public string SearchUserName { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsActive")]
        public bool SearchIsActive { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchMaxPath")]
        public int SearchMaxPath { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchMaxWeight")]
        public int SearchMaxWeight { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchMinWeight")]
        public int SearchMinWeight { get; set; }



        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchadvancefreight")]
        //public bool Searchadvancefreight { get; set; }


        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchfreightforward")]
        //public bool Searchfreightforward { get; set; }


        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchcod")]
        //public bool Searchcod { get; set; }


        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsPishtaz")]
        //public bool SearchIsPishtaz { get; set; }

        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsSefareshi")]
        //public bool SearchIsSefareshi { get; set; }

        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsVIje")]
        //public bool SearchIsVIje { get; set; }

        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsNromal")]
        //public bool SearchIsNromal { get; set; }

        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsDroonOstani")]
        //public bool SearchIsDroonOstani { get; set; }

        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsAdjoining")]
        //public bool SearchIsAdjoining { get; set; }

        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsNotAdjacent")]
        //public bool SearchIsNotAdjacent { get; set; }

        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsHeavyTransport")]
        //public bool SearchIsHeavyTransport { get; set; }

        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsForeign")]
        //public bool SearchIsForeign { get; set; }

        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsInCity")]
        //public bool SearchIsInCity { get; set; }
        
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsAmanat")]
        //public bool SearchIsAmanat { get; set; }

        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchIsTwoStep")]
        //public bool SearchIsTwoStep { get; set; }

        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchHasHagheMaghar")]
        //public bool SearchHasHagheMaghar { get; set; }




        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchStartTime")]
        public TimeSpan SearchStartTime { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchEndTime")]
        public TimeSpan SearchEndTime { get; set; }


        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchStoreId")]
        public int SearchStoreId { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchOfficeId")]
        public int SearchOfficeId { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.CollectorSearchProviderId")]
        public int SearchProviderId { get; set; }

        public IList<SelectListItem> ListProvider { get; set; }
        public IList<SelectListItem> ListOffice_State { get; set; }
        public IList<SelectListItem> ListOffice_City { get; set; }
        public IList<SelectListItem> ListStores { get; set; }



    }
}
