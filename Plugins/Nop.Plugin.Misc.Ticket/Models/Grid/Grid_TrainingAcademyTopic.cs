using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Models.Grid
{
    public class Grid_TrainingAcademyTopic
    {
        public int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_TrainingAcademyTopic_UrlImage")]
        public string Grid_TrainingAcademyTopic_UrlImage { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_TrainingAcademyTopic_Title")]
        public string Grid_TrainingAcademyTopic_Title { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_TrainingAcademyTopic_Description")]
        public string Grid_TrainingAcademyTopic_Description { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_TrainingAcademyTopic_SystemName")]
        public string Grid_TrainingAcademyTopic_SystemName { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_TrainingAcademyTopic_StoresName")]
        public string Grid_TrainingAcademyTopic_StoresName { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_TrainingAcademyTopic_IsActive")]
        public bool Grid_TrainingAcademyTopic_IsActive { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_TrainingAcademyTopic_DateInsert")]
        public DateTime Grid_TrainingAcademyTopic_DateInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_TrainingAcademyTopic_DateUpdate")]
        public DateTime? Grid_TrainingAcademyTopic_DateUpdate { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_TrainingAcademyTopic_UserInsert")]
        public string Grid_TrainingAcademyTopic_UserInsert { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Grid_TrainingAcademyTopic_UserUpdate")]
        public string Grid_TrainingAcademyTopic_UserUpdate { get; set; }

        public int IdTopic { get; set; }
        public List<int> stores { get; set; }
        public string UrlPage { get; set; }
    }
}
