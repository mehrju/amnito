using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class AssignAgentAmountRuleModel : BaseEntity
    {
        [DisplayName("نماینده")]
        public int CustomerId { get; set; }
        [DisplayName("قانون حق نمایندگی")]
        public int AgentAmountRuleId { get; set; }
        [DisplayName("تاریخ انتصاب")]
        public DateTime? AssignmentDate { get; set; }
        [DisplayName("فعال؟")]
        public bool IsActive { get; set; }
        [DisplayName("کاربر انتصاب دهنده")]
        public int AssignmentCustomerId { get; set; }
        [DisplayName("تاریخ غیر فعال شدن")]
        public DateTime? DeActiveDate { get; set; }
        [DisplayName("کاربر غیر فعال کننده")]
        public int? DeActiveCustomerId { get; set; }
        [NotMapped]
        public string CustmoerName { get; set; }
        [NotMapped]
        public string AgentAmountRuleName { get; set; }
        [NotMapped]
        public List<SelectListItem> AvailableAgentList { get; set; }
        [NotMapped]
        public List<SelectListItem> AvailableAgentAmountRule { get; set; }
        [NotMapped]
        public string AgentAmountRuleTitle { get; set; }
        [NotMapped]
        public string StrIsActive { get; set; }
        [NotMapped]
        public int Price { get; set; }
        [NotMapped]
        public string RuleTitle { get; set; }
        [NotMapped]
        public int MinCount { get; set; }
        [NotMapped]
        public int MaxCount { get; set; }

    }
}
