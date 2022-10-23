using BS.Plugin.Orders.ExtendedShipment.Infrastructure;
using BS.Plugin.Orders.ExtendedShipment.Models.Optime.Authenticate;
using BS.Plugin.Orders.ExtendedShipment.Models.Optime.GetToolResponseCluster;
using BS.Plugin.Orders.ExtendedShipment.Models.Optime.ToolApiCall;
using Newtonsoft.Json;
using Nop.Core.Data;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.plugin.Orders.ExtendedShipment.Models.Optime.ExecuteTask;
using Nop.plugin.Orders.ExtendedShipment.Models.Optime.ToolApiCall;
using Nop.plugin.Orders.ExtendedShipment.Services;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BS.Plugin.Orders.ExtendedShipment.Services
{
    public class OptimeApiService : IOptimeApiService
    {
        private readonly OptimeSetting _optimeSetting;
        private readonly RestClient _restClient;
        private readonly IDbContext _dbContext;
        private readonly IRepository<Tbl_OptimeListDetailes> _repositoryOptimeListDetailes;
        private readonly IRepository<Tbl_OpTimeList> _repositoryTbl_OpTimeList;
        private readonly IRepository<Tbl_CollectorOptimeUser> _repository_CollectorUser;
        private string token = "";

        public OptimeApiService(OptimeSetting optimeSetting, IDbContext dbContext
            , IRepository<Tbl_OptimeListDetailes> repositoryOptimeListDetailes
            , IRepository<Tbl_OpTimeList> repositoryTbl_OpTimeList
            , IRepository<Tbl_CollectorOptimeUser> repository_collectorUser
            )
        {
            _optimeSetting = optimeSetting;
            _dbContext = dbContext;
            _repositoryTbl_OpTimeList = repositoryTbl_OpTimeList;
            _repository_CollectorUser = repository_collectorUser;
            _repositoryOptimeListDetailes = repositoryOptimeListDetailes;
            _restClient = new RestClient(_optimeSetting.BaseUrl);
        }

        public IRestRequest ConfigRequest(string resource, Method method, object parameter)
        {
            var request = new RestRequest(resource, method);
            request.JsonSerializer = new NewtonSoftSerializer();
            if (!string.IsNullOrEmpty(token))
            {
                AddHeadertoRequest(request);
            }
            if (parameter != null)
            {
                request.AddJsonBody(parameter);
            }
            return request;
        }

        private void AddHeadertoRequest(IRestRequest request)
        {
            request.AddHeader("Authorization", "Bearer " + token);
        }

        public IRestResponse Execute(IRestRequest request)
        {
            foreach (var oldAuthHeader in request.Parameters.Where(p => p.Name.Equals("Authorization", StringComparison.OrdinalIgnoreCase)).ToArray())
            {
                request.Parameters.Remove(oldAuthHeader);
            }
            AddHeadertoRequest(request);
            return _restClient.Execute(request);
        }

        public bool Authenticate(string userName, string password)
        {
            var user = new OptimeUserModel()
            {
                Password = password,
                UserName = userName,
                RememberMe = true
            };
            var request = ConfigRequest("/auth/signin", Method.POST, user);
            var response = Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var authResponse = JsonConvert.DeserializeObject<AuthenticationResultModel>(response.Content);
                token = authResponse.AccessToken;
                return true;
            }
            return false;
        }

        public CallResponseModel NewFakePlanForPhoneOrder(int collectorCustomerId, CallRequestModel model, int phoneOrderId)
        {
            Authenticate(collectorCustomerId);
            var request = ConfigRequest("planning/newplan", Method.POST, model);
            var response = Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseObj = JsonConvert.DeserializeObject<CallResponseModel>(response.Content);
                if (responseObj != null)
                {
                    int ListId = InsertCollectorAgentList(null, responseObj.Token, collectorCustomerId, true, phoneOrderId);
                    if (ExecuteSelectedTask(responseObj.Token))
                    {
                        SetListStatus(ListId, 1);//send for Optimized
                        return JsonConvert.DeserializeObject<CallResponseModel>(response.Content);
                    }
                    else
                    {
                        SetListStatus(ListId, 3);//Optimize Filed
                    }
                }
            }
            return null;
        }


        public CallResponseModel NewPlan(CallRequestModel model)
        {
            var request = ConfigRequest("planning/newplan", Method.POST, model);
            var response = Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<CallResponseModel>(response.Content);
            }
            return null;
        }

        public bool ExecuteSelectedTask(string planIdOrToken)
        {
            var request = ConfigRequest("/planservice/executetool", Method.POST, new { planIdOrToken });
            var response = Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        public bool Delete(int collectorCustomerId, string planIdOrToken)
        {
            Authenticate(collectorCustomerId);
            var request = ConfigRequest("/Planning/Delete", Method.DELETE, null);
            request.AddQueryParameter("IdOrToken", planIdOrToken);
            var response = Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        public ResponseModel GetToolResponseCluster(string token)
        {
            var request = ConfigRequest("planservice/GetExternal", Method.GET, null);
            request.AddParameter("Id", token);

            var response = Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<ResponseModel>(response.Content);
            }
            return null;
        }

        /// <summary>
        /// دریافت لیست مرسولاتی که جمع آور باید امروز جمع کند
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public CallRequestModel getCollectorAgentList(int CustomerId)
        {
            string query = $@"EXEC dbo.Sp_AgentNavyDetail @CustomerId";

            SqlParameter P_serviceId = new SqlParameter()
            {
                ParameterName = "CustomerId",
                SqlDbType = SqlDbType.Int,
                Value = (object)CustomerId
            };

            SqlParameter[] prms = new SqlParameter[]{
                P_serviceId
            };
            var data = _dbContext.SqlQuery<SpCallRequestModel>(query, prms).ToList();

            if (data == null || data.Count == 0)
                return null;

            var firstRow = data.FirstOrDefault();
            CallRequestModel requestModel = new CallRequestModel()
            {
                Description = firstRow.description,
                FileExtention = firstRow.fileExtension,
                Owner = firstRow.owner,
                PlanName = firstRow.planName,
                ToolName = firstRow.toolName,
                PlanConfigDto = new PlanConfigDto()
                {
                    Config = new List<Config>(),
                    Option = new List<Option>()
                }
            };

            requestModel.PlanConfigDto.Option.Add(new Option()
            {
                DriverMustReturnToWareHouse = firstRow.driverComeBack,
                DriverOptimization = false,
                ShiftsCode = firstRow.shiftsCode,
                UseGeoCode = false
            });

            data.ToList().ForEach(y =>
                {
                    requestModel.PlanConfigDto.Config.Add(new Config()
                    {
                        Name = y.carName,
                        Out = "1",
                        Type = y.VehicleTypeEnum == 1 || y.VehicleTypeEnum == 2 ? "car" : y.VehicleTypeEnum == 0 ? "bike" : "truck",
                        Volume = y.CapacityVolume,
                        Weight = y.CapacityWeight,
                        Zone = "0"
                    });
                });

            var fileContents = GetCollectorAgentTodayList(CustomerId);
            if (fileContents != null && fileContents.Count > 0)
            {
                requestModel.FileContent = fileContents.Select(p => new FileContent()
                {
                    Id = p.Id.ToString(),
                    Address = p.Address,
                    CustomerName = p.CustomerName,
                    CustomerPhoneNumber = p.CustomerPhoneNumber,
                    CustomerTimeWindow = p.CustomerTimeWindow,
                    District = p.District,
                    Latitude = p.Latitude.ToString(),
                    Longitude = p.Longitude.ToString(),
                    MissionType = p.MissionType,
                    //ServiceTime = p.ser
                    Vehicle = p.Vehicle,
                    Volume = p.Volume,
                    Weight = p.Weight
                }).ToList();
            }
            else
                return null;
            return requestModel;
        }
        private List<SpFileContent> GetCollectorAgentTodayList(int CustomerId)
        {
            string query = $@"EXEC dbo.Sp_GetCollectorAgentTodayList @CustomerId";

            SqlParameter P_serviceId = new SqlParameter()
            {
                ParameterName = "CustomerId",
                SqlDbType = SqlDbType.Int,
                Value = (object)CustomerId
            };

            SqlParameter[] prms = new SqlParameter[]{
                P_serviceId
            };
            return _dbContext.SqlQuery<SpFileContent>(query, prms).ToList();
        }
        /// <summary>
        /// ذخیره لیست مرسولات جمع آور
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Token"></param>
        /// <param name="IsUpload"></param>
        /// <param name="UploadId"></param>
        /// <param name="CollectorCustomerId"></param>
        /// <returns></returns>
        public int InsertCollectorAgentList(CallRequestModel model, string Token, int CollectorCustomerId, bool isFake = false, int? phoneOrderId = null)
        {
            Tbl_OpTimeList _OpTimeList = new Tbl_OpTimeList();
            _OpTimeList.CollectorCustomerId = CollectorCustomerId;
            _OpTimeList.CreateDate = DateTime.Now.Date;
            _OpTimeList.Token = Token;
            _OpTimeList.IsFake = isFake;
            _OpTimeList.PhoneOrderId = phoneOrderId;
            _OpTimeList.ListJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(model);
            _repositoryTbl_OpTimeList.Insert(_OpTimeList);
            return _OpTimeList.Id;
        }
        private void SetListStatus(int ListId, int StatusId)
        {
            var data = _repositoryTbl_OpTimeList.Table.Where(p => p.Id == ListId).FirstOrDefault();
            data._Status = StatusId;
            _repositoryTbl_OpTimeList.Update(data);
        }
        /// <summary>
        /// ارسال جهت بهینه سازی مسیر ها
        /// </summary>
        public void SendForOptimizeRout()
        {
            try
            {
                string query = $@"EXEC dbo.Sp_GetCollectorAgentToday";
                var CollectorList = _dbContext.SqlQuery<int>(query, new object[0]).ToList();
                foreach (var item in CollectorList)
                {
                    if (!Authenticate(item))
                    {
                        Log($"عدم شناسایی نماینده {item} در سامانه آپ تایم", "");
                        continue;
                    }
                    var CollectorAgentShipmentList = getCollectorAgentList(item);
                    if (CollectorAgentShipmentList == null)
                        continue;
                    var OpTimeResult = NewPlan(CollectorAgentShipmentList);
                    if (OpTimeResult != null)
                    {
                        int ListId = InsertCollectorAgentList(CollectorAgentShipmentList, OpTimeResult.Token, item);
                        if (ExecuteSelectedTask(OpTimeResult.Token))
                        {
                            SetListStatus(ListId, 1);//send for Optimized
                        }
                        else
                        {
                            SetListStatus(ListId, 3);//Optimize Filed
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        public void CheckForOptimizedTask()
        {
            try
            {
                var dt = DateTime.Now.Date;
                var lists = _repositoryTbl_OpTimeList.Table.Where(p => (p._Status == null || p._Status == 1) && p.CreateDate == dt).ToList();
                foreach (var item in lists)
                {
                    Authenticate(item.CollectorCustomerId);
                    var list = Newtonsoft.Json.JsonConvert.DeserializeObject<CallRequestModel>(item.ListJsonString);
                    var result = GetToolResponseCluster(item.Token);
                    if (result != null)
                    {
                        SetListStatus(item.Id, 2);
                    }
                    item.ResponceJsonString = JsonConvert.SerializeObject(result);
                    _repositoryTbl_OpTimeList.Update(item);
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        public bool Authenticate(int collectorCustomerId)
        {
            try
            {
                string query=$@"SELECT
	                           top(1) TCOU.*
                            FROM
	                            dbo.Tbl_AgentNearpostNode AS TANN
	                            INNER JOIN dbo.StateProvince AS SP ON SP.Id = TANN.NearSateId
	                            INNER JOIN dbo.Tbl_CollectorOptimeUser AS TCOU ON SP.CountryId = TCOU.CountryId
                            WHERE
	                            TANN.AgentCustomerId = {collectorCustomerId}";
                var collectorUser = _dbContext.SqlQuery<Tbl_CollectorOptimeUser>(query,new object[0]).FirstOrDefault();
                if (collectorUser != null)
                {
                  var result= Authenticate(collectorUser.UserName, collectorUser.Password);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogException(ex);
                return false;
            }
        }

        //public void CheckInProcesingList()
        //{
        //    try
        //    {
        //        //string query = $@"SELECT
        //        // TOTL.UploadId
        //        //FROM
        //        // dbo.Tbl_OpTimeList AS TOTL
        //        //WHERE
        //        // CAST(TOTL.CreateDate AS DATE) = CAST(GETDATE() AS DATE)
        //        // AND TOTL._Status = 1";
        //        //var data = _dbContext.SqlQuery<string>(query, new object[0]).ToList();
        //        //foreach (var item in data)
        //        //{
        //            CheckForOptimizedTask();
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        LogException(ex);
        //    }
        //}
        public void LogException(Exception ex)
        {
            DataSettingsManager p = new DataSettingsManager();
            var dataSettings = p.LoadSettings();
            string connectionString = dataSettings.DataConnectionString;
            var fullMessage = ex?.ToString() ?? string.Empty;

            // define INSERT query with parameters
            string query = $@"INSERT INTO dbo.Log
		                        (
			                        LogLevelId
			                        , ShortMessage
			                        , FullMessage
			                        , IpAddress
			                        , CustomerId
			                        , PageUrl
			                        , ReferrerUrl
			                        , CreatedOnUtc
		                        )
		                        VALUES
		                        (	40 -- LogLevelId - int
			                        , N'{ex.Message.Replace("'", "''")}' -- ShortMessage - nvarchar(max)
			                        , N'{fullMessage.Replace("'", "''")}' -- FullMessage - nvarchar(max)
			                        , N'127.0.0.1' -- IpAddress - nvarchar(200)
			                        , NULL -- CustomerId - int
			                        , N'خطا های ثبت شده در زمان ارتباط با سرویس های همکار' -- PageUrl - nvarchar(max)
			                        , N'' -- ReferrerUrl - nvarchar(max)
			                        , GETDATE() -- CreatedOnUtc - datetime
			                        )";

            // create connection and command
            using (SqlConnection cn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }
        public void Log(string shortMessage, string fullMessage)
        {
            DataSettingsManager p = new DataSettingsManager();
            var dataSettings = p.LoadSettings();
            string connectionString = dataSettings.DataConnectionString;


            // define INSERT query with parameters
            string query = $@"INSERT INTO dbo.Log
		                        (
			                        LogLevelId
			                        , ShortMessage
			                        , FullMessage
			                        , IpAddress
			                        , CustomerId
			                        , PageUrl
			                        , ReferrerUrl
			                        , CreatedOnUtc
		                        )
		                        VALUES
		                        (	40 -- LogLevelId - int
			                        , N'{shortMessage.Replace("'", "''")}' -- ShortMessage - nvarchar(max)
			                        , N'{fullMessage.Replace("'", "''")}' -- FullMessage - nvarchar(max)
			                        , N'127.0.0.1' -- IpAddress - nvarchar(200)
			                        , NULL -- CustomerId - int
			                        , N'خطا های ثبت شده در زمان ارتباط با سرویس های همکار' -- PageUrl - nvarchar(max)
			                        , N'' -- ReferrerUrl - nvarchar(max)
			                        , GETDATE() -- CreatedOnUtc - datetime
			                        )";

            // create connection and command
            using (SqlConnection cn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }
    }
}
