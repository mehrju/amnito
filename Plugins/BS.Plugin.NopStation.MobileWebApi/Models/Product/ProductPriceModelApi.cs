using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Product
{
    public class ProductPriceModelApi
    {
        private bool? _accessPrintBill = null;
        public uint Weight { get; set; }
        public uint ServiceId { get; set; }
        public string InsuranceName { get; set; }
        public string CartonSizeName { get; set; }
        public bool IsCod { get; set; }
        public uint CodGoodsPrice { get; set; }
        public uint StateProvinceId { get; set; }
        public uint CityId { get; set; }
        public bool AccessPrintBill
        {
            get => (_accessPrintBill ?? true);
            set => _accessPrintBill = value;
        }
        public bool IsValid(out string msg)
        {
            if (IsCod && (StateProvinceId == 0 || CityId == 0))
            {
                msg = "وارد کردن شناسه استان و شهر الزامی می باشد";
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

            if (ServiceId == 0)
            {
                msg = "وارد کردن نوع پست الزامی می باشد";
                return false;
            }
            msg = "";
            return true;
        }
    }
}
