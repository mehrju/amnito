using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.YarBox
{
    /// <summary> 
    /// مدل ورودی ای  پی ای ثبت بسته
    /// طبق داکیومنت تمام فیلد ها باید پر شوند
    /// در این ای پی ای نام شهر مبدا و مقصد به صورت متنی ارسال میشود
    /// <para>count تعداد بسته:باید بزرگتر از صفر باشد</para>
    /// <para>origin شهر مبدا به صورت متنی</para>
    /// <para>destination شهر مقصد به صورت متنی</para>
    /// <para>apPackingTypeId نوع بسته بندی مثلا کارتن با سایز یک: این مورد باید از ای پی ای دیگری گرفته شود</para>
    /// <para>apTypeId نوع بسته مثلا از صفر تا یک کیلو : این مورد باید از ای پی ای دیگری گرفته شود</para>
    /// <para>insurance بیمه</para>
    /// <para>receiveType حتما برابر صفر باشد</para>
    /// <para>senderPhone موبایل فرستنده</para>
    /// <para>receiverPhone موبایل گیرنده</para>
    /// <para>latitudeعرض جغرافیایی مبدا</para>
    /// <para>longitude طول جغرافیایی مبدا</para>
    /// <para>destinationAddress ادرس مقصد</para>
    /// <para>originAddress ادرس مبدا</para>
    /// <para></para>
    /// 
    /// </summary>
    public class Params_YarBox_AddPostPacks
    {
        public int    count              { get; set; }
        public string origin_City       { get; set; }
        public string origin_State      { get; set; }
        public string destination_City  { get; set; }
        public string destination_State  { get; set; }
        public int    apPackingTypeId    { get; set; }
        public int    apTypeId           { get; set; }
        public int    insurance          { get; set; }
        public int    receiveType        { get; set; }
        public string senderPhone        { get; set; }
        public string receiverPhone      { get; set; }
        public string latitude           { get; set; }
        public string longitude          { get; set; }
        public string destinationAddress { get; set; }
        public string originAddress      { get; set; }



        /// <summary>
        /// تابع چک کردن مدل ورودی ای پی ای ثبت بسته
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public (bool, string) IsValidParamsAddPostPacks()
        {
            bool result = true;
            string Message = "";
            if (count <= 0)
            {
                result = false;
                Message = "Field count must be greater than zero";
            }
            if (receiveType != 0)
            {
                result = false;
                Message = "Field receiveType must be zero";
            }
            if (apPackingTypeId <= 0)
            {
                result = false;
                Message = "Field apPackingTypeId must be greater than zero";
            }
            if (apTypeId <= 0)
            {
                result = false;
                Message = "Field apTypeId must be greater than zero";
            }
            if (insurance < 0)
            {
                result = false;
                Message = "Field insurance must be greater or equal than zero";
            }
            if (string.IsNullOrEmpty((destinationAddress ?? "").Trim())
                || string.IsNullOrEmpty((originAddress ?? "").Trim())
                || string.IsNullOrEmpty((destinationAddress ?? "").Trim())
                || string.IsNullOrEmpty((longitude ?? "").Trim())
                || string.IsNullOrEmpty((latitude ?? "").Trim())
                || string.IsNullOrEmpty((receiverPhone ?? "").Trim())
                || string.IsNullOrEmpty((senderPhone ?? "").Trim())
                || string.IsNullOrEmpty((destination_City ?? "").Trim())
                //|| string.IsNullOrEmpty((destination_State ?? "").Trim())
                || string.IsNullOrEmpty((origin_City ?? "").Trim())
                //|| string.IsNullOrEmpty((origin_State ?? "").Trim())
                )
            {
                result = false;
                Message = "All Params fields must be completed";
            }


            return (result, Message);
        }
    }
    public class _Params_YarBox_AddPostPacks
    {
        public int count { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public int apPackingTypeId { get; set; }
        public int apTypeId { get; set; }
        public int insurance { get; set; }
        public int receiveType { get; set; }
        public string senderPhone { get; set; }
        public string receiverPhone { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string destinationAddress { get; set; }
        public string originAddress { get; set; }



    }
}
