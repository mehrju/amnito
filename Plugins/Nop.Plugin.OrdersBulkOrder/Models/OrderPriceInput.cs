using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.BulkOrder.Models
{
    public class OrderPriceInput
    {
        public int? ServiceId { get; set; }
        public int Weight { get; set; }
        public string InsuranceName { get; set; }
        public PackingDimension PackingDimension { get; set; }
        public int SenderCityId { get; set; }
        public int ReceiverCityId { get; set; }
        public string ReceiverCountry { get; set; }
        public int GoodsValue { get; set; }
        public bool PrintBill { get; set; }
        public bool PrintLogo { get; set; }
        public bool NeedCartoon { get; set; }
        public bool IsCod { get; set; }
        public bool IsFreePost { get; set; }
        public bool SendSms { get; set; }
        public string CartonSizeName { get; set; }
        public string Address { get; set; }
        //public int? CodPrice { get; set; }
        //public bool IncludeCollectionPrice { get; set; } = true;

        public bool IsValid(out string msg)
        {
            if (SenderCityId == 0 || ReceiverCityId == 0)
            {
                msg = "وارد کردن شناسه شهر مبدا و مقصد الزامی می باشد";
                return false;
            }
            if (this.Weight <= 0)
            {
                msg = "مقدار وزن وارد شده صحیح نمی باشد";
                return false;
            }

            if (string.IsNullOrEmpty(InsuranceName) || string.IsNullOrWhiteSpace(InsuranceName))
            {
                msg = "وارد کردن عنوان بیمه الزامی می باشد";
                return false;
            }

            //if (ServiceId == 0)
            //{
            //    msg = "وارد کردن نوع پست الزامی می باشد";
            //    return false;
            //}
            if (PackingDimension == null || PackingDimension.Height == 0 || PackingDimension.Length == 0 || PackingDimension.Width == 0)
            {
                msg = "لطفا اطلاعات سایز بسته را وارد کنید";
                return false;
            }
            msg = "";
            return true;
        }

    }

    public class PackingDimension
    {
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string CartonName { get; set; }
    }
}
