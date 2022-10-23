using Nop.Core.Data;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.plugin.Orders.ExtendedShipment.Models.PreOrderModel;
using System;
using System.Linq;


namespace Nop.plugin.Orders.ExtendedShipment.Services.PreOrderService
{
    public class PreOrderService : IPreOrderService
    {
        private readonly IRepository<Tb_preOrder> _repositoryPreOrder;
        private readonly IDbContext _dbContext;
        private readonly INotificationService _notificationService;
        public PreOrderService(
            IRepository<Tb_preOrder> repositoryPreOrder,
            INotificationService notificationService,
            IDbContext dbContext)
        {
            _notificationService = notificationService;
            _repositoryPreOrder = repositoryPreOrder;
            _dbContext = dbContext;
        }
        public int InsertPreOrder(CheckoutParcellModel Model, int CustomerId, out string Error, out string url)
        {
            Error = "";
            url = "";
            try
            {
                var result = _repositoryPreOrder.Table.Where(p => p.CustomerId == CustomerId && p.UniqRefrenceId == Model.UniqueReferenceNo).FirstOrDefault();
                if (result != null)
                {
                    return result.Id;
                }
                string PreOrderJson = Newtonsoft.Json.JsonConvert.SerializeObject(Model);
                if (string.IsNullOrEmpty(PreOrderJson))
                {
                    Error = "داده های ورودی دارای اشکال می باشد";
                    return 0;
                }
                Tb_preOrder preOrder = new Tb_preOrder()
                {
                    CustomerId = CustomerId,
                    OrderDate = DateTime.Now,
                    PreOrderJson = PreOrderJson,
                    UniqRefrenceId = Model.UniqueReferenceNo
                };
                _repositoryPreOrder.Insert(preOrder);
                if (preOrder.Id > 0)
                {
                    url = "https://postex.ir/NewCheckout/PreOrderInfo?PreOrderId=" + preOrder.Id;
                    string text = $@"متقاضی محترم کارت بازرگانی، درخواست شما جهت ارسال پستی کارت بازرگانی ثبت گردید. لطفا جهت تایید و پرداخت به آدرس زیر مراجعه نمایید.";
                    text += url;
                    _notificationService._sendSms(Model.ParcellList.First().ReceiverAddress.PhoneNumber, text);
                }
                return preOrder.Id;
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                Error = "بروز خطای نامشخص";
                return 0;
            }
        }

        public CheckoutParcellModel PreOrderCheckout(int PreOrderId)
        {
            var obj = _repositoryPreOrder.GetById(PreOrderId);
            var model = Newtonsoft.Json.JsonConvert.DeserializeObject<CheckoutParcellModel>(obj.PreOrderJson);
            return model;
        }

        public bool SetOrderId(string UniqueReferenceNo, int OrderId)
        {
            string query = $@"UPDATE dbo.Tb_preOrder 
                                SET OrderId = { OrderId }
                             WHERE JSON_VALUE(PreOrderJson, '$.UniqueReferenceNo') LIKE { UniqueReferenceNo }";
            return _dbContext.SqlQuery<bool?>(query).FirstOrDefault().GetValueOrDefault(false);
        }
    }
}
