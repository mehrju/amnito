using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Nop.Services.Authentication.External;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public class AuthorizationResultApi /*: AuthorizationResult*/
    {
        //public AuthorizationResultApi(ExternalAuthenticationStatus status, int customerId) : base(status)
        //{
        //    this.CustomerId = customerId;
        //}

        public int CustomerId { get; set; }
    }
}
