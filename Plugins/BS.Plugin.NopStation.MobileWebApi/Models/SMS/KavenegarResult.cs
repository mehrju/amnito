using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models.SMS
{
    public partial class KavenegarResult
    {
        [JsonProperty("return")]
        public Return Return { get; set; }

        [JsonProperty("entries")]
        public List<Entry> Entries { get; set; }
    }

    public partial class Entry
    {
        [JsonProperty("messageid")]
        public long MessageId { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }

        [JsonProperty("receptor")]
        public string Receptor { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }
    }

    public partial class Return
    {
        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
