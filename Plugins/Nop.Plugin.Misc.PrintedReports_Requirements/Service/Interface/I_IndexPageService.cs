using Nop.Plugin.Misc.PrintedReports_Requirements.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PrintedReports_Requirements.Service.Interface
{
    public interface I_IndexPageService
    {
        vmSlidShow_Index GetSlidshowAsync();
        vmServiceProvider_Index GetServiceProvider();
    }
}
