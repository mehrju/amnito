using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Security.Cryptography.X509Certificates;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Safiran
{
   public class Safiran_MyPolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(
            ServicePoint srvPoint
            , X509Certificate certificate
            , WebRequest request
            , int certificateProblem)
        {
            //Return True to force the certificate to be accepted.
            return true;
        } // end CheckValidationResult
    }
}
