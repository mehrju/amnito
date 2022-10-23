namespace BS.Plugin.NopStation.MobileWebApi.Services.Sms
{
    public interface ISmsReceiverService
    {
        void SmsReceived(string from, string to, string message, string messageId);
    }
}