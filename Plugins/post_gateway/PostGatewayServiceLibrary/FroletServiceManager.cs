using FrotelServiceLibrary.Input;
using FrotelServiceLibrary.Output;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ConsoleApp2;
using System.Net;
using System.Linq;

namespace FrotelServiceLibrary
{
    public class PostGatewayServiceManager : IDisposable
    {
        private readonly string _apiBaseUri = "http://gateway.post.ir";
        private readonly HttpClient _httpClient;
        private readonly CookieContainer _cookieContainer;
        private readonly HttpClientHandler _httpClientHandler;

        public PostGatewayServiceManager()
        {
            _cookieContainer = new CookieContainer();
            _httpClientHandler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer
            };
            _httpClient = new HttpClient(_httpClientHandler);
        }

        public async Task<LoginOutput> Login(LoginInput loginInput)
        {
            return await sendResuest<LoginOutput>(HttpMethod.Post, "", loginInput, null);
        }
        public async Task<NewShopOutput> NewShop(NewShopInput registerInput, IEnumerable<Cookie> cookies)
        {
            return await sendResuest<NewShopOutput>(HttpMethod.Post, "Seller/Newshop", registerInput, cookies);
        }
        private async Task<T> sendResuest<T>(HttpMethod httpMethod, string route, object postParams, IEnumerable<Cookie> cookies)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(httpMethod.Method), $"{_apiBaseUri}/{route}");
            string apiResponse = "";
            if (postParams != null)
            {
                requestMessage.Content = new FormUrlEncodedContent(postParams.ToKeyValue());
            }
            if (cookies != null)
            {
                foreach (var cookie in cookies)
                {
                    _cookieContainer.Add(cookie);
                }
            }
            HttpResponseMessage response = null;
            try
            {
                response = await _httpClient.SendAsync(requestMessage);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Response status code is {response.StatusCode.ToString()}");
                }
                apiResponse = await response.Content.ReadAsStringAsync();
                BaseOutput result = null;

                if (typeof(T) == typeof(LoginOutput))
                {
                    result = new LoginOutput();
                    var token = "<input type=\"submit\" value=\"ورود به سامانه\" name=\"B1\" style=\"font-family: Tahoma; font-size: 9pt\">";

                    if (apiResponse.Contains(token))
                    {
                        result.Successfull = false;
                    }
                    else
                    {
                        result.Successfull = true;
                        Uri address = new Uri($"{_apiBaseUri}/{route}");
                        (result as LoginOutput).Cookies = _cookieContainer.GetCookies(address).Cast<Cookie>();
                    }
                }
                else
                {
                    result = new NewShopOutput();
                    if (apiResponse.Contains("این نام کاربری در سیستم موجود میباشد"))
                    {
                        result.Successfull = false;
                        (result as NewShopOutput).Message = "این نام کاربری در سیستم موجود میباشد";
                    }
                    else if (apiResponse.Contains("تکمیل تمامی اطلاعات الزامی میباشد"))
                    {
                        result.Successfull = false;
                        (result as NewShopOutput).Message = "تکمیل تمامی اطلاعات الزامی میباشد";
                    }
                    else if (apiResponse.Contains("فروشگاه مورد نظر با موفقیت در سامانه ثبت گردید"))
                    {
                        result.Successfull = true;
                        (result as NewShopOutput).Message = "فروشگاه مورد نظر با موفقیت در سامانه ثبت گردید";
                    }
                    else
                    {
                        throw new Exception(apiResponse);
                    }
                }


                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch (Exception ex)
            {
                throw new Exception($"An error ocurred while calling the API. It responded with the following message: {response.StatusCode} {response.ReasonPhrase} {ex.ToString()} apiResponse {apiResponse}");
            }
        }
        public void Dispose()
        {
            _httpClientHandler.Dispose();
            _httpClient.Dispose();
        }
    }
}