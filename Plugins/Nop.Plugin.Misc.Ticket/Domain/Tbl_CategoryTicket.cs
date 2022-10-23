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
    public class Tbl_CategoryTicket : BaseEntity
    {
        public Tbl_CategoryTicket()
        {
            ListDepartments = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.NameCategoryTicket")]
        [DataType(DataType.Text)]
        public String NameCategoryTicket { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.DepartmentIdeCategoryTicket")]
        public int DepartmentId { get; set; }
        public Tbl_Ticket_Department Department { get; set; }



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
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.NewCategoryTicketListDepartments")]
        public IList<SelectListItem> ListDepartments { get; set; }
    }
}
