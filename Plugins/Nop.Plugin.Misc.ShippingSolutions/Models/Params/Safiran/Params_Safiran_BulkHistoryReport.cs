using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran
{
    /// <summary>
    /// مدل ورودی دریافت تاریخچخه یا وضهیت بارنامهها به صورت گروهی
    /// <para>User مدل ورودی یوزر</para>
    /// <para>مدل یوزر از تنظیمات تکمیل میگردد و نیازی به پر کردن ان ندارید</para>
    /// <para>bulk لیستی از کد بارنامه ها</para>
    /// 
    /// </summary>
    public class Params_Safiran_BulkHistoryReport
    {
        public User user { get; set; }
        public List<string> bulk { get; set; }

        public (bool,string) IsValidParams_Safiran_BulkHistoryReport()
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
            if (bulk.Count == 0)
            {
                Status = false;
                Message = "The bulk list  must have at least one member";

            }
            foreach (var item in bulk)
            {
                if(string.IsNullOrEmpty((item ?? "").Trim()))
                {
                    Status = false;
                    Message = "At least one item in bulk list is empty ";

                }
            }
            return (Status, Message);
        }
    }
    public class User
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
