using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrotelServiceLibrary.Output
{
    public class CheckPayOutput : BaseOutput
    {
        [JsonProperty("id")]
        public int Id { get; set; }  // شناسه پرداخت 


        [JsonProperty("verify")]
        public int IsSuccessfull { get; set; }  // وضعیت پرداخت ، پرداخت موفقیت آمیز 1 و در غیر این صورت 0


        [JsonProperty("message")]
        public int PayMessage { get; set; } // پیغام وضعیت 

        [JsonProperty("code")]
        public int PayCode { get; set; } // کد رهگیری برای پرداخت های موفق

    }
}
