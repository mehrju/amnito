using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Customer;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Product;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Models.Common;
using FluentValidation;
using NUglify.Helpers;

namespace BS.Plugin.NopStation.MobileWebApi.Extensions
{
    public static class ValidationExtension
    {

        #region Utility
        private static bool IsNotValidEmail(this string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return !Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        private static bool IsNull(this object value)
        {
            return value == null;
        }

        private static bool IsNotRightLength(this string value, int min,int max)
        {
            if (value == null)
                return false;

            return !(value.Length >= min && value.Length <= max);
        }

        private static bool IsNotEqual(this string value, string checkValue)
        {
            if (checkValue == null)
                return false;

            return !value.Equals(checkValue);
        }
        private static void WithMessage(this bool flag, ModelStateDictionary modelState, string message)
        {
            if (flag == true)
            {
                modelState.AddModelError("", message);
            }
        }
        #endregion
        #region validation helpers
        public static void AddressValidator(ModelStateDictionary modelState, Address model,ILocalizationService localizationService, AddressSettings addressSettings,IStateProvinceService stateProvinceService)
        {
            model.FirstName.IsNullOrWhiteSpace()
                .WithMessage(modelState, localizationService.GetResource("Address.Fields.FirstName.Required"));
            model.LastName.IsNullOrWhiteSpace()
                .WithMessage(modelState, localizationService.GetResource("Address.Fields.LastName.Required"));
            model.Email.IsNullOrWhiteSpace().
                WithMessage(modelState, localizationService.GetResource("Address.Fields.Email.Required"));
            model.Email.IsNotValidEmail().
                WithMessage(modelState, localizationService.GetResource("Common.WrongEmail"));

            if (addressSettings.CountryEnabled)
            {

                model.CountryId.IsNull().
                    WithMessage(modelState,localizationService.GetResource("Address.Fields.Country.Required"));
                model.CountryId.Equals(0)
                    .WithMessage(modelState, localizationService.GetResource("Address.Fields.Country.Required"));
            }
            if (addressSettings.CountryEnabled && addressSettings.StateProvinceEnabled)
            {

                //does selected country has states?
                var countryId = model.CountryId.HasValue ? model.CountryId.Value : 0;
                var hasStates = stateProvinceService.GetStateProvincesByCountryId(countryId).Count > 0;

                if (hasStates)
                {
                    //if yes, then ensure that state is selected
                    if (!model.StateProvinceId.HasValue || model.StateProvinceId.Value == 0)
                    {
                        modelState.AddModelError("StateProvinceId", localizationService.GetResource("Address.Fields.StateProvince.Required"));
                    }
                }

            }
            if (addressSettings.CompanyRequired && addressSettings.CompanyEnabled)
            {
                model.Company.IsNullOrWhiteSpace()
                    .WithMessage(modelState, localizationService.GetResource("Address.Fields.Company.Required"));
            }
            if (addressSettings.StreetAddressRequired && addressSettings.StreetAddressEnabled)
            {
                model.Address1.IsNullOrWhiteSpace().
                    WithMessage(modelState,localizationService.GetResource("Address.Fields.StreetAddress.Required"));
             
            }
            if (addressSettings.StreetAddress2Required && addressSettings.StreetAddress2Enabled)
            {
                model.Address2.IsNullOrWhiteSpace().WithMessage(modelState,localizationService.GetResource("Address.Fields.StreetAddress2.Required"));
               
            }
            if (addressSettings.ZipPostalCodeRequired && addressSettings.ZipPostalCodeEnabled)
            {
                model.ZipPostalCode.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.ZipPostalCode.Required"));
            }
            if (addressSettings.CityRequired && addressSettings.CityEnabled)
            {
                model.City.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.City.Required"));
            }
            if (addressSettings.PhoneRequired && addressSettings.PhoneEnabled)
            {
                model.PhoneNumber.IsNullOrWhiteSpace().WithMessage(modelState,localizationService.GetResource("Address.Fields.Phone.Required"));
            }
            if (addressSettings.FaxRequired && addressSettings.FaxEnabled)
            {
                model.FaxNumber.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.Fax.Required"));
            }
        }



        public static void CustomerInfoValidator(ModelStateDictionary modelState,CustomerInfoQueryModel model, ILocalizationService localizationService,
            IStateProvinceService stateProvinceService, 
            CustomerSettings customerSettings)
        {
            model.FirstName.IsNullOrWhiteSpace()
                .WithMessage(modelState, localizationService.GetResource("Address.Fields.FirstName.Required"));
            model.LastName.IsNullOrWhiteSpace()
                .WithMessage(modelState, localizationService.GetResource("Address.Fields.LastName.Required"));
            model.Email.IsNullOrWhiteSpace().
                WithMessage(modelState, localizationService.GetResource("Address.Fields.Email.Required"));
            model.Email.IsNotValidEmail().
                WithMessage(modelState, localizationService.GetResource("Common.WrongEmail"));

            if (customerSettings.UsernamesEnabled && customerSettings.AllowUsersToChangeUsernames)
            {
                model.Username.IsNullOrWhiteSpace().WithMessage(modelState,localizationService.GetResource("Account.Fields.Username.Required"));
            }

            //form fields
            if (customerSettings.CountryEnabled && customerSettings.CountryRequired)
            {
                model.CountryId.Equals(0)
                    .WithMessage(modelState, localizationService.GetResource("Address.Fields.Country.Required"));
            }
            if (customerSettings.CountryEnabled && 
                customerSettings.StateProvinceEnabled &&
                customerSettings.StateProvinceRequired)
            {
               
                var hasStates = stateProvinceService.GetStateProvincesByCountryId(model.CountryId).Count > 0;

                if (hasStates)
                {
                    //if yes, then ensure that state is selected
                    if (model.StateProvinceId == 0)
                    {
                        modelState.AddModelError("StateProvinceId", localizationService.GetResource("Address.Fields.StateProvince.Required"));
                    }
                }
            }
            if (customerSettings.DateOfBirthRequired && customerSettings.DateOfBirthEnabled)
            {
                 var dateOfBirth = model.ParseDateOfBirth();
                    if (dateOfBirth == null)
                    {
                        modelState.AddModelError("", localizationService.GetResource("Account.Fields.DateOfBirth.Required"));
                    }
                    
            }
            
            if (customerSettings.CompanyRequired && customerSettings.CompanyEnabled)
            {
                model.Company.IsNullOrWhiteSpace()
                    .WithMessage(modelState, localizationService.GetResource("Address.Fields.Company.Required"));
            }
            if (customerSettings.StreetAddressRequired && customerSettings.StreetAddressEnabled)
            {
                model.StreetAddress.IsNullOrWhiteSpace().
                    WithMessage(modelState, localizationService.GetResource("Address.Fields.StreetAddress.Required"));

            }
            if (customerSettings.StreetAddress2Required && customerSettings.StreetAddress2Enabled)
            {
                model.StreetAddress2.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.StreetAddress2.Required"));

            }
            if (customerSettings.ZipPostalCodeRequired && customerSettings.ZipPostalCodeEnabled)
            {
                model.ZipPostalCode.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.ZipPostalCode.Required"));
            }
            if (customerSettings.CityRequired && customerSettings.CityEnabled)
            {
                model.City.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.City.Required"));
            }
            if (customerSettings.PhoneRequired && customerSettings.PhoneEnabled)
            {
                model.Phone.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.Phone.Required"));
            }
            if (customerSettings.FaxRequired && customerSettings.FaxEnabled)
            {
                model.Fax.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.Fax.Required"));
            }
        }

        public static void RegisterValidator(ModelStateDictionary modelState, RegisterQueryModel model, ILocalizationService localizationService, 
            IStateProvinceService stateProvinceService,
            CustomerSettings customerSettings)
        {
            //model.FirstName.IsNullOrWhiteSpace()
            //    .WithMessage(modelState, localizationService.GetResource("Address.Fields.FirstName.Required"));
            //model.LastName.IsNullOrWhiteSpace()
            //    .WithMessage(modelState, localizationService.GetResource("Address.Fields.LastName.Required"));
            //model.Email.IsNullOrWhiteSpace().
            //    WithMessage(modelState, localizationService.GetResource("Address.Fields.Email.Required"));
            //model.Email.IsNotValidEmail().
            //    WithMessage(modelState, localizationService.GetResource("Common.WrongEmail"));

            if (customerSettings.UsernamesEnabled && customerSettings.AllowUsersToChangeUsernames)
            {
                model.Username.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Account.Fields.Username.Required"));

            }
            
           
            model.Password.IsNullOrWhiteSpace().WithMessage(modelState,localizationService.GetResource("Account.Fields.Password.Required"));
            model.CodeMeli.IsNullOrWhiteSpace().WithMessage(modelState, "CodeMeli Is Required");
            if (!CheckCodeMeli.IsValidNationalCode(model.CodeMeli))
            {
                model.CodeMeli.IsValidNationalCode().WithMessage(modelState, "Code Meli Not Valid");
            }


            model.Password.IsNotRightLength(customerSettings.PasswordMinLength, 999).WithMessage(modelState, string.Format(localizationService.GetResource("Account.Fields.Password.LengthValidation"), customerSettings.PasswordMinLength));
            model.ConfirmPassword.IsNullOrWhiteSpace().WithMessage(modelState,localizationService.GetResource("Account.Fields.ConfirmPassword.Required"));
            model.ConfirmPassword.IsNotEqual(model.Password).WithMessage(modelState,localizationService.GetResource("Account.Fields.Password.EnteredPasswordsDoNotMatch"));

            if (customerSettings.CountryEnabled && customerSettings.CountryRequired)
            {
                model.CountryId.Equals(0)
                    .WithMessage(modelState, localizationService.GetResource("Address.Fields.Country.Required"));
            }
            if (customerSettings.CountryEnabled &&
                customerSettings.StateProvinceEnabled &&
                customerSettings.StateProvinceRequired)
            {

                var hasStates = stateProvinceService.GetStateProvincesByCountryId(model.CountryId).Count > 0;

                if (hasStates)
                {
                    //if yes, then ensure that state is selected
                    if (model.StateProvinceId == 0)
                    {
                        modelState.AddModelError("StateProvinceId", localizationService.GetResource("Address.Fields.StateProvince.Required"));
                    }
                }
            }
            if (customerSettings.DateOfBirthRequired && customerSettings.DateOfBirthEnabled)
            {
                var dateOfBirth = model.ParseDateOfBirth();
                if (dateOfBirth == null)
                {
                    modelState.AddModelError("", localizationService.GetResource("Account.Fields.DateOfBirth.Required"));
                }

            }

            if (customerSettings.CompanyRequired && customerSettings.CompanyEnabled)
            {
                model.Company.IsNullOrWhiteSpace()
                    .WithMessage(modelState, localizationService.GetResource("Address.Fields.Company.Required"));
            }
            if (customerSettings.StreetAddressRequired && customerSettings.StreetAddressEnabled)
            {
                model.StreetAddress.IsNullOrWhiteSpace().
                    WithMessage(modelState, localizationService.GetResource("Address.Fields.StreetAddress.Required"));

            }
            if (customerSettings.StreetAddress2Required && customerSettings.StreetAddress2Enabled)
            {
                model.StreetAddress2.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.StreetAddress2.Required"));

            }
            if (customerSettings.ZipPostalCodeRequired && customerSettings.ZipPostalCodeEnabled)
            {
                model.ZipPostalCode.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.ZipPostalCode.Required"));
            }
            if (customerSettings.CityRequired && customerSettings.CityEnabled)
            {
                model.City.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.City.Required"));
            }
            if (customerSettings.PhoneRequired && customerSettings.PhoneEnabled)
            {
                model.Phone.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.Phone.Required"));
            }
            if (customerSettings.FaxRequired && customerSettings.FaxEnabled)
            {
                model.Fax.IsNullOrWhiteSpace().WithMessage(modelState, localizationService.GetResource("Address.Fields.Fax.Required"));
            }
        }

        public static void  LoginValidator(ModelStateDictionary modelState, LoginQueryModel model,ILocalizationService localizationService, CustomerSettings customerSettings)
        {
            if (!customerSettings.UsernamesEnabled)
            {
                //login by email
                model.Email.IsNullOrWhiteSpace().WithMessage(modelState,localizationService.GetResource("Account.Login.Fields.Email.Required"));
                model.Email.IsNotValidEmail().WithMessage(modelState,localizationService.GetResource("Common.WrongEmail"));
            }
        }

        public static void WriteReviewValidator(ModelStateDictionary modelState, ProductReviewQueryModel model, ILocalizationService localizationService)
        {

            model.Title.IsNullOrWhiteSpace().WithMessage(modelState,localizationService.GetResource("Reviews.Fields.Title.Required"));
            model.Title.IsNotRightLength(1, 200)
                .WithMessage(modelState,
                    string.Format(localizationService.GetResource("Reviews.Fields.Title.MaxLengthValidation"), 200));
            model.ReviewText.IsNullOrWhiteSpace()
                .WithMessage(modelState, localizationService.GetResource("Reviews.Fields.ReviewText.Required"));
        }
        #endregion
    }
    public static class CheckCodeMeli
    {
        /// <summary>
        /// تعیین معتبر بودن کد ملی
        /// </summary>
        /// <param name="nationalCode">کد ملی وارد شده</param>
        /// <returns>
        /// در صورتی که کد ملی صحیح باشد خروجی <c>true</c> و در صورتی که کد ملی اشتباه باشد خروجی <c>false</c> خواهد بود
        /// </returns>
        /// <exception cref="System.Exception"></exception>
        public static Boolean IsValidNationalCode(this String nationalCode)
        {
            //در صورتی که کد ملی وارد شده تهی باشد

            if (String.IsNullOrEmpty(nationalCode))
                throw new Exception("لطفا کد ملی را صحیح وارد نمایید");


            //در صورتی که کد ملی وارد شده طولش کمتر از 10 رقم باشد
            if (nationalCode.Length != 10)
                throw new Exception("طول کد ملی باید ده کاراکتر باشد");

            //در صورتی که کد ملی ده رقم عددی نباشد
            var regex = new Regex(@"\d{10}");
            if (!regex.IsMatch(nationalCode))
                throw new Exception("کد ملی تشکیل شده از ده رقم عددی می‌باشد؛ لطفا کد ملی را صحیح وارد نمایید");

            //در صورتی که رقم‌های کد ملی وارد شده یکسان باشد
            var allDigitEqual = new[] { "0000000000", "1111111111", "2222222222", "3333333333", "4444444444", "5555555555", "6666666666", "7777777777", "8888888888", "9999999999" };
            if (allDigitEqual.Contains(nationalCode)) return false;


            //عملیات شرح داده شده در بالا
            var chArray = nationalCode.ToCharArray();
            var num0 = Convert.ToInt32(chArray[0].ToString()) * 10;
            var num2 = Convert.ToInt32(chArray[1].ToString()) * 9;
            var num3 = Convert.ToInt32(chArray[2].ToString()) * 8;
            var num4 = Convert.ToInt32(chArray[3].ToString()) * 7;
            var num5 = Convert.ToInt32(chArray[4].ToString()) * 6;
            var num6 = Convert.ToInt32(chArray[5].ToString()) * 5;
            var num7 = Convert.ToInt32(chArray[6].ToString()) * 4;
            var num8 = Convert.ToInt32(chArray[7].ToString()) * 3;
            var num9 = Convert.ToInt32(chArray[8].ToString()) * 2;
            var a = Convert.ToInt32(chArray[9].ToString());

            var b = (((((((num0 + num2) + num3) + num4) + num5) + num6) + num7) + num8) + num9;
            var c = b % 11;

            return (((c < 2) && (a == c)) || ((c >= 2) && ((11 - c) == a)));
        }
    }
}
