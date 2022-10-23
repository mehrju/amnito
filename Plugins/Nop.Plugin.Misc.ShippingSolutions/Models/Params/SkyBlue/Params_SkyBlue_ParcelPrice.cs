using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.SkyBlue
{
    public class Params_SkyBlue_ParcelPrice
    {

        public int Weight      { get; set; }
        public int Length      { get; set; }
        public int Width       { get; set; }
        public int Height      {get; set; }
        public string Countrycode { get; set; }
        public int ParcelType { get; set; }




        public (bool, string) IsValidParams_SkyBlue_ParcelPrice()
        {
            bool Status = true;
            string Message = "";
            if (Weight<=0)
            {
                Status = false;
                Message = "(Weight in Params_SkyBlue_ParcelPrice) is null";

            }
            if (string.IsNullOrEmpty(Countrycode))
            {
                Status = false;
                Message = "(Countrycode in Params_SkyBlue_ParcelPrice) is null";

            }
            if (ParcelType<=0)
            {
                Status = false;
                Message = "(ParcelType in Params_SkyBlue_ParcelPrice) is null";

            }
           

            return (Status, Message);
        }
    }
}
