using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Nop.Data;
using System.Linq;

namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public static class Extensions
    {
        //public static string SumAddressValue(this Address model)
        //{
        //    if (model == null)
        //        return "";

        //    return model.CountryId.ToString() + "|" + model.StateProvinceId.ToString() + "|" + ((model.FirstName + "|" ?? "") + (model.LastName + "|" ?? "") + (model.PhoneNumber + "|" ?? "") +
        //              (model.Address1 + "|" ?? "") + (model.ZipPostalCode + "|" ?? "") + (model.Company + "|" ?? ""));
        //}
        public static string SumAddress(this Address model)
        {
            if (model == null)
                return "";

            return (model.Country?.Name ?? "") + "-" + (model.StateProvince?.Name ?? "") + "-" + ((model.FirstName + "-" ?? "") + (model.LastName + "-" ?? "") + (model.PhoneNumber + "-" ?? "") +
                      (model.Address1 + "-" ?? "") + (model.ZipPostalCode + "-" ?? "") + (model.Company ?? ""));
        }
        public static string SumAddress1(this Address model)
        {
            if (model == null)
                return "";

            return (model.Address1 + "-" ?? "") + (model.FirstName + "-" ?? "") + (model.LastName + "-" ?? "") + (model.PhoneNumber  ?? "");
        }
    }
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }
    }
    public static class DtsAddressExtensions
    {
        public static int getDtsStateId(this Address model)
        {
            var _dbContext = EngineContext.Current.Resolve<IDbContext>();
            string query = $@"SELECT
	                            TDSP.City_Id
                            FROM
	                            dbo.Tb_Dts_StateProvince AS TDSP
                            WHERE
	                            TDSP.StateId = " + model.StateProvinceId;
            return _dbContext.SqlQuery<int?>(query).SingleOrDefault().GetValueOrDefault(0);
        }
    }
}
