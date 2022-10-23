using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.PDE
{
    public class Params_PDE_DomesticCalculator
    {
        public int OriginCity { get; set; }
        public int DestCity { get; set; }
        /// <summary>
        /// وزن ترازویی
        /// </summary>
        public decimal Chw { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Length { get; set; }
        /// <summary>
        /// 101
        /// کد مختص شرکت پست بار
        /// </summary>
        public int Ccode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }

        public (bool, string) IsValid_PDE_DomesticCalculator()
        {
            bool result = true;
            string Message = "";
            if (OriginCity <= 0)
            {
                result = false;
                Message = "شهر مبدا به درستی دریافت نشده";
            }
            if (DestCity <= 0)
            {
                result = false;
                Message = "شهر مقصد به درستی دریافت نشده";
            }
            //if (Width <= 0)
            //{
            //    result = false;
            //    Message = "Field Width must be greater than zero";
            //}
            //if (Height <= 0)
            //{
            //    result = false;
            //    Message = "Field Height must be greater than zero";
            //}
            //if (Length <= 0)
            //{
            //    result = false;
            //    Message = "Field Length must be greater than zero";
            //}

            if (Ccode <= 0)
            {
                result = false;
                Message = "PDE اشکال در ارتباط با سامانه";
            }

            if (string.IsNullOrEmpty((Password ?? "").Trim()))
            {
                result = false;
                Message = "PDE اشکال در ارتباط با سامانه";
            }
            return (result, Message);
        }
    }
}
