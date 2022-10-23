using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PrintedReports_Requirements.Service.Interface
{
    interface IGetByteAvatarCustomer_Service
    {
        (bool, byte[]) GetByteAvatarCustomer(int CustomerId);
    }
}
