using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
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
    public class Tbl_Offices : BaseEntity
    {
        public Tbl_Offices()
        {
            WorkingTimes = new List<Tbl_WorkingTime>();
            //ListCustomer = new List<SelectListItem>();
            //ListAddress = new List<SelectListItem>();
        }
        //public int Id { get; set; }
        [ScaffoldColumn(false)]
        public int ProviderId { get; set; }
        // 0 Provider   1 Collector
        [ScaffoldColumn(false)]
        public bool TypeOffice { get; set; }
        [ScaffoldColumn(false)]
        public int CollectorId { get; set; }
        ////city id
        [ScaffoldColumn(false)]
        public int StateProvinceId { get; set; }
        ////user id
        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.OfficeCustomerID")]
        //public int CustomerID { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.OfficeWarehouseState")]
        public bool WarehouseState { get; set; }

        // Warehouse Address
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.OfficeWarehouseState")]
        public string WarehouseAddress { get; set; }

        
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.OfficeAddressId")]
        public int? AddressId { get; set; }

        [NotMapped]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.OfficeLat")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Please enter valid Double Number")]
        public double Lat { get; set; }

        [NotMapped]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.OfficeLong")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Please enter valid Double Number")]
        public double Long { get; set; }

        [Required]
        [NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.OfficeHolidaysState")]
        public bool HolidaysState { get; set; }

        //شهر و کد مپ شده
        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.OfficeIdState")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        //public int IdState { get; set; }
        //[Required]
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.OfficeIdCity")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        //public int IdCity { get; set; }
        
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.OfficeNameState")]
        //[DataType(DataType.Text)]
        //public string NameState { get; set; }
        
        //[NopResourceDisplayName("Nop.Plugin.Misc.ShippingSolutions.OfficeNameCity")]
        //[DataType(DataType.Text)]
        //public string NameCity { get; set; }

        [ScaffoldColumn(false)]
        public bool IsActive { get; set; }
        [ScaffoldColumn(false)]
        public DateTime DateInsert { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? DateUpdate { get; set; }
        [ScaffoldColumn(false)]
        public int IdUserInsert { get; set; }
        [ScaffoldColumn(false)]
        public int? IdUserUpdate { get; set; }

        //public Tbl_ServicesProviders Provider { get; set; }
        //public Tbl_Collectors Collector { get; set; }

        public List<Tbl_WorkingTime> WorkingTimes { get; set; }


        //[NotMapped]
        //public IList<SelectListItem> ListCustomer { get; set; }
        //[NotMapped]
        //public IList<SelectListItem> ListAddress { get; set; }
        [NotMapped]
        public string fNameAddress { get; set; }

        [NotMapped]
        public string lNameAddress { get; set; }


        [NotMapped]
        public string EmailAddress { get; set; }


        [NotMapped]
        public string MobileAddress { get; set; }


        [NotMapped]
        public string DetailAddress { get; set; }




        [NotMapped]
        public string NameOffice { get; set; }

        [NotMapped]
        public TimeSpan Monday_StartTime { get; set; }
        [NotMapped]
        public TimeSpan Monday_EndTime { get; set; }
        [NotMapped]
        public TimeSpan Tuesday_StartTime { get; set; }
        [NotMapped]
        public TimeSpan Tuesday_EndTime { get; set; }
        [NotMapped]
        public TimeSpan Wednesday_StartTime { get; set; }
        [NotMapped]
        public TimeSpan Wednesday_EndTime { get; set; }
        [NotMapped]
        public TimeSpan Thursday_StartTime { get; set; }
        [NotMapped]
        public TimeSpan Thursday_EndTime { get; set; }
        [NotMapped]
        public TimeSpan Friday_StartTime { get; set; }
        [NotMapped]
        public TimeSpan Friday_EndTime { get; set; }
        [NotMapped]
        public TimeSpan Saturday_StartTime { get; set; }
        [NotMapped]
        public TimeSpan Saturday_EndTime { get; set; }
        [NotMapped]
        public TimeSpan Sunday_StartTime { get; set; }
        [NotMapped]
        public TimeSpan Sunday_EndTime { get; set; }
    }

}
