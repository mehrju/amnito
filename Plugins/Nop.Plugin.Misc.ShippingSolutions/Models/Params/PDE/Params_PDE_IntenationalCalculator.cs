using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params
{
    /// <summary>
    /// مدل ورودی استعلام قیمت بین الملل
    /// <para>همه موارد اجباری هستند</para>
    /// <para>OriginCountryآیدی کشور مبدا</para>
    /// <para>DestCountry آیدی کشور مقصد</para>
    /// <para>Chwوزن مرسوله</para>
    /// <para>Widthعرض مرسوله</para>
    /// <para>Heightارتفاع یا قد مرسوله</para>
    /// <para>Length طول مرسوله</para>
    /// <para>ConsType // 0 Or 1 // 0 for Document // 1 for Non-Document</para>
    /// <para> دو مو.رد زیر در تنظیمات وارد شود</para>
    /// <para>Passwordکلمه عبور مختص شرکت postbar</para>
    /// <para>Ccode کد مختص شرکت postbar</para>
    /// <para></para>
    /// 
    /// </summary>
    public class Params_PDE_IntenationalCalculator
    {
        public int OriginCountry { get; set; }
        public int DestCountry { get; set; }
        public decimal Chw { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Length { get; set; }
        // 0 Or 1
        // 0 for Document
        // 1 for Non-Document
        public byte ConsType { get; set; }
        public int Ccode { get; set; }
        public string Password { get; set; }



        public (bool, string) IsValid_PDE_IntenationalCalculator()
        {
            bool result = true;
            string Message = "";
            if (OriginCountry <= 0)
            {
                result = false;
                Message = "Field OriginCountry must be greater than zero";
            }
            if (DestCountry <= 0)
            {
                result = false;
                Message = "Field DestCountry must be greater than zero";
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
            if (Length <= 0)
            {
                result = false;
                Message = "Field Length must be greater than zero";
            }

            if (ConsType != 0 && ConsType != 1)
            {
                result = false;
                Message = "Field ConsType must be 0 or 1";
            }

            if (Ccode <= 0)
            {
                result = false;
                Message = "Setting Is Null";
            }

            if (string.IsNullOrEmpty((Password ?? "").Trim()))
            {
                result = false;
                Message = "Setting Is Null";
            }
            return (result, Message);
        }
    }
}
