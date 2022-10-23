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
    public class Tbl_TrainingAcademyTopic : BaseEntity
    {
        public Tbl_TrainingAcademyTopic()
        {
            ListTopic = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.IdTopic")]
        public int IdTopic { get; set; }

        [ScaffoldColumn(false)]
        public String UrlImage { get; set; }

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
        public IList<SelectListItem> ListTopic { get; set; }

        [NotMapped]
        public string NameTopic4Edit { get; set; }

        
    }
}
