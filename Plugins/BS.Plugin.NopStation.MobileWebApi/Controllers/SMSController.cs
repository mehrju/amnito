using BS.Plugin.NopStation.MobileWebApi.Models.App.Results;
using BS.Plugin.NopStation.MobileWebApi.Models.SMS;
using Microsoft.AspNetCore.Mvc;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Services.Customers;
using Nop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class SMSController : BasePublicController // BaseApiController
    {
        private readonly INotificationService _notificationService;
        private readonly ICustomerService _customerService;


        public SMSController(INotificationService notificationService, ICustomerService customerService)
        {
            _notificationService = notificationService;
            _customerService = customerService;
        }

        [HttpPost("api/SMS/Send")]
        public IActionResult Send([FromBody]SendSmsInput sendSmsInput)
        {
            bool StateSendSMS = false;
            //string resultMessage = "";
            #region check params
            if (string.IsNullOrEmpty((sendSmsInput.UserName ?? "").Trim())
                 || string.IsNullOrEmpty((sendSmsInput.Passwrod ?? "").Trim())
                 || string.IsNullOrEmpty((sendSmsInput.Receiver ?? "").Trim())
                 || string.IsNullOrEmpty((sendSmsInput.Message ?? "").Trim()))
            {
                StateSendSMS = false;
            }

            else
            {

                if (sendSmsInput.UserName.ToLower() != "postexsms" || sendSmsInput.Passwrod.ToLower() != "postextsms2a1234")
                {
                    StateSendSMS = false;
                }
                else
                {
                    StateSendSMS = _notificationService._sendSms(sendSmsInput.Receiver, sendSmsInput.Message);
                }
            }
            #endregion

            return Json(new { State = StateSendSMS });
        }

        [Route("api/SMS/SendSMS_API")]
        [HttpPost]
        public IActionResult SendSMS_API(string Username, String Pass, string receiver, string msg)
        {
            bool StateSendSMS = false;
            //string resultMessage = "";
            #region check params
            if (string.IsNullOrEmpty((Username ?? "").Trim())
                 || string.IsNullOrEmpty((Pass ?? "").Trim())
                 || string.IsNullOrEmpty((receiver ?? "").Trim())
                 || string.IsNullOrEmpty((msg ?? "").Trim())
                )
            {
                StateSendSMS = false;
            }

            else
            {

                string HashUsername = String.Format("{0:X}", Username.GetHashCode());
                string HashPass = String.Format("{0:X}", Pass.GetHashCode());
                if (HashUsername != "FD875BB6" || HashPass != "99A406AA")
                {
                    StateSendSMS = false;
                }
                else
                {
                    StateSendSMS = _notificationService._sendSms(receiver, msg);
                }
            }
            #endregion

            return Json(new { State = StateSendSMS });
        }

        [Route("api/Tc_SendSms")]
        [HttpPost]
        public IActionResult Tc_SendSMs(string Username, String Pass, string receiver, string msg)
        {
            bool StateSendSMS = false;
            //string resultMessage = "";
            #region check params
            if (string.IsNullOrEmpty((Username ?? "").Trim())
                 || string.IsNullOrEmpty((Pass ?? "").Trim())
                 || string.IsNullOrEmpty((receiver ?? "").Trim())
                 || string.IsNullOrEmpty((msg ?? "").Trim())
                )
            {
                StateSendSMS = false;
            }

            else
            {

                string HashUsername = String.Format("{0:X}", Username.GetHashCode());
                string HashPass = String.Format("{0:X}", Pass.GetHashCode());
                if (HashUsername != "FD875BB6" || HashPass != "99A406AA")
                {
                    StateSendSMS = false;
                }
                else
                {
                    StateSendSMS = _notificationService._sendSms(receiver, msg);
                }
            }
            #endregion

            return Json(new { State = StateSendSMS });
        }

        [Route("api/SMS/SendSMS")]
        [HttpPost]
        public object SendSMS(string Username, String Pass, string receiver, string msg)
        {
            try
            {

                if (string.IsNullOrEmpty((Username ?? "").Trim())
                     || string.IsNullOrEmpty((Pass ?? "").Trim())
                     || string.IsNullOrEmpty((receiver ?? "").Trim())
                     || string.IsNullOrEmpty((msg ?? "").Trim())
                    )
                {

                    return mResponseList.error_invalid_param;
                }

                else
                {

                    string HashUsername = String.Format("{0:X}", Username.GetHashCode());
                    string HashPass = String.Format("{0:X}", Pass.GetHashCode());
                    if (HashUsername != "FD875BB6" || HashPass != "99A406AA")
                    {
                        bool StateSendSMS = _notificationService._sendSms(receiver, msg);
                        if (StateSendSMS)
                            return mResponseList.error_invalid_param;
                        else
                            return mResponseList.error_Execption;
                    }
                    else
                    {
                        return mResponseList.success;
                    }
                }

            }
            catch (Exception ee)
            {
                mResponseList.error_Execption.data = ee.Message;
                return mResponseList.error_Execption;
            }

        }

        [Route("api/Tc_AddAgent")]
        [HttpPost]
        public void AddCollector(Collector myCollector)
        {
            //if(myCollector== null)
            //    return Json(new {result=false,message="اطلاعات ارسالی نامعتبر می باشد" })
            //string MonileNumber= myCollector.
            //_customerService.GetCustomerByUsername()
        }
       
        public class Collector
        {
            public int AgentId { get; set; }
            public string Name { get; set; }
            public string  Family { get; set; }
            public string MobileNumber { get; set; }

        }
    }
}
