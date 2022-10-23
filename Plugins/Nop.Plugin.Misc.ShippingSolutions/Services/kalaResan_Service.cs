using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.ShippingSolutions.Models.kalaResan;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services
{
    /// <summary>
    /// سرویس کالا رسان
    /// </summary>
    public class kalaResan_Service : IkalaResan_Service
    {
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly ShippingSettings _ShippingSettings;
        private readonly ISettingService _settingService;
        public kalaResan_Service(ILogger logger, IWorkContext workContext, ShippingSettings ShippingSettings, ISettingService settingService)
        {
            this._logger = logger;
            this._workContext = workContext;
            this._settingService = settingService;
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
        /// <summary>
        /// ثبت بارنامه جدید
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<RegisterParcelOutoutModel> RegisterShipment(RegisterParcelInputModel model)
        {
            model.apiCode = "32e5ddaad52dee1a0cd1c6279ea5d436";
            string reuslt = await SendRequest(model);
            if (string.IsNullOrEmpty(reuslt))
                return null;
            return JsonConvert.DeserializeObject<RegisterParcelOutoutModel>(reuslt);
        }
        /// <summary>
        /// رهگیری مرسوله
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<TrackingOutputtModel> TrackShipment(TrackingInputModel model)
        {
            model.apiCode = "fd888a2f67cdd457dc4fde46e50d7058";
            string reuslt = await SendRequest(model);
            if (string.IsNullOrEmpty(reuslt))
                return null;
            return JsonConvert.DeserializeObject<TrackingOutputtModel>(reuslt);
        }
        /// <summary>
        /// ابطال مرسوله
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<CancelOrderOutputModel> CancelOrder(TrackingInputModel model)
        {
            model.apiCode = "e3f2e79dc6b0fac0b4c336e66feab57b";
            string reuslt = await SendRequest(model);
            if (string.IsNullOrEmpty(reuslt))
                return null;
            return JsonConvert.DeserializeObject<CancelOrderOutputModel>(reuslt);
        }
        /// <summary>
        /// استعلم قیمت
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<GetPriceOutputModel> GetPrice(GetPriceInputModel model)
        {
            model.apiCode = "5d184f99571b34d62f8aa07186e289d2";
            string reuslt = await SendRequest(model);
            if (string.IsNullOrEmpty(reuslt))
                return null;
            return JsonConvert.DeserializeObject<GetPriceOutputModel>(reuslt);  
        }
        private async Task<string> SendRequest(object model)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://panel.baritom.com/api/postex.php?postexApi=1");
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = "POST";
            string result = "";
            var json = JsonConvert.SerializeObject(model);
            using (var streamWriter = new StreamWriter(await httpWebRequest.GetRequestStreamAsync()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            try
            {
                using (var response = httpWebRequest.GetResponse() as HttpWebResponse)
                {
                    if (httpWebRequest.HaveResponse && response != null)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            result = reader.ReadToEnd();

                        }
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            return result;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (WebException e)
            {
                return null;
            }
        }

    }
}
