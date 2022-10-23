using Nop.Core.Domain.Common;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.Data;
using Nop.Core.Infrastructure;
using Nop.plugin.Orders.ExtendedShipment.Services;

namespace Nop.Plugin.Orders.MultiShipping.Model
{
    public class NewCheckoutModel
    {

        public bool IsValid(out string message)
        {
            message = "";


            if (ServiceId <= 0)
            {
                message = "سرویس انتخاب شده نامعتبر می باشد" + " \r\n ";
            }
            if (Weight <= 0)
            {
                message += "وزن وارد شده نامعتبر می باشد" + " \r\n ";
            }
            if (string.IsNullOrEmpty(UbbraTruckType) && (string.IsNullOrEmpty(InsuranceName) || string.IsNullOrWhiteSpace(InsuranceName)))
            {
                message += "لطفا آیتم بیمه رو به درستی انتخاب نمایید" + " \r\n ";
            }
            if (Count <= 0)
            {
                message += "تعداد بسته نا معتبر می باشد";
            }
            if (new int[] { 700, 706 }.Contains(ServiceId) && ((this.boxType == "بسته" && (this.length == 0 || this.height == 0 || this.width == 0))
                || (this.boxType == "پاکت" && (this.height == 0 || this.width == 0))))
            {
                message += "وارد کردن طول و عرض و ارتفاع در سرویس های خارجی و اکسپرس الزامی باشد";
            }
            #region فرستنده
            if (string.IsNullOrEmpty(billingAddressModel.FirstName) || string.IsNullOrWhiteSpace(billingAddressModel.FirstName))
            {
                message += "نام فرستنده نامعتبر می باشد" + " \r\n ";
            }
            if (string.IsNullOrEmpty(billingAddressModel.LastName) || string.IsNullOrWhiteSpace(billingAddressModel.LastName))
            {
                message += "نام خانودگی فرستنده نامعتبر می باشد" + " \r\n ";
            }
            if (string.IsNullOrEmpty(billingAddressModel.Address1) || string.IsNullOrWhiteSpace(billingAddressModel.Address1))
            {
                message += "آدرس فرستنده نامعتبر می باشد" + " \r\n ";
            }
            if (billingAddressModel.CountryId <= 0)
            {
                message += "استان فرستنده نامعتبر می باشد" + " \r\n ";
            }
            if (billingAddressModel.StateProvinceId <= 0)
            {
                message += "شهرستان فرستنده نامعتبر می باشد" + " \r\n ";
            }
            if (string.IsNullOrEmpty(billingAddressModel.PhoneNumber)
                || string.IsNullOrWhiteSpace(billingAddressModel.PhoneNumber)
                || billingAddressModel.PhoneNumber?.Length < 10
                || billingAddressModel.PhoneNumber?.Length > 15)
            {
                message += "موبایل فرستنده نامعتبر می باشد" + " \r\n ";
            }
            #endregion

            #region گیرنده

            if (string.IsNullOrEmpty(shippingAddressModel.FirstName) || string.IsNullOrWhiteSpace(shippingAddressModel.FirstName))
            {
                message += "نام گیرنده نامعتبر می باشد" + " \r\n ";
            }
            if (string.IsNullOrEmpty(shippingAddressModel.LastName) || string.IsNullOrWhiteSpace(shippingAddressModel.LastName))
            {
                message += "نام خانودگی گیرنده نامعتبر می باشد" + " \r\n ";
            }
            if (string.IsNullOrEmpty(shippingAddressModel.Address1) || string.IsNullOrWhiteSpace(shippingAddressModel.Address1))
            {
                message += "آدرس گیرنده نامعتبر می باشد" + " \r\n ";
            }
            var catInfo = getCategoryInfo();
            if (catInfo.IsForeign)
            {
                if (this.receiver_ForeginCountry <= 0)
                    message += "انتخاب کشور مقصد در سفارشات خارجی الزامی می باشد";
            }
            else
            {
                if (shippingAddressModel.CountryId <= 0)
                {
                    message += "استان گیرنده نامعتبر می باشد" + " \r\n ";
                }
                if (shippingAddressModel.StateProvinceId <= 0)
                {
                    message += "شهرستان گیرنده نامعتبر می باشد" + " \r\n ";
                }
                if (string.IsNullOrEmpty(shippingAddressModel.PhoneNumber)
                    || string.IsNullOrWhiteSpace(shippingAddressModel.PhoneNumber)
                    || shippingAddressModel.PhoneNumber?.Length < 10
                    || shippingAddressModel.PhoneNumber?.Length > 15)
                {
                    message += "موبایل گیرنده نامعتبر می باشد" + " \r\n ";
                }
            }
            #endregion


            return true;
        }
        public CategoryInfoModel getCategoryInfo()
        {
            return EngineContext.Current.Resolve<IExtendedShipmentService>().GetCategoryInfo(this.ServiceId);
        }
        public int TempId { get; set; }
        public int ServiceId { get; set; }
        public string GoodsType { get; set; }
        public int ApproximateValue { get; set; }
        public int? CodGoodsPrice { get; set; }
        public int Weight { get; set; }
        public string InsuranceName { get; set; }
        public string CartonSizeName { get; set; }
        public Address billingAddressModel { get; set; }
        public Address shippingAddressModel { get; set; }
        public bool IsCOD { get; set; }
        public bool IsFreePost { get; set; }
        public bool HasAccessToPrinter { get; set; }
        public bool hasNotifRequest { get; set; }
        public bool getItNow { get; set; }
        public int AgentSaleAmount { get; set; }
        public string discountCouponCode { get; set; }
        public int Count { get; set; }
        public float? length { get; set; }
        public float? width { get; set; }
        public float? height { get; set; }
        public string boxType { get; set; }
        public int receiver_ForeginCountry { get; set; }
        public string receiver_ForeginCountryName { get; set; }
        public string receiver_ForeginCountryNameEn { get; set; }
        public string receiver_ForeginCityName { get; set; }
        public string UbbraTruckType { get; set; }
        public string VechileOptions { get; set; }
        public string UbbarPackingLoad { get; set; }
        public string dispatch_date { get; set; }
        public DateTime? _dispatch_date { get; set; }
        public bool RequestPrintAvatar { get; set; }
        public float? SenderLat { get; set; }
        public float? SenderLon { get; set; }
        public float? ReciverLat { get; set; }
        public float? ReciverLon { get; set; }
        public bool IsFromAp { get; set; }
        public bool IsFromSep { get; set; }
        public string tehranCityArea { get; set; }
        public string collectorArea { get; set; }
        public bool trafficArea { get; set; }
        public bool isInCityArea { get; set; }
        public bool needCaton { get; set; }

        public string imgFile1 { get; set; }
        public string imgFile2 { get; set; }
        public string imgFile3 { get; set; }

        public string UniqueReferenceNo { get; set; }
        public string StoreLink { get; set; }
        public bool IsSecondhand { get; set; }
        public bool IsSafeBuy { get; set; }
        public bool IsFromFava { get; set; }
        public int ShipmentTempId { get; set; }
        public bool NeedCollector { get; set; }
        public bool NeedDistributer { get; set; }

    }


    public class NewCheckout_Sp_Input
    {
        public string JsonData { get; set; }
        public string JsonOrderList { get; set; }
        public string JsonDataOut { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

    }

    public class NewCheckout_Sp_Output
    {
        public bool? SendSms { get; set; }
        public int orderId { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }


}
