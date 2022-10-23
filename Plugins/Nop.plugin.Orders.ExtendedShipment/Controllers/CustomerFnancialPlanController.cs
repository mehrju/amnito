using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Misc.ShippingSolutions.Domain;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{
    [AdminAntiForgery(true)]
    public class CustomerFnancialPlanController : BaseAdminController
    {

        private readonly IWorkContext _workContext;
        private readonly IRepository<Tbl_AffiliateToCustomer> _repositoryTbl_AffiliateToCustomer;
        private readonly IRepository<Tbl_IncentivePlanCustomer> _repositoryTbl_IncentivePlanCustomer;
        private readonly IRepository<Tbl_DiscountPlan_AgentCustomer> _repositoryTbl_DiscountPlan_AgentCustomer;
        private readonly ICustomerService _customerServices;
        private readonly IRepository<Affiliate> _repositoryAffiliate;
        private readonly IAddressService _AddressService;
        private readonly IRepository<Address> _repositoryTbl_Address;
        private readonly IRepository<Tbl_CustomerDepositCode> _repository_TblCustomerDepositCode;
        private readonly IContractService _contractService;

        public CustomerFnancialPlanController(

            IWorkContext workContext,
            IRepository<Tbl_IncentivePlanCustomer> repositoryTbl_IncentivePlanCustomer,
            IRepository<Tbl_AffiliateToCustomer> repositoryTbl_AffiliateToCustomer,
            IRepository<Tbl_DiscountPlan_AgentCustomer> repositoryTbl_DiscountPlan_AgentCustomer,
            ICustomerService customerServices,
            IRepository<Affiliate> repositoryAffiliate,
            IAddressService AddressService,
            IRepository<Address> repositoryTbl_Address,
            IRepository<Tbl_CustomerDepositCode> repository_TblCustomerDepositCode,
            IContractService contractService
            )
        {
            _workContext = workContext;
            _repositoryTbl_IncentivePlanCustomer = repositoryTbl_IncentivePlanCustomer;
            _repositoryTbl_AffiliateToCustomer = repositoryTbl_AffiliateToCustomer;
            _repositoryTbl_DiscountPlan_AgentCustomer = repositoryTbl_DiscountPlan_AgentCustomer;
            _customerServices = customerServices;
            _repositoryAffiliate = repositoryAffiliate;
            _AddressService = AddressService;
            _repositoryTbl_Address = repositoryTbl_Address;
            _repository_TblCustomerDepositCode = repository_TblCustomerDepositCode;
            _contractService = contractService;
        }
        [HttpGet]
        public IActionResult GetDiscountPlan()
        {
            var listDeps = _repositoryTbl_DiscountPlan_AgentCustomer.Table.Where(p => p.IsActive == true && p.IsAgent == false).Select(p => new
            {
                Value = p.Id,
                Text = p.Name + " از میلغ:" + p.OfAmount.ToString() + " تا مبلغ:" + p.UpAmount.ToString() + " درصد تشویق : " + p._Percent.ToString()
            }).ToList();
            return Json(listDeps);
        }

        [HttpGet]
        public IActionResult GetAffliateList()
        {
            var affiliates = _repositoryAffiliate.Table.Where(p => p.Active).ToList();

            var affiliates1 = (from f in affiliates
                               select new
                               {
                                   Value = f.Id,
                                   Text = _repositoryTbl_Address.Table.Where(p => p.Id == f.AddressId).FirstOrDefault().LastName + " " + _repositoryTbl_Address.Table.Where(p => p.Id == f.AddressId).FirstOrDefault().FirstName + "-" + f.FriendlyUrlName
                               }).ToList();

            //var affiliates = (from f in _repositoryAffiliate.Table.ToList()
            //                  join a in _repositoryTbl_Address.Table.ToList() on f.AddressId equals a.Id
            //                  where f.Active
            //                  select new
            //                  {
            //                      Value = f.Id,
            //                      Text = a.LastName + " " + a.FirstName + "-" + f.FriendlyUrlName
            //                  }).ToList();
            return Json(affiliates1);

        }

        [HttpPost]
        public IActionResult GetCurrentDiscountPlan(int customerId)
        {
            var depositCode = _repository_TblCustomerDepositCode.Table.FirstOrDefault(p => p.CustomerId == customerId)?.DepositCode;
            var c = _repositoryTbl_IncentivePlanCustomer.Table.Where(p => p.CustomerId == customerId).OrderByDescending(p => p.Id).FirstOrDefault();

            if (c != null)
            {
                //    s = (c.IsActiveIncentivePlan ? "فعال" : "غیرفعال") + ":";
                //    if (c.IdDiscountPlan != 0)
                //    {
                //    s = _repositoryTbl_DiscountPlan_AgentCustomer.GetById(c.IdDiscountPlan).Name + ":" ;

                //    }
                //s += (c.IsAutomaticIncentivePlan ? "اتوماتیک" : "دستی");
                //
                //
                string s1 = c.IsActiveIncentivePlan == true ? "1" : "0";
                string s2 = c.IsAutomaticIncentivePlan == true ? "1" : "0";
                return Json(new { success = true, statusplan = s1, statusauto = s2, idplan = c.IdDiscountPlan, depositCode = depositCode });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    statusplan = "0",
                    statusauto = "0",
                    idplan = 0,
                    depositCode = depositCode
                });
            }


        }

        [HttpPost]
        public IActionResult GetCurrentAffliate(int customerId)
        {

            var c = _repositoryTbl_AffiliateToCustomer.Table.Where(p => p.CustomerId == customerId).OrderByDescending(p => p.Id).FirstOrDefault();
            if (c != null)
            {
                return Json(new { statusAffiliate = c.IsActive, idAffiliate = c.AffiliateId });
            }
            else
            {
                return Json(new
                {
                    statusAffiliate = false,
                    idAffiliate = 0
                });
            }


        }

        [HttpPost]
        [AdminAntiForgery(true)]
        public IActionResult RegisterDiscountPlan(bool _isactive, bool _isauto, int _idDiscount, int _CustomerId)
        {
            try
            {
                Tbl_IncentivePlanCustomer temp = new Tbl_IncentivePlanCustomer();
                temp.IdDiscountPlan = _idDiscount;
                temp.IsActiveIncentivePlan = _isactive;
                temp.IsAutomaticIncentivePlan = _isauto;
                temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                temp.DateInsert = DateTime.Now;
                temp.CustomerId = _CustomerId;
                _repositoryTbl_IncentivePlanCustomer.Insert(temp);
                return Json(new { success = true });
            }
            catch (Exception)
            {

                return Json(new { success = false });
            }



        }

        [HttpPost]
        [AdminAntiForgery(true)]
        public IActionResult RegisterAfflaite(bool _isactive, int _idAfflaite, int _CustomerId)
        {
            try
            {
                Tbl_AffiliateToCustomer temp = new Tbl_AffiliateToCustomer();
                temp.AffiliateId = _idAfflaite;
                temp.IsActive = _isactive;
                temp.registerUserId = _workContext.CurrentCustomer.Id;
                temp.registerDate = DateTime.Now;
                temp.CustomerId = _CustomerId;
                temp.LastActiveUser = _workContext.CurrentCustomer.Id;
                temp.LastActiveDate = DateTime.Now;
                _repositoryTbl_AffiliateToCustomer.Insert(temp);
                return Json(new { success = true });
            }
            catch (Exception)
            {

                return Json(new { success = false });
            }



        }
        public IActionResult CustomerContractPlansIndex()
        {
            return View("~/Plugins/Orders.ExtendedShipment/Views/_CustomerContractPlans.cshtml");
        }
        [HttpPost]
        [AdminAntiForgery(true)]
        public IActionResult SaveContract(ContractItems model)
        {
            string resultMessage = "";
            int ContractId = _contractService.saveContract(model, out resultMessage);
            return Json(new { success = ContractId > 0, message = resultMessage, contractId = ContractId });
        }
        [HttpPost]
        [AdminAntiForgery(true)]
        public IActionResult getCustomerContractPakcingItems(int ContractItemId_fk)
        {
            var gridModel = new DataSourceResult
            {
                Data = new List<CustomerContractPakcingItems>(),
                Total = 0
            };
            return Ok(gridModel);
        }
    }

}
