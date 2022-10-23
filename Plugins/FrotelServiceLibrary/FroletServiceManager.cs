using FrotelServiceLibrary.Input;
using FrotelServiceLibrary.Output;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using ConsoleApp2;
using Newtonsoft.Json.Linq;
using FrotelServiceLibrary.Enum;

namespace FrotelServiceLibrary
{
    public class FroletServiceManager : IDisposable
    {
        private readonly string _apiBaseUri = "http://webservice1.link/ws/v1/rest";
        private readonly HttpClient _httpClient;

        public FroletServiceManager()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        ///در این مرحله شما باید جمع کل مبلغ سفارش , وزن کل سفارش , شهر مقصد, روش های ارسال و روش های پرداخت را به این متد ارسال نمایید تا هزینه ارسال , مالیات ارسال و هزینه خدمات فروتل به شما برگردانده شود
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<GetPricesOutput> GetPrices(GetPricesInput input)
        {
            return await sendResuest<GetPricesOutput>(HttpMethod.Post, "order/getPrices.json", input);
        }

        /// <summary>
        /// در این مرحله شما باید يك سبد خرید + اطلاعات خریدار + يك روش پرداخت + يك روش ارسال را ارسال نمایید
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<RegisterOrderOutput> RegisterOrderAsync(RegisterOrderInput input)
        {
            return await sendResuest<RegisterOrderOutput>(HttpMethod.Post, "order/registerOrder.json", input);
        }

        /// <summary>
        /// با استفاده از این متد شما می توانید وضعیت یک سفارش را با جزئيات كامل رهگيري و دریافت نمایید.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<TrackingOutput> Tracking(TrackingInput input)
        {
            return await sendResuest<TrackingOutput>(HttpMethod.Post, "order/tracking.json", input);
        }

        private async Task<T> sendResuest<T>(HttpMethod httpMethod, string route, object postParams)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(httpMethod.Method), $"{_apiBaseUri}/{route}");
            string apiResponse = "";
            if (postParams != null)
            {
                requestMessage.Content = new FormUrlEncodedContent(postParams.ToKeyValue());
            }
            HttpResponseMessage response = null;
            try
            {
                response = await _httpClient.SendAsync(requestMessage);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"Response status code is {response.StatusCode.ToString()}");
                }
                apiResponse = await response.Content.ReadAsStringAsync();
                if (apiResponse != "")
                {
                    if (typeof(T) == typeof(GetPricesOutput))
                    {
                        var result = deserializeGetPricesOutput(apiResponse);
                        return (T)Convert.ChangeType(result, typeof(T));
                    }
                    return JsonConvert.DeserializeObject<T>(apiResponse);
                }
                else
                {
                    throw new Exception("Response is null");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error ocurred while calling the API. It responded with the following message: {response.StatusCode} {response.ReasonPhrase} {ex.ToString()} apiResponse {apiResponse}");
            }
        }

        private GetPricesOutput deserializeGetPricesOutput(string json)
        {
            GetPricesOutput getPricesOutput = new GetPricesOutput();

            JObject jObject = JObject.Parse(json);
            int code = (int)jObject["code"];
            string message = (string)jObject["message"];
            if (code != 0)
            {
                return new GetPricesOutput()
                {
                    Code = code,
                    Message = message,
                };
            }
            var resultObject = jObject["result"];
            if (resultObject["posti"] != null)
            {
                var resultNaghdiObject = resultObject["posti"];

                List<string> sendTypes = new List<string> { "1", "2", "3", "4" };
                foreach (var sendType in sendTypes)
                {
                    if (resultNaghdiObject[sendType] != null)
                    {
                        //var getPricesOutputResult = JsonConvert.DeserializeObject<GetPricesOutputResult>((string)resultNaghdiObject[sendType]);
                        var item = resultNaghdiObject[sendType];
                        getPricesOutput.Result.Add(new GetPricesOutputResult()
                        {
                            BuyType = BuyType.PardakhtDarMahal,
                            SendType = (SendType)int.Parse(sendType),
                            FrotelServicePrice = int.Parse((string)item["frotel_service"]),
                            PostPrice = int.Parse((string)item["post"]),
                            PackingPrice = int.Parse((string)item["packing"]),
                            TaxPrice = int.Parse((string)item["tax"]),
                        });
                    }
                }
            }
            return getPricesOutput;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}