using Nop.plugin.Orders.ExtendedShipment.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class StringEx
    {
        public static DateTime FromPersianToGregorianDate(this string value, char betweenChar = '/')
        {
            return Common.PersianToDateTime(value, betweenChar);
        }

        public static int ToEnDigit(this string str)
        {
            try
            {
                var inputValue = convertDigit(str).Trim();
                var newStringBuilder = new StringBuilder();
                newStringBuilder.Append(inputValue.Normalize(NormalizationForm.FormKD).Where(x => x < 128).ToArray());
                return Convert.ToInt32(newStringBuilder.ToString().Trim().Split('.')[0]);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static string ToEnDigitString(this string str)
        {
            try
            {
                var inputValue = convertDigit(str).Trim();
                var newStringBuilder = new StringBuilder();
                newStringBuilder.Append(inputValue.Normalize(NormalizationForm.FormKD).Where(x => x < 128).ToArray());
                return newStringBuilder.ToString().Trim().Split('.')[0];
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string convertDigit(string Str)
        {
            var res = "";
            foreach (var chr in Str)
                switch (chr)
                {
                    case '۰':
                        {
                            res += "0";
                            break;
                        }
                    case '۱':
                        {
                            res += "1";
                            break;
                        }
                    case '۲':
                        {
                            res += "2";
                            break;
                        }
                    case '۳':
                        {
                            res += "3";
                            break;
                        }
                    case '۴':
                        {
                            res += "4";
                            break;
                        }
                    case '۵':
                        {
                            res += "5";
                            break;
                        }
                    case '۶':
                        {
                            res += "6";
                            break;
                        }
                    case '۷':
                        {
                            res += "7";
                            break;
                        }
                    case '۸':
                        {
                            res += "8";
                            break;
                        }
                    case '۹':
                        {
                            res += "9";
                            break;
                        }

                    //#region Arabic
                    case '٠':
                        {
                            res += "0";
                            break;
                        }
                    case '١':
                        {
                            res += "1";
                            break;
                        }
                    case '٢':
                        {
                            res += "2";
                            break;
                        }
                    case '٣':
                        {
                            res += "3";
                            break;
                        }
                    case '٤':
                        {
                            res += "4";
                            break;
                        }
                    case '٥':
                        {
                            res += "5";
                            break;
                        }
                    case '٦':
                        {
                            res += "6";
                            break;
                        }
                    case '٧':
                        {
                            res += "7";
                            break;
                        }
                    case '٨':
                        {
                            res += "8";
                            break;
                        }
                    case '٩':
                        {
                            res += "9";
                            break;
                        }

                    //#endregion
                    case ' ':
                        {
                            break;
                        }
                    default:
                        {
                            res += chr;
                            break;
                        }
                }
            return res;
        }


        public static string Right(this string value, int length)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            return value.Substring(value.Length - length);
        }

        public static string RemoveWhiteSpaces(this string value)
        {
            string[] charactersToReplace = new string[] { Environment.NewLine, "\n", "\t", "\r", " " };
            foreach (string s in charactersToReplace)
            {
                value = value.Replace(s, "");
            }
            return value;
        }
    }
}
