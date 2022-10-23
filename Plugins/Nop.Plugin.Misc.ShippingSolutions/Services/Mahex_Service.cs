using Nop.Services.Logging;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces;
using Nop.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Mahex;
using Nop.Data;
using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Mahex;

namespace Nop.Plugin.Misc.ShippingSolutions.Services
{
    /// <summary>
    /// سرویس ماهکس ورژن 1400/1/27 
    /// </summary>
    public class Mahex_Service : IMahex_Service
    {
        private readonly ILogger _logger;
        private readonly ShippingSettings _ShippingSettings;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly ISettingService _settingService;

        public Mahex_Service(ILogger logger
            ,IDbContext dbContext
            , IWorkContext workContext
            , ShippingSettings ShippingSettings
            , ISettingService settingService)
        {
            this._logger = logger;
            this._workContext = workContext;
            this._settingService = settingService;
            _dbContext = dbContext;
            this._ShippingSettings = Configuration();
        }

        /// <summary>
        /// خواندن تنظیمات فروشگاه دیفالت
        /// </summary>
        /// <returns></returns>
        private ShippingSettings Configuration()
        {

            var _storeContext = EngineContext.Current.Resolve<IStoreContext>();
            var Settings = _settingService.LoadSetting<ShippingSettings>(_storeContext.CurrentStore.Id);
            return Settings;
        }

        //public string GetMahexPartNumber(int orderId,int shipmentId)
        //{
        //    string number= orderId.ToString()+shipmentId.ToString();
        //    foreach (var item in number)
        //    {

