using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    [Route("api/[controller]")]
    public class TozicoController : BaseApiController
    {
        public TozicoController()
        {

        }

        [HttpGet("[action]/{id}")]
        public IActionResult RemoveRequest(int id)
        {
            return Ok();
        }
    }
}
