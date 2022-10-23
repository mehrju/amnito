using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
	public class RewardPointCashoutService : IRewardPointCashoutService
	{
		private readonly IDbContext _dbContext;

		public RewardPointCashoutService(IDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public List<ChargeWalletHistoryResult> GetCustomerWalletHistory(DateTime? DateFrom, DateTime? DateTo,long? UserId, string chargeWalletType,bool? Paid, int PageSize, int PageIndex, out int count)
		{
			count = 0;
			SqlParameter P_DateFrom = new SqlParameter()
			{
				ParameterName = "DateFrom",
				SqlDbType = SqlDbType.DateTime,
				Value = !DateFrom.HasValue ? (object)DBNull.Value : (object)DateFrom
			};
			SqlParameter P_DateTo = new SqlParameter()
			{
				ParameterName = "DateTo",
				SqlDbType = SqlDbType.DateTime,
				Value = !DateTo.HasValue ? (object)DBNull.Value : (object)DateTo
			};
			SqlParameter P_UserId = new SqlParameter()
			{
				ParameterName = "UserId",
				SqlDbType = SqlDbType.BigInt,
				Value = !UserId.HasValue ? (object)DBNull.Value : (object)UserId
			};
			SqlParameter P_Paid = new SqlParameter()
			{
				ParameterName = "Paid",
				SqlDbType = SqlDbType.Bit,
				Value = !Paid.HasValue ? (object)DBNull.Value : (object)Paid
};
			SqlParameter P_ChargeWalletType = new SqlParameter()
			{
				ParameterName = "ChargeWalletType",
				SqlDbType = SqlDbType.NVarChar,
				Value = string.IsNullOrEmpty(chargeWalletType) ? (object)DBNull.Value : (object)chargeWalletType
			};
			SqlParameter P_count = new SqlParameter() { ParameterName = "Count", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

			SqlParameter[] prms = new SqlParameter[]{
				P_DateFrom,
				P_DateTo,
				P_UserId,
				P_Paid,
				P_ChargeWalletType,
				new SqlParameter() { ParameterName = "PageSize", SqlDbType = SqlDbType.Int, Value = PageSize,Direction = ParameterDirection.Input },
				new SqlParameter() { ParameterName = "PageIndex", SqlDbType = SqlDbType.Int, Value = PageIndex,Direction = ParameterDirection.Input },
				P_count
			};
			var allShipment = _dbContext.SqlQuery<ChargeWalletHistoryResult>(@"EXECUTE [dbo].[Sp_SearchCustomerWalletChargeHistory] @DateFrom,@DateTo,@UserId,@ChargeWalletType,@Paid,@PageSize,@PageIndex,@count OUTPUT", prms).ToList();

			count = (int)P_count.Value;
			return allShipment.ToList();
		}

		public List<RewardPointsCashoutModel> getRewardPointCashout(DateTime? DateFrom, DateTime? DateTo, string customers, string chargeWalletType,string excelfilename, int PageSize, int PageIndex, out int count)
		{
			count = 0;
			SqlParameter P_DateFrom = new SqlParameter()
			{
				ParameterName = "DateFrom",
				SqlDbType = SqlDbType.DateTime,
				Value = !DateFrom.HasValue ? (object)DBNull.Value : (object)DateFrom
			};
			SqlParameter P_DateTo = new SqlParameter()
			{
				ParameterName = "DateTo",
				SqlDbType = SqlDbType.DateTime,
				Value = !DateTo.HasValue ? (object)DBNull.Value : (object)DateTo
			};
			SqlParameter P_ChargeWalletType = new SqlParameter()
			{
				ParameterName = "ChargeWalletType",
				SqlDbType = SqlDbType.NVarChar,
				Value = string.IsNullOrEmpty(chargeWalletType) ? (object)DBNull.Value : (object)chargeWalletType
			};
			SqlParameter P_customers = new SqlParameter()
			{
				ParameterName = "customerIds",
				SqlDbType = SqlDbType.NVarChar,
				Value = string.IsNullOrEmpty(customers) ? (object)DBNull.Value : (object)customers
            };
			SqlParameter P_ExcelFilename = new SqlParameter()
			{
				ParameterName = "ExcelFilename",
				SqlDbType = SqlDbType.NVarChar,
				Value = string.IsNullOrEmpty(excelfilename) ? (object)DBNull.Value : (object)excelfilename
			};
			SqlParameter P_count = new SqlParameter() { ParameterName = "Count", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

			SqlParameter[] prms = new SqlParameter[]{
				P_DateFrom,
				P_DateTo,
				P_customers,
				P_ChargeWalletType,
				P_ExcelFilename,
				new SqlParameter() { ParameterName = "PageSize", SqlDbType = SqlDbType.Int, Value = PageSize,Direction = ParameterDirection.Input },
				new SqlParameter() { ParameterName = "PageIndex", SqlDbType = SqlDbType.Int, Value = PageIndex,Direction = ParameterDirection.Input },
				P_count
			};
			var allShipment = _dbContext.SqlQuery<RewardPointsCashoutModel>(@"EXECUTE [dbo].[Sp_RewardPointsCashout] @DateFrom,@DateTo,@ChargeWalletType,@ExcelFilename,@customerIds,@PageSize,@PageIndex,@count OUTPUT", prms).ToList();

			count = (int)P_count.Value;
			return allShipment.ToList();
		}


		public List<SelectListItem> getchargeWalletTypes()
		{
			return _dbContext.SqlQuery<SelectListItem>(@"SELECT CAST(id AS NVARCHAR(20)) AS [Value],CAST(TCWT.ChargeWalletTypeName AS NVARCHAR(MAX)) AS [Text] FROM dbo.Tb_ChargeWalletType AS TCWT WHERE TCWT.CanCashout = 1").ToList();
		}

		public List<SelectListItem> getcustomers(string searchtext)
		{
			return _dbContext.SqlQuery<SelectListItem>($@"SELECT TOP 50 ISNULL(NOC.Name,C.Username) AS [Text],CAST(C.Id AS NVARCHAR(20)) AS [Value]
								FROM dbo.Customer AS C
										LEFT JOIN dbo.[Name Of Customer] AS NOC ON C.Id = NOC.EntityId
								WHERE C.Active = 1 
									  AND C.Username IS NOT NULL
									  AND (NOC.Name LIKE N'%{searchtext}%' OR C.Username LIKE N'%{searchtext}%')
							").ToList();
		}

		public string saveRewardPointCashout(string jsonTableExcel)
		{
			SqlParameter[] prms = new SqlParameter[]{
				new SqlParameter() { ParameterName = "JsonTableExcel", SqlDbType = SqlDbType.NVarChar, Value = jsonTableExcel,Direction = ParameterDirection.Input }
			};

			var a = _dbContext.SqlQuery<string>(@"EXECUTE [dbo].[Sp_saveRewardPointCashout] @JsonTableExcel", prms).ToList();
			
			return (string)a.FirstOrDefault();
		}

        public string RequestRewardPointCashout(string chargeWalletIds,bool? CashoutTypeId,string ShebaToInsert, long UserId)
        {
            SqlParameter[] prms = new SqlParameter[]{
                new SqlParameter() { ParameterName = "ChargeWalletIds", SqlDbType = SqlDbType.NVarChar, Value = chargeWalletIds,Direction = ParameterDirection.Input },
                new SqlParameter() { ParameterName = "UserId", SqlDbType = SqlDbType.BigInt, Value = UserId,Direction = ParameterDirection.Input },
                new SqlParameter() { ParameterName = "CashoutTypeId", SqlDbType = SqlDbType.Bit, Value = CashoutTypeId,Direction = ParameterDirection.Input },
                new SqlParameter() { ParameterName = "ShebaToInsert", SqlDbType = SqlDbType.VarChar, Value = string.IsNullOrEmpty(ShebaToInsert) ? (object)DBNull.Value : (object)ShebaToInsert ,Direction = ParameterDirection.Input }
            };
            var res = _dbContext.SqlQuery<string>(@"EXECUTE [dbo].[Sp_RequestWalletCashout]  @ChargeWalletIds,@CashoutTypeId,@ShebaToInsert,@UserId", prms).ToList();
            return res.FirstOrDefault();
        }
        public string SaveSettingCashout(string jsonSettings, long UserId)
        {
            SqlParameter[] prms = new SqlParameter[]{
                new SqlParameter() { ParameterName = "JsonSettings", SqlDbType = SqlDbType.NVarChar, Value = string.IsNullOrEmpty(jsonSettings) ? (object)DBNull.Value:jsonSettings ,Direction = ParameterDirection.Input },
                new SqlParameter() { ParameterName = "UserId", SqlDbType = SqlDbType.BigInt, Value = UserId,Direction = ParameterDirection.Input }
            };
            var res = _dbContext.SqlQuery<string>(@"EXECUTE [dbo].[Sp_SaveCashoutSettings]  @JsonSettings,@UserId", prms).ToList();
            return res.FirstOrDefault();
        }
		public T[] execSpNormal<T>(string SpName,object obj)
        {
			string paramNames = "";
			var prms = new List<SqlParameter>();

			foreach (System.Reflection.PropertyInfo pinfo in obj.GetType().GetProperties())
			{
				var val = pinfo.GetValue(obj);
				prms.Add(new SqlParameter()
				{
					ParameterName = pinfo.Name,
					SqlDbType = SqlHelper.GetDbType(pinfo.PropertyType),
					Direction = ParameterDirection.Input,
					Value = (val == null || val == "") ? DBNull.Value: val
				});
				paramNames += $"@{pinfo.Name},";
			}
			if (!string.IsNullOrEmpty(paramNames)) paramNames = paramNames.Substring(0, paramNames.Length - 1);

			var ret = _dbContext.SqlQuery<T>($"EXECUTE [dbo].[{SpName}] {paramNames}", prms.ToArray()).ToList();
			return ret.ToArray();
		}
		public T[] execSpNormal<T>(string SpName, object obj,object objectOut)
		{
			string paramNames = "";
			var prms = new List<SqlParameter>();

			foreach (System.Reflection.PropertyInfo pinfo in obj.GetType().GetProperties())
			{
				var val = pinfo.GetValue(obj);
				prms.Add(new SqlParameter()
				{
					ParameterName = pinfo.Name,
					SqlDbType = SqlHelper.GetDbType(pinfo.PropertyType),
					Direction = ParameterDirection.Input,
					Value = (val == null || val == "") ? DBNull.Value : val
				});

				paramNames += $"@{pinfo.Name},";
			}
			if(objectOut != null)
			foreach (System.Reflection.PropertyInfo pinfo in objectOut.GetType().GetProperties())
			{
				prms.Add(new SqlParameter()
				{
					ParameterName = pinfo.Name,
					SqlDbType = SqlHelper.GetDbType(pinfo.PropertyType),
					Direction = ParameterDirection.Output
				});

				paramNames += $"@{pinfo.Name} OUT,";
			}
			if (!string.IsNullOrEmpty(paramNames)) paramNames = paramNames.Substring(0, paramNames.Length - 1);

			var ret = _dbContext.SqlQuery<T>($"EXECUTE [dbo].[{SpName}] {paramNames}", prms.ToArray()).ToList();

			if (objectOut != null)
			foreach (var p1 in prms.Where(x=>x.Direction == ParameterDirection.Output))
            {
					foreach (System.Reflection.PropertyInfo pinfo in objectOut.GetType().GetProperties())
                    {
						if(pinfo.Name == p1.ParameterName)
							pinfo.SetValue(objectOut,p1.Value);
					}
			}
			return ret.ToArray();
		}
		public T[] execSp<T>(string SpName,object obj)
        {
			string ser = JsonConvert.SerializeObject(obj);
			var p1 = new SqlParameter()
			{
				ParameterName = "JsonOutput",
				SqlDbType = SqlDbType.NVarChar,
				Direction = ParameterDirection.Output,
				Size = -1
			};
			var prms = new SqlParameter[]{
				new SqlParameter()
				{
					ParameterName = "JsonInput",
					SqlDbType = SqlDbType.NVarChar,
					Direction = ParameterDirection.Input,
					Value = (object)(ser),
					Size = -1
				},p1
			};

			var ret = _dbContext.SqlQuery<T>($"EXECUTE [dbo].[{SpName}] @JsonInput,@JsonOutput OUTPUT", prms).ToList();
			return ret.ToArray();
		}

        public T[] execSp<T>(string SpName, object obj, out string jsonout)
        {
			string ser = JsonConvert.SerializeObject(obj);
			var p1 = new SqlParameter()
			{
				ParameterName = "JsonOutput",
				SqlDbType = SqlDbType.NVarChar,
				Direction = ParameterDirection.Output,
				Size = -1
			};
			var prms = new SqlParameter[]{
				new SqlParameter()
				{
					ParameterName = "JsonInput",
					SqlDbType = SqlDbType.NVarChar,
					Value = (object)(ser),
					Size = -1
				},p1
			};

			var ret = _dbContext.SqlQuery<T>($"EXECUTE [dbo].[{SpName}] @JsonInput,@JsonOutput OUTPUT", prms).ToArray();
			jsonout = p1.Value.ToString();
			return ret;
		}
    }

	public static class SqlHelper
	{
		private static Dictionary<Type, SqlDbType> typeMap;

		// Create and populate the dictionary in the static constructor
		static SqlHelper()
		{
			typeMap = new Dictionary<Type, SqlDbType>();

			typeMap[typeof(string)] = SqlDbType.NVarChar;
			typeMap[typeof(char[])] = SqlDbType.NVarChar;
			typeMap[typeof(byte)] = SqlDbType.TinyInt;
			typeMap[typeof(short)] = SqlDbType.SmallInt;
			typeMap[typeof(int)] = SqlDbType.Int;
			typeMap[typeof(long)] = SqlDbType.BigInt;
			typeMap[typeof(byte[])] = SqlDbType.Image;
			typeMap[typeof(bool)] = SqlDbType.Bit;
			typeMap[typeof(DateTime)] = SqlDbType.DateTime2;
			typeMap[typeof(DateTimeOffset)] = SqlDbType.DateTimeOffset;
			typeMap[typeof(decimal)] = SqlDbType.Money;
			typeMap[typeof(float)] = SqlDbType.Real;
			typeMap[typeof(double)] = SqlDbType.Float;
			typeMap[typeof(TimeSpan)] = SqlDbType.Time;
			/* ... and so on ... */
		}

		// Non-generic argument-based method
		public static SqlDbType GetDbType(Type giveType)
		{
			// Allow nullable types to be handled
			giveType = Nullable.GetUnderlyingType(giveType) ?? giveType;

			if (typeMap.ContainsKey(giveType))
			{
				return typeMap[giveType];
			}

			throw new ArgumentException($"{giveType.FullName} is not a supported .NET class");
		}

		// Generic version
		public static SqlDbType GetDbType<T>()
		{
			return GetDbType(typeof(T));
		}
	}
}
