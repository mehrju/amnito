using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.PDE
{
    public class Result_PDE_TrackingParcels
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public DetailOrder Detail { get; set; }

    }
    public class DetailOrder
    {

        public string Location { get; set; }
        public string EnStatus { get; set; }
        public string FaStatus { get; set; }
        public string FaLocation { get; set; }
        public DateTime LocTime { get; set; }
        public object ShamsiLocDate { get; set; }
        public object ShamsiRevisedDate { get; set; }
        public object Signer { get; set; }
        public object RevisedDate { get; set; }
        public bool IsFinal { get; set; }
        public object Courier { get; set; }
        public int TrackId { get; set; }
    }
}
