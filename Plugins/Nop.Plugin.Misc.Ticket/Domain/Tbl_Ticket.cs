using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Domain
{
    public class Tbl_Ticket : BaseEntity
    {
        public Tbl_Ticket()
        {
            ticket_Details = new List<Tbl_Ticket_Detail>();
        }

        [ScaffoldColumn(false)]
        public int IdCustomer { get; set; }

        [ScaffoldColumn(false)]
        public bool TypeTicket { get; set; } // flase public  true order
        
        

        [ScaffoldColumn(false)]
        public int? OrderCode { get; set; }


        [ScaffoldColumn(false)]
        public string  TrackingCode { get; set; }

        [ScaffoldColumn(false)]
        public int StoreId { get; set; }


        [ScaffoldColumn(false)]
        public int ProrityId { get; set; }




        [ScaffoldColumn(false)]
        public int DepartmentId { get; set; }

        [ScaffoldColumn(false)]
        public int IdCategoryTicket { get; set; }
        


        [ScaffoldColumn(false)]
        public string  Issue { get; set; }



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
        public bool ShowCustomer { get; set; }

        public List<Tbl_Ticket_Detail> ticket_Details { get; set; }


    }
}
