using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models
{
    public class vm_AddPaternPricingPolicy
    {

        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int MinCount { get; set; }
        public int MaxCount { get; set; }
        public int MinWeight { get; set; }
        public int MaxWeight { get; set; }
        public float Percent { get; set; }
        public double Mablagh { get; set; }
        public double Tashim { get; set; }
        public int IdParent { get; set; }
        public float PercentTashim { get; set;}
    }
}
