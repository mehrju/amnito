using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Domain
{
    public class Tbl_WorkingTime : BaseEntity
    {
        //public int Id { get; set; }
        public int OfficeId { get; set; }
        public String DayName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        //public bool IsActive { get; set; }
        //public DateTime DateInsert { get; set; }
        //public DateTime? DateUpdate { get; set; }
        //public int IdUserInsert { get; set; }
        //public int? IdUserUpdate { get; set; }

        public Tbl_Offices Offices { get; set; }

    }
}