        //    }
        //}
        /// <returns></returns>
        public List<Result_Mahex_GetCities> GetState()
        {
           return _dbContext.SqlQuery<Result_Mahex_GetCities>(@"SELECT 
		            SP.Name,
		            TMSC.Code 
	            FROM
		            dbo.StateProvince AS SP
		            INNER JOIN dbo.Tb_MahexStateCode AS TMSC ON TMSC.StateProvinceId = SP.Id
	            WHERE
		            SP.Published = 1").ToList() ;
        }
        public Result_Mahex_GetCities GetCityCode(int StateId)
        {
            return _dbContext.SqlQuery<Result_Mahex_GetCities>($@"SELECT 
		            SP.Name,
		            TMSC.Code 
	            FROM
		            dbo.StateProvince AS SP
		            INNER JOIN dbo.Tb_MahexStateCode AS TMSC ON TMSC.StateProvinceId = SP.Id
	            WHERE
		            SP.Published = 1
                AND SP.Id = {StateId}").FirstOrDefault();
        }


        public async Task<Result_Mahex_GetQuote> GetQuote(Params_Mahex_GetPrices param)
        {
            Result_Mahex_GetQuote Result = new Result_Mahex_GetQuote();
            try
            {
                #region Check Param
                //var IsValid = param.IsValidParamsGetQuote();
                //if (IsValid.Item1 == false)
                //{
                //    Result.status.state = "failed";
                //    Result.status.message = IsValid.Item2;
                //    return Result;
                //}
                _ShippingSettings.Mahex_RateUrl="http://api.mahex.com/v2/rates";
                _ShippingSettings.Mahex_Username= "8UANr6GlKwGkd2cdowhKFop4x6yVFd5bTpXc5umTSoC1JInL";
                //_ShippingSettings.Mahex_Username="obtEuZpQ3Ha4MnfL4kYFnGp23hqntcZUP72cF7AhVvyckCwa";
                //if (string.IsNullOrEmpty((_ShippingSettings.Mahex_RateUrl ?? "").Trim()))
                //{
                //    Result.status.state = "failed";
                //    Result.status.message = "Setting(Mahex_RateUrl URL GetQuote) is null";
                //    return Result;
                //}
                //if (string.IsNullOrEmpty((_ShippingSettings.Mahex_Username ?? "").Trim()))
                //{
                //    Result.status.state = "failed";
                //    Result.status.message = "Setting(APP AUTH) is null";
                //    return Result;
                //}

                #endregion
                string JsonData = JsonConvert.SerializeObject(param);
                //This URL not exist, it's only an example.
                string url = _ShippingSettings.Mahex_RateUrl;
                //Instantiate new CustomWebRequest class
                var wr = new Mahex_CustomWebRequest(url);
                
                //PostData
                var Text = wr.PostData(_ShippingSettings.Mahex_Username, JsonData);
                Common.Log("mahexLog", Text);
                //Set responsestring to textbox1
                //string Text = wr.ResponseString;
                var t = JsonConvert.DeserializeObject<Result_Mahex_GetQuote>(Text);
                
                return t;
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                Result.status.state = "failed";
                Result.status.message = "An error has occurred in the Service Mahex";
                return Result;
            }
        }

      
        public async Task<Result_Mahex_Bulkimport> Bulkimport(Params_Mahex_createShipment param)
        {
            var Result = new Result_Mahex_Bulkimport();
            try
            {
                #region Check Param
                
                _ShippingSettings.Mahex_CreateShipmentUrl = "http://api.mahex.com/v2/shipments";
                _ShippingSettings.Mahex_Username = "8UANr6GlKwGkd2cdowhKFop4x6yVFd5bTpXc5umTSoC1JInL";
                //_ShippingSettings.Mahex_Username="obtEuZpQ3Ha4MnfL4kYFnGp23hqntcZUP72cF7AhVvyckCwa";

                #endregion
                string JsonData = JsonConvert.SerializeObject(param);

                string url = _ShippingSettings.Mahex_CreateShipmentUrl;

                var wr = new Mahex_CustomWebRequest(url);

                string Text = wr.PostData(_ShippingSettings.Mahex_Username, JsonData);
                
                var t = JsonConvert.DeserializeObject<Result_Mahex_Bulkimport>(Text);
                if (t.status.code == 201)
                {
                    Result.status.code = 201;
                    Result.status.state = "success";
                    Result.status.message = "Ok";
                    Result.data.shipment_uuid = t.data.shipment_uuid;
                }
                else
                {
                    Result.status.state = "failed";
                    if (t.status != null && t.data.shipment_uuid != null && t.data.shipment_uuid != null && t.status.code != 200)
                        Result.status.message = t.status.message;
                    else
                        Result.status.message = t.status.message.ToString();
                }
                return Result;
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                Result.status.state = "failed";
                Result.status.message = "An error has occurred in the Service Mahex";
                return Result;
            }
        }

        /// <summary>
        /// returns empty string if shipment updates tracking
        /// </summary>
        /// <returns></returns>
        public GetShipmentResult UpdateShipmentTracking(string UUID)
        {
            string url = "http://api.mahex.com/v2/shipments/" + UUID;
            _ShippingSettings.Mahex_Username = "8UANr6GlKwGkd2cdowhKFop4x6yVFd5bTpXc5umTSoC1JInL";
            //_ShippingSettings.Mahex_Username="obtEuZpQ3Ha4MnfL4kYFnGp23hqntcZUP72cF7AhVvyckCwa";
            var wr = new Mahex_CustomWebRequest(url);
            string Text = wr.PostData(_ShippingSettings.Mahex_Username, "", "GET");
            var Result = JsonConvert.DeserializeObject<GetShipmentResult>(Text);            
            return Result;
        }


        public VoidShipmentResult VoidShipment(string waybill_number)
        {
            string url = " http://api.mahex.com/v2/shipments/" + waybill_number + "/void";
            _ShippingSettings.Mahex_Username = "8UANr6GlKwGkd2cdowhKFop4x6yVFd5bTpXc5umTSoC1JInL";
            //_ShippingSettings.Mahex_Username="obtEuZpQ3Ha4MnfL4kYFnGp23hqntcZUP72cF7AhVvyckCwa";
            var wr = new Mahex_CustomWebRequest(url);
            string Text = wr.PostData(_ShippingSettings.Mahex_Username, "", "PUT");
            var Result = JsonConvert.DeserializeObject<VoidShipmentResult>(Text);
            return Result;
        }

        public Result_Mahex_Tracking Tracking(Params_Mahex_Tracking param)
        {
            string url = "http://api.mahex.com/v2/track/" + param.waybill_number;
            _ShippingSettings.Mahex_Username = "8UANr6GlKwGkd2cdowhKFop4x6yVFd5bTpXc5umTSoC1JInL";
            //_ShippingSettings.Mahex_Username="obtEuZpQ3Ha4MnfL4kYFnGp23hqntcZUP72cF7AhVvyckCwa";
            var wr = new Mahex_CustomWebRequest(url);
            string Text = wr.PostData(_ShippingSettings.Mahex_Username, "", "GET");
            var Result = JsonConvert.DeserializeObject<Result_Mahex_Tracking>(Text);
            return Result;
        }

    }
}
