using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Snappbox;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Snappbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces
{
    public interface ISnappbox_Service
    {
        Task<Result_Snappbox_GetPrice> GetPrice(Params_Snappbox_Get_Price param);
        Task<Result_Snappbox_CreateOrder> CreateOrder(Params_Snappbox_create_order param);
        Result_Snappbox_Get_Order_Detail Get_Order_Detail(Params_Snappbox_Get_Order_Details param);
        Result_Snappbox_Get_Order_List Get_Order_List(Params_Snappbox_Get_Order_List param);
        Result_Snappbox_Get_Account_Balance GetAccountBalance();
        Result_Snappbox_Cancel_Order CancelOrder(Params_Snappbox_Cancel_Order param);

    }
}
