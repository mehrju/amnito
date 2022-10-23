using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Plugin.Misc.ShippingSolutions.Domain;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Nop.Plugin.Misc.ShippingSolutions.Controllers
{

    public class ManageOfficeController : BaseAdminController
    {
        private readonly IRepository<Tbl_ServicesProviders> _repositoryTbl_ServicesProviders;
        private readonly IRepository<Tbl_Collectors> _repositoryTbl_Collectors;
        private readonly IRepository<Tbl_Offices> _repositoryTbl_Offices;
        private readonly IRepository<Tbl_WorkingTime> _repositoryTbl_WorkingTime;
        private readonly IRepository<Address> _repositoryTbl_Address;
        private readonly IAddressService _addressService;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerService _customerService;
        private readonly IStateProvinceService _StateProvinceService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IStoreService _storeService;
        private readonly IAddressService _AddressService;
        private readonly IRepository<Tbl_Address_LatLong> _repositoryTbl_Address_LatLong;
        
        public ManageOfficeController
            (
            IAddressService addressService,
             IWorkContext workContext,
            IDbContext dbContext,
            IPermissionService permissionService,
            IStaticCacheManager cacheManager,
            IStateProvinceService StateProvinceService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ICustomerService customerService,
            IStoreService storeService,
            IRepository<Tbl_ServicesProviders> repositoryTbl_ServicesProviders,
            IRepository<Tbl_Collectors> repositoryTbl_Collectors,
            IRepository<Tbl_Offices> repositoryTbl_Offices,
            IRepository<Tbl_WorkingTime> repositoryTbl_WorkingTime,
            IRepository<Address> repositoryTbl_Address,
            IRepository<Tbl_Address_LatLong> repositoryTbl_Address_LatLong

            )
        {
            _repositoryTbl_Address_LatLong = repositoryTbl_Address_LatLong;
            _addressService = addressService;
            _workContext = workContext;
            _dbContext = dbContext;
            _permissionService = permissionService;
            _cacheManager = cacheManager;
            _StateProvinceService = StateProvinceService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _customerService = customerService;
            _storeService = storeService;
            _repositoryTbl_ServicesProviders = repositoryTbl_ServicesProviders;
            _repositoryTbl_Collectors = repositoryTbl_Collectors;
            _repositoryTbl_Offices = repositoryTbl_Offices;
            _repositoryTbl_WorkingTime = repositoryTbl_WorkingTime;
            _repositoryTbl_Address = repositoryTbl_Address;
        }


        public virtual IActionResult EditOffice(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Office = _repositoryTbl_Offices.GetById(id);
            if (Office == null || Office.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("Edit", new { id = Office.CollectorId });
            var state_city = _StateProvinceService.GetStateProvinceById(Office.StateProvinceId);
            var NameCollector = _repositoryTbl_Collectors.GetById(Office.CollectorId);
            var model = new Tbl_Offices();
            model = Office;
            model.NameOffice = " " + state_city.Country.Name + " : " + state_city.Name + " : " + NameCollector.CollectorName;

            //model.NameOffice = "دفتر-استان:" + state_city.Country.Name + "شهر:" + state_city.Name + " جمع آورکننده:" + NameCollector.CollectorName;
            #region user
            //if (Office.CustomerID > 0)
            //{
            //    var Users = _customerService.GetCustomerById(Office.CustomerID);
            //    if (Users != null)
            //    {


            //        model.ListCustomer.Add(new SelectListItem { Text = Users.GetFullName(), Value = Users.Id.ToString(), Selected = true });


            //    }
            //    else
            //    {
            //        model.ListCustomer.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            //    }
            //}



            #endregion
            #region address
            if (Office.AddressId != null)
            {
                Address address = _addressService.GetAddressById(Office.AddressId.GetValueOrDefault());
                if (address != null)
                {
                    model.DetailAddress = address.Address1;
                    model.fNameAddress = address.FirstName;
                    model.lNameAddress = address.LastName;
                    model.EmailAddress = address.Email;
                    model.MobileAddress = address.PhoneNumber;

                }


                // lat & long
                var latlon = _repositoryTbl_Address_LatLong.Table.Where(p => p.AddressId == Office.AddressId).OrderByDescending(p => p.Id).FirstOrDefault();
                if (latlon != null)
                {
                    model.Lat = latlon.Lat;
                    model.Long = latlon.Long;
                }

            }
         

            //var AddressinIdCity_user = (dynamic)null;
            //int IdUser = 0;


            //if (Office.TypeOffice == true)
            //{
            //    IdUser = _repositoryTbl_Collectors.Table.Where(p => p.Id == Office.CollectorId).Select(p => p.UserId).FirstOrDefault();
            //    var ListAddressUser = _customerService.GetCustomerById(IdUser).Addresses;
            //    if (ListAddressUser.Count > 0)
            //        AddressinIdCity_user = ListAddressUser.Where(p => p.StateProvinceId == Office.StateProvinceId).ToList();

            //}
            //else
            //{
            //    var AllListAdress = _repositoryTbl_Address.Table.ToList();
            //    if (AllListAdress.Count > 0)
            //        AddressinIdCity_user = AllListAdress.Where(p => p.StateProvinceId == Office.StateProvinceId).ToList();

            //}

            //if (AddressinIdCity_user != null)
            //{
            //    model.ListAddress.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
            //    foreach (var item in AddressinIdCity_user)
            //    {

            //        model.ListAddress.Add(new SelectListItem { Text = item.Country.Name + "-"+item.StateProvince.Name + "-" + item.City + "-" + item.Address1 + "-" + item.ZipPostalCode + "-" + item.PhoneNumber, Value = item.Id.ToString() });

            //    }
            //}
            //else
            //{
            //    model.ListAddress.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            //}



            #endregion
            #region Time
            var Times = _repositoryTbl_WorkingTime.Table.Where(p => p.OfficeId == Office.Id).ToList();
            foreach (var itemT in Times)
            {
                switch (itemT.DayName)
                {
                    case "Saturday":
                        model.Saturday_EndTime = itemT.EndTime;
                        model.Saturday_StartTime = itemT.StartTime;
                        break;

                    case "Sunday":
                        model.Sunday_EndTime = itemT.EndTime;
                        model.Sunday_StartTime = itemT.StartTime;

                        break;
                    case "Monday":
                        model.Monday_EndTime = itemT.EndTime;
                        model.Monday_StartTime = itemT.StartTime;

                        break;
                    case "Tuesday":
                        model.Tuesday_EndTime = itemT.EndTime;
                        model.Tuesday_StartTime = itemT.StartTime;

                        break;
                    case "Wednesday":
                        model.Wednesday_EndTime = itemT.EndTime;
                        model.Wednesday_StartTime = itemT.StartTime;

                        break;
                    case "Thursday":
                        model.Thursday_EndTime = itemT.EndTime;
                        model.Thursday_StartTime = itemT.StartTime;

                        break;
                    case "Friday":
                        model.Friday_EndTime = itemT.EndTime;
                        model.Friday_StartTime = itemT.StartTime;

                        break;


                    default:
                        break;
                }
            }
            #endregion
            return View("/Plugins/Misc.ShippingSolutions/Views/Office/Edit.cshtml", model);
        }



        [Area(AreaNames.Admin)]
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(Tbl_Offices model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Office = _repositoryTbl_Offices.GetById(model.Id);
            if (Office == null || Office.IsActive == false)
            {
                if (Office.TypeOffice == true)
                {
                    //collector
                    return RedirectToAction("Edit", new RouteValueDictionary(
                        new { controller = "ManageCollector", action = "Edit", id = Office.CollectorId }));

                }
                else
                {
                    return RedirectToAction("Edit", new RouteValueDictionary(
                      new { controller = "ManageServiceProvider", action = "Edit", id = Office.CollectorId }));

                }
            }

            //add address
            Address address = new Address();
            address.FirstName = model.fNameAddress;
            address.LastName = model.lNameAddress;
            address.PhoneNumber = model.MobileAddress;
            address.Email = model.EmailAddress;
            address.Address1 = model.DetailAddress;
            _addressService.InsertAddress(address);

            //lat lonf
            Tbl_Address_LatLong templatlon = new Tbl_Address_LatLong();
            templatlon.Lat = model.Lat;
            templatlon.Long = model.Long;
            templatlon.AddressId = address.Id;
            _repositoryTbl_Address_LatLong.Insert(templatlon);

            //update
            Office.WarehouseState = model.WarehouseState;
            Office.HolidaysState = model.HolidaysState;
            Office.AddressId = address.Id;
            Office.WarehouseAddress = model.WarehouseAddress;
            Office.Lat = model.Lat;
            Office.Long = model.Long;

            Office.DateUpdate = DateTime.Now;
            Office.IdUserUpdate = _workContext.CurrentCustomer.Id;
            _repositoryTbl_Offices.Update(Office);

            var Times = _repositoryTbl_WorkingTime.Table.Where(p => p.OfficeId == Office.Id).ToList();
            foreach (var itemT in Times)
            {
                switch (itemT.DayName)
                {
                    case "Saturday":
                        itemT.StartTime = model.Saturday_StartTime;
                        itemT.EndTime = model.Saturday_EndTime;
                        _repositoryTbl_WorkingTime.Update(itemT);
                        break;

                    case "Sunday":
                        itemT.StartTime = model.Sunday_StartTime;
                        itemT.EndTime = model.Sunday_EndTime;
                        _repositoryTbl_WorkingTime.Update(itemT);

                        break;
                    case "Monday":
                        itemT.StartTime = model.Monday_StartTime;
                        itemT.EndTime = model.Monday_EndTime;
                        _repositoryTbl_WorkingTime.Update(itemT);

                        break;
                    case "Tuesday":
                        itemT.StartTime = model.Tuesday_StartTime;
                        itemT.EndTime = model.Tuesday_EndTime;
                        _repositoryTbl_WorkingTime.Update(itemT);

                        break;
                    case "Wednesday":
                        itemT.StartTime = model.Wednesday_StartTime;
                        itemT.EndTime = model.Wednesday_EndTime;
                        _repositoryTbl_WorkingTime.Update(itemT);

                        break;
                    case "Thursday":
                        itemT.StartTime = model.Thursday_StartTime;
                        itemT.EndTime = model.Thursday_EndTime;
                        _repositoryTbl_WorkingTime.Update(itemT);

                        break;
                    case "Friday":
                        itemT.StartTime = model.Friday_StartTime;
                        itemT.EndTime = model.Friday_EndTime;
                        _repositoryTbl_WorkingTime.Update(itemT);

                        break;


                    default:
                        break;
                }
            }

            //activity log
            _customerActivityService.InsertActivity("EditOffice", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"), "Office Id:" + Office.Id.ToString());

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"));
            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("EditOffice", new { id = Office.Id });
            }
            else
            {
                if (Office.TypeOffice == true)
                {
                    //collector
                    return RedirectToAction("Edit", new RouteValueDictionary(
                        new { controller = "ManageCollector", action = "Edit", id = Office.CollectorId }));

                }
                else
                {
                    return RedirectToAction("Edit", new RouteValueDictionary(
                      new { controller = "ManageServiceProvider", action = "Edit", id = Office.CollectorId }));

                }
            }

        }

    }
}
