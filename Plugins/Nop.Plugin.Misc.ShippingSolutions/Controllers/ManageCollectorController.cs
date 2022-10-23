using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Plugin.Misc.ShippingSolutions.Domain;
using Nop.Plugin.Misc.ShippingSolutions.Models.Grid;
using Nop.Plugin.Misc.ShippingSolutions.Models.Search;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Controllers
{
    [AdminAntiForgery(true)]
    public class ManageCollectorController : BaseAdminController
    {
        private readonly IRepository<Tbl_ServicesProviders> _repositoryTbl_ServicesProviders;
        private readonly IRepository<Tbl_Collectors> _repositoryTbl_Collectors;
        private readonly IRepository<Tbl_CollectoreStores> _repositoryTbl_CollectoreStores;
        private readonly IRepository<Tbl_CollectorsServiceProvider> _repositoryTbl_CollectorsServiceProvider;
        private readonly IRepository<Tbl_Offices> _repositoryTbl_Offices;
        private readonly IRepository<Tbl_WorkingTime> _repositoryTbl_WorkingTime;
        private readonly IRepository<Address> _repositoryTbl_Address;
        private readonly IAddressService _addressService;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerService _customerService;
        private readonly ICategoryService _CategoryService;
        private readonly IStateProvinceService _StateProvinceService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IStoreService _storeService;
        private readonly IAddressService _AddressService;
        private readonly IRepository<Tbl_Address_LatLong> _repositoryTbl_Address_LatLong;

        public IEnumerable<object> ListUsers { get; private set; }

        public ManageCollectorController
            (
             IAddressService addressService,
            IWorkContext workContext,
            IDbContext dbContext,
            IPermissionService permissionService,
            ICategoryService CategoryService,
            IStaticCacheManager cacheManager,
            IStateProvinceService StateProvinceService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ICustomerService customerService,
            IStoreService storeService,
            IRepository<Tbl_ServicesProviders> repositoryTbl_ServicesProviders,
            IRepository<Tbl_Collectors> repositoryTbl_Collectors,
            IRepository<Tbl_CollectoreStores> repositoryTbl_CollectoreStores,
            IRepository<Tbl_CollectorsServiceProvider> repositoryTbl_CollectorsServiceProvider,
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
            _CategoryService = CategoryService;
            _cacheManager = cacheManager;
            _StateProvinceService = StateProvinceService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _customerService = customerService;
            _storeService = storeService;
            _repositoryTbl_ServicesProviders = repositoryTbl_ServicesProviders;
            _repositoryTbl_Collectors = repositoryTbl_Collectors;
            _repositoryTbl_CollectoreStores = repositoryTbl_CollectoreStores;
            _repositoryTbl_CollectorsServiceProvider = repositoryTbl_CollectorsServiceProvider;
            _repositoryTbl_Offices = repositoryTbl_Offices;
            _repositoryTbl_WorkingTime = repositoryTbl_WorkingTime;
            _repositoryTbl_Address = repositoryTbl_Address;
        }
        #region Index & List
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }
        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            //if (!_workContext.CurrentCustomer.IsAdmin())
            //    return AccessDeniedView();

            var Model = new Search_Collector_Model();




            var Providers = _repositoryTbl_ServicesProviders.Table.Where(p => p.IsActive == true).ToList();
            if (Providers.Count > 0)
            {
                Model.ListProvider.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var c in Providers)
                {

                    Model.ListProvider.Add(new SelectListItem { Text = c.ServicesProviderName, Value = c.Id.ToString() });
                }
            }
            else
            {
                Model.ListProvider.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }




            var Cities = _StateProvinceService.GetStateProvinces();
            if (Cities != null && Cities.Count > 0)
            {
                Model.ListOffice_City.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var c in Cities)
                {
                    Model.ListOffice_City.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
                }

            }
            else
            {
                Model.ListOffice_City.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }




            var Stores = _storeService.GetAllStores(true).ToList();
            if (Stores.Count > 0)
            {
                Model.ListStores.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var s in Stores)
                {

                    Model.ListStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
                }
            }
            else
            {
                Model.ListStores.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }



            return View("/Plugins/Misc.ShippingSolutions/Views/Collectors/ListCollectors.cshtml", Model);
        }
        [HttpPost]
        public virtual IActionResult CollectorsList(DataSourceRequest command, Search_Collector_Model model)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();


            List<Tbl_Collectors> Collectors = new List<Tbl_Collectors>();
            List<Tbl_Offices> ListOffice = _repositoryTbl_Offices.Table.Where(p => p.IsActive == true && p.TypeOffice==true).ToList();
            List<Tbl_CollectoreStores> ListStore = _repositoryTbl_CollectoreStores.Table.Where(p => p.IsActive == true).ToList();
            var gridModel = new DataSourceResult();
            var Final_Collectors = (dynamic)null;
            try
            {
                Collectors = _repositoryTbl_Collectors.Table.ToList();
                if (Collectors.Count > 0)
                {
                    if (model.ActiveSearch == true)
                    {
                        #region Search
                        if (!string.IsNullOrEmpty(model.SearchName))
                        {
                            Collectors = Collectors.Where(p => p.CollectorName.Contains(model.SearchName)).ToList();
                        }
                        if (!string.IsNullOrEmpty(model.SearchUserName))
                        {
                            var x = _customerService.GetAllCustomers().Where(p => p.GetFullName().Contains(model.SearchName)).ToList();
                            Collectors = Collectors.Where(p => x.Exists(u => u.Id == p.UserId)).ToList();
                        }
                        Collectors = Collectors.Where(p => p.IsActive == model.SearchIsActive).ToList();

                        if (model.SearchMaxPath > 0)
                        {
                            Collectors = Collectors.Where(p => p.MaxPath == model.SearchMaxPath).ToList();
                        }

                        if (model.SearchMaxWeight > 0)
                        {
                            Collectors = Collectors.Where(p => p.MaxWeight == model.SearchMaxWeight).ToList();
                        }
                        if (model.SearchMinWeight > 0)
                        {
                            Collectors = Collectors.Where(p => p.MaxWeight == model.SearchMinWeight).ToList();
                        }



                        //Collectors = Collectors.Where(p => p.advancefreight == model.Searchadvancefreight).ToList();
                        //Collectors = Collectors.Where(p => p.freightforward == model.Searchfreightforward).ToList();
                        //Collectors = Collectors.Where(p => p.IsPishtaz == model.SearchIsPishtaz).ToList();
                        //Collectors = Collectors.Where(p => p.IsSefareshi == model.SearchIsSefareshi).ToList();
                        //Collectors = Collectors.Where(p => p.IsVIje == model.SearchIsVIje).ToList();
                        //Collectors = Collectors.Where(p => p.IsNromal == model.SearchIsNromal).ToList();
                        //Collectors = Collectors.Where(p => p.IsDroonOstani == model.SearchIsDroonOstani).ToList();
                        //Collectors = Collectors.Where(p => p.IsAdjoining == model.SearchIsAdjoining).ToList();
                        //Collectors = Collectors.Where(p => p.IsNotAdjacent == model.SearchIsNotAdjacent).ToList();
                        //Collectors = Collectors.Where(p => p.IsHeavyTransport == model.SearchIsHeavyTransport).ToList();
                        //Collectors = Collectors.Where(p => p.IsForeign == model.SearchIsForeign).ToList();
                        //Collectors = Collectors.Where(p => p.IsInCity == model.SearchIsInCity).ToList();
                        //Collectors = Collectors.Where(p => p.IsAmanat == model.SearchIsAmanat).ToList();
                        //Collectors = Collectors.Where(p => p.IsTwoStep == model.SearchIsTwoStep).ToList();
                        //Collectors = Collectors.Where(p => p.HasHagheMaghar == model.SearchHasHagheMaghar).ToList();



                        if (model.SearchProviderId > 0)
                        {
                            Collectors = Collectors.Where(p => p.CollectorsServiceProvider.Any(x => x.ProviderId == model.SearchProviderId)).ToList();

                        }

                        if (model.SearchOfficeId > 0)
                        {

                            var temp = (from c in Collectors
                                       join o in ListOffice on c.Id equals o.CollectorId
                                       where o.StateProvinceId == model.SearchOfficeId
                                       select c).Distinct().ToList();

                            Collectors = temp;
                        }
                        if (model.SearchStartTime != null && (model.SearchStartTime.Hours != 0 || model.SearchStartTime.Minutes != 0 || model.SearchStartTime.Seconds != 0))
                        {
                            var temp = (from c in Collectors
                                        join o in ListOffice on c.Id equals o.CollectorId
                                        where o.WorkingTimes.Exists(p=>p.StartTime== model.SearchStartTime)
                                        select c).Distinct().ToList();

                            Collectors = temp;
                            
                        }
                        if (model.SearchEndTime != null && (model.SearchEndTime.Hours != 0 || model.SearchEndTime.Minutes != 0 || model.SearchEndTime.Seconds != 0))
                        {
                            var temp = (from c in Collectors
                                        join o in ListOffice on c.Id equals o.CollectorId
                                        where o.WorkingTimes.Exists(p => p.StartTime == model.SearchEndTime)
                                        select c).Distinct().ToList();

                            Collectors = temp;
                           
                        }
                        if (model.SearchStoreId > 0)
                        {
                            var temp = (from c in Collectors
                                        join s in ListStore on c.Id equals s.CollectorId
                                        where s.StoreId == model.SearchStoreId
                                        select c).Distinct().ToList();
                            Collectors = temp;

                        }
                        #endregion
                    }
                }
                else
                {
                    ErrrorMassege = "No Exsit Data";
                }

                Final_Collectors = (from q in Collectors
                                    select new Grid_Collector_Model
                                    {
                                        Id = q.Id,
                                        Grid_Collector_Name = q.CollectorName,
                                        Grid_Collector_UserName = _customerService.GetCustomerById(q.UserId).GetFullName(),
                                        Grid_Collector_MaxPath = q.MaxPath.ToString(),
                                        Grid_Collector_MaxWeight = q.MaxWeight.ToString(),
                                        Grid_Collector_MinWeight = q.MinWeight.ToString(),
                                        //Grid_Collector_advancefreight = q.advancefreight,//== true ? "بله" : "خیر"
                                        //Grid_Collector_freightforward = q.freightforward,//== true ? "بله" : "خیر"
                                        //Grid_Collector_cod = q.cod,//== true? "بله" : "خیر"
                                        Grid_Collector_IsActive = q.IsActive,//== true ? "فعال" : "غیرفعال"
                                        Grid_Collector_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                        Grid_Collector_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                        Grid_Collector_DateInsert = q.DateInsert,
                                        Grid_Collector_DateUpdate = q.DateUpdate,
                                        //Grid_Collector_IsPishtaz = q.IsPishtaz,
                                        //Grid_Collector_IsSefareshi = q.IsSefareshi,
                                        //Grid_Collector_IsVIje = q.IsVIje,
                                        //Grid_Collector_IsNromal = q.IsNromal,
                                        //Grid_Collector_IsDroonOstani = q.IsDroonOstani,
                                        //Grid_Collector_IsAdjoining = q.IsAdjoining,
                                        //Grid_Collector_IsNotAdjacent = q.IsNotAdjacent,
                                        //Grid_Collector_IsHeavyTransport = q.IsHeavyTransport,
                                        //Grid_Collector_IsForeign = q.IsForeign,
                                        //Grid_Collector_IsInCity = q.IsInCity,
                                        //Grid_Collector_IsAmanat = q.IsAmanat,
                                        //Grid_Collector_IsTwoStep = q.IsTwoStep,
                                        //Grid_Collector_HasHagheMaghar = q.HasHagheMaghar,
                                    }).ToList();
            }
            catch (Exception ex)
            {

                ErrrorMassege = ex.ToString();
            }
            finally
            {
                gridModel = new DataSourceResult
                {
                    Data = Final_Collectors,
                    Total = Collectors.Count,
                    Errors = ErrrorMassege,
                };

            }
            return Json(gridModel);

        }

        [HttpPost]
        public virtual IActionResult ProvidersList(DataSourceRequest command, int Id)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();


            List<Tbl_CollectorsServiceProvider> tbl_CollectorsServiceProvider = new List<Tbl_CollectorsServiceProvider>();
            var gridModel = new DataSourceResult();
            var Final_CollectorsServiceProvider = (dynamic)null;
            try
            {
                tbl_CollectorsServiceProvider = _repositoryTbl_CollectorsServiceProvider.Table.Where(p => p.CollectorId == Id).ToList();
                if (tbl_CollectorsServiceProvider.Count > 0)
                {
                    Final_CollectorsServiceProvider = (from q in tbl_CollectorsServiceProvider
                                                       select new Grid_Provider_In_Collector
                                                       {
                                                           Id = q.Id,
                                                           Grid_Provider_Name = _repositoryTbl_ServicesProviders.GetById(q.ProviderId).ServicesProviderName,
                                                           Grid_Provider_IsActive = q.IsActive,
                                                           Grid_Provider_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                                           Grid_Provider_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                                           Grid_Provider_DateInsert = q.DateInsert,
                                                           Grid_Provider_DateUpdate = q.DateUpdate,
                                                       }).ToList();
                }
                else
                {
                    ErrrorMassege = "No Exsit Data";
                }


            }
            catch (Exception ex)
            {

                ErrrorMassege = ex.ToString();
            }
            finally
            {
                gridModel = new DataSourceResult
                {
                    Data = Final_CollectorsServiceProvider,
                    Total = tbl_CollectorsServiceProvider.Count,
                    Errors = ErrrorMassege,
                };

            }
            return Json(gridModel);

        }

        [HttpPost]
        public virtual IActionResult StoresList(DataSourceRequest command, int Id)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();


            List<Tbl_CollectoreStores> CollectoreStores = new List<Tbl_CollectoreStores>();
            var gridModel = new DataSourceResult();
            var Final_CollectoreStores = (dynamic)null;
            try
            {
                CollectoreStores = _repositoryTbl_CollectoreStores.Table.Where(p => p.CollectorId == Id).ToList();
                if (CollectoreStores.Count > 0)
                {
                    Final_CollectoreStores = (from q in CollectoreStores
                                              select new Grid_ProviderStores
                                              {
                                                  Id = q.Id,
                                                  Grid_ProviderStore_Name = _storeService.GetStoreById(q.StoreId).Name,
                                                  Grid_ProviderStore_IsActive = q.IsActive,
                                                  Grid_ProviderStore_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                                  Grid_ProviderStore_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                                  Grid_ProviderStore_DateInsert = q.DateInsert,
                                                  Grid_ProviderStore_DateUpdate = q.DateUpdate,
                                              }).ToList();
                }
                else
                {
                    ErrrorMassege = "No Exsit Data";
                }


            }
            catch (Exception ex)
            {

                ErrrorMassege = ex.ToString();
            }
            finally
            {
                gridModel = new DataSourceResult
                {
                    Data = Final_CollectoreStores,
                    Total = CollectoreStores.Count,
                    Errors = ErrrorMassege,
                };

            }
            return Json(gridModel);

        }

        [HttpPost]
        public virtual IActionResult StateProvincesList(DataSourceRequest command, int Id)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();


            List<Tbl_Offices> CollectoreOffices = new List<Tbl_Offices>();
            var gridModel = new DataSourceResult();
            var Final_Offices = (dynamic)null;
            try
            {
                CollectoreOffices = _repositoryTbl_Offices.Table.Where(p => p.CollectorId == Id && p.TypeOffice == true).ToList();
                if (CollectoreOffices.Count > 0)
                {
                    Final_Offices = (from q in CollectoreOffices
                                     select new Grid_StateProvinces
                                     {
                                         Id = q.Id,
                                         Grid_StateProvinces_NameCountry = _StateProvinceService.GetStateProvinceById(q.StateProvinceId).Country.Name,
                                         Grid_StateProvinces_NameProvinces = _StateProvinceService.GetStateProvinceById(q.StateProvinceId).Name,
                                         Grid_StateProvinces_IdCityMaping =0,// q.IdCity,
                                         Grid_StateProvinces_IdStateMaping =0,//, q.IdState,
                                         Grid_StateProvinces_NameCityMaping ="",// q.NameCity,
                                         Grid_StateProvinces_NameStateMaping ="",// q.NameState,
                                         Grid_StateProvinces_IsActive = q.IsActive,
                                         Grid_StateProvinces_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                         Grid_StateProvinces_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                         Grid_StateProvinces_DateInsert = q.DateInsert,
                                         Grid_StateProvinces_DateUpdate = q.DateUpdate,
                                     }).ToList();
                }
                else
                {
                    ErrrorMassege = "No Exsit Data";
                }


            }
            catch (Exception ex)
            {

                ErrrorMassege = ex.ToString();
            }
            finally
            {
                gridModel = new DataSourceResult
                {
                    Data = Final_Offices,
                    Total = CollectoreOffices.Count,
                    Errors = ErrrorMassege,
                };

            }
            return Json(gridModel);

        }



        [HttpGet]
        public IActionResult GetCustomerList(string searchtext)
        {
            //(p=> new {id=p.Value,text=p.Text })
            //.Where(p => p.GetFullName().Contains(searchtext)) //.Select(p => new {p.Id, Text = p.GetFullName() })
            //IList<SelectListItem> temp = new List<SelectListItem>();
            var temp = (dynamic)null;
            if (!string.IsNullOrEmpty(searchtext))
            {

                IPagedList<Core.Domain.Customers.Customer> data = _customerService.GetAllCustomers(username: searchtext);
                if (data != null)
                {
                    temp = data.Select(p => new { id = p.Id, text = p.GetFullName() });
                    //foreach (var c in data)
                    //{
                    //    temp.
                    //    //temp.Add(new SelectListItem { Text = c.GetFullName(), Value = c.Id.ToString() });
                    //}
                }

            }


            return Json(new { results = temp });
        }
        #endregion
        #region Active & Disable
        public virtual IActionResult Disable(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            try
            {
                Tbl_Collectors Collector = _repositoryTbl_Collectors.Table.Where(p => p.Id == id).FirstOrDefault();

                if (Collector == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                //a vendor should have access only to his products
                //if (_workContext.CurrentVendor != null && Providre.VendorId != _workContext.CurrentVendor.Id)
                //    return RedirectToAction("List");

                else
                {
                    Collector.IsActive = false;
                    Collector.DateUpdate = DateTime.Now;
                    Collector.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_Collectors.Update(Collector);
                }

                //activity log
                _customerActivityService.InsertActivity("DeleteCollector", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"), Collector.CollectorName);

                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));
            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DisableSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();


            try
            {
                if (selectedIds != null)
                {
                    foreach (var item in selectedIds)
                    {
                        Tbl_Collectors Collector = _repositoryTbl_Collectors.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (Collector != null)
                        {

                            Collector.IsActive = false;
                            Collector.DateUpdate = DateTime.Now;
                            Collector.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_Collectors.Update(Collector);
                        }
                    }
                    _customerActivityService.InsertActivity("DisableCollector", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));
                    SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));

                }
            }
            catch (Exception ex)
            {
                ErrorNotification(ex.ToString());

            }

            return Json(new { Result = true });
        }

        //Active
        [HttpPost]
        public virtual IActionResult Active(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_Collectors Collector = _repositoryTbl_Collectors.Table.Where(p => p.Id == id).FirstOrDefault();

                if (Collector == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                //a vendor should have access only to his products
                //if (_workContext.CurrentVendor != null && Providre.VendorId != _workContext.CurrentVendor.Id)
                //    return RedirectToAction("List");

                else
                {
                    Collector.IsActive = true;
                    Collector.DateUpdate = DateTime.Now;
                    Collector.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_Collectors.Update(Collector);
                }
                //activity log
                _customerActivityService.InsertActivity("ActiveCollector", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"), Collector.CollectorName);
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));
            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return RedirectToAction("List");
        }
        [HttpPost]
        public virtual IActionResult ActiveSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();


            try
            {
                if (selectedIds != null)
                {
                    foreach (var item in selectedIds)
                    {
                        Tbl_Collectors Collector = _repositoryTbl_Collectors.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (Collector != null)
                        {

                            Collector.IsActive = true;
                            Collector.DateUpdate = DateTime.Now;
                            Collector.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_Collectors.Update(Collector);
                        }
                    }
                }
                _customerActivityService.InsertActivity("ActiveCollector", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));

            }
            catch (Exception ex)
            {
                ErrorNotification(ex.ToString());

            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult DisableProvider(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_CollectorsServiceProvider temp = _repositoryTbl_CollectorsServiceProvider.Table.Where(p => p.Id == id).FirstOrDefault();

                if (temp != null)
                {

                    temp.IsActive = false;
                    temp.DateUpdate = DateTime.Now;
                    temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_CollectorsServiceProvider.Update(temp);

                }

                //activity log
                _customerActivityService.InsertActivity("DisableProviderinCollector", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"), temp.Provider.ServicesProviderName + " " + temp.Collector.CollectorName);
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));
            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return Json(new { Result = true });

        }

        [HttpPost]
        public virtual IActionResult ActiveProvider(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_CollectorsServiceProvider temp = _repositoryTbl_CollectorsServiceProvider.Table.Where(p => p.Id == id).FirstOrDefault();

                if (temp != null)
                {

                    temp.IsActive = true;
                    temp.DateUpdate = DateTime.Now;
                    temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_CollectorsServiceProvider.Update(temp);

                }

                //activity log
                _customerActivityService.InsertActivity("ActiveProviderinCollector", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"), temp.Provider.ServicesProviderName + " " + temp.Collector.CollectorName);
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));

            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return Json(new { Result = true });

        }


        [HttpPost]
        public virtual IActionResult DisableStore(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_CollectoreStores temp = _repositoryTbl_CollectoreStores.Table.Where(p => p.Id == id).FirstOrDefault();

                if (temp != null)
                {

                    temp.IsActive = false;
                    temp.DateUpdate = DateTime.Now;
                    temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_CollectoreStores.Update(temp);

                }

                //activity log
                _customerActivityService.InsertActivity("DisableStoreCollector", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"), temp.Collector.CollectorName);
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));
            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return Json(new { Result = true });

        }

        [HttpPost]
        public virtual IActionResult ActiveStore(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_CollectoreStores temp = _repositoryTbl_CollectoreStores.Table.Where(p => p.Id == id).FirstOrDefault();

                if (temp != null)
                {

                    temp.IsActive = true;
                    temp.DateUpdate = DateTime.Now;
                    temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_CollectoreStores.Update(temp);

                }

                //activity log
                _customerActivityService.InsertActivity("ActiveStoreCollector", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"), temp.Collector.CollectorName);
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));

            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return Json(new { Result = true });

        }



        [HttpPost]
        public virtual IActionResult DisableOffice(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_Offices temp = _repositoryTbl_Offices.Table.Where(p => p.Id == id).FirstOrDefault();

                if (temp != null)
                {

                    temp.IsActive = false;
                    temp.DateUpdate = DateTime.Now;
                    temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_Offices.Update(temp);

                }

                //activity log
                _customerActivityService.InsertActivity("DisableOfficeCollector", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));
            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return Json(new { Result = true });

        }

        [HttpPost]
        public virtual IActionResult ActiveOffice(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_Offices temp = _repositoryTbl_Offices.Table.Where(p => p.Id == id).FirstOrDefault();

                if (temp != null)
                {

                    temp.IsActive = true;
                    temp.DateUpdate = DateTime.Now;
                    temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_Offices.Update(temp);

                }

                //activity log
                _customerActivityService.InsertActivity("ActiveOfficeCollector", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));

            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return Json(new { Result = true });

        }

        #endregion
        #region Create / Edit 

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            var model = new Tbl_Collectors();

            #region  get list provider
            var Providers = _repositoryTbl_ServicesProviders.Table.Where(p => p.IsActive == true).ToList();
            if (Providers.Count > 0)
            {
                model.ListProviders.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var c in Providers)
                {

                    model.ListProviders.Add(new SelectListItem { Text = c.ServicesProviderName, Value = c.Id.ToString() });
                }
            }
            else
            {
                model.ListProviders.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            #endregion

            #region Get user
            //model.ListUsers.Add(new SelectListItem { Text = "نام کاربر را وارد کنید", Value = "0" });
            //var Users = _customerService.GetAllCustomers();//.Select(p => new { Name = p.GetFullName().ToString(), p.Id }).ToList();
            //if (Users != null && Users.Count > 0)
            //{
            //    model.ListUsers.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
            //    foreach (var c in Users)
            //    {
            //        model.ListUsers.Add(new SelectListItem { Text = c.GetFullName(), Value = c.Id.ToString() });
            //    }

            //}
            //else
            //{
            //    model.ListUsers.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            //}


            #endregion

            #region get Stores
            var Stores = _storeService.GetAllStores(true).ToList();
            if (Stores.Count > 0)
            {
                model.ListStores.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var s in Stores)
                {

                    model.ListStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
                }
            }
            else
            {
                model.ListStores.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            #endregion

            return View("/Plugins/Misc.ShippingSolutions/Views/Collectors/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(Tbl_Collectors model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //if (ModelState.IsValid)
            //{
            #region Check Duplicate
            Tbl_Collectors duplicate = _repositoryTbl_Collectors.Table.Where(p => p.CollectorName == model.CollectorName).FirstOrDefault();
            if (duplicate != null)
            {
                ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Duplicate"));
                return View("/Plugins/Misc.ShippingSolutions/Views/Collectors/Create.cshtml");

            }
            #endregion
            Tbl_Collectors Collector = new Tbl_Collectors();
            Collector.CollectorName = model.CollectorName;
            Collector.IsActive = true;
            Collector.UserId = model.UserId;
            //Collector.advancefreight = model.advancefreight;
            //Collector.cod = model.cod;
            //Collector.freightforward = model.freightforward;
            Collector.MaxPath = model.MaxPath;
            Collector.MaxWeight = model.MaxWeight;
            Collector.MinWeight = model.MinWeight;
            //Collector.IsPishtaz = model.IsPishtaz;
            //Collector.IsSefareshi = model.IsSefareshi;
            //Collector.IsVIje = model.IsVIje;
            //Collector.IsNromal = model.IsNromal;
            //Collector.IsDroonOstani = model.IsDroonOstani;
            //Collector.IsAdjoining = model.IsAdjoining;
            //Collector.IsNotAdjacent = model.IsNotAdjacent;
            //Collector.IsHeavyTransport = model.IsHeavyTransport;
            //Collector.IsForeign = model.IsForeign;
            //Collector.IsInCity = model.IsInCity;
            //Collector.IsAmanat = model.IsAmanat;
            //Collector.IsTwoStep = model.IsTwoStep;
            //Collector.HasHagheMaghar = model.HasHagheMaghar;
            Collector.DateInsert = DateTime.Now;
            Collector.IdUserInsert = _workContext.CurrentCustomer.Id;

            _repositoryTbl_Collectors.Insert(Collector);


            //activity log
            _customerActivityService.InsertActivity("AddNewCollector", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"), Collector.CollectorName);

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = Collector.Id });
            }
            return RedirectToAction("List");
            //}


            //return View("/Plugins/Misc.ShippingSolutions/Views/ServiceProviders/Create.cshtml",model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Collector = _repositoryTbl_Collectors.GetById(id);
            if (Collector == null || Collector.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            var model = new Tbl_Collectors();
            model = Collector;


            #region  get list provider
            var Providers = _repositoryTbl_ServicesProviders.Table.Where(p => p.IsActive == true).ToList();
            if (Providers.Count > 0)
            {
                model.ListProviders.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var c in Providers)
                {

                    model.ListProviders.Add(new SelectListItem { Text = c.ServicesProviderName, Value = c.Id.ToString() });
                }
            }
            else
            {
                model.ListProviders.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            #endregion

            #region user

            var Users = _customerService.GetCustomerById(Collector.UserId);//.Select(p => new { Name = p.GetFullName().ToString(), p.Id }).ToList();
            //IPagedList<Core.Domain.Customers.Customer> Users = _customerService.GetAllCustomers(id: Collector.UserId);

            if (Users != null)
            {

                //model.ListUsers.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                //foreach (var c in Users)
                //{
                model.ListUsers.Add(new SelectListItem { Text = Users.GetFullName(), Value = Users.Id.ToString(), Selected = true });
                //}

            }
            else
            {
                model.ListUsers.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }


            #endregion

            #region get Stores
            var Stores = _storeService.GetAllStores(true).ToList();
            if (Stores.Count > 0)
            {
                model.ListStores.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var s in Stores)
                {

                    model.ListStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
                }
            }
            else
            {
                model.ListStores.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            #endregion

            #region list Get State Provinces
            var Cities = _StateProvinceService.GetStateProvinces();
            if (Cities != null && Cities.Count > 0)
            {
                model.ListStateProvince.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var c in Cities)
                {
                    model.ListStateProvince.Add(new SelectListItem { Text = c.Country.Name + "-" + c.Name, Value = c.Id.ToString() });
                }

            }
            else
            {
                model.ListStateProvince.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            #endregion

            return View("/Plugins/Misc.ShippingSolutions/Views/Collectors/Edit.cshtml", model);
        }

        [Area(AreaNames.Admin)]
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(Tbl_Collectors model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Collector = _repositoryTbl_Collectors.GetById(model.Id);
            if (Collector == null || Collector.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            //update
            Collector.CollectorName = model.CollectorName;
            Collector.IsActive = true;
            Collector.UserId = model.UserId;
            //Collector.advancefreight = model.advancefreight;
            //Collector.cod = model.cod;
            //Collector.freightforward = model.freightforward;
            Collector.MaxPath = model.MaxPath;
            Collector.MaxWeight = model.MaxWeight;
            Collector.MinWeight = model.MinWeight;
            //Collector.IsPishtaz = model.IsPishtaz;
            //Collector.IsSefareshi = model.IsSefareshi;
            //Collector.IsVIje = model.IsVIje;
            //Collector.IsNromal = model.IsNromal;
            //Collector.IsDroonOstani = model.IsDroonOstani;
            //Collector.IsAdjoining = model.IsAdjoining;
            //Collector.IsNotAdjacent = model.IsNotAdjacent;
            //Collector.IsHeavyTransport = model.IsHeavyTransport;
            //Collector.IsForeign = model.IsForeign;
            //Collector.IsInCity = model.IsInCity;
            //Collector.IsAmanat = model.IsAmanat;
            //Collector.IsTwoStep = model.IsTwoStep;
            //Collector.HasHagheMaghar = model.HasHagheMaghar;
            Collector.DateUpdate = DateTime.Now;
            Collector.IdUserUpdate = _workContext.CurrentCustomer.Id;
            _repositoryTbl_Collectors.Update(Collector);

            if (model.ProviderId.Count() > 0)
            {
                foreach (var item in model.ProviderId)
                {
                    if (item > 0)
                    {  //check duplicate
                        Tbl_CollectorsServiceProvider Duplicate = _repositoryTbl_CollectorsServiceProvider.Table.Where(p => p.CollectorId == model.Id && p.ProviderId == item).FirstOrDefault();
                        if (Duplicate != null)
                        {
                            if (Duplicate.IsActive == false)
                            {
                                Duplicate.IsActive = true;
                                Duplicate.DateUpdate = DateTime.Now;
                                Duplicate.IdUserUpdate = _workContext.CurrentCustomer.Id;
                                _repositoryTbl_CollectorsServiceProvider.Update(Duplicate);
                            }
                            //ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Duplicate"));
                            //return RedirectToAction("Edit", new { id = Provider.Id });

                        }
                        Tbl_CollectorsServiceProvider temp = new Tbl_CollectorsServiceProvider();
                        temp.DateInsert = DateTime.Now;
                        temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                        temp.IsActive = true;
                        temp.CollectorId = model.Id;
                        temp.ProviderId = item;
                        _repositoryTbl_CollectorsServiceProvider.Insert(temp);

                    }

                }

            }

            if (model.StoreId.Count() > 0)
            {
                foreach (var item in model.StoreId)
                {
                    if (item > 0)
                    {  //check duplicate
                        Tbl_CollectoreStores Duplicate = _repositoryTbl_CollectoreStores.Table.Where(p => p.CollectorId == model.Id && p.StoreId == item).FirstOrDefault();
                        if (Duplicate != null)
                        {
                            if (Duplicate.IsActive == false)
                            {
                                Duplicate.IsActive = true;
                                Duplicate.DateUpdate = DateTime.Now;
                                Duplicate.IdUserUpdate = _workContext.CurrentCustomer.Id;
                                _repositoryTbl_CollectoreStores.Update(Duplicate);
                            }
                            //ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Duplicate"));
                            //return RedirectToAction("Edit", new { id = Provider.Id });

                        }
                        Tbl_CollectoreStores temp = new Tbl_CollectoreStores();
                        temp.DateInsert = DateTime.Now;
                        temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                        temp.IsActive = true;
                        temp.CollectorId = model.Id;
                        temp.StoreId = item;
                        _repositoryTbl_CollectoreStores.Insert(temp);


                    }

                }
            }

            ///////
            if (model.StateProvinceId.Count() > 0)
            {
                foreach (var item in model.StateProvinceId)
                {
                    if (item > 0)
                    {  //check duplicate
                        Tbl_Offices Duplicate = _repositoryTbl_Offices.Table.Where(p => p.CollectorId == model.Id && p.StateProvinceId == item).FirstOrDefault();
                        if (Duplicate != null)
                        {
                            if (Duplicate.IsActive == false)
                            {
                                Duplicate.IsActive = true;
                                Duplicate.DateUpdate = DateTime.Now;
                                Duplicate.IdUserUpdate = _workContext.CurrentCustomer.Id;
                                _repositoryTbl_Offices.Update(Duplicate);
                            }
                            //ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Duplicate"));
                            //return RedirectToAction("Edit", new { id = Provider.Id });

                        }
                        Tbl_Offices temp = new Tbl_Offices();
                        temp.DateInsert = DateTime.Now;
                        temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                        temp.IsActive = true;
                        temp.CollectorId = model.Id;
                        temp.StateProvinceId = item;
                        temp.TypeOffice = true;
                        _repositoryTbl_Offices.Insert(temp);
                        //insert Tbl Time
                        string[] DayName= new[] { "Monday", "Tuesday", "Wednesday", "Thursday",
                        "Friday", "Saturday", "Sunday" };
                        foreach (var itemDay in DayName)
                        {
                            Tbl_WorkingTime t = new Tbl_WorkingTime();
                            t.DayName = itemDay;
                            t.OfficeId = temp.Id;
                            TimeSpan timeSpan = new TimeSpan(0, 0, 0);
                            t.StartTime = timeSpan;
                            t.EndTime = timeSpan;
                            _repositoryTbl_WorkingTime.Insert(t);

                        }

                    }

                }
            }

            //activity log
            _customerActivityService.InsertActivity("EditCollector", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"), Collector.CollectorName);

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"));
            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = Collector.Id });
            }
            return RedirectToAction("List");




            //return View("/Plugins/Misc.ShippingSolutions/Views/ServiceProviders/Edit.cshtml", model);

        }

        public virtual IActionResult EditOffice(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Office = _repositoryTbl_Offices.GetById(id);
            if (Office == null || Office.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("Edit", new {id=Office.CollectorId });
            var state_city = _StateProvinceService.GetStateProvinceById(Office.StateProvinceId);
            var NameCollector = _repositoryTbl_Collectors.GetById(Office.CollectorId);
            var model = new Tbl_Offices();
            model = Office;
            model.NameOffice = " " + state_city.Country.Name + " : " + state_city.Name + " : " + NameCollector.CollectorName;

            //model.NameOffice = "دفتر-استان:" + state_city.Country.Name + "شهر:" + state_city.Name+" جمع آورکننده:"+NameCollector.CollectorName;
            #region user
            //if (Office.CustomerID > 0)
            //{
            //    var Users = _customerService.GetCustomerById(Office.CustomerID);//.Select(p => new { Name = p.GetFullName().ToString(), p.Id }).ToList();
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
            //    var ListAddressUser = _customerService.GetCustomerById(IdUser).Addresses.ToList();
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
            //        model.ListAddress.Add(new SelectListItem { Text = item.Country.Name + "-" + item.StateProvince.Name + "-" + item.City + "-" + item.Address1 + "-" + item.ZipPostalCode + "-" + item.PhoneNumber, Value = item.Id.ToString() });

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
        #endregion


    }
}
