using BS.Plugin.NopStation.MobileWebApi.Services.Sms;
using BS.Plugin.NopStation.MobileWebApi.Validator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    [Route("ReceiveSms")]
    public class ReceiveSmsWebHookController : Controller
    {
        private ISmsReceiverService _smsReceiverService;

        public ReceiveSmsWebHookController(ISmsReceiverService smsReceiverService)
        {
            _smsReceiverService = smsReceiverService;
        }

        [AllowAnonymous]
        [HttpGet]
        [ServiceFilter(typeof(KavenegarFilterAttribute))]
        public IActionResult Get(string from, string to, string message, string messageId)
        {
            _smsReceiverService.SmsReceived(from, to, message, messageId);
            return Ok();
        }

    }
}
