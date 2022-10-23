using Microsoft.AspNetCore.Mvc;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Orders.MultiShipping.Models.Extra_Status_Field_Shipment;
using Nop.Plugin.Orders.MultiShipping.Models.Tbl_Extra_Status_Field_Shipment;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Web.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Controllers
{
    public class HistoryNoteOrderController : BasePublicController
    {
        private readonly IRepository<TblExtraFiledShipment> _repositoryTblExtraFiledShipment;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerService _customerService;
        public HistoryNoteOrderController
            (
            IRepository<TblExtraFiledShipment> repositoryTblExtraFiledShipment,

            IRepository<OrderNote> orderNoteRepository,
            IPermissionService permissionService,
            ICustomerService customerService
            )
        {
            _repositoryTblExtraFiledShipment = repositoryTblExtraFiledShipment;
            _orderNoteRepository = orderNoteRepository;
            _permissionService = permissionService;
            _customerService = customerService;
        }

        [Area(AreaNames.Admin)]
        [AdminAntiForgery]
        [AuthorizeAdmin]
        [HttpPost]
        [HttpPost]
        public virtual IActionResult HistoryList(int type, int ShippingId)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();

            List<TblExtraFiledShipment> Tbl_History = new List<TblExtraFiledShipment>();
            var gridModel = new DataSourceResult();
            var Final_history = new List<GridHistory>();
            try
            {
                Tbl_History = _repositoryTblExtraFiledShipment.Table.Where(p => p.ShippingId == ShippingId && p.Type == type).ToList().OrderByDescending(p => p.Id).ToList();
                if (Tbl_History.Count > 0)
                {
                    Final_history = Tbl_History.Select(q => new GridHistory()
                    {
                        Name = q.Type == 1 ? "مفقود شده" : q.Type == 2 ? "غرامت خسارت" : q.Type == 3 ? "غرامت مفقودی" : q.Type == 4 ? "غرامت تاخیر" : q.Type == 5 ? "شکایت" : "",
                        Status = q.value == 1 ? "دارد" : "برطرف شده",
                        Text = _orderNoteRepository.GetById(q.OrderNoteId).Note,
                        User = _customerService.GetCustomerById((int)q.IdUserInsert).GetFullName(),
                        Date = q.DateInsert,
                    }).ToList(); ;

                }
                else
                {
                    ErrrorMassege = "اطلاعاتی یافت نشد";
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                ErrrorMassege = "خطا در زمان واکشی اطلاعات";
            }
            finally
            {
                gridModel = new DataSourceResult
                {
                    Data = Final_history,
                    Total = Final_history.Count,
                    Errors = ErrrorMassege,
                };
            }
            return Json(gridModel);
        }
    }
}
