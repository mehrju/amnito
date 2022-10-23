using System.Collections.Generic;
using System.IO;
using Nop.Plugin.Misc.PostbarDashboard.Models;
using Nop.Plugin.Misc.Ticket.Domain;
using Nop.Services.Orders;

namespace Nop.Plugin.Misc.PostbarDashboard.Services
{
    public interface ICODRequestService
    {
        void GetCODRequestPdf(Stream stream, string cityName,
            string customerFullName, string customerSenderAddress,
            string customerPostalCode,string code);
        void GetCODRequestExcel(Stream stream, string customerFullName,
            string customerAddress, string customerTel,
            string customerMobile, string customerPostalCode);
        PlaceOrderResult ProcessCODRequestOrder(Tbl_RequestCODCustomer newRequestCOD, string paymentMethod);
        void CODRequestPaid(Tbl_RequestCODCustomer requestCOD, bool insertTicket = true);
        bool CustomerHasActiveCOD(int customerId);
        FullCODRequestResultModel FullCODRequest(AddRequestCODModel codModel);
        List<FullCODRequestResultModel> BulkFullCODRequest(List<AddRequestCODModel> codModelList);
    }
}