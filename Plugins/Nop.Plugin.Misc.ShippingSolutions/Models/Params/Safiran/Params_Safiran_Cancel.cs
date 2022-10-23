using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran
{
    /// <summary>
    /// مدل ورودی کنل کردن بارنامه
    /// <para>همه موارد اجباری هستند</para>
    /// <para>consignment_no       فیلد برگشتی از متد pickup</para>
    /// <para>consignment_no=pickup->objects->cons </para>
    /// <para>reason دلیل کنسل کردن</para>
    /// </summary>
    public class Params_Safiran_Cancel
    {
        public string consignment_no { get; set; }
        public string reason { get; set; }


        public (bool,string) IsValidParams_Safiran_Cancel()
        {
            string Massege="";
            bool state = true;
            if (string.IsNullOrEmpty(consignment_no))
            {
                state = false;
                Massege = "consignment_no is Null";
            }
            if (string.IsNullOrEmpty(consignment_no))
            {
                state = false;
                Massege = "reason is Null";
            }
            return (state, Massege);
        }

    }
}
