using System.Collections.Generic;
using System.Collections.Specialized;
using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using Nop.Core.Domain.Common;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;


namespace BS.Plugin.NopStation.MobileWebApi.Extensions
{
    public static class MappingExtensions
    {

       
        public static NameValueCollection ToNameValueCollection(this List<KeyValueApi> formValues)
        {
            var form = new NameValueCollection();
            foreach (var values in formValues)
            {
                form.Add(values.Key, values.Value);
            }
            return form;
        }

        public static Address AddressFromToModel(this NameValueCollection form, string prefix)
        {
            prefix = prefix + ".";
            var address = new Address()
            {
                FirstName = form[prefix + "FirstName"],
                LastName = form[prefix + "LastName"],
                Email = form[prefix + "Email"],
                Address1 = form[prefix + "Address1"],
                Address2 = form[prefix + "Address2"],
                Company = form[prefix + "Company"],
                ZipPostalCode = form[prefix + "ZipPostalCode"],
                City = form[prefix + "City"],
                PhoneNumber = form[prefix + "PhoneNumber"],
                FaxNumber = form[prefix + "FaxNumber"],

            };
            int cId = 0;
            int.TryParse(form[prefix + "CountryId"], out cId);
            if (cId > 0)
            {
                address.CountryId = cId;
            }
            int sId = 0;
            int.TryParse(form[prefix + "StateProvinceId"], out sId);
            if (sId > 0)
            {
                address.StateProvinceId = sId;
            }
            return address;
        }

        //address
        public static Address ToEntity(this AddressModel model, bool trimFields = true)
        {
            if (model == null)
                return null;

            var entity = new Address();
            return ToEntity(model, entity, trimFields);
        }

        public static Address ToEntity(this AddressModel model, Address destination, bool trimFields = true)
        {
            if (model == null)
                return destination;

            if (trimFields)
            {
                if (model.FirstName != null)
                    model.FirstName = model.FirstName.Trim();
                if (model.LastName != null)
                    model.LastName = model.LastName.Trim();
                if (model.Email != null)
                    model.Email = model.Email.Trim();
                if (model.Company != null)
                    model.Company = model.Company.Trim();
                if (model.City != null)
                    model.City = model.City.Trim();
                if (model.Address1 != null)
                    model.Address1 = model.Address1.Trim();
                if (model.Address2 != null)
                    model.Address2 = model.Address2.Trim();
                if (model.ZipPostalCode != null)
                    model.ZipPostalCode = model.ZipPostalCode.Trim();
                if (model.PhoneNumber != null)
                    model.PhoneNumber = model.PhoneNumber.Trim();
                if (model.FaxNumber != null)
                    model.FaxNumber = model.FaxNumber.Trim();
            }
            destination.Id = model.Id;
            destination.FirstName = model.FirstName;
            destination.LastName = model.LastName;
            destination.Email = model.Email;
            destination.Company = model.Company;
            destination.CountryId = model.CountryId;
            destination.StateProvinceId = model.StateProvinceId;
            destination.City = model.City;
            destination.Address1 = model.Address1;
            destination.Address2 = model.Address2;
            destination.ZipPostalCode = model.ZipPostalCode;
            destination.PhoneNumber = model.PhoneNumber;
            destination.FaxNumber = model.FaxNumber;

            return destination;
        }

    }
}
