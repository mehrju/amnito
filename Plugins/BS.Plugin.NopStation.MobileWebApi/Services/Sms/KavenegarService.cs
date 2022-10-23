using BS.Plugin.NopStation.MobileWebApi.Infrastructure;
using BS.Plugin.NopStation.MobileWebApi.Models.SMS;
using Nop.plugin.Orders.ExtendedShipment.Services;
using RestSharp;
using System;

namespace BS.Plugin.NopStation.MobileWebApi.Services.Sms
{
    public class KavenegarService : IKavenegarService
    {
        private readonly string _apiBaseUri = "https://api.kavenegar.com/v1/{0}";
        private readonly RestClient _restClient;
        private readonly KavenegarSetting _kavenegarSetting;

        public KavenegarService(KavenegarSetting kavenegarSetting)
        {
            _restClient = new RestClient(string.Format(kavenegarSetting.BaseAddress, kavenegarSetting.ApiKey));
            _apiBaseUri = kavenegarSetting.BaseAddress;
            _kavenegarSetting = kavenegarSetting;
        }

        public IRestRequest ConfigRequest(string resource, Method method, object parameter)
        {
            var request = new RestRequest(resource, method);
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            if (parameter != null)
            {
                request.AddJsonBody(parameter);
            }
            return request;
        }


        public IRestResponse<T> Execute<T>(IRestRequest request) where  T : new()
        {
            return _restClient.Execute<T>(request);
        }


        public KavenegarResult GetUnreadSms()
        {
            var request = ConfigRequest($"/sms/receive.json?linenumber={_kavenegarSetting.LineNumber}&isread=0", Method.GET, null);
            var response = Execute<KavenegarResult>(request);

            common.Log("kavenegar sms received api", "output:" + response.Content);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return response.Data;
            }

            common.Log("kavenegar sms received api", "error: " + response.StatusDescription);
            throw new Exception($"Kavenegar API didn't response: {response.StatusDescription}");
        }
    }
}
