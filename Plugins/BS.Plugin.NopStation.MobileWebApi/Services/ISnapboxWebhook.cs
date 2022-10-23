using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public interface ISnapboxWebhook
    {
        void SnapBoxAccepted(int orderId, string SnappOrderId, string bikerPhoneNumber, string bikerName);
        void SnapboxPickup(int orderId, string bycerMobileNo);
        void SnapboxDeliver(int orderId);
        void SnapboxARRIVIED(int orderId, string bycerMobileNo);
        void SnapboxBikerCancel(int orderId);
    }
}
