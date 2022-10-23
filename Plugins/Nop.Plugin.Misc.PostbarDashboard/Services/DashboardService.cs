using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Plugin.Misc.PostbarDashboard.Models;
using Nop.Plugin.Misc.ShippingSolutions.Services;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IRepository<Ticket.Domain.Tbl_RequestCODCustomer> _repositoryTbl_RequestCODCustomer;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService;
        private readonly IDbContext _dbContext;
        private readonly IWorkContext _workContext;
        private readonly IRewardPointService _rewardPointService;
        private readonly IEncryptionService _encryptionService;
        private readonly ILocalizationService _localizationService;
        private readonly CustomerSettings _customerSettings;
        private readonly IEventPublisher _eventPublisher;

        public DashboardService(IRepository<Ticket.Domain.Tbl_RequestCODCustomer> repositoryTbl_RequestCODCustomer,
            IDbContext dbContext,
            IWorkContext workContext,
            IRewardPointService rewardPointService,
            IStoreContext storeContext,
            IEncryptionService encryptionService,
            ILocalizationService localizationService,
            CustomerSettings customerSettings,
            IEventPublisher eventPublisher,
            ICustomerService customerService)
        {
            _repositoryTbl_RequestCODCustomer = repositoryTbl_RequestCODCustomer;
            _storeContext = storeContext;
            _dbContext = dbContext;
            this._workContext = workContext;
            this._rewardPointService = rewardPointService;
            _encryptionService = encryptionService;
            _localizationService = localizationService;
            _customerSettings = customerSettings;
            _eventPublisher = eventPublisher;
            _customerService = customerService;
        }
        public int InsertRequestCOD(AddRequestCODModel param)
        {
            try
            {
                #region  new RequestCOD
                var customer = _customerService.GetCustomerById(param.CustomerId);
                Ticket.Domain.Tbl_RequestCODCustomer newRequestCOD = new Ticket.Domain.Tbl_RequestCODCustomer();
                newRequestCOD.Fname = param.Fname;
                newRequestCOD.Lname = param.Lname;
                newRequestCOD.NatinolCode = param.NationalCode;
                newRequestCOD.Shaba = param.AccountIBAN;
                newRequestCOD.Address = param.Address;

                newRequestCOD.IdCustomer = param.CustomerId;
                newRequestCOD.IsActive = true;
                newRequestCOD.StoreId = _storeContext.CurrentStore.Id;
                newRequestCOD.DateInsert = DateTime.Now;
                newRequestCOD.Status = 0;
                newRequestCOD.Username = customer.Username;
                newRequestCOD.CodePosti = param.PostalCode;
                _repositoryTbl_RequestCODCustomer.Insert(newRequestCOD);
                return newRequestCOD.Id;
                #endregion
            }
            catch (Exception ex)
            {
                Common.LogException(ex);
                return 0;
            }
        }
        public void UpdateRequestCODFileName(int requestId, string FileName)
        {
            var request = _repositoryTbl_RequestCODCustomer.Table.Single(p => p.Id == requestId);
            request.UrlFile = FileName;
            _repositoryTbl_RequestCODCustomer.Update(request);
        }


        public DashboardInfoResult GetCustomerInfo()
        {
            var ret = execSp<DashboardInfoResult>(new { Operation = "GetInfo", CustomerId = _workContext.CurrentCustomer.Id }).FirstOrDefault();
            if(ret == null)
                ret = new DashboardInfoResult();
            ret.Balance = _rewardPointService.GetRewardPointsBalance(
                customerId: _workContext.CurrentCustomer.Id,
                storeId: _storeContext.CurrentStore.Id);
            return ret;
        }
        public T[] execSp<T>(string procedureName, object obj)
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

            var ret = _dbContext.SqlQuery<T>($"EXECUTE [dbo].[{procedureName}] @JsonInput,@JsonOutput OUTPUT", prms).ToList();
            return ret.ToArray();
        }
        public T[] execSp<T>(object obj)
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

            var ret = _dbContext.SqlQuery<T>("EXECUTE [dbo].[MyDashboard_Sp_Operations] @JsonInput,@JsonOutput OUTPUT", prms).ToList();
            return ret.ToArray();
        }
        public T[] execSp<T>(object obj,out string jsonout)
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

            var ret = _dbContext.SqlQuery<T>("EXECUTE [dbo].[MyDashboard_Sp_Operations] @JsonInput,@JsonOutput OUTPUT", prms).ToArray();
            jsonout = p1.Value.ToString();
            return ret;
        }

        public ChangePasswordResult ChangePassword(ChangePasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var result = new ChangePasswordResult();
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.EmailIsNotProvided"));
                return result;
            }
            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.PasswordIsNotProvided"));
                return result;
            }

            var customer = _customerService.GetCustomerById(Int32.Parse(request.Email));
            if (customer == null)
            {
                result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.UserNotFound"));
                return result;
            }

            if (request.ValidateRequest)
            {
                //request isn't valid
                if (!PasswordsMatch(_customerService.GetCurrentPassword(customer.Id), request.OldPassword))
                {
                    result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.OldPasswordDoesntMatch"));
                    return result;
                }
            }

            //check for duplicates
            if (_customerSettings.UnduplicatedPasswordsNumber > 0)
            {
                //get some of previous passwords
                var previousPasswords = _customerService.GetCustomerPasswords(customer.Id, passwordsToReturn: _customerSettings.UnduplicatedPasswordsNumber);

                var newPasswordMatchesWithPrevious = previousPasswords.Any(password => PasswordsMatch(password, request.NewPassword));
                if (newPasswordMatchesWithPrevious)
                {
                    result.AddError(_localizationService.GetResource("Account.ChangePassword.Errors.PasswordMatchesWithPrevious"));
                    return result;
                }
            }

            //at this point request is valid
            var customerPassword = new CustomerPassword
            {
                Customer = customer,
                PasswordFormat = request.NewPasswordFormat,
                CreatedOnUtc = DateTime.UtcNow
            };
            switch (request.NewPasswordFormat)
            {
                case PasswordFormat.Clear:
                    customerPassword.Password = request.NewPassword;
                    break;
                case PasswordFormat.Encrypted:
                    customerPassword.Password = _encryptionService.EncryptText(request.NewPassword);
                    break;
                case PasswordFormat.Hashed:
                    {
                        var saltKey = _encryptionService.CreateSaltKey(5);
                        customerPassword.PasswordSalt = saltKey;
                        customerPassword.Password = _encryptionService.CreatePasswordHash(request.NewPassword, saltKey, _customerSettings.HashedPasswordFormat);
                    }
                    break;
            }
            _customerService.InsertCustomerPassword(customerPassword);

            //publish event
            _eventPublisher.Publish(new CustomerPasswordChangedEvent(customerPassword));

            return result;
        }

        protected bool PasswordsMatch(CustomerPassword customerPassword, string enteredPassword)
        {
            if (customerPassword == null || string.IsNullOrEmpty(enteredPassword))
                return false;

            var savedPassword = string.Empty;
            switch (customerPassword.PasswordFormat)
            {
                case PasswordFormat.Clear:
                    savedPassword = enteredPassword;
                    break;
                case PasswordFormat.Encrypted:
                    savedPassword = _encryptionService.EncryptText(enteredPassword);
                    break;
                case PasswordFormat.Hashed:
                    savedPassword = _encryptionService.CreatePasswordHash(enteredPassword, customerPassword.PasswordSalt, _customerSettings.HashedPasswordFormat);
                    break;
            }

            if (customerPassword.Password == null)
                return false;

            return customerPassword.Password.Equals(savedPassword);
        }
    }
}
