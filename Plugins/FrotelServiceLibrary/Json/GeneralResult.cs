using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrotelServiceLibrary.Json
{
    public class GeneralResult
    {

        [JsonProperty("result")]
        public object Result { get; set; }
    }
}