using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Infrastructure.WebApi.Logger
{
//    public class ErrorResult : IActionResult
//    {
//        private readonly string _errorMessage;
//        private readonly HttpRequestMessage _requestMessage;
//        private readonly HttpStatusCode _statusCode;

//        public ErrorResult(HttpRequestMessage requestMessage, HttpStatusCode statusCode, string errorMessage)
//        {
//            _requestMessage = requestMessage;
//            _statusCode = statusCode;
//            _errorMessage = errorMessage;
//        }

//        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
//        {
//            return Task.FromResult(_requestMessage.CreateErrorResponse(_statusCode, _errorMessage));
//        }

//        public Task ExecuteResultAsync(ActionContext context)
//        {
//            return Task.FromResult(_requestMessage.CreateErrorResponse(_statusCode, _errorMessage));
            
//        }
//    }
}
