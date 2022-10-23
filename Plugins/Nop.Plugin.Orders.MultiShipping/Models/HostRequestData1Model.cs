using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class HostRequestData1Model
    {
        /// <summary>
        ///Host Request
        /// </summary>
        public string hreq { get; set; }
        /// <summary>
        /// Host Request sign
        /// </summary>
        public string hsign { get; set; }
        /// <summary>
        /// SDK protocol Version Info
        /// </summary>
        public string ver { get; set; }
    }
    public class HostRequestData2Model
    {
        /// <summary>
        /// Host Id
        /// </summary>
        public int hi { get; set; }
        /// <summary>
        /// Host Tran Id
        /// </summary>
        public long htran { get; set; }
        /// <summary>
        /// Host Request Time
        /// </summary>
        public long htime { get; set; }
        /// <summary>
        /// Host OpCode
        /// </summary>
        public int hop { get; set; }
        /// <summary>
        /// Host Api key
        /// </summary>
        public string hkey { get; set; }
        /// <summary>
        /// mobile
        /// </summary>
        public string mo { get; set; }
        /// <summary>
        /// Amount
        /// </summary>
        public long? ao { get; set; }
        /// <summary>
        /// merchant code
        /// </summary>
        public string merch { get; set; }
        /// <summary>
        /// iban share
        /// </summary>
        public string iban { get; set; }
        /// <summary>
        /// payment Id
        /// </summary>
        public string pid { get; set; }
        /// <summary>
        /// Request Send Time
        /// </summary>
        public long? stime { get; set; }
        /// <summary>
        /// Unique Tran Id
        /// </summary>
        public long? utran { get; set; }
        /// <summary>
        /// settle Token
        /// </summary>
        public string stkn { get; set; }
        /// <summary>
        /// authentication Token
        /// </summary>
        public string atkn { get; set; }

    }
    public class HostResponceBoundel
    {
        public string hresp { get; set; }
        public string hsign { get; set; }
    }
    public class HostResponseModel
    {
        /// <summary>
        /// Host Id
        /// </summary>
        public int hi { get; set; }
        /// <summary>
        /// Host Tran Id
        /// </summary>
        public long? htran { get; set; }
        /// <summary>
        /// Request Send Time
        /// </summary>
        public long htime { get; set; }
        /// <summary>
        /// Host OpCode
        /// </summary>
        public int hop { get; set; }
        /// <summary>
        /// Status Code
        /// </summary>
        public int st { get; set; }
        /// <summary>
        /// Status Message
        /// </summary>
        public string stm { get; set; }
        /// <summary>
        /// Amount
        /// </summary>
        public long? ao { get; set; }
        /// <summary>
        /// settle Token
        /// </summary>
        public string stkn { get; set; }
        /// <summary>
        /// RRN
        /// </summary>
        public string rrn { get; set; }
        /// <summary>
        /// mobile
        /// </summary>
        public string mo { get; set; }

    }
}
