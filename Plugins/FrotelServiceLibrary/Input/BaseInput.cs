using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrotelServiceLibrary.Input
{
    public class BaseInput
    {
        [JsonProperty("api")]
        public string ApiKey { get; set; }
    }
}
