using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Services.Logging;
using Nop.Web.Controllers;
using Nop.Services.Common;
using Nop.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Orders.BulkOrder.Services;
using Nop.Plugin.Orders.MultiShipping.Models.RSVP;
using BS.Plugin.NopStation.MobileWebApi.Models.RSVP;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class RSVPController : BasePublicController
    {
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly IRepository<Tbl_RSVP_Webhook> _repositoryTbl_RSVP_Webhook;

        public RSVPController
            (
             ILogger logger, IWorkContext workContext,
             IRepository<Tbl_RSVP_Webhook> repositoryTbl_RSVP_Webhook
            )
        {
            _logger = logger;
            _workContext = workContext;
            _repositoryTbl_RSVP_Webhook = repositoryTbl_RSVP_Webhook;
        }


        [Route("api/RSVP/Webhook")]
        [HttpPost]
        public IActionResult Webhook(ParmsRSVP param)
        {
            if (param.Mobile!=""|| param.Mobile!=null)
            {
                common.Log(" params rsvp ",param.Mobile.ToString());
                Tbl_RSVP_Webhook temp = new Tbl_RSVP_Webhook();
                temp.Mobile = param.Mobile;
                temp.DateInsert = DateTime.Now;
                _repositoryTbl_RSVP_Webhook.Insert(temp);
            }
            return Json(new { State = true });
        }
            
    }
}
