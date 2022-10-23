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

namespace Nop.Plugin.Misc.Ticket.Domain
{
    public class Tbl_Ticket_Staff_Department : BaseEntity
    {
        public Tbl_Ticket_Staff_Department()
        {
            ListUsers =new List<SelectListItem>();
            ListDepartments = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.NewStaffIdDepartment")]
        public int IdDepartment { get; set; }
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.NewStaffUserId")]
        public int UserId { get; set; }


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


        [NotMapped]
        public IList<SelectListItem> ListUsers { get; set; }
        [NotMapped]
        public IList<SelectListItem> ListDepartments { get; set; }

        public Tbl_Ticket_Department Department { get; set; }
    }
}
