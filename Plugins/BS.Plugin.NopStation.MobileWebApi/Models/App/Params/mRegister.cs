using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models.App.Params
{
    public class mRegister
    {
        public string Mobile { get; set; }
        public string CodeMeli { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
    }
}
