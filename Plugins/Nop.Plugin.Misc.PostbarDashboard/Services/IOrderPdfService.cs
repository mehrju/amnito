using Nop.Plugin.Misc.PostbarDashboard.Models;
using System.Collections.Generic;
using System.IO;

namespace Nop.Plugin.Misc.PostbarDashboard.Services
{
    public interface IOrderPdfService
    {
         void GetOrderBarcodePdf(IList<OrderTrackingBarcode> orders, OrderSearchCondition searchCondition, Stream stream);
         void GetOrderFactorPdf(IList<DashboardFactorRequestMRTModel> orders, Stream stream);
    }
}