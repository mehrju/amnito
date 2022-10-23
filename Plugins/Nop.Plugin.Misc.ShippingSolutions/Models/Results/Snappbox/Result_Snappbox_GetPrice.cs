using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Snappbox
{
    public class Result_Snappbox_GetPrice
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public Detail_Result_Snappbox_GetPrice Detail_Result_Snappbox_GetPrice { get; set; }

    }
    public class Detail_Result_Snappbox_GetPrice
    {
        public double timeFactor { get; set; }
        public int totalFare { get; set; }
        public string pricingId { get; set; }
        public int subsidy { get; set; }
        public int finalCustomerFare { get; set; }
        public int totalSurgeAmount { get; set; }
    }
}
