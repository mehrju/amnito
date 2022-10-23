
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
    public class PostalAddress
    {
        public PostalAddress()
        {
        }

        public int CountryId { get; set; }

        public int? StateProvinceId { get; set; }

        public string Address1 { get; set; }

        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public float? Lat{ get; set; }
        public float? Lon{ get; set; }
      
    }
}