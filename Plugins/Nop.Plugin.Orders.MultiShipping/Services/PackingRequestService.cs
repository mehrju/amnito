using Nop.Core.Data;
using Nop.Plugin.Orders.MultiShipping.Domain;
using Nop.Plugin.Orders.MultiShipping.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public class PackingRequestService : IPackingRequestService
    {
        private readonly IRepository<Tbl_ShipmentPackingRequest> _repository;

        public PackingRequestService(IRepository<Tbl_ShipmentPackingRequest> repository)
        {
            _repository = repository;
        }

        public bool ShipmentHasPackingRequest(int shipmentId)
        {
            return _repository.Table.Any(p => p.ShipmentId == shipmentId && p.IsSmsSent);
        }

        public void InsertPackingRequest(Tbl_ShipmentPackingRequest shipmentPackingRequest)
        {
            if (shipmentPackingRequest == null)
            {
                throw new ArgumentNullException(nameof(shipmentPackingRequest));
            }
            _repository.Insert(shipmentPackingRequest);
        }

        public void UpdatePackingRequest(Tbl_ShipmentPackingRequest shipmentPackingRequest)
        {
            if (shipmentPackingRequest == null)
            {
                throw new ArgumentNullException(nameof(shipmentPackingRequest));
            }
            _repository.Update(shipmentPackingRequest);
        }

        public Tbl_ShipmentPackingRequest GetPackingRequestById(int id)
        {
            return _repository.GetById(id);
        }

        public bool IsRequestedPackingPurchased(int shipmentId, List<CartonSizeSelected> cartonSizes, out string msg)
        {
            var packingRequest = SearchPackingRequests(shipmentId).OrderByDescending(p => p.Id).FirstOrDefault();
            if (packingRequest != null)
            {
                if (!cartonSizes.Any(p => p.Name == packingRequest.KartonSizeItemName))
                {
                    msg = $"برای این مرسوله شما باید بسته بندی نوع {packingRequest.KartonSizeItemName} را خریداری کنید";
                    return false;
                }
            }
            msg = "";
            return true;
        }

        public IEnumerable<Tbl_ShipmentPackingRequest> SearchPackingRequests(int shipmentId = 0,string kartonSizeItemName = null,
            int? length = null,int? width = null,int? height = null,
            ShipmentPackingRequestStatus? packingRequestStatus = null,bool? isSmsSent = null)
        {
            var query = _repository.Table;
            if(shipmentId > 0)
            {
                query = query.Where(p => p.ShipmentId == shipmentId);
            }
            if(!string.IsNullOrEmpty( kartonSizeItemName))
            {
                query = query.Where(p => p.KartonSizeItemName == kartonSizeItemName);
            }
            if (length.HasValue)
            {
                query = query.Where(p => p.Length == length);
            }
            if (height.HasValue)
            {
                query = query.Where(p => p.Height == height);
            }
            if (width.HasValue)
            {
                query = query.Where(p => p.Width == width);
            }
            if (packingRequestStatus.HasValue)
            {
                query = query.Where(p => p.Status == packingRequestStatus);
            }
            if (isSmsSent.HasValue)
            {
                query = query.Where(p => p.IsSmsSent == isSmsSent);
            }

            return query.ToList();
        }
    }
}
