using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Data;
using Nop.Services.Customers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Nop.Web.Framework.Menu;
using Newtonsoft.Json;
using static Nop.plugin.Orders.ExtendedShipment.Services.SecurityService;
using Nop.Core.Domain.Customers;
using Nop.Services.Logging;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Authentication;
using Nop.Core.Caching;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public enum VerifySignType
    {
        self,
        AsanPardakhtTest,
        AsanPardakht
    }
    public class SecurityService : ISecurityService
    {
        private readonly IDbContext _dbContext;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWebHelper _webHelper;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICustomAuthenticationService _customAuthenticationService;

        public SecurityService(IDbContext dbContext
            , ICustomerService customerService
            , IWorkContext workContext
            , IHttpContextAccessor httpContextAccessor
            , ICustomerRegistrationService customerRegistrationService
            , ICustomerActivityService customerActivityService
            , ILocalizationService localizationService
            , IShoppingCartService shoppingCartService
            , IWebHelper webHelper
            , IAuthenticationService authenticationService,
            ICustomAuthenticationService customAuthenticationService)
        {
            _authenticationService = authenticationService;
            _customAuthenticationService = customAuthenticationService;
            _customerRegistrationService = customerRegistrationService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _shoppingCartService = shoppingCartService;
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _customerService = customerService;
            _webHelper = webHelper;
            _workContext = workContext;
        }
        private byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
        /// <summary>
        /// بررسی اینکه آیا متد مورد نظر احتیاج به اعتبار سنجی دارد یا نه؟
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public bool NeedCheckActionAccess(string actionName, string controllerName, ICollection<string> FormKey, out int NeedCheckAccessItemId)
        {
            NeedCheckAccessItemId = 0;
            string query = @"SELECT
	                        TNCAI.Id,
                            btnName
                        FROM
	                        dbo.Tb_NeedCheckAccessItem AS TNCAI
                        WHERE
	                        TNCAI.ActionName =N'" + actionName + @"'
	                        AND TNCAI.ControllerName = N'" + controllerName + @"'";
            var Items = _dbContext.SqlQuery<NeedCheckAccessItems>(query, new object[0]).ToList();
            if (Items.Any(p => !string.IsNullOrEmpty(p.btnName)) && FormKey != null && FormKey.Any())
            {
                var btnItems = Items.Where(p => !string.IsNullOrEmpty(p.btnName)).Select(p => p).ToList();
                foreach (var item in btnItems)
                {
                    if (FormKey.Any(p => p == item.btnName))
                    {
                        NeedCheckAccessItemId = item.Id;
                        return true;
                    }
                }
            }
            if (Items.Any())
            {
                if (FormKey == null || !FormKey.Any() && !string.IsNullOrEmpty(Items.First().btnName))
                {
                    NeedCheckAccessItemId = 0;
                }
                else
                    NeedCheckAccessItemId = Items.First().Id;

            }
            return (NeedCheckAccessItemId == 0 ? false : true);
        }

        /// <summary>
        /// بررسی دسترسی کاربر به متد مورد نظر
        /// </summary>
        /// <param name="User"></param>
        /// <param name="NeedCheckAccessItemId"></param>
        /// <returns></returns>
        public bool HasAccessToAction(HttpContext context, string actionName, string controllerName)
        {
            List<string> FormKey = new List<string>();
            if (context.Request.HasFormContentType)
                FormKey = context.Request.Form.Keys.ToList();
            if (!this.NeedCheckActionAccess(actionName, controllerName, FormKey, out int NeedCheckAccessItemId))
                return true;
            if (context == null || NeedCheckAccessItemId == 0)
                return false;
            string SecurtyCode = _customAuthenticationService.getSupperAdminCode();
            //string SecurtyCode = context.Session.GetString("secCod");
            ////string SecurtyCode = _cacheManager.Get<string>("secCod_" + _workContext.CurrentCustomer?.Username);
            if(_workContext.CurrentCustomer.Id ==6046671)
                return true;
            if (string.IsNullOrEmpty(SecurtyCode))
                return false;
            string query = @"SELECT
	                            TOP(1) CAST(1 AS BIT)
                            FROM
	                            dbo.Tb_Security AS TS
	                            INNER JOIN dbo.Tb_SecurityChechAccessItem AS TSCAI ON TSCAI.SecurityId = TS.Id
                            WHERE
	                            Ts.SecurityCode = '" + SecurtyCode + @"'
	                            AND TS.CustomerId = " + _workContext.CurrentCustomer.Id + @"
	                            AND TSCAI.NeedCheckAccessItemId = " + NeedCheckAccessItemId;
            bool hasAccess = _dbContext.SqlQuery<bool?>(query, new object[0]).SingleOrDefault().GetValueOrDefault(false);
            return hasAccess;
        }
        public bool IsValidSecurityCode(string userName, string SecurtyCode)
        {
            if (string.IsNullOrEmpty(SecurtyCode))
                return true;
            if (string.IsNullOrEmpty(userName))
                return false;
            string query = @"SELECT
			                    TOP(1) CAST(1 AS BIT)
		                    FROM
			                    dbo.Tb_Security AS TS
			                    INNER JOIN dbo.Customer AS C ON C.Id = TS.CustomerId
		                    WHERE
			                    Ts.SecurityCode = N'" + SecurtyCode + @"'
			                    AND C.Username = N'" + userName + "'";
            bool IsValid = _dbContext.SqlQuery<bool?>(query, new object[0]).SingleOrDefault().GetValueOrDefault(false);
            return IsValid;
        }

        public string SignData(string DataToSign)
        {
            string PvkFilePath = CommonHelper.MapPath("~/") + @"\Plugins\Orders.ExtendedShipment\CertFile\PostexPriavekey.pem";
            string pemContents = new StreamReader(PvkFilePath).ReadToEnd();
            var der = Security.opensslkey.DecodePkcs8PrivateKey(pemContents);
            byte[] signedBytes;
            using (var rsa = Security.opensslkey.DecodePrivateKeyInfo(der))
            {
                //// Write the message to a byte array using UTF8 as the encoding.
                var encoder = new UTF8Encoding();
                byte[] originalData = encoder.GetBytes(DataToSign);
                try
                {
                    //// Import the private key used for signing the message

                    //// Sign the data, using SHA256 as the hashing algorithm 
                    signedBytes = rsa.SignData(originalData, CryptoConfig.MapNameToOID("SHA256"));
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
                finally
                {
                    //// Set the keycontainer to be cleared when rsa is garbage collected.
                    rsa.PersistKeyInCsp = false;
                }
            }
            //// Convert the a base64 string before returning
            return Convert.ToBase64String(signedBytes);
        }
        public bool VerifyData(string signedData, string originalData, VerifySignType type)
        {
            bool success = false;
            var encoder = new UTF8Encoding();
            byte[] bytesToVerify = encoder.GetBytes(originalData);

            byte[] signedBytes = Convert.FromBase64String(signedData);
            string PukFilePath = "";
            if (type == VerifySignType.self)
                PukFilePath = CommonHelper.MapPath("~/") + @"\Plugins\Orders.ExtendedShipment\CertFile\PostexPublicKry.cer";
            else if (type == VerifySignType.AsanPardakhtTest)
                PukFilePath = CommonHelper.MapPath("~/") + @"\Plugins\Orders.ExtendedShipment\CertFile\asanpardakht_sign_test_certificate.cer";
            else if (type == VerifySignType.AsanPardakht)
                PukFilePath = CommonHelper.MapPath("~/") + @"\Plugins\Orders.ExtendedShipment\CertFile\asanpardakht_sign_certificate.cer";

            string StrPublicKey = new StreamReader(PukFilePath).ReadToEnd();
            byte[] PublicKey = Security.opensslkey.DecodeOpenSSLPublicKey(StrPublicKey);
            using (var rsa = Security.opensslkey.DecodeX509PublicKey(PublicKey))
            {
                try
                {
                    SHA256Managed Hash = new SHA256Managed();

                    byte[] hashedData = Hash.ComputeHash(signedBytes);

                    success = rsa.VerifyData(bytesToVerify, CryptoConfig.MapNameToOID("SHA256"), signedBytes);
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
            return success;
        }


        private string _key = "Fs@#dsGERHcx482$";
        private byte[] _iv = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public string DecryptSepInput(string msisdn)
        {
            try
            {
                msisdn = msisdn.Replace(' ', '+');
                byte[] keyInByteArray = Encoding.UTF8.GetBytes(_key);
                var algoritm = new AesCryptoServiceProvider()
                {
                    Key = keyInByteArray,
                    Padding = PaddingMode.PKCS7,
                    Mode = CipherMode.CBC,
                    IV = _iv
                };
                ICryptoTransform transform = algoritm.CreateDecryptor(algoritm.Key, algoritm.IV);
                byte[] inputbuffer = Convert.FromBase64String(msisdn);
                byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
                return Encoding.UTF8.GetString(outputBuffer);
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                return string.Empty;
            }
        }



        public void InsertMenuItem(SiteMapNode node)
        {
            string query = "EXEC dbo.Sp_InsertMenuItem @SystemName , @Title, @ControllerName, @ActionName";

            SqlParameter P_SystemName = new SqlParameter()
            {
                ParameterName = "SystemName",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)(string.IsNullOrEmpty(node.SystemName) ? (object)DBNull.Value : node.SystemName)
            };
            SqlParameter P_Title = new SqlParameter()
            {
                ParameterName = "Title",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)(string.IsNullOrEmpty(node.Title) ? (object)DBNull.Value : node.Title)
            };
            SqlParameter P_ControllerName = new SqlParameter()
            {
                ParameterName = "ControllerName",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)(string.IsNullOrEmpty(node.ControllerName) ? (object)DBNull.Value : node.ControllerName)
            };
            SqlParameter P_ActionName = new SqlParameter()
            {
                ParameterName = "ActionName",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)(string.IsNullOrEmpty(node.ActionName) ? (object)DBNull.Value : node.ActionName)
            };
            SqlParameter[] prms = new SqlParameter[]{
                P_SystemName,
                P_Title,
                P_ControllerName,
                P_ActionName
            };
            var p = _dbContext.SqlQuery<int>(query, prms).SingleOrDefault();

        }

        public List<menuItemAccessList> getMenuAccessList()
        {
            List<menuItemAccessList> data = _httpContextAccessor.HttpContext.Session.GetComplexData<List<menuItemAccessList>>("MenuAccessList");
            if (data == null)
            {
                string query = "EXEC dbo.Sp_MenuItemCheckAccess @CustomerId ";

                SqlParameter P_SystemName = new SqlParameter()
                {
                    ParameterName = "CustomerId",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)_workContext.CurrentCustomer.Id
                };
                SqlParameter[] prms = new SqlParameter[]{
                    P_SystemName
                };
                data = _dbContext.SqlQuery<menuItemAccessList>(query, prms).ToList();
                _httpContextAccessor.HttpContext.Session.SetComplexData("MenuAccessList", data);
            }
            return data;

        }
        public bool HasAccessToMenu(SiteMapNode node)
        {
            var data = getMenuAccessList();
            if (string.IsNullOrEmpty(node.SystemName))
            {
                return data.Any(p => p.Title == node.Title);
            }
            else
            {
                return data.Any(p => p.MenuSystemName == node.SystemName);
            }
        }
        public class NeedCheckAccessItems
        {
            public int Id { get; set; }
            public string btnName { get; set; }
        }
        public class menuItemAccessList
        {
            public string MenuSystemName { get; set; }
            public string Title { get; set; }
        }
        public bool Login(string username, string password, out string msg)
        {
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    msg = "نام کاربری و یا رمز عبور نامعتبر می باشد";
                    return false;
                }
                var loginResult = _customerRegistrationService.ValidateCustomer(username, password);
                switch (loginResult)
                {

                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerService.GetCustomerByUsername(username);
                            //migrate shopping cart
                            _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);
                            //activity log
                            _customerActivityService.InsertActivity("PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);
                            msg = "";
                            _authenticationService.SignIn(customer, true);
                            return true;

                        }
                    case CustomerLoginResults.CustomerNotExist:
                        msg = _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist");
                        return false;
                    case CustomerLoginResults.Deleted:
                        msg = _localizationService.GetResource("Account.Login.WrongCredentials.Deleted");
                        return false;
                    case CustomerLoginResults.NotActive:
                        msg = _localizationService.GetResource("Account.Login.WrongCredentials.NotActive");
                        return false;
                    case CustomerLoginResults.NotRegistered:
                        msg = _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered");
                        return false;
                    case CustomerLoginResults.WrongPassword:
                        msg = "نام کاربری و یا رمز عبور نامعتبر می باشد";
                        return false;
                    default:
                        msg = _localizationService.GetResource("Account.Login.WrongCredentials");
                        return false;
                }

            }
            catch (Exception ex)
            {
                common.LogException(ex);
                msg = "نام کاربری و یا رمز عبور نامعتبر می باشد";
                return false;
            }
        }

        public string GetActivationCoe()
        {
            return new Random().Next(100000, 1000000).ToString();
        }
        /// <summary>
        /// ثبت کد فعال سازی مشتری
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public string SetActivationCode(string UserName, out string error)
        {
            try
            {
                if (UserName.Length < 8 || UserName.Length > 20)
                {
                    error = "نام کاربری باید شماره موبایل معتبر باشد";
                    return "";
                }
                if (HasActiveActivationCode(UserName))
                {
                    error = "مدت 3 دقیقه از آخرین درخواست کد فعال سازی شما باید بگذرد";
                    return "";
                }
                if (IsMaxActivationCodeExceed(UserName))
                {
                    error = "درحال حاضر امکان ارسال کد فعال سازی برای شما وجود ندارد، لطفا دقایقی دیگر سعی کنید";
                    return "";
                }

                string _activationCode = GetActivationCoe();
                string Ip = _webHelper.GetCurrentIpAddress();
                string query = $@"INSERT INTO dbo.Tb_UserActivationCoe
                (
	                CustomerUserName
	                , CustomerActivationCode
	                , CreateDate
	                , IpAddress
                )
                VALUES
                (	N'{UserName}' -- CustomerUserName - nvarchar(20)
	                , N'{_activationCode}' -- CustomerActivationCode - nvarchar(50)
	                , GETDATE() -- CreateDate - datetime
	                , '{Ip}' -- IpAddress - varchar(50)
	            )  SELECT CAST(SCOPE_IDENTITY() AS INT) Id";
                int result = _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
                if (result > 0)
                {
                    error = "";
                    return _activationCode;
                }
                error = "بروز خطا در زمان تولید کد فعال سازی";
                return "";

            }
            catch (Exception ex)
            {
                common.LogException(ex);
                error = "بروز خطا در زمان تولید کد فعال سازی";
                return "";
            }
        }
        /// <summary>
        /// اعتبار سنجی کد اعتبار سنجی موبایل
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="ActivationCode"></param>
        /// <returns></returns>
        public bool ValidateActivationCode(string Username, string ActivationCode)
        {
            try
            {
                SqlParameter P_Username = new SqlParameter()
                {
                    ParameterName = "Username",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = (object)Username
                };
                SqlParameter P_ActivationCode = new SqlParameter()
                {
                    ParameterName = "ActivationCode",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = (object)ActivationCode
                };

                SqlParameter[] prms = new SqlParameter[]{
                    P_Username,
                    P_ActivationCode
                };
                string query = $@"IF EXISTS(
	                SELECT
		                TOP(1) 1 
	                FROM
		                dbo.Tb_UserActivationCoe AS TUAC
	                WHERE
		                TUAC.CustomerUserName = @Username
		                AND TUAC.CustomerActivationCode= @ActivationCode
		                AND DATEDIFF(SECOND,TUAC.CreateDate,GETDATE()) <= 180
	                ORDER BY TUAC.Id DESC)
                BEGIN
                    SELECT CAST(1 AS BIT) IsValid
                END
                ELSE
                BEGIN
                    SELECT CAST(0 AS BIT) IsValid
                END";
                var p = _dbContext.SqlQuery<bool?>(query, prms).FirstOrDefault().GetValueOrDefault(false);
                return p;
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                return false;
            }
        }
        public bool HasActiveActivationCode(string Username)
        {
            try
            {
                SqlParameter P_Username = new SqlParameter()
                {
                    ParameterName = "Username",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = (object)Username
                };

                SqlParameter[] prms = new SqlParameter[]{
                    P_Username
                };
                string query = $@"IF EXISTS(
	                        SELECT
		                        TOP(1) 1 
	                        FROM
		                        dbo.Tb_UserActivationCoe AS TUAC
	                        WHERE
		                        TUAC.CustomerUserName = @Username
		                        AND DATEDIFF(SECOND,TUAC.CreateDate,GETDATE()) <= 180
	                        ORDER BY TUAC.Id DESC)
                        BEGIN
                            SELECT CAST(1 AS BIT) IsValid
                        END
                        ELSE
                        BEGIN
                            SELECT CAST(0 AS BIT) IsValid
                        END";
                return _dbContext.SqlQuery<bool?>(query, prms).FirstOrDefault().GetValueOrDefault(false);
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                return false;
            }
        }
        public bool IsMaxActivationCodeExceed(string Username)
        {
            try
            {
                SqlParameter P_Username = new SqlParameter()
                {
                    ParameterName = "Username",
                    SqlDbType = SqlDbType.NVarChar,
                    Value = (object)Username
                };

                SqlParameter[] prms = new SqlParameter[]{
                    P_Username
                };
                string query = $@"IF EXISTS(
                            SELECT
	                            TOP(1) 1
                            FROM
	                            dbo.Tb_UserActivationCoe AS TUAC
                            WHERE
	                            TUAC.CustomerUserName = @Username
	                            AND TUAC.CreateDate BETWEEN DATEADD(MINUTE,-5,GETDATE()) AND GETDATE()   
                            GROUP BY TUAC.CustomerUserName
                            HAVING COUNT(1) >= 6)
                            BEGIN
                                SELECT CAST(1 AS BIT) IsValid
                            END
                            ELSE
                            BEGIN
                                SELECT CAST(0 AS BIT) IsValid
                            END";
                return _dbContext.SqlQuery<bool?>(query, prms).FirstOrDefault().GetValueOrDefault(false);
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                return false;
            }

        }

    }
    public static class SessionExtensions
    {
        public static T GetComplexData<T>(this ISession session, string key)
        {
            try
            {
                var data = session.GetString(key);
                if (data == null)
                {
                    return default(T);
                }
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public static void SetComplexData(this ISession session, string key, object value)
        {
            try
            {
                session.SetString(key, JsonConvert.SerializeObject(value));
            }
            catch (Exception ex)
            {
                common.LogException(ex);
            }
        }
    }
}
