using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.PreOrderModel
{
    public class CheckoutParcellModel
    {
        public ParcellAddress SenderAddress { get; set; }
        public List<ParcellInfo> ParcellList { get; set; }
        public string DiscountCoupon { get; set; }
        public string UniqueReferenceNo { get; set; }
        public int SourceId { get; set; }
        public int ServiceId { get; set; }
        public bool IsValid(out string error)
        {
            error = "";
            #region SenderAddress
            if (this.SenderAddress == null)
            {
                error = "اطلاعات فرستنده نامعتبر می باشد";
                return false;
            }
            if (string.IsNullOrEmpty(this.SenderAddress.FirstName))
                error = "نام فرستنده را وارد کنید";
            if (string.IsNullOrEmpty(this.SenderAddress.LastName))
                error += "نام خانوادگی فرستنده را وارد کنید";

            if (string.IsNullOrEmpty(this.SenderAddress.PhoneNumber))
                error += "شماره موبایل فرستنده را وارد کنید";

            if (string.IsNullOrEmpty(this.SenderAddress.Latitude) || string.IsNullOrEmpty(this.SenderAddress.Longitude)
                || Convert.ToDouble(this.SenderAddress.Latitude) == 0 || Convert.ToDouble(this.SenderAddress.Longitude) == 0)
                error += "نقطه جغرافیایی فرستنده به درستی وارد نشده";
            if (this.SenderAddress.StateId == 0)
                error += "استان مبدا به درستی مشخص نشده";
            if (this.SenderAddress.CityId == 0)
                error += "شهرستان مبدا به درستی مشخص نشده";
            if (string.IsNullOrEmpty(this.SenderAddress.Address))
                error += "فیلد آدرس فرستنده به درستی مشخص نشده";
            #endregion
            if (this.ParcellList == null)
            {
                error += "اطلاعات مرسولات به درستی وارد نشده";
                return false;
            }
            if (!this.ParcellList.Any())
            {
                error += "اطلاعات مرسولات به درستی وارد نشده";
                return false;
            }
            foreach (var parcell in this.ParcellList)
            {
                #region ReceiverAddress
                if (parcell.ReceiverAddress == null)
                {
                    error += "اطلاعات گیرنده نامعتبر می باشد";
                    return false;
                }
                if (string.IsNullOrEmpty(parcell.ReceiverAddress.FirstName))
                    error += "نام گیرنده را وارد کنید";
                if (string.IsNullOrEmpty(parcell.ReceiverAddress.LastName))
                    error += "نام خانوادگی گیرنده را وارد کنید";

                if (string.IsNullOrEmpty(parcell.ReceiverAddress.PhoneNumber))
                    error += "شماره موبایل گیرنده را وارد کنید";
                if (parcell.ReceiverAddress.StateId == 0)
                    error += "استان مقصد به درستی مشخص نشده";
                if (parcell.ReceiverAddress.CityId == 0)
                    error += "شهرستان مقصد به درستی مشخص نشده";
                if (string.IsNullOrEmpty(parcell.ReceiverAddress.Address))
                    error += "فیلد آدرس گیرنده به درستی مشخص نشده";
                #endregion

                if (parcell.ContentValuePrice == 0)
                    error += "ارزش ریالی محتویات مرسوله به درستی وارد نشده";
                if (parcell.Height == 0 || parcell.Width == 0 || parcell.Lenght == 0)
                    error += "ابعاد مرسوله به درستی وارد نشده";
                if (parcell.Weight == 0)
                    error += "وزن وارد شده نا معتبر می  باشد";
                if (error != "")
                    return false;
            }
            if (this.SourceId == 0)
                error += "کد شرکت سفارش کننده به درستی وارد نشده";
            if (string.IsNullOrEmpty(this.UniqueReferenceNo))
            {
                error += "کد یکتای سفارش به درستی وارد نشده";
            }
            if (error != "")
                return false;
            return true;
        }
    }
    public class ParcellAddress
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string NationalCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public string Address { get; set; }
    }
    public class ParcellInfo
    {
        public ParcellAddress ReceiverAddress { get; set; }

        public int Weight { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Lenght { get; set; }
        public string InsuranceTitle { get; set; }
        public string Content { get; set; }
        public int ContentValuePrice { get; set; }
        public bool RequiresPackaging { get; set; }
        public bool SMSNotification { get; set; }
        public bool PrintCommercialLogo { get; set; }
        public bool HasAccessToPrinter { get; set; }

    }
}
