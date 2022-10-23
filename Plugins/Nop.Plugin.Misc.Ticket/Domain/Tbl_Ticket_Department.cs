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
    public class Tbl_Ticket_Department : BaseEntity
    {
        public Tbl_Ticket_Department()
        {
            ListStores = new List<SelectListItem>();
            CategoryTicket = new List<Tbl_CategoryTicket>();
        }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.NewDepName")]
        [DataType(DataType.Text)]
        public String Name { get; set; }

        [ScaffoldColumn(false)]
        public int StoreId { get; set; }

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
        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.NewListStore")]
        public IList<SelectListItem> ListStores { get; set; }

        public List<Tbl_Ticket_Staff_Department> Staff_Departments { get; set; }
        public List<Tbl_CategoryTicket> CategoryTicket { get; set; }

    }
}
