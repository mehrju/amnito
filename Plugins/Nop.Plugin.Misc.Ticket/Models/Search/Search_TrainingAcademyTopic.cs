using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Models.Search
{
    public class Search_TrainingAcademyTopic
    {
        public Search_TrainingAcademyTopic()
        {
            ListStore = new List<SelectListItem>();
            ListTopic = new List<SelectListItem>();
        }

        public bool Search_TrainingAcademyTopic_ActiveSearch { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_TrainingAcademyTopic_Title")]
        public string Search_TrainingAcademyTopic_Title { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_TrainingAcademyTopic_Description")]
        public string Search_TrainingAcademyTopic_Description { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_TrainingAcademyTopic_SystemName")]
        public string Search_TrainingAcademyTopic_SystemName { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_TrainingAcademyTopic_IsActive")]
        public bool Search_TrainingAcademyTopic_IsActive { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_TrainingAcademyTopic_StoreId")]
        public int Search_TrainingAcademyTopic_StoreId { get; set; }
        public IList<SelectListItem> ListStore { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Misc.Ticket.Search_TrainingAcademyTopic_TopicId")]
        public int Search_TrainingAcademyTopic_TopicId { get; set; }
        public IList<SelectListItem> ListTopic { get; set; }
    }
}
