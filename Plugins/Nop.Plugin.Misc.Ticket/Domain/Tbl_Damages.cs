using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Domain
{
    public class Tbl_Damages : BaseEntity
    {
        public Tbl_Damages()
        {
            Damages_Details = new List<Tbl_Damages_Detail>();
        }

        [ScaffoldColumn(false)]
        public int IdCustomer { get; set; }

        [ScaffoldColumn(false)]
        public string TrackingCode { get; set; }
        //
        [ScaffoldColumn(false)]
        public string NameGoods { get; set; }
        [ScaffoldColumn(false)]
        public string Berand { get; set; }

        [ScaffoldColumn(false)]
        public bool Stock { get; set; }

        [ScaffoldColumn(false)]
        public double Price { get; set; }

        [ScaffoldColumn(false)]
        public string Shaba { get; set; }


        //
        [ScaffoldColumn(false)]
        public int StoreId { get; set; }

        [ScaffoldColumn(false)]
        public bool IsActive { get; set; }
        [ScaffoldColumn(false)]
        public int Status { get; set; }


        [ScaffoldColumn(false)]
        public DateTime DateInsert { get; set; }
        [ScaffoldColumn(false)]
        public int? StaffIdAccept { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? DateStaffAccept { get; set; }
        [ScaffoldColumn(false)]
        public int? StaffIdLastAnswer { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? DateStaffLastAnswer { get; set; }
        [ScaffoldColumn(false)]
        public int? IdUserUpdate { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? DateUpdate { get; set; }
       
        [ScaffoldColumn(false)]
        public int IdTicket { get; set; }


        public List<Tbl_Damages_Detail> Damages_Details { get; set; }

    }
}
