using BS.Plugin.NopStation.MobileWebApi.Models.SMS;

namespace BS.Plugin.NopStation.MobileWebApi.Services.Sms
{
    public interface IKavenegarService
    {
        KavenegarResult GetUnreadSms();
    }
}