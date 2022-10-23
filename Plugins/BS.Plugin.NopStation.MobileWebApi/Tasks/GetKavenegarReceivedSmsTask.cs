using BS.Plugin.NopStation.MobileWebApi.Services.Sms;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Tasks
{
    public partial class GetKavenegarReceivedSmsTask : IScheduleTask
    {
        private readonly IKavenegarService _kavenegarService;
        private readonly ISmsReceiverService _smsReceiverService;

        public GetKavenegarReceivedSmsTask(IKavenegarService kavenegarService,
            ISmsReceiverService smsReceiverService)
        {
            _kavenegarService = kavenegarService;
            _smsReceiverService = smsReceiverService;
        }

        public void Execute()
        {
            var unReadSms = _kavenegarService.GetUnreadSms();
            if(unReadSms != null && unReadSms.Return.Status == 200 && unReadSms.Entries != null && unReadSms.Entries.Any())
            {
                common.Log("kavenegar sms received api","Count " + unReadSms.Entries.Count().ToString());
                foreach (var item in unReadSms.Entries)
                {
                    _smsReceiverService.SmsReceived(item.Sender, item.Receptor, item.Message, item.MessageId.ToString());
                }
                common.Log("kavenegar sms received api", "finished");

            }
        }
    }
}
