using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models.Extra_Status_Field_Shipment
{
    public class GridHistory
    {
        public string Name { get; set; }
        public string Status { get; set; }

        
        public string Text { get; set; }

        
        public string User { get; set; }

      
        public DateTime Date { get; set; }

    }
}
