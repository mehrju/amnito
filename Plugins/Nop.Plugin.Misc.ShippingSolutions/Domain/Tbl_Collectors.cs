using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Domain
{
    public class Tbl_Collectors : BaseEntity
    {
        public Tbl_Collectors()
        {
            CollectorsServiceProvider = new List<Tbl_CollectorsServiceProvider>();
            CollectoreStores = new List<Tbl_CollectoreStores>();
            ListStores = new List<SelectListItem>();
            //Offices = new List<Tbl_Offices>();
            ListProviders = new List<SelectListItem>();
            ListUsers = new List<SelectListItem>();
            ListStateProvince = new List<SelectListItem>();

        }
        //public int Id { get; set; }
        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorName")]
        [DataType(DataType.Text)]
        public string CollectorName { get; set; }

        [Required]
        //[ScaffoldColumn(false)]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorUserId")]
        public int UserId { get; set; }

        [ScaffoldColumn(false)]
        public bool IsActive { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorMaxPath")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int MaxPath { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorMaxWeight")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int MaxWeight { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorMinWeight")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int MinWeight { get; set; }


        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectoradvancefreight")]
        ////[Range(typeof(bool), "false", "true")]
        //public bool advancefreight { get; set; }

        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorfreightforward")]

        //public bool freightforward { get; set; }

        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorcod")]

        //public bool cod { get; set; }

        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsPishtaz")]
        //public bool IsPishtaz { get; set; }
        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsSefareshi")]
        //public bool IsSefareshi { get; set; }
        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsVIje")]
        //public bool IsVIje { get; set; }
        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsNromal")]
        //public bool IsNromal { get; set; }
        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsDroonOstani")]
        //public bool IsDroonOstani { get; set; }
        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsAdjoining")]
        //public bool IsAdjoining { get; set; }
        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsNotAdjacent")]
        //public bool IsNotAdjacent { get; set; }
        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsHeavyTransport")]
        //public bool IsHeavyTransport { get; set; }
        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsForeign")]
        //public bool IsForeign { get; set; }
        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsInCity")]
        //public bool IsInCity { get; set; }

        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsAmanat")]
        //public bool IsAmanat { get; set; }

        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorIsTwoStep")]
        //public bool IsTwoStep { get; set; }
        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorHasHagheMaghar")]
        //public bool HasHagheMaghar { get; set; }

        [ScaffoldColumn(false)]
        public DateTime DateInsert { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? DateUpdate { get; set; }
        [ScaffoldColumn(false)]
        public int IdUserInsert { get; set; }
        [ScaffoldColumn(false)]
        public int? IdUserUpdate { get; set; }





        public List<Tbl_CollectorsServiceProvider> CollectorsServiceProvider { get; set; }
        public List<Tbl_CollectoreStores> CollectoreStores { get; set; }
        //public List<Tbl_Offices> Offices { get; set; }



        [NotMapped]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorStoreId")]
        public IList<int> StoreId { get; set; }
        [NotMapped]
        public IList<SelectListItem> ListStores { get; set; }

        [NotMapped]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorProviderId")]
        public IList<int> ProviderId { get; set; }
        [NotMapped]
        public IList<SelectListItem> ListProviders { get; set; }

        [NotMapped]
        public IList<SelectListItem> ListUsers { get; set; }


        [NotMapped]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.FiledNameInNewCollectorStateProvinceId")]
        public IList<int> StateProvinceId { get; set; }
        [NotMapped]
        public IList<SelectListItem> ListStateProvince { get; set; }
    }
}
