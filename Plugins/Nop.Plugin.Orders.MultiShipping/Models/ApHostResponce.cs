using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class ApHostResponce
    {

        public int paymentId { get; set; }
        public string message { get; set; }
        public string host_response { get; set; }
        public string host_response_sign { get; set; }
        public int status_code { get; set; }
        public string unique_tran_id { get; set; }
        public long payment_id { get; set; }
        public HostResponseModel _host_response { get; set; }
        public bool IsCod { get; set; }
    }
}
