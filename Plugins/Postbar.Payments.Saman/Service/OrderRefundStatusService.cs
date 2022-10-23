using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Services.Events;
using Nop.Services.Logging;
using NopFarsi.Payments.SepShaparak.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NopFarsi.Payments.SepShaparak.ir.sep.srtm;

namespace NopFarsi.Payments.SepShaparak.Service
{
    public class OrderRefundStatusService : IOrderRefundStatusService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<OrderRefundStatus> _orderRefundStatusRepository;
        private readonly ICacheManager _cacheManager;
        private readonly sepsettings _sepPaymentSettings;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="eventPublisher">Event publisher</param>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="taxRateRepository">Tax rate repository</param>
        public OrderRefundStatusService(
            IEventPublisher eventPublisher,
            ICacheManager cacheManager,
            IRepository<OrderRefundStatus> orderRefundStatusRepository,
            sepsettings sepPaymentSettings,
            ILogger logger)
        {
            _eventPublisher = eventPublisher;
            _cacheManager = cacheManager;
            _orderRefundStatusRepository = orderRefundStatusRepository;
            _sepPaymentSettings = sepPaymentSettings;
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts a tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        public virtual void InsertOrderRefundStatus(OrderRefundStatus orderRefundStatus)
        {
            if (orderRefundStatus == null)
                throw new ArgumentNullException(nameof(orderRefundStatus));

            _orderRefundStatusRepository.Insert(orderRefundStatus);

            //event notification
            _eventPublisher.EntityInserted(orderRefundStatus);
        }

        public bool IsSuccessfulRefund(int orderId)
        {
            var orderRefundStatus = _orderRefundStatusRepository.Table.FirstOrDefault(x => x.OrderId == orderId);
            if (orderRefundStatus == null)
            {
                return false;
            }
            if (orderRefundStatus.RefundStatus == RefundStatus.Ok)
            {
                return true;
            }

            LoggingExtensions.Information(_logger, $"GetRefundStatus with params userName:{_sepPaymentSettings.RefundUserName}, password:{_sepPaymentSettings.RefundPassword}," +
                $"refundId:{orderRefundStatus.RefundRefrenceId}, termId:{_sepPaymentSettings.TransactionTermId} Time:{DateTime.Now}", null, null);

            srvRefund srvRefund = new srvRefund();

            var result = srvRefund.GetRefundStatus(
                userName: _sepPaymentSettings.RefundUserName,
                password: _sepPaymentSettings.RefundPassword,
                refundId: orderRefundStatus.RefundRefrenceId,
                refundIdSpecified: true,
                termId: _sepPaymentSettings.TransactionTermId,
                termIdSpecified: true);
            LoggingExtensions.Information(_logger, $"GetRefundStatus -> ActionName:{result.ActionName} Description: {result.Description} ErrorCode:{result.ErrorCode} ErrorMessage:{result.ErrorMessage} RequestStatus:{(RefundStatus)result.RequestStatus} ReferenceId:{result.ReferenceId}", null, null);

            if (result.ErrorCode == ErrCode.Success)
            {
                orderRefundStatus.RefundStatus = (RefundStatus)result.RequestStatus;
                _orderRefundStatusRepository.Update(orderRefundStatus);
                return orderRefundStatus.RefundStatus == RefundStatus.Ok;
            }
            else
            {
                LoggingExtensions.Information(_logger, $"OrderId:{orderRefundStatus.OrderId} RefundRefrenceId:{orderRefundStatus.RefundRefrenceId} " +
                    $"result:{result.ActionName}-{result.Description}-{result.ErrorCode}-{result.ErrorMessage}-{result.RequestStatus}", null, null);
            }
            return false;
        }


        #endregion

    }
}
