using Newtonsoft.Json;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Tools;
using RestSharp;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{

    public class PostBarCodeService : IPostBarCodeService
    {
        RestClient _restClient = new RestClient();
        public PostBarCodeService()
        {
            _restClient.BaseUrl = new Uri("http://poffice.post.ir/restservice/api");
        }

        public RestRequest ConfigRequestForGetBarCode(PostBarcodeGeneratorInputModel model)
        {
            var request = new RestRequest();
            request.Resource = "barcodeCredit/getbarcode";
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.Method = Method.POST;
            request.AddJsonBody(model);
            return request;
        }

        public IRestResponse<T> ExcuteRequest<T>(IRestRequest restRequest) where T : new()
        {
            return _restClient.Execute<T>(restRequest);
        }

        public PostBarcodeGeneratorOutputModel GenerateAndGetBarcode(PostBarcodeGeneratorInputModel model)
        {
            var str= Newtonsoft.Json.JsonConvert.SerializeObject(model); ;
            common.Log("اطلاعات ارسالی جهت تولید بارکددر سوریس جدید", str);
            var request = ConfigRequestForGetBarCode(model);
            var response = ExcuteRequest<PostBarcodeGeneratorOutputModel>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                common.Log("post api didnt response", $"{response.Content}, {Environment.NewLine} {response.StatusDescription}");
            }
            return response.Data;
        }
    }


    public class NewtonsoftJsonSerializer : ISerializer
    {
        public NewtonsoftJsonSerializer()
        {
            ContentType = "application/json";
            _serializer = new Newtonsoft.Json.JsonSerializer
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Include,
                DefaultValueHandling = DefaultValueHandling.Include,
                ContractResolver = new LowerCaseContractResolver()
            };
        }

        //public NewtonsoftJsonSerializer(JsonSerializer serializer)
        //{
        //    ContentType = "application/json";
        //    _serializer = serializer;
        //}


        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = Formatting.Indented;
                    jsonTextWriter.QuoteChar = '"';

                    _serializer.Serialize(jsonTextWriter, obj);

                    var result = stringWriter.ToString();
                    return result;
                }
            }
        }

        public string DateFormat { get; set; }
        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string ContentType { get; set; }

        private Newtonsoft.Json.JsonSerializer _serializer;
    }
}

