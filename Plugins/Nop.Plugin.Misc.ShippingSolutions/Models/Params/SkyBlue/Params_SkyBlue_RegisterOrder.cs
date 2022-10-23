using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.SkyBlue
{
    public class Params_SkyBlue_RegisterOrder
    {

        public string SenderName { get; set; }
        public string SenderAddress { get; set; }
        public string SenderPostCode { get; set; }
        public string SenderPhone { get; set; }
        public string SenderEmail { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverPostCode { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceiverEmail { get; set; }
        public string Content { get; set; }
        public string ContentValue { get; set; }
        public int Weight { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Countrycode { get; set; }
        public int ParcelType { get; set; }
        public int ServiceType { get; set; }




        public (bool, string) IsValidParams_SkyBlue_RegisterOrder()
        {
            bool Status = true;
            string Message = "";
            if (string.IsNullOrEmpty((ReceiverPhone ?? "").Trim()))
            {
                Status = false;
                Message = "( ReceiverPhone in Params_SkyBlue_RegisterOrder) is null";

            }
            if (string.IsNullOrEmpty((ReceiverAddress ?? "").Trim()))
            {
                Status = false;
                Message = "(ReceiverAddress  in Params_SkyBlue_RegisterOrder) is null";

            }
            if (string.IsNullOrEmpty((ReceiverName ?? "").Trim()))
            {
                Status = false;
                Message = "(ReceiverName  in Params_SkyBlue_RegisterOrder) is null";

            }
            if (string.IsNullOrEmpty((SenderPhone ?? "").Trim()))
            {
                Status = false;
                Message = "(SenderPhone  in Params_SkyBlue_RegisterOrder) is null";

            }
            if (string.IsNullOrEmpty((SenderAddress ?? "").Trim()))
            {
                Status = false;
                Message = "(SenderAddress  in Params_SkyBlue_RegisterOrder) is null";

            }
            if (string.IsNullOrEmpty((SenderName ?? "").Trim()))
            {
                Status = false;
                Message = "( SenderName in Params_SkyBlue_RegisterOrder) is null";

            }

            if (Weight <= 0)
            {
                Status = false;
                Message = "(Weight in Params_SkyBlue_RegisterOrder) is null";

            }
            if (string.IsNullOrEmpty(Countrycode))
            {
                Status = false;
                Message = "(Countrycode in Params_SkyBlue_RegisterOrder) is null";

            }
            if (ParcelType <= 0)
            {
                Status = false;
                Message = "(ParcelType in Params_SkyBlue_RegisterOrder) is null";

            }


            return (Status, Message);
        }


    }
}
