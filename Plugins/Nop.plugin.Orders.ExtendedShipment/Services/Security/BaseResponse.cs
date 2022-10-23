using System.Collections.Generic;

namespace Nop.plugin.Orders.ExtendedShipment.Services.Security
{
    public class BaseResponse
    {
        public BaseResponse()
        {
            StatusCode = (int)ErrorType.Ok;
            ErrorList = new List<string>();
        }

        public string SuccessMessage { get; set; }
        public int StatusCode { get; set; }
        public List<string> ErrorList { get; set; }
    }
    public enum ErrorType
    {
        Ok = 200,
        NotOk = 400,
        AuthenticationError = 600
    }
}