using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nop.Plugin.Orders.BulkOrder.Models
{
    public class OrderList
    {
        public string BoxSize { get; set; }
        public string GoodsType { get; set; }
        public int ApproximateValue { get; set; }
        public int CodGoodsPrice { get; set; }
        public int Wehight_g { get; set; }
        //public int Wehight_Kg { get; set; }
        public string Insurance { get; set; }
        public string Carton { get; set; }
        public string Sender_FristName { get; set; }
        public string Sender_LastName { get; set; }
        public string Sender_mobile { get; set; }
        public string Sender_Country { get; set; }
        public string Sender_State { get; set; }
        public string Reciver_City { get; set; }
        public string Sender_PostCode { get; set; }
        public string Sender_Address { get; set; }
        public string Sender_Email { get; set; }
        public float? Sender_Lat { get; set; }
        public float? Sender_Lon { get; set; }

        public string Reciver_FristName { get; set; }
        public string Reciver_LastName { get; set; }
        public string Reciver_mobile { get; set; }
        public string Reciver_Country { get; set; }
        public string Reciver_State { get; set; }
        public string Sender_City { get; set; }
        public string Reciver_vilage { get; set; }
        public string Reciver_PostCode { get; set; }
        public string Reciver_Address { get; set; }
        public string Reciver_Email { get; set; }
        public  string HasAccessToPrinter { get; set; }
        public int AgentSaleAmount { get; set; }
        public int? length { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
        public int? receiver_ForeginCountry { get; set; }
        public string receiver_ForeginCountryName { get; set; }
        public int? SenderAddressId { get; set; }
        public int? ReciverAddressId { get; set; }
        public int? ServiceId { get; set; }
        public string BoxType { get; set; }
        public int CalcPrice { get; set; }
        public string NeedCarton { get; set; }
        public string GetCodGoodsPrice { get; set; }
    }
    [XmlRoot("ArrayOfOrderList")]
    public class ListOfOrderList
    {
        [XmlElement("ListOfOrderList")]
        public List<OrderList> list { get; set; }
        [NotMapped]
        public string error { get; set; }
    }


    public class CheckoutItemApi
    {
        public int IsValid(out string message)
        {
            message = "";

            #region فرستنده
            if (ServiceId <= 0)
            {
                message = ApiMessage.GetErrorMsg(6);
                return 6;
            }
            if (Weight <= 0)
            {
                message = ApiMessage.GetErrorMsg(7);
                return 7;
            }
            if (string.IsNullOrEmpty(InsuranceName) || string.IsNullOrWhiteSpace(InsuranceName))
            {
                message = ApiMessage.GetErrorMsg(8);
                return 8;
            }
            if (string.IsNullOrEmpty(Sender_FristName) || string.IsNullOrWhiteSpace(Sender_FristName))
            {
                message = ApiMessage.GetErrorMsg(1) + "-نام فرستنده";
                return 1;
            }
            if (string.IsNullOrEmpty(Sender_LastName) || string.IsNullOrWhiteSpace(Sender_LastName))
            {
                message = ApiMessage.GetErrorMsg(1) + "-نام خانوادگی فرستنده";
                return 1;
            }
            if (string.IsNullOrEmpty(Sender_Address) || string.IsNullOrWhiteSpace(Sender_Address))
            {
                message = ApiMessage.GetErrorMsg(1) + "-آدرس فرستنده";
                return 1;
            }
            if (Sender_StateId <= 0)
            {
                message = ApiMessage.GetErrorMsg(1) + "-استان فرستنده";
                return 1;
            }
            if (Sender_townId <= 0)
            {
                message = ApiMessage.GetErrorMsg(1) + "-شهرستان فرستنده";
                return 1;
            }
            if (string.IsNullOrEmpty(Sender_mobile) || string.IsNullOrWhiteSpace(Sender_mobile))
            {
                message = ApiMessage.GetErrorMsg(1) + "-موبایل فرستنده";
                return 1;
            }
            #endregion

            #region گیرنده

            if (string.IsNullOrEmpty(Reciver_FristName) || string.IsNullOrWhiteSpace(Reciver_FristName))
            {
                message = ApiMessage.GetErrorMsg(1) + "-نام گیرنده";
                return 1;
            }
            if (string.IsNullOrEmpty(Reciver_LastName) || string.IsNullOrWhiteSpace(Reciver_LastName))
            {
                message = ApiMessage.GetErrorMsg(1) + "-نام خانوادگی گیرنده";
                return 1;
            }
            if (string.IsNullOrEmpty(Reciver_Address) || string.IsNullOrWhiteSpace(Reciver_Address))
            {
                message = ApiMessage.GetErrorMsg(1) + "-آدرس گیرنده";
                return 1;
            }
            if (Reciver_StateId <= 0)
            {
                message = ApiMessage.GetErrorMsg(1) + "-استان گیرنده";
                return 1;
            }
            if (Reciver_townId <= 0)
            {
                message = ApiMessage.GetErrorMsg(1) + "-شهرستان گیرنده";
                return 1;
            }
            if (string.IsNullOrEmpty(Reciver_mobile) || string.IsNullOrWhiteSpace(Reciver_mobile))
            {
                message = ApiMessage.GetErrorMsg(1) + "-موبایل گیرنده";
                return 1;
            } 
            #endregion

            return 0;
        }
        public int IsValidGhasedak(out string message)
        {
            message = "";

            #region فرستنده
            if (ServiceId <= 0)
            {
                message = ApiMessage.GetErrorMsg(6);
                return 6;
            }
            //if (Weight <= 0)
            //{
            //    message = ApiMessage.GetErrorMsg(7);
            //    return 7;
            //}
            if (string.IsNullOrEmpty(InsuranceName) || string.IsNullOrWhiteSpace(InsuranceName))
            {
                message = ApiMessage.GetErrorMsg(8);
                return 8;
            }
            if (string.IsNullOrEmpty(Sender_FristName) || string.IsNullOrWhiteSpace(Sender_FristName))
            {
                message = ApiMessage.GetErrorMsg(1) + "-نام فرستنده";
                return 1;
            }
            if (string.IsNullOrEmpty(Sender_LastName) || string.IsNullOrWhiteSpace(Sender_LastName))
            {
                message = ApiMessage.GetErrorMsg(1) + "-نام خانوادگی فرستنده";
                return 1;
            }
            if (string.IsNullOrEmpty(Sender_Address) || string.IsNullOrWhiteSpace(Sender_Address))
            {
                message = ApiMessage.GetErrorMsg(1) + "-آدرس فرستنده";
                return 1;
            }
            if (Sender_StateId <= 0)
            {
                message = ApiMessage.GetErrorMsg(1) + "-استان فرستنده";
                return 1;
            }
            if (Sender_townId <= 0)
            {
                message = ApiMessage.GetErrorMsg(1) + "-شهرستان فرستنده";
                return 1;
            }
            if (string.IsNullOrEmpty(Sender_mobile) || string.IsNullOrWhiteSpace(Sender_mobile))
            {
                message = ApiMessage.GetErrorMsg(1) + "-موبایل فرستنده";
                return 1;
            }
            #endregion

            #region گیرنده

            if (string.IsNullOrEmpty(Reciver_FristName) || string.IsNullOrWhiteSpace(Reciver_FristName))
            {
                message = ApiMessage.GetErrorMsg(1) + "-نام گیرنده";
                return 1;
            }
            if (string.IsNullOrEmpty(Reciver_LastName) || string.IsNullOrWhiteSpace(Reciver_LastName))
            {
                message = ApiMessage.GetErrorMsg(1) + "-نام خانوادگی گیرنده";
                return 1;
            }
            if (string.IsNullOrEmpty(Reciver_Address) || string.IsNullOrWhiteSpace(Reciver_Address))
            {
                message = ApiMessage.GetErrorMsg(1) + "-آدرس گیرنده";
                return 1;
            }
            if (Reciver_StateId <= 0)
            {
                message = ApiMessage.GetErrorMsg(1) + "-استان گیرنده";
                return 1;
            }
            if (Reciver_townId <= 0)
            {
                message = ApiMessage.GetErrorMsg(1) + "-شهرستان گیرنده";
                return 1;
            }
            if (string.IsNullOrEmpty(Reciver_mobile) || string.IsNullOrWhiteSpace(Reciver_mobile))
            {
                message = ApiMessage.GetErrorMsg(1) + "-موبایل گیرنده";
                return 1;
            }
            #endregion

            return 0;
        }
        public int ServiceId { get; set; }
        public string GoodsType { get; set; }
        public int ApproximateValue { get; set; }
        public int CodGoodsPrice { get; set; }
        public int Weight { get; set; }
        public string InsuranceName { get; set; }
        public string CartonSizeName { get; set; }
        public string Sender_FristName { get; set; }
        public string Sender_LastName { get; set; }
        public string Sender_mobile { get; set; }
        public int Sender_StateId { get; set; }
        public string Sender_City { get; set; }
        public int Sender_townId { get; set; }
        public string Sender_PostCode { get; set; }
        public string Sender_Address { get; set; }
        public string Sender_Email { get; set; }

        public string Reciver_FristName { get; set; }
        public string Reciver_LastName { get; set; }
        public string Reciver_mobile { get; set; }
        public int Reciver_StateId { get; set; }
        public string Reciver_City { get; set; }
        public int Reciver_townId { get; set; }
        public string Reciver_PostCode { get; set; }
        public string Reciver_Address { get; set; }
        public string Reciver_Email { get; set; }
        public bool IsCOD { get; set; }
        public bool IsFreePost { get; set; }
        public bool IsSafeBuy { get; set; }
        public bool HasAccessToPrinter { get; set; }
        public int AgentSaleAmount { get; set; }
        public string discountCouponCode { get; set; }
        public int? length { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
        public string boxType { get; set; }
        public int? receiver_ForeginCountry { get; set; }
        public string receiver_ForeginCountryName { get; set; }
        public string UbbraTruckType { get; set; }
        public string VechileOptions { get; set; }
        public string UbbarPackingLoad { get; set; }
        public string dispatch_date { get; set; }
        public DateTime? _dispatch_date { get; set; }
        public int Count { get; set; }
        public bool printLogo { get; set; }
        public bool notifBySms { get; set; }

        /// <summary>
        /// 13= api
        /// </summary>
        public int orderSource { get; set; }
        public string receiver_ForeginCityName { get; set; }
        public string refrenceNo { get; set; }
        public float? SenderLat { get; set; }
        public float? SenderLon { get; set; }
        public float? ReciverLat { get; set; }
        public float? ReciverLon { get; set; }
        public string domainName { get; set; }
        public string IpAddress { get; set; }
        public string pluginType { get; set; }
        public string pluginVersion { get; set; }
        public bool NeedCarton { get; set; }
        public int shipmentTempId { get; set; }
    }
    public class SetPayModel
    {
        public string AuthorizationTransactionId { get; set; }
        public int OrderId { get; set; }
        public DateTime PayDate { get; set; }
    }
    public static class ApiMessage
    {
        private static readonly Dictionary<int, string> _errorDict = new Dictionary<int, string>
        {
            {0, "عملیات با موفقیت انجام شد"},
            {1, "اطلاعات وارد صحیح نمی باشد"},
            {2, "نوع سرویس وارد شده در حال حاضر فعال نمی باشد با پشتیبانی تماس بگیرید"},
            {3, "بروز خطا در سرویس اعلام قیمت پس کرایه-متن خطای اصلی در سرویس برگردانده میشود"},
            {4, "اطلاعات وارد شده نامعتبر می باشد"},
            {5, "شناسه سرویس مورد نظر معتبر نمی باشد و یا امکان استفاده آن برای شما وجود ندارد"},
            {6, "نوع سرویس وارد شده نامعتبر می باشد"},
            {7, "وزن وارد شده نامعتبر می باشد"},
            {8, "وارد کردن فیلد بیمه الزامی می باشد"},
            {9,"بروز خطا در زمان ثبت سفارشات"},
            {10,"موجودی کیف پول شما برای ثبت این سفارش کافی نمی باشد"},
            {11,"موجوی کیف پول شما باید حداقل 1،500،000 ریال باشد "},
            {12,"امکان ثبت همزمان سفارش پس کرایه و پیش کرایه وجود ندارد"},
            {13,"سفارش مورد نظر یافت نشد"},
            //14 reserve
            {15,"دسترسی این سرویس برای شما تعریف نشده با واحد پشتیبانی تماس حاصل نمایید"},
            {16,"ابتدا عملیات لاگین را انجام دهید"},
            {17,"سفارش شما با موفقیت ثبت شد ولی تولید بارکد با مشکل مواجه شد، لطفا با پشتیبانی تماس بگیرید"},
            {18,"در حال حاضر امکان ثبت سفارش از مبدا انتخابی شما وجود ندارد"},
            {19,"درخواست {0} قبلا با شماره {1} ثبت شده است" },
            {20,"انتخاب سایز مرسوله و یا وارد کردن ابعاد بسته مرسوله الزامی می باشد."},
            {21,"وارد کردن طول و عرض جغرافیایی فرستنده الزامی می باشد"},
            {22,"بروز مشکل در زمان ثبت سفارش لطفا مجددا سعی کنید"},
            {23, "این سفارش متعلق به شما نیست" },
            {24, "امکان کنسل کردن این سفارش وجود ندارد" },
            {25, "وضعیت ارسالی وجود ندارد" },
            {30, "خطاهای متفرقه" },
            {31, "اطلاعاتی برای سرویس مورد نظر یافت نشد" }
        };

        public static string GetErrorMsg(int key)
        {
            _errorDict.TryGetValue(key, out var value);
            return string.IsNullOrEmpty(value) ? "" : value;
        }
    }
   
}
