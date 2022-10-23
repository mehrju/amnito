using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrotelServiceLibrary.Input
{
    public class OrderPayInput : BaseInput
    {
        [JsonProperty("factor")]
        public string Factor { get; set; }
    }
}