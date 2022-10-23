using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace BS.Plugin.Orders.ExtendedShipment.Models.Optime.ToolApiCall
{
    public partial class CallRequestModel
    {
        [JsonProperty("fileContent")]
        public List<FileContent> FileContent { get; set; }

        [JsonProperty("fileExtention")]
        public string FileExtention { get; set; } = "json";

        [JsonProperty("planName")]
        public string PlanName { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("toolName")]
        public string ToolName { get; set; } = "FULL_REVERSE_LOGISTIC";

        [JsonProperty("planConfigDto")]
        public PlanConfigDto PlanConfigDto { get; set; }
    }

    

    public partial class PlanConfigDto
    {
        [JsonProperty("config")]
        public List<Config> Config { get; set; }

        [JsonProperty("option")]
        public List<Option> Option { get; set; }
    }

    public partial class Config
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = "car";

        [JsonProperty("zone")]
        public string Zone { get; set; } = "0";

        [JsonProperty("out")]
        public string Out { get; set; } = "1";

        [JsonProperty("volume")]
        public long Volume { get; set; }

        [JsonProperty("weight")]
        public long Weight { get; set; }
    }

    public partial class Option
    {
        [JsonProperty("shiftsCode", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ShiftsCode { get; set; } = new List<string>() { "9-16"};

        [JsonProperty("useGeoCode", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FluffyParseStringConverter))]
        public bool? UseGeoCode { get; set; } = false;

        [JsonProperty("driverOptimization", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FluffyParseStringConverter))]
        public bool? DriverOptimization { get; set; } = false;

        [JsonProperty("driverMustReturnToWareHouse", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(FluffyParseStringConverter))]
        public bool? DriverMustReturnToWareHouse { get; set; } = true;
    }

    public partial class CallRequestModel
    {
        public static CallRequestModel FromJson(string json) => JsonConvert.DeserializeObject<CallRequestModel>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this CallRequestModel self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

   
    internal class FluffyParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(bool) || t == typeof(bool?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (Boolean.TryParse(value, out bool b))
            {
                return b;
            }
            throw new Exception("Cannot unmarshal type bool");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (bool)untypedValue;
            var boolString = value ? "true" : "false";
            serializer.Serialize(writer, boolString);
            return;
        }

        public static readonly FluffyParseStringConverter Singleton = new FluffyParseStringConverter();
    }
}
