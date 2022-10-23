using BS.Plugin.Orders.ExtendedShipment.Models.Optime.Authenticate;
using BS.Plugin.Orders.ExtendedShipment.Models.Optime.GetToolResponseCluster;
using BS.Plugin.Orders.ExtendedShipment.Models.Optime.ToolApiCall;
using BS.Plugin.Orders.ExtendedShipment.Services;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Controllers;

namespace BS.Plugin.Orders.ExtendedShipment.Controllers
{
    public class OptimeApiController : BaseAdminController
    {
        private readonly IOptimeApiService _optimeApiService;

        public OptimeApiController(IOptimeApiService optimeApiService)
        {
            _optimeApiService = optimeApiService;
        }

        //[HttpPost]
        //[Route("api/Authenticate")]
        //public IActionResult Authenticate(OptimeUserModel model)
        //{
        //    //TODO : implement
        //    AuthenticationResultModel result = _optimeApiService.Authenticate();
        //    return Ok(result);
        //}

        [HttpPost]
        [Route("api/ToolApiCall")]
        public IActionResult ToolApiCall(CallRequestModel model)
        {
            //TODO : implement
            CallResponseModel result = _optimeApiService.NewPlan(model);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/ExecuteSelectedTask")]
        public IActionResult ExecuteSelectedTask(string deliveryTaskId)
        {
            //TODO : implement
            bool result = _optimeApiService.ExecuteSelectedTask(deliveryTaskId);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/GetToolResponseCluster")]
        public IActionResult GetToolResponseCluster(string token)
        {
            //TODO : implement
            ResponseModel result = _optimeApiService.GetToolResponseCluster(token);
            return Ok(result);
        }

        [HttpGet]
        [Route("api/optime/CheckForOptimizedTask")]
        public IActionResult CheckForOptimizedTask()
        {
            _optimeApiService.CheckForOptimizedTask();
            return Ok();
        }
    }
}
