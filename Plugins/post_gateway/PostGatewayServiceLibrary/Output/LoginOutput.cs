using System.Collections.Generic;
using System.Net;

namespace FrotelServiceLibrary.Output
{
    public class LoginOutput : BaseOutput
    {
        public IEnumerable<Cookie> Cookies { get; set; }
    }
}
