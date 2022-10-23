using BS.Plugin.Orders.ExtendedShipment.Models.Optime.Authenticate;
using BS.Plugin.Orders.ExtendedShipment.Models.Optime.GetToolResponseCluster;
using BS.Plugin.Orders.ExtendedShipment.Models.Optime.ToolApiCall;

namespace BS.Plugin.Orders.ExtendedShipment.Services
{
    public interface IOptimeApiService
    {
        bool Authenticate(string userName, string password);
        CallResponseModel NewPlan(CallRequestModel model);
        bool ExecuteSelectedTask(string planIdOrToken);

        /// <summary>
        /// get optimized clusters 
        /// </summary>
        /// <param name="ListName">enter you token that you got from ToolApiCall Response</param>
        /// <param name="Vehicle">1-	ALL // get all vehicle with no filter
        /// 2-	car // just get car’s clusters
        /// 3-	bike // just get bike’s clusters 
        /// 4-	truck
        /// 5-	transit
        /// </param>
        ResponseModel GetToolResponseCluster(string token);
        CallRequestModel getCollectorAgentList(int CustomerId);
        int InsertCollectorAgentList(CallRequestModel model, string Token, int CollectorCustomerId, bool isFake, int? phoneOrderId);
        //void CheckInProcesingList();
        void CheckForOptimizedTask();
        void SendForOptimizeRout();
        bool Authenticate(int collectorCustomerId);
        bool Delete(int collectorCustomerId, string planIdOrToken);
        CallResponseModel NewFakePlanForPhoneOrder(int collectorCustomerId, CallRequestModel model, int phoneOrderId);

    }
}
