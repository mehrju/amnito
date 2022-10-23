using System;

namespace Nop.Plugin.Payments.Mellat
{
    public static class Utility
    {
        public static string ErrorCode(string codeType = "", string code = "")
        {
            string result;

            switch (code)
            {
                case "0":
                    result = string.Format("تراکنش با موفقیت انجام شد", new object[0]);
                    return result;
                    break;
                case "11":
                    return "شماره كارت نامعتبر است";

                case "12":
                    result = string.Format("موجودی کافی نیست", new object[0]);
                    return result;
                    break;
                case "13":
                    return "رمز نادرست است";
                    break;
                case "14":
                    return "تعداد دفعات وارد كردن رمز بيش از حد مجاز است";
                    break;
                case "15":
                    return "كارت نامعتبر است";
                    break;
                case "16":
                    return "دفعات برداشت وجه بيش از حد مجاز است";
                    break;
                case "17":
                    return "كاربر از انجام تراكنش منصرف شده است";
                    break;
                case "18":
                    return "تاريخ انقضاي كارت گذشته است";
                    break;
                case "19":
                    return "مبلغ برداشت وجه بيش از حد مجاز است";
                    break;
                case "111":
                    return "صادر كننده كارت نامعتبر است";
                    break;
                case "112":
                    return "خطاي سوييچ صادر كننده كارت";
                    break;
                case "113":
                    return "پاسخي از صادر كننده كارت دريافت نشد";
                    break;
                case "114":
                    return "دارنده كارت مجاز به انجام اين تراكنش نيست";
                    break;
                case "21":
                    return "ذيرنده نامعتبر است";
                    break;
                case "23":
                    return "خطاي امنيتي رخ داده است";
                    break;
                case "24":
                    return "اطلاعات كاربري پذيرنده نامعتبر است";
                    break;
                case "25":
                    return "مبلغ نامعتبر است";
                    break;
                case "31":
                    return "پاسخ نامعتبر است";
                    break;
                case "32":
                    return "فرمت اطلاعات وارد شده صحيح نمي باشد";
                    break;
                case "33":
                    return "حساب نامعتبر است";
                    break;
                case "34":
                    return "خطاي سيستمي";
                    break;
                case "35":
                    return "تاريخ نامعتبر است";
                    break;
                case "41":
                    return "شماره درخواست تكراري است";
                    break;
                case "42":
                    return "تراكنش Sale يافت نشد";
                    break;
                case "43":
                    return "قبلا درخواست Verify داده شده است";
                    break;
                case "421":
                    return "IP نامعتبر است";
                    break;
                case "51":
                    return "تراكنش تكراري است";
                    break;
                case "54":
                    return "راكنش مرجع موجود نيست";
                    break;
                case "55":
                    return "تراكنش نامعتبر است";
                    break;
                case "خطا در واريز":
                    return "61";
                    break;
                case "416":
                    return "خطا در ثبت اطلاعات";
                    break;
                case "415":
                    return "زمان جلسه كاري به پايان رسيده است";
                    break;
                case "414":
                    return "سازمان صادر كننده قبض نامعتبر است";
                    break;
                case "413":
                    return "شناسه پرداخت نادرست است";
                    break;
                case "412":
                    return "شناسه قبض نادرست است";
                    break;
                case "49":
                    return "تراكنش Refund يافت نشد";
                    break;


                default:
                    return "خطاهای متفرقه";
                    break;
            }

        }
    }
}