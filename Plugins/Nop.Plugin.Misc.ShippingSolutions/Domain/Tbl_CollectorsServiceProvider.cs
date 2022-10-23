using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Domain
{
    public class Tbl_CollectorsServiceProvider : BaseEntity
    {
        public int CollectorId { get; set; }
        public int ProviderId { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateInsert { get; set; }
        public DateTime? DateUpdate { get; set; }
        public int IdUserInsert { get; set; }
        public int? IdUserUpdate { get; set; }

        public Tbl_Collectors Collector { get; set; }
        public Tbl_ServicesProviders Provider { get; set; }

    }
}
