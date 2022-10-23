using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NopFarsi.Payments.SepShaparak.Models
{
    public class SepTokenResultModel
    {
        public int status { get; set; }
        public int errorCode { get; set; }
        public string errorDesc { get; set; }
        public string token { get; set; }
    }
}
