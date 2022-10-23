using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Domain
{
   public class Tbl_CountryISO3166 : BaseEntity
    {
        public string Name_E { get; set; }
        public string Alpha2Code { get; set; }
        public int NumericCode { get; set; }
        public string Name_F { get; set; }
        public bool IsActive { get; set; }
        public bool PDEId { get; set; }


    }
}
