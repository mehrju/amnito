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
    public class TaskHistoryModel : BaseEntity
    {
        public string Name { get; set; }
        public DateTime ExecDate { get; set; }
        public string value { get; set; }
    }
}
