using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.BulkOrder.Models
{
    public class _getServiceInfoModel
    {

        public bool Isvalid(out List<string> errorMassaege,out List<string> warningMassaege)
        {
            errorMassaege = new List<string>();
            warningMassaege = new List<string>();
            if (senderStateId == 0)
            {
                errorMassaege.Add("استان فرستنده الزامی می باشد");
            }
            if (senderTownId == 0)
            {
                errorMassaege.Add("شهر فرستنده الزامی می باشد");
            }
            if (receiverStateId == 0 && receiver_ForeginCountry == 0)
            {
                errorMassaege.Add("استان گیرنده الزامی می باشد");
            }
            if (receiverTownId == 0 && receiver_ForeginCountry == 0)
            {
                errorMassaege.Add("شهر گیرنده الزامی می باشد");
            }
            if (weightItem == 0)
            {
                errorMassaege.Add("وزن مرسوله نامعتبر می باشد");
            }
            if (AproximateValue == 0)
            {
                errorMassaege.Add("ارزش ریالی مرسوله نامعتبر می باشد");
            }
            if (weightItem <= 100000 && !boxType.HasValue)
            {
                errorMassaege.Add("نوع مرسوله نامشخص می باشد");
            }
            if (receiver_ForeginCountry > 0)
            {
                if(receiverStateId >0 || receiverTownId > 0)
                {
                    errorMassaege.Add("در پست خارجی امکان انتخاب استان و شهر گیرنده وجود ندارد");
                }
            }
            if (height == 0 || width == 0 || length == 0)
            {
                if (receiver_ForeginCountry > 0)
                {
                    errorMassaege.Add("وارد کردن ابعاد برای پست خارجی الزامی می باشد");
                }
                else
                {
                    warningMassaege.Add("برای پست های هوایی و اکسپرس وارد کردن ابعاد الزامی است");
                }
            }
            if (weightItem > 100000)
            {
                if (string.IsNullOrEmpty(dispatch_date))
                {
                    errorMassaege.Add("وارد کردن تاریخ و زمان بارگیری در حمل و نقل سنگین الزامی می باشد");
                }
                if (string.IsNullOrEmpty(TruckType))
                {
                    errorMassaege.Add("وارد کردن نوع خودرو در حمل و نقل سنگین الزامی می باشد");
                }
                if (string.IsNullOrEmpty(VechileOptions))
                {
                    errorMassaege.Add("وارد کردن ویژگی خودرو در حمل و نقل سنگین الزامی می باشد");
                }
                if (string.IsNullOrEmpty(VechileOptions))
                {
                    errorMassaege.Add("وارد کردن نوع بسته بندی بار در حمل و نقل سنگین الزامی می باشد");
                }
                if (string.IsNullOrEmpty(VechileOptions))
                {
                    errorMassaege.Add("وارد کردن محتویات بار در حمل و نقل سنگین الزامی می باشد");
                }
            }
            if (errorMassaege.Any())
                return false;
            return true;
        }
        /// <summary>
        /// 0 = List,1=Fastest,2=cheapest
        /// </summary>
        public int ListType { get; set; }
        public int serviceId { get; set; }
        public int senderStateId { get; set; }
        public int senderTownId { get; set; }
        public int receiverStateId { get; set; }
        public int receiverTownId { get; set; }
        public int weightItem { get; set; }
        public int AproximateValue { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public int length { get; set; }
        public string dispatch_date { get; set; }
        public string TruckType { get; set; }
        public string VechileOptions { get; set; }
        public string PackingLoad { get; set; }
        public string Content { get; set; }
        public int receiver_ForeginCountry { get; set; }
        /// <summary>
        /// 0 = pakat , 1 = baste
        /// </summary>
        public byte? boxType { get; set; }
        public bool? IsCod { get; set; }
        public int customerId { get; set; }
    }
}
