using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

using System.Text;
using Nop.plugin.Orders.ExtendedShipment.Models.Tozico.Input;
using Nop.plugin.Orders.ExtendedShipment.Models.Tozico.Output;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace Nop.plugin.Orders.ExtendedShipment.Services.Tozico
{
    public class TozicoService : ITozicoService
    {
        private readonly string _apiBaseUri = "http://api.tozico.com/api";
        private readonly RestClient _restClient;
        private readonly TozicoSetting _tozicoSetting;


        public TozicoService(TozicoSetting tozicoSetting)
        {
            _restClient = new RestClient(tozicoSetting.BaseAddress);
            _tozicoSetting = tozicoSetting;
            _apiBaseUri = tozicoSetting.BaseAddress;
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


        public IRestResponse Execute(IRestRequest request)
        {
            return _restClient.Execute(request);
        }



        public TozicoResult AddOrUpdateBranches(List<Branch> branches)
        {
            var request = ConfigRequest("/edit/branchs", Method.POST, branches);
            var response = Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (JObjectEx.TryParseJSON(response.Content, out JObject obj))
                {
                    return JsonConvert.DeserializeObject<TozicoResult>(response.Content);
                }
            }
            return null;
        }


        public TozicoResult AddOrUpdateVehicles(List<Vehicle> vehicles)
        {
            var request = ConfigRequest("/edit/vehicles", Method.POST, vehicles);
            var response = Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (JObjectEx.TryParseJSON(response.Content, out JObject obj))
                {
                    return JsonConvert.DeserializeObject<TozicoResult>(response.Content);
                }
            }
            return null;
        }


        public TozicoResult AddOrUpdateCustomers(List<TozicoCustomer> customers)
        {
            var request = ConfigRequest("/edit/customers", Method.POST, customers);
            var response = Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (JObjectEx.TryParseJSON(response.Content, out JObject obj))
                {
                    return JsonConvert.DeserializeObject<TozicoResult>(response.Content);
                }
            }
            return null;
        }

        public TozicoTokenResult getLoginToken(int customerId)
        {
            TozicoTokenResult TokenResult = new TozicoTokenResult();
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://smart.postex.ir/api/token/" + customerId.ToString());
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                string result = "";
                
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
                            TokenResult = JsonConvert.DeserializeObject<TozicoTokenResult>(result);
                           
                        }
                        else
                        {
                            TokenResult.success = false;
                            TokenResult.message = "در حال حاضر امکان اتصال به سامانه هوشمند جمع آوری وجود ندارد";
                        }
                    }
                    else
                    {
                        TokenResult.success = false;
                        TokenResult.message = "در حال حاضر امکان اتصال به سامانه هوشمند جمع آوری وجود ندارد";
                    }
                }
               
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                TokenResult.success = false;
                TokenResult.message = "خطا در زمان دریافت توکن از سامانه هوشمند جمع آوری و توزیع";
            }
            return TokenResult;

        }

    }
   
}