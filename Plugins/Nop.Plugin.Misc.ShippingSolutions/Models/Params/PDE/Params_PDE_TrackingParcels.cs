using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.PDE
{
    public class Params_PDE_TrackingParcels
    {
        public Int64 IdOrder { get; set; }
        public (bool, string) IsValid_PDE_TrackingParcels()
        {
            bool result = true;
            string Message = "";
            if (IdOrder <= 0)
            {
                result = false;
                Message = "Field IdOrder must be greater than zero";
            }
            return (result, Message);
        }
    }
}
