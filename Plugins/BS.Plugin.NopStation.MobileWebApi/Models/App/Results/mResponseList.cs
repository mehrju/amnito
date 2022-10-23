using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models.App.Results
{
    public class mResponseList
    {
        public static mResponse success = new mResponse
        {
            id = 1,
            Status = 200,
            message_en = "ok!",
            message_fa = "تایید شد!",
            data = null
        };
        public static mResponse error_Execption = new mResponse
        {
            id = 2,
            Status = 400,
            message_en = "An error has occurred",
            message_fa = "خطایی اتفاق افتاده است!",
            data = null
        };
        public static mResponse error_invalid_param = new mResponse
        {
            id = 3,
            Status = 401,
            message_en = "invalid params!",
            message_fa = "ورودی ها صحیح نمی باشد!",
            data = null
        };
        public static mResponse error_invalid_request_id = new mResponse
        {
            id = 4,
            Status=402,
            message_en = "invalid request id!",
            message_fa = "کد درخواست صحیح نمی باشد!",
            data = null
        };
        public static mResponse error_Register_Disable = new mResponse
        {
            id = 5,
            Status = 403,
            message_en = "Register User Disable",
            message_fa = "ثبت نام کاربران غیرفعال میباشد!",
            data = null
        };
        public static mResponse error_Register_Duplicate = new mResponse
        {
            id = 6,
            Status = 404,
            message_en = " User Duplicate",
            message_fa = "کاربری با مشخصات ارسالی وجود دارد!",
            data = null
        };
        public static mResponse error_No_Data_Exist = new mResponse
        {
            id = 7,
            Status = 405,
            message_en = "No data exists",
            message_fa = "دیتایی وجود ندارد!",
            data = null
        };
        public static mResponse error_CustomerNoRegistered = new mResponse
        {
            id = 8,
            Status = 406,
            message_en = "Customer No Registered",
            message_fa = "کاربر ثبت نام نشده است!",
            data = null
        };
        public static mResponse error_CustomerNoAllowToUploadAvatar = new mResponse
        {
            id = 9,
            Status = 407,
            message_en = "Customer No Allow To Upload Avatar",
            message_fa = "کاربر مجاز به تغییر آواتار نیست!",
            data = null
        };
        public static mResponse error_MaximumUploadedFileSize = new mResponse
        {
            id = 10,
            Status = 408,
            message_en = "Maximum Uploaded File Size",
            message_fa = "سایز عکس بیش از تنظیمات میباشد!",
            data = null
        };
    }
}
