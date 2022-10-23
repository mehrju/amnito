using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{
    public class UserStatesController : BaseAdminController
    {
        private readonly IPermissionService _permissionService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IUserStatesService _userStatesService;
        private readonly ICustomerService _customerService;
        private readonly ICountryService _countryService;
        public UserStatesController(IPermissionService permissionService,
            IStateProvinceService stateProvinceService
            , IUserStatesService userStatesService
            , ICustomerService customerService
            , ICountryService countryService)
        {
            _stateProvinceService = stateProvinceService;
            _permissionService = permissionService;
            _userStatesService = userStatesService;
            _customerService = customerService;
            _countryService = countryService;
        }

        public IActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();
            UserStetesModel model = new UserStetesModel();
            model.AvailableCountries = new List<SelectListItem>();
            model.AvailableCountries.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var c in _countryService.GetAllCountries())
                model.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });


            model.AvailableCustomer = new List<SelectListItem>();
            model.AvailableCustomer.Add(new SelectListItem { Text = "*", Value = "0" });

            model.AvailableRoles = new List<SelectListItem>();
            model.AvailableRoles.Add(new SelectListItem { Text = "*", Value = "0" });
            foreach (var c in _customerService.GetAllCustomerRoles())
                model.AvailableRoles.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });

            return View("/Plugins/Orders.ExtendedShipment/Views/UserStates.cshtml", model);
        }
        public IActionResult getStateByCountry(int countryId, int customerId)
        {
            if (countryId == 0)
                return new EmptyResult();
            var userStates = new List<int>();
            if (customerId != 0)
            {
                userStates = _userStatesService.getUserStates(customerId);
            }
            var states = _stateProvinceService.GetStateProvincesByCountryId(countryId).Select(p => new
            {
                p.Name,
                p.Id,
                selected = userStates.Any() && (userStates.Any(n => n == p.Id))
            }).ToList();
            return Json(states);
        }
        public IActionResult getCustomerInRole(int roleId)
        {
            var customers = _customerService.GetAllCustomers(customerRoleIds: new int[]{ roleId }).Select(p => new SelectListItem()
            {
                Text = p.Username + "-" + p.Email,
                Value = p.Id.ToString()
            }).OrderBy(p => p.Text);
            return Json(customers);
        }
        [HttpPost]
        public IActionResult saveUserState(int customerId,int[] states,int CountryId)
        {
            if (_userStatesService.Insert(customerId, states.ToList(),CountryId))
                return Json(new { success = true, message = "عملیات ثبت با موفقیت انجام شد" });
            return Json(new {success = false, message = "خطا در زمان ثبت ، لطفا لاگ سیستم را بررسی کنید"});
        }
    }
}
