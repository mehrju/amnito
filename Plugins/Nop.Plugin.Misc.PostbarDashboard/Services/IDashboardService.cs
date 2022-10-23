using Nop.Plugin.Misc.PostbarDashboard.Models;
using Nop.Services.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Services
{
    public interface IDashboardService
    {
        int InsertRequestCOD(AddRequestCODModel param);
        void UpdateRequestCODFileName(int requestId, string fileName);
        DashboardInfoResult GetCustomerInfo();
        T[] execSp<T>(string procedureName,object obj);
        T[] execSp<T>(object obj);
        T[] execSp<T>(object obj, out string jsonout);
        ChangePasswordResult ChangePassword(ChangePasswordRequest request);
    }
}
