using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces
{
    public interface ICheckRegionDesVijePost
    {
        bool CheckValidSourceDistination(int SenderCountrId, int SenderStateId, int ReciverCountrId, int ReciverStateId);
        void fill();
        bool CheckValidSourceForInternationalPost(int SenderCountrId, int SenderStateId);
    }
}
