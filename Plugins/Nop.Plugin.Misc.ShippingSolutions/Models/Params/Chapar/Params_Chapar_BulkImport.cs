using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Chapar
{
    /// <summary>
    /// مدل ورودی متد ثبت گروهی بارنامه-مخصوص مشتری
    /// <para> شما در این مدل میتوانید به ازای یک فرستنده چندین محموله برای گیرنده ها ارسال بفرمایید</para>
    /// <para> تفاوت با متد معمولی ثبت بارنامه این است که کد کالا توسط خود سیستم ثبت میشود</para>
    /// <para>User مدل یوزر که از تنظمات تکمیل میوشد و شما نیازی به تکمیل ان ندارید</para>
    /// <para>Bulk لیستی از محموله ها</para>
    /// <para></para>
    /// 
    /// </summary>
    public class Params_Chapar_BulkImport
    {
        public User user { get; set; }
        public List<Bulk> bulk { get; set; }


        public (bool, string) IsValidParamBulkImport()
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
                if( string.IsNullOrEmpty((item.cn.assinged_pieces ?? "").Trim())
                    || string.IsNullOrEmpty((item.cn.value ?? "").Trim())
                    || string.IsNullOrEmpty((item.cn.weight ?? "").Trim())
                    || string.IsNullOrEmpty((item.cn.service ?? "").Trim())
                    


                    )
                {
                    Status = false;
                    Message = "All mandatory requirements in CnBulkImport must be completed in accordance with the description";
                }

                if (string.IsNullOrEmpty((item.receiver.address ?? "").Trim())
                    || string.IsNullOrEmpty((item.receiver.city_no ?? "").Trim())
                    || string.IsNullOrEmpty((item.receiver.mobile ?? "").Trim())
                    || string.IsNullOrEmpty((item.receiver.person ?? "").Trim())
                    )
                {
                    Status = false;
                    Message = "اطلاعات گیرنده به صورت کامل وارد نشده";
                }
                if (string.IsNullOrEmpty((item.sender.address ?? "").Trim())
                    || string.IsNullOrEmpty((item.sender.city_no ?? "").Trim())
                    || string.IsNullOrEmpty((item.sender.mobile ?? "").Trim())
                    || string.IsNullOrEmpty((item.sender.person ?? "").Trim())
                    )
                {
                    Status = false;
                    Message = "اطلاعات فرستنده به صورت کامل وارد نشده";
                }
            }
            
            return (Status, Message);
        }
    }

    /// <summary>
    /// مدل ورودی محموله - بارنامه
    ///<para>همه موراد اجباری</para>
    /// <para>weight وزن بارنامه حداکثر 20 کیلوگرم</para>
    /// <para>اگر وزن محموله بیش از 20 کیلو شود باید باتوجه به وزن در چندین بارنامه ثبت شود</para>
    ///<para>reference شماره فاکتور- کد محلی محموله</para>
    ///<para>کدمحلی ارسالی تکراری نباشد</para>
    /// <para>assiged_pieces تعداد قطعه</para>
    /// <para>payment_term روش پرداخت کرایه : عدد صفر پیش کرایه و عدد یک پسکرایه</para>
    /// <para>value ارزش بارنامه به ریال</para>
    /// <para> date : "2019-5-13"</para>
    /// <para>فیلد دیتا را به صورت بالا تکمیل کنید</para>
    /// <para>service = COD,11=post,1 = zamini,6 havaii,35 chaparPlus</para>
    /// <para>payment_term براساس داکیومنت</para>
    /// <para>content توضیحات</para>
    /// </summary>
    public class CnBulkImport
    {
        public int reference { get; set; }
        public string date { get; set; }
        public string assinged_pieces { get; set; }
        public string service { get; set; }
        public string value { get; set; }
        public int payment_term { get; set; }
        public int payment_terms { get; set; }
        public string weight { get; set; }
        public string content { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int length { get; set; }
        public int inv_value { get; set; }
    }
    /// <summary>
    /// مدل ورودی دریافت کننده
    /// <para>به غیر از نام شرکت ،تلفن  و کد پستی مابقی اجباری هستند</para>
    /// <para>city_no کد شهر دریافت کننده</para>
    /// <para>person شخص دریافت کننده</para>
    /// <para>company نالم شرکت دریافت کننده</para>
    /// <para>postcode کد پستی درسافت کننده</para>
    /// <para>telephone تلفن دریافت کننده</para>
    /// <para>mobile موبایل دریافت کننده</para>
    /// <para>email ایمیل دریافت کننده</para>
    /// 
    /// </summary>
    public class ReceiverBulkImport
    {
       
        public string person { get; set; }
        public string company { get; set; }
        public string city_no { get; set; }
        public string telephone { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string postcode { get; set; }
       
    }

    public class SenderBulkImport
    {
        public string person { get; set; }
        public string company { get; set; }
        public string city_no { get; set; }
        public string telephone { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string postcode { get; set; }
    }
    /// <summary>
    /// مدل یک محموله
    /// <para>CnBulkImport مدل محموله</para>
    /// <para>ReceiverBulkImport گیرنده</para>
    /// 
    /// </summary>
    public class Bulk
    {
        public CnBulkImport cn { get; set; }
        public SenderBulkImport sender { get; set; }
        public ReceiverBulkImport receiver { get; set; }
    }
}
