using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Snappbox
{
    public class Result_Snappbox_Get_Account_Balance
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public DetailResult_Snappbox_Get_Account_Balance DetailResult_Snappbox_Get_Account_Balance { get; set; }
    }
    public class DetailResult_Snappbox_Get_Account_Balance
    {
        public double currentBalance { get; set; }
    }
}
