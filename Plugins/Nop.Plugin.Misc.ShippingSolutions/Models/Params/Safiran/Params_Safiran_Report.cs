using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran
{
    /// <summary>
    /// مدل ورودی  متد دریافت گزارش عملکرد
    /// <para>user مدل یوزر</para>
    /// <para>date مدل تاریخ</para>
    /// <para>maximum_records حداکثر تعداد برگشتی</para>
    /// 
    /// </summary>
    public class Params_Safiran_Report
    {
        public UserSafiranReport user { get; set; }
        public DateSafiranReport date { get; set; }
        public int maximum_records { get; set; }

        public (bool,string) IsValidParams_Safiran_Report()
        {
            bool Status = true;
            string Message = "";
            if (string.IsNullOrEmpty((user.username ?? "").Trim()))
            {
                Status = false;
                Message = "Setting(username) is null";
                
            }
            if (string.IsNullOrEmpty((user.password ?? "").Trim()))
            {
               Status = false;
                Message = "Setting(password) is null";
                
            }
            if (string.IsNullOrEmpty((date.from ?? "").Trim()))
            {
                Status = false;
                Message = "date from is null";
                
            }
            if (string.IsNullOrEmpty((date.to ?? "").Trim()))
            {
               Status = false;
               Message = "date to is null";
                
            }

            return (Status, Message);
        }
    }
    /// <summary>
    /// مدل یوزر متد دریافت گزارش عملکرد
    /// <para>مقدار 2 فیلد این مدل از طریق تنظیمات پر میشوند</para>
    /// </summary>
    public class UserSafiranReport
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    /// <summary>
    /// مدل تاریخ  از  تا تاریخ میلادی باشد.
    /// </summary>
    public class DateSafiranReport
    {
        public string from { get; set; }
        public string to { get; set; }
    }
}
