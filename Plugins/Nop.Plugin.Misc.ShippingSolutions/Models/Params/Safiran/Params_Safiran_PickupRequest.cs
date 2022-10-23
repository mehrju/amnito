using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran
{
    /// <summary>
    /// مدل ورودی ثبت بارنامه PickUP
    /// به غیر از نام شرکت ، ایمیل و مکان جغرافیاییو مقادیر ثابت الباقی موارد اجباری هستند
    /// <para> در این مدل یک سری فیلد ها اعم از </para>
    /// <para>pickupman,code</para>
    /// <para> ثابت هستند از تنظمیات گرفته میشود و نیازی نیست وارد نمایید</para>
    /// <para>pickup_man کد نمایندگی شرکت پست بار که فعلا برابر8000 میباشد</para>
    /// <para>sender مدل ارسال کننده</para>
    /// <para>receiver مدل دریافت کننده</para>
    /// <para>cn مدل محموله</para>
    /// <para>awb مدل لیست بسته ها یا کالاها</para>
    /// <para>cod مشخص کننده امانی بودن کالا میباشد، در صورت امانی بودن یعنی اینکه هنگام تحویل مبلغ کالا از گیرنده تحویل و به حساب فرستنده واریز شود در این حالت هزینه پست گران تر میشود</para>
    /// </summary>
    public class Params_Safiran_PickupRequest
    {
        public string pickup_man { get; set; }
        public Sender sender { get; set; }
        public Receiver receiver { get; set; }
        public Cn cn { get; set; }
        public bool cod { get; set; }
        public Awb awb { get; set; }

        public (bool, string) IsValidParamPickupRequest()
        {
            bool State = true;
            string Message = "";
            if (string.IsNullOrEmpty((pickup_man ?? "").Trim()))
            {
                State = false;
                Message = " Pick Up Man IS Null";
            }
            if (cn.service != "1" && cn.service != "6" && cn.service != "11")
            {
                State = false;
                Message = "The cn.service field  Must be Between 1:Ground 6:Air 11:Post , Postman Must Use 1 ";
            }
            if (cn.payment_term != "0" && cn.payment_term != "1")
            {
                State = false;
                Message = "The cn.payment_term field  Must be Between 0:advance freight 1:freight forward ";
            }
            //if (awb.package.Count == 0)
            //{
            //    State = false;
            //    Message = "The awb.package  list must have more than one member";
            //}
            if (awb.package.Any(x => x.Length != 11))
            {
                State = false;
                Message = "The  awb.package Item Must have 11 digits ";
            }
            float w = 0;
            bool IsNumber = float.TryParse(cn.weight, out w);
            if (IsNumber == false)
            {
                State = false;
                Message = "The  cn.weight Must be a number ";
            }
            if (IsNumber == true)
            {
                if (w > 100 || w < 0)
                {
                    State = false;
                    Message = "The cn.weight  Must be Between 0 or 20 ";
                }
            }
            if (string.IsNullOrEmpty((sender.address ?? "").Trim())
                || string.IsNullOrEmpty((sender.city_no ?? "").Trim())
                || string.IsNullOrEmpty((sender.code ?? "").Trim())
                || string.IsNullOrEmpty((sender.mobile ?? "").Trim())
                || string.IsNullOrEmpty((sender.person ?? "").Trim())
                || string.IsNullOrEmpty((sender.postcode ?? "").Trim())
                //|| string.IsNullOrEmpty((sender.telephone ?? "").Trim())
                || string.IsNullOrEmpty((receiver.address ?? "").Trim())
                || string.IsNullOrEmpty((receiver.city_no ?? "").Trim())
                || string.IsNullOrEmpty((receiver.mobile ?? "").Trim())
                || string.IsNullOrEmpty((receiver.person ?? "").Trim())
                || string.IsNullOrEmpty((receiver.postcode ?? "").Trim())
                //|| string.IsNullOrEmpty((receiver.telephone ?? "").Trim())
                || string.IsNullOrEmpty((cn.weight ?? "").Trim())
                || string.IsNullOrEmpty((cn.content ?? "").Trim())
                || string.IsNullOrEmpty((cn.service ?? "").Trim())
                || string.IsNullOrEmpty((cn.assiged_pieces ?? "").Trim())
                || string.IsNullOrEmpty((cn.payment_term ?? "").Trim())
                || string.IsNullOrEmpty((cn.value ?? "").Trim())


                )//end if 
            {
                State = false;
                Message = "All mandatory requirements must be completed in accordance with the description ";
            }
            return (State, Message);
        }
    }
    /// <summary>
    /// مدل ورودی مکان جغرافیایی 
    /// <para>lat</para>
    /// <para>lng</para>
    /// 
    /// </summary>
    public class GEO
    {
        public string lat { get; set; }
        public string lng { get; set; }
    }
    /// <summary>
    /// مدل ورودی ارسال کننده
    /// <para>code کد ارسال کننده شرکت پست بار 10825</para>
    /// <para>city_no کد شهر ارسال کننده</para>
    /// <para>person شخص ارسال کننده</para>
    /// <para>company نام شرکت ارسال کننده</para>
    /// <para>address ادرس ارسال کننده</para>
    /// <para>postcode کد پستی</para>
    /// <para>telephone تلفن</para>
    /// <para>mobile موبایل</para>
    /// <para>email ایمیل ارسال کننده</para>
    /// <para>GEO مکان جغرافیایی ارسال کننده</para>
    /// 
    /// </summary>
    public class Sender
    {
        public string code { get; set; }
        public string city_no { get; set; }
        public string person { get; set; }
        public string company { get; set; }
        public string address { get; set; }
        public string postcode { get; set; }
        public string telephone { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public GEO GEO { get; set; }
    }
    /// <summary>
    /// مدل ورودی دریافت کننده
    /// <para>code: میتواند خالی باشد</para>
    /// <para>city_no کد شهر دریافت کننده</para>
    /// <para>person شخص دریافت کننده</para>
    /// <para>company نالم شرکت دریافت کننده</para>
    /// <para>postcode کد پستی درسافت کننده</para>
    /// <para>telephone تلفن دریافت کننده</para>
    /// <para>mobile موبایل دریافت کننده</para>
    /// <para>email ایمیل دریافت کننده</para>
    /// 
    /// </summary>
    public class Receiver
    {
        public string code { get; set; }
        public string city_no { get; set; }
        public string person { get; set; }
        public string company { get; set; }
        public string address { get; set; }
        public string postcode { get; set; }
        public string telephone { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
    }
    /// <summary>
    /// مدل ورودی محموله - بارنامه
    /// <para>weight وزن بارنامه حداکثر 20 کیلوگرم</para>
    /// <para>اگر وزن محموله بیش از 20 کیلو شود باید باتوجه به وزن در چندین بارنامه ثبت شود</para>
    /// <para>width عرض</para>
    /// <para>height ارتفاع</para>
    /// <para>length طول</para>
    /// <para>content نام کالا داخل محموله</para>
    /// <para>service نوع ارسال محموله</para>
    /// <para>عدد یک زمینی،عدد شش هوایی و عدد یازده پستی که گفته شد شرکت پست بار عدد یک را انتخاب کند</para>
    /// <para>assiged_pieces تعداد قطعه</para>
    /// <para>payment_term روش پرداخت کرایه : عدد صفر پیش کرایه و عدد یک پسکرایه</para>
    /// <para>value ارزش بارنامه به ریال</para>
    /// 
    /// </summary>
    public class Cn
    {
        public string weight { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string length { get; set; }
        public string content { get; set; }
        public string service { get; set; }
        public string assiged_pieces { get; set; }
        public string payment_term { get; set; }
        public string value { get; set; }
    }
    /// <summary>
    /// مدل ورودی لیست کد کالاها
    /// <para>طبق صحبت انجام شده قرارشد یک بازه 11 رقمی از اعداد به شرکت پست بار تخصیص داده شود تا </para>
    /// <para>برروی هریک کالاها ارسالی یک عدد تولید و درهنگام ثبت بارنامه کد کالاهای داخل محموله ارسال شود</para>
    /// <para>package لیست از کد کالاها:نباید خالی باشد</para>
    /// <para></para>
    /// 
    /// </summary>
    public class Awb
    {
        public List<string> package { get; set; }
    }

}
