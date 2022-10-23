using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params
{
    /// <summary>
    /// مدل ورودی ثبت سفارش خارجی
    /// <para>APIRegisterOrderViewModel = ?Chw=1</para>
    /// <para>Commodity1-5 نام محصول</para>
    /// <para>HsCode1-5</para>
    /// <para>Quantity1-5 تعداد محصول در بسته</para>
    /// <para>UnitValue1-5 ارزش کالا</para>
    /// <para>Consignment  اگر اوراق 1 اگر بسته0</para>
    /// <para>ConsValue ارزش بسته ارسالی</para>
    /// <para>CustCode کد مختص شرکت پست بار:در تنظیمات وارد شود</para>
    /// <para>OrigCId آیدی کشور مبدا</para>
    /// <para>DestCId آیدی کشور مقصد</para>
    /// <para>Nw وزن ترازویی</para>
    /// <para>Quantity تعداد کل مرسولات</para>
    /// <para>Password کلمه عبور مختص شرکت پست بار در تنظیمات وارد شود</para>
    /// <para>RecieverAddressآدرس گیرنده</para>
    /// <para>Company girandeنام شرکت گیرنده</para>
    /// <para>RecieverEmail ایمیل گیرنده</para>
    /// <para>RecieverPostalCodeنام گیرنده</para>
    /// <para>RecieverLastNameنام خانوادگی گیرنده</para>
    /// <para>RefNo شماره رفرنس شرکت پست بار</para>
    /// <para>Remarks توضیحات</para>
    /// <para>SenderAddress آدرس فرستنده</para>
    /// <para>SenderCompanyName نام شرکت فرستنده</para>
    /// <para>ServiceTypeId  RP  1 SP        2</para>
    /// <para>TransportationFee هزینه حمل این عدد دقیقا باید باربر با عددی باشد که ماشین حساب محاسبه کرده</para>
    /// <para>ClientId = 23553 شماره مشتری مختص شرکت پست بار در تنظیمات وارد شود</para> 
    /// <para> SystemType=2 خارجی</para>
    /// </summary>
    public class Params_PDE_RegisterInternationalOrder
    {
        //public string APIRegisterOrderViewModel { get; set; }
        public string Commodity1 { get; set; }
        public string HsCode1 { get; set; }
        public int Quantity1 { get; set; }
        public decimal UnitValue1 { get; set; }
        public int Consignment { get; set; }
        public decimal ConsValue { get; set; }
        public int CustCode { get; set; }
        public int OrigCId { get; set; }
        public int DestCId { get; set; }
        public decimal Nw { get; set; }
        public decimal Chw { get; set; }
        public int Quantity { get; set; }
        public string Password { get; set; }
        public string RecieverAddress { get; set; }
        public string RecieverCompanyName { get; set; }
        public string RecieverEmail { get; set; }
        public string RecieverLastName { get; set; }
        public string RecieverMobile { get; set; }
        public string RecieverName { get; set; }
        public string RecieverPostalCode { get; set; }
        public string RecieverTel { get; set; }
        public string RefNo { get; set; }
        public string Remarks { get; set; }
        public string SenderAddress { get; set; }
        public string SenderCompanyName { get; set; }
        public string SenderEmail { get; set; }
        public string SenderLastName { get; set; }
        public string SenderMobile { get; set; }
        public string SenderName { get; set; }
        public string SenderPostalCode { get; set; }
        public string SenderTel { get; set; }
        public int ServiceTypeId { get; set; }
        public decimal TransportationFee { get; set; }
        public int ClientId { get; set; }
        public byte SystemType { get; set; }
        public string Origin { get; set; }
        public string Dest { get; set; }
        public string Commodity2 { get; set; }
        public string HsCode2 { get; set; }
        public int Quantity2 { get; set; }
        public decimal UnitValue2 { get; set; }
        public string Commodity3 { get; set; }
        public string HsCode3 { get; set; }
        public int Quantity3 { get; set; }
        public decimal UnitValue3 { get; set; }
        public string Commodity4 { get; set; }
        public string HsCode4 { get; set; }
        public int Quantity4 { get; set; }
        public decimal UnitValue4 { get; set; }
        public string Commodity5 { get; set; }
        public string HsCode5 { get; set; }
        public int Quantity5 { get; set; }
        public decimal UnitValue5 { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }



        public (bool, string) IsValid_PDE_RegisterInternationalOrder()
        {
            bool result = true;
            string Message = "";
            if (Length <= 0)
            {
                result = false;
                Message = "Field Length must be greater than zero";
            }
            if (Width <= 0)
            {
                result = false;
                Message = "Field Width must be greater than zero";
            }
            if (Height <= 0)
            {
                result = false;
                Message = "Field Height must be greater than zero";
            }
            if (OrigCId <= 0)
            {
                result = false;
                Message = "Field OrigCId must be greater than zero";
            }
            if (DestCId <= 0)
            {
                result = false;
                Message = "Field DestCId must be greater than zero";
            }
            if (Quantity <= 0)
            {
                result = false;
                Message = "Field Quantity must be greater than zero";
            }
            if (Quantity1 <= 0)
            {
                result = false;
                Message = "Field Quantity1 must be greater than zero";
            }
            if (string.IsNullOrEmpty(Commodity1 ??"".Trim()))
            {
                result = false;
                Message = "Field Commodity1 is Null";
            }
            //if (string.IsNullOrEmpty(HsCode1 ?? "".Trim()))
            //{
            //    result = false;
            //    Message = "Field HsCode1 is Null";
            //}
            if(SystemType!= 1)
            {
                result = false;
                Message = "SystemType = 1";
            }
            if (Consignment != 0 && Consignment != 1)
            {
                result = false;
                Message = "Consignment is 0 or 1";
            }
            return (result, Message);
        }



    }
}
