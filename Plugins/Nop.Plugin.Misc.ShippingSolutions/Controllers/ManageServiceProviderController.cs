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
using Nop.Web.Areas.Admin.Helpers;
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
    public class ManageServiceProviderController : BaseAdminController
    {
        private readonly IRepository<Tbl_ServicesTypes> _repositoryTbl_ServicesTypes;
        private readonly IRepository<Tbl_Dealer> _repositoryTransportationTypes;
        private readonly IRepository<Tbl_ServicesProviders> _repositoryServicesProviders;
        private readonly IRepository<Tbl_Offices> _repositoryOffices;
        private readonly IRepository<Tbl_ServiceTypesProvider> _repositoryServiceTypesProvider;
        private readonly IRepository<Tbl_ProviderStores> _repositoryProviderStores;
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
        private readonly IRepository<Tbl_Address_LatLong> _repositoryTbl_Address_LatLong;


        public ManageServiceProviderController
            (
             IAddressService addressService,
            IWorkContext workContext,
            IDbContext dbContext,
            IPermissionService permissionService,
            ICategoryService CategoryService,
            IStaticCacheManager cacheManager,
            IStateProvinceService StateProvinceService,
            IRepository<Tbl_ServicesTypes> repositoryTbl_ServicesTypes,
            IRepository<Tbl_Dealer> repositoryTransportationTypes,
            IRepository<Tbl_ServicesProviders> repositoryServicesProviders,
            IRepository<Tbl_Offices> repositoryOffices,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ICustomerService customerService,
            IStoreService storeService,
            IRepository<Tbl_ServiceTypesProvider> repositoryServiceTypesProvider,
            IRepository<Tbl_ProviderStores> repositoryProviderStores,
            IRepository<Tbl_Offices> repositoryTbl_Offices,
            IRepository<Tbl_WorkingTime> repositoryTbl_WorkingTime,
            IRepository<Address> repositoryTbl_Address,
            IRepository<Tbl_Address_LatLong> repositoryTbl_Address_LatLong

            )
        {
            _repositoryTbl_Address_LatLong = repositoryTbl_Address_LatLong;
            _addressService = addressService;
            _repositoryServiceTypesProvider = repositoryServiceTypesProvider;
            _workContext = workContext;
            _dbContext = dbContext;
            _permissionService = permissionService;
            _CategoryService = CategoryService;
            _cacheManager = cacheManager;
            _StateProvinceService = StateProvinceService;
            _repositoryTbl_ServicesTypes = repositoryTbl_ServicesTypes;
            _repositoryTransportationTypes = repositoryTransportationTypes;
            _repositoryServicesProviders = repositoryServicesProviders;
            _repositoryOffices = repositoryOffices;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _customerService = customerService;
            _storeService = storeService;
            _repositoryProviderStores = repositoryProviderStores;
            _repositoryTbl_Offices = repositoryTbl_Offices;
            _repositoryTbl_WorkingTime = repositoryTbl_WorkingTime;
            _repositoryTbl_Address = repositoryTbl_Address;
        }
        #region index and lists
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

            var Model = new Search_Provider_Model();


            //1

            var categories = SelectListHelper.GetCategoryList(_CategoryService, _cacheManager, true);
            if (categories.Count > 0)
            {
                Model.ListCategory.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var c in categories)
                {

                    Model.ListCategory.Add(c);
                }
            }
            else
            {
                Model.ListCategory.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }


            //2

            var Cities = _StateProvinceService.GetStateProvinces();
            if (Cities != null && Cities.Count > 0)
            {
                Model.ListOffice_City.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var c in Cities)
                {
                    Model.ListOffice_City.Add(new SelectListItem { Text =c.Country.Name+"-"+ c.Name, Value = c.Id.ToString() });
                }

            }
            else
            {
                Model.ListOffice_City.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }



            //3

            var temp_list_servicetypes = _repositoryTbl_ServicesTypes.Table.ToList();
            if (temp_list_servicetypes != null && temp_list_servicetypes.Count > 0)
            {
                Model.ListServiceTypes.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0",Selected=true });
                foreach (var itemservicetypes in temp_list_servicetypes)
                {
                    Model.ListServiceTypes.Add(new SelectListItem { Text = itemservicetypes.Name, Value = itemservicetypes.Id.ToString() });
                }
            }
            else
            {
                Model.ListServiceTypes.Add(new SelectListItem { Text = "داده ای وجود ندراد", Value = "0" });
            }

            //4

            //var temp_list_TransportationTypes = _repositoryTransportationTypes.Table.ToList();
            //if (temp_list_TransportationTypes != null && temp_list_TransportationTypes.Count > 0)
            //{
            //    Model.ListTransportationTypes.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
            //    foreach (var itemsTransportationTypes in temp_list_TransportationTypes)
            //    {
            //        Model.ListTransportationTypes.Add(new SelectListItem { Text = itemsTransportationTypes.Name, Value = itemsTransportationTypes.Id.ToString() });
            //    }
            //}
            //else
            //{
            //    Model.ListTransportationTypes.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            //}
            //5
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



            return View("/Plugins/Misc.ShippingSolutions/Views/ServiceProviders/ListProviders.cshtml", Model);
        }
        [HttpPost]
        public virtual IActionResult ServiceProvidresList(DataSourceRequest command, Search_Provider_Model model)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();


            List<Tbl_ServicesProviders> providers = new List<Tbl_ServicesProviders>();
          
            var gridModel = new DataSourceResult();
            var Final_providers = (dynamic)null;
            try
            {
                providers = _repositoryServicesProviders.Table.ToList();
                if (providers.Count > 0)
                {
                    if (model.ActiveSearch == true)
                    {
                        List<Tbl_Offices> ListOffice = _repositoryTbl_Offices.Table.Where(p => p.IsActive == true && p.TypeOffice == false).ToList();
                        List<Tbl_ProviderStores> ListStore = _repositoryProviderStores.Table.Where(p => p.IsActive == true).ToList();
                        #region Search
                        if (!string.IsNullOrEmpty(model.SearchServicesProviderName))
                        {
                            providers = providers.Where(p => p.ServicesProviderName.Contains(model.SearchServicesProviderName)).ToList();
                        }
                        if (!string.IsNullOrEmpty(model.SearchServicesProviderAgentName))
                        {
                            providers = providers.Where(p => p.AgentName.Contains(model.SearchServicesProviderAgentName)).ToList();
                        }
                        providers = providers.Where(p => p.IsActive == model.SearchIsActive).ToList();

                        if (model.SearchMaxOrder > 0)
                        {
                            providers = providers.Where(p => p.MaxOrder == model.SearchMaxOrder).ToList();
                        }

                        if (model.SearchMaxWeight > 0)
                        {
                            providers = providers.Where(p => p.MaxWeight == model.SearchMaxWeight).ToList();
                        }
                        if (model.SearchMinWeight > 0)
                        {
                            providers = providers.Where(p => p.MaxWeight == model.SearchMinWeight).ToList();
                        }

                        if (model.SearchMaxTimeDeliver > 0)
                        {
                            providers = providers.Where(p => p.MaxTimeDeliver == model.SearchMaxTimeDeliver).ToList();
                        }





                        //
                        if (model.SearchMaxbillingamountCOD > 0)
                        {
                            providers = providers.Where(p => p.MaxbillingamountCOD == model.SearchMaxbillingamountCOD).ToList();
                        }
                        if (model.SearchMaxheight > 0)
                        {
                            providers = providers.Where(p => p.Maxheight == model.SearchMaxheight).ToList();
                        }
                        if (model.SearchMaxlength > 0)
                        {
                            providers = providers.Where(p => p.Maxlength == model.SearchMaxlength).ToList();
                        }
                        if (model.SearchMaxwidth > 0)
                        {
                            providers = providers.Where(p => p.Maxwidth == model.SearchMaxwidth).ToList();
                        }




                        providers = providers.Where(p => p.advancefreight == model.Searchadvancefreight).ToList();
                        providers = providers.Where(p => p.freightforward == model.Searchfreightforward).ToList();
                        providers = providers.Where(p => p.IsPishtaz == model.SearchIsPishtaz).ToList();
                        providers = providers.Where(p => p.IsSefareshi == model.SearchIsSefareshi).ToList();
                        providers = providers.Where(p => p.IsVIje == model.SearchIsVIje).ToList();
                        providers = providers.Where(p => p.IsNromal == model.SearchIsNromal).ToList();
                        providers = providers.Where(p => p.IsDroonOstani == model.SearchIsDroonOstani).ToList();
                        providers = providers.Where(p => p.IsAdjoining == model.SearchIsAdjoining).ToList();
                        providers = providers.Where(p => p.IsNotAdjacent == model.SearchIsNotAdjacent).ToList();
                        providers = providers.Where(p => p.IsHeavyTransport == model.SearchIsHeavyTransport).ToList();
                        providers = providers.Where(p => p.IsForeign == model.SearchIsForeign).ToList();
                        providers = providers.Where(p => p.IsInCity == model.SearchIsInCity).ToList();
                        //providers = providers.Where(p => p.IsAmanat == model.SearchIsAmanat).ToList();
                        providers = providers.Where(p => p.IsTwoStep == model.SearchIsTwoStep).ToList();
                        providers = providers.Where(p => p.HasHagheMaghar == model.SearchHasHagheMaghar).ToList();



                        if (model.SearchCategoryId > 0)
                        {
                            providers = providers.Where(p => p.CategoryId == model.SearchCategoryId).ToList();
                        }
                        if (model.SearchServiceTypeId > 0)
                        {
                            providers = providers.Where(p => p.ServiceTypeId == model.SearchServiceTypeId).ToList();
                        }
                        //if (model.SearchServiceTypeId > 0)
                        //{
                        //    providers = providers.Where(p => p.ServiceTypesProvider.Any(x => x.ServiceTypeId == model.SearchServiceTypeId)).ToList();
                        //}
                        //if (model.SearchTransportationId > 0)
                        //{
                        //    providers = providers.Where(p => p.TransportationTypesProvider.Any(x => x.TransportationTypeId == model.SearchTransportationId)).ToList();
                        //}
                        if (model.SearchOfficeId > 0)
                        {

                            var temp = (from c in providers
                                        join o in ListOffice on c.Id equals o.ProviderId
                                        where o.StateProvinceId == model.SearchOfficeId
                                        select c).Distinct().ToList();

                            providers = temp;
                        }
                        if (model.SearchStartTime != null && (model.SearchStartTime.Hours != 0 || model.SearchStartTime.Minutes != 0 || model.SearchStartTime.Seconds != 0))
                        {
                            var temp = (from c in providers
                                        join o in ListOffice on c.Id equals o.CollectorId
                                        where o.WorkingTimes.Exists(p => p.StartTime == model.SearchStartTime)
                                        select c).Distinct().ToList();

                            providers = temp;

                        }
                        if (model.SearchEndTime != null && (model.SearchEndTime.Hours != 0 || model.SearchEndTime.Minutes != 0 || model.SearchEndTime.Seconds != 0))
                        {
                            var temp = (from c in providers
                                        join o in ListOffice on c.Id equals o.CollectorId
                                        where o.WorkingTimes.Exists(p => p.StartTime == model.SearchEndTime)
                                        select c).Distinct().ToList();

                            providers = temp;

                        }

                        if (model.SearchStoreId > 0)
                        {
                            var temp = (from c in providers
                                        join s in ListStore on c.Id equals s.ProviderId
                                        where s.StoreId == model.SearchStoreId
                                        select c).Distinct().ToList();
                            providers = temp;
                        }
                        #endregion
                    }
                }
                else
                {
                    ErrrorMassege = "No Exsit Data";
                }

                Final_providers = (from q in providers
                                   select new Grid_Provider_Model
                                   {
                                       Id = q.Id,
                                       Grid_Provider_Name = q.ServicesProviderName,
                                       Grid_Provider_AgentName=q.AgentName,
                                       Grid_Provider_ServiceTypeName=_repositoryTbl_ServicesTypes.GetById(q.ServiceTypeId).Name,
                                       Grid_Provider_CategoryName = _CategoryService.GetCategoryById(q.CategoryId).Name,
                                       Grid_Provider_MaxOrder = q.MaxOrder.ToString(),
                                       Grid_Provider_MaxTimeDeliver = q.MaxTimeDeliver.ToString(),
                                       Grid_Provider_MaxWeight = q.MaxWeight.ToString(),
                                       Grid_Provider_MinWeight = q.MinWeight.ToString(),
                                       Grid_Provider_MaxbillingamountCOD=q.MaxbillingamountCOD,
                                       Grid_Provider_Maxheight=q.Maxheight,
                                       Grid_Provider_Maxlength=q.Maxlength,
                                       Grid_Provider_Maxwidth=q.Maxwidth,
                                       Grid_Provider_advancefreight = q.advancefreight,//== true ? "بله" : "خیر"
                                       Grid_Provider_freightforward = q.freightforward,//== true ? "بله" : "خیر"
                                       Grid_Provider_cod = q.cod,//== true? "بله" : "خیر"
                                       Grid_Provider_IsActive = q.IsActive,//== true ? "فعال" : "غیرفعال"
                                       Grid_Provider_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                       Grid_Provider_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                       Grid_Provider_DateInsert = q.DateInsert,
                                       Grid_Provider_DateUpdate = q.DateUpdate,
                                       Grid_Provider_IsPishtaz = q.IsPishtaz,
                                       Grid_Provider_IsSefareshi = q.IsSefareshi,
                                       Grid_Provider_IsVIje = q.IsVIje,
                                       Grid_Provider_IsNromal = q.IsNromal,
                                       Grid_Provider_IsDroonOstani = q.IsDroonOstani,
                                       Grid_Provider_IsAdjoining = q.IsAdjoining,
                                       Grid_Provider_IsNotAdjacent = q.IsNotAdjacent,
                                       Grid_Provider_IsHeavyTransport = q.IsHeavyTransport,
                                       Grid_Provider_IsForeign = q.IsForeign,
                                       Grid_Provider_IsInCity = q.IsInCity,
                                       Grid_Provider_IsTwoStep = q.IsTwoStep,
                                       Grid_Provider_HasHagheMaghar = q.HasHagheMaghar,
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
                    Data = Final_providers,
                    Total = providers.Count,
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


            List<Tbl_ProviderStores> Tbl_ProviderStores = new List<Tbl_ProviderStores>();
            var gridModel = new DataSourceResult();
            var Final_ProviderStores = (dynamic)null;
            try
            {
                Tbl_ProviderStores = _repositoryProviderStores.Table.Where(p => p.ProviderId == Id).ToList();
                if (Tbl_ProviderStores.Count > 0)
                {
                    Final_ProviderStores = (from q in Tbl_ProviderStores
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
                    Data = Final_ProviderStores,
                    Total = Tbl_ProviderStores.Count,
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


            List<Tbl_Offices> ProviderOffices = new List<Tbl_Offices>();
            var gridModel = new DataSourceResult();
            var Final_Offices = (dynamic)null;
            try
            {
                ProviderOffices = _repositoryTbl_Offices.Table.Where(p => p.ProviderId == Id && p.TypeOffice == false).ToList();//
                if (ProviderOffices.Count > 0)
                {
                    Final_Offices = (from q in ProviderOffices
                                     select new Grid_StateProvinces
                                     {
                                         Id = q.Id,
                                         Grid_StateProvinces_NameCountry = _StateProvinceService.GetStateProvinceById(q.StateProvinceId).Country.Name,
                                         Grid_StateProvinces_NameProvinces = _StateProvinceService.GetStateProvinceById(q.StateProvinceId).Name,
                                         Grid_StateProvinces_IdCityMaping = 0,//q.IdCity,
                                         Grid_StateProvinces_IdStateMaping = 0,//q.IdState,
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
                    Total = ProviderOffices.Count,
                    Errors = ErrrorMassege,
                };

            }
            return Json(gridModel);

        }


        #endregion
        #region Disable & active
        [HttpPost]
        public virtual IActionResult Disable(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            try
            {
                Tbl_ServicesProviders Providre = _repositoryServicesProviders.Table.Where(p => p.Id == id).FirstOrDefault();

                if (Providre == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                //a vendor should have access only to his products
                //if (_workContext.CurrentVendor != null && Providre.VendorId != _workContext.CurrentVendor.Id)
                //    return RedirectToAction("List");

                else
                {
                    Providre.IsActive = false;
                    _repositoryServicesProviders.Update(Providre);
                }

                //activity log
                _customerActivityService.InsertActivity("DeleteServiceProvider", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"), Providre.ServicesProviderName);

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
                        Tbl_ServicesProviders Providre = _repositoryServicesProviders.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (Providre != null)
                        {

                            Providre.IsActive = false;
                            Providre.DateUpdate = DateTime.Now;
                            Providre.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryServicesProviders.Update(Providre);
                        }
                    }
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
                Tbl_ServicesProviders Provider = _repositoryServicesProviders.Table.Where(p => p.Id == id).FirstOrDefault();

                if (Provider == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                //a vendor should have access only to his products
                //if (_workContext.CurrentVendor != null && Providre.VendorId != _workContext.CurrentVendor.Id)
                //    return RedirectToAction("List");

                else
                {
                    Provider.IsActive = true;
                    _repositoryServicesProviders.Update(Provider);
                }
                //activity log
                _customerActivityService.InsertActivity("ActiveServiceProvider", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"), Provider.ServicesProviderName);
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
                        Tbl_ServicesProviders Provider = _repositoryServicesProviders.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (Provider != null)
                        {

                            Provider.IsActive = true;
                            Provider.DateUpdate = DateTime.Now;
                            Provider.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryServicesProviders.Update(Provider);
                        }
                    }
                }
                _customerActivityService.InsertActivity("ActiveServiceProvider", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));

            }
            catch (Exception ex)
            {
                ErrorNotification(ex.ToString());

            }

            return Json(new { Result = true });
        }

      

        [HttpPost]
        public virtual IActionResult DisableStoreProvider(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_ProviderStores temp = _repositoryProviderStores.Table.Where(p => p.Id == id).FirstOrDefault();

                if (temp != null)
                {

                    temp.IsActive = false;
                    temp.DateUpdate = DateTime.Now;
                    temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryProviderStores.Update(temp);

                }

                //activity log
                _customerActivityService.InsertActivity("DisableStoreProvider", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"), temp.Provider.ServicesProviderName);
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));
            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return Json(new { Result = true });

        }

        [HttpPost]
        public virtual IActionResult ActiveStoreProvider(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_ProviderStores temp = _repositoryProviderStores.Table.Where(p => p.Id == id).FirstOrDefault();

                if (temp != null)
                {

                    temp.IsActive = true;
                    temp.DateUpdate = DateTime.Now;
                    temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryProviderStores.Update(temp);

                }

                //activity log
                _customerActivityService.InsertActivity("ActiveStoreProvider", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"), temp.Provider.ServicesProviderName);
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
            var model = new Tbl_ServicesProviders();
            #region  get list category
            var categories = SelectListHelper.GetCategoryList(_CategoryService, _cacheManager, true);
            if (categories.Count > 0)
            {

                model.ListCategory.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var c in categories)
                {

                    model.ListCategory.Add(c);
                }
            }
            else
            {
                model.ListCategory.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            #endregion
            #region Get city s

            //var Cities = _StateProvinceService.GetStateProvinces();
            //if (Cities != null && Cities.Count > 0)
            //{
            //    model.ListOffice_City.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
            //    foreach (var c in Cities)
            //    {
            //        model.ListOffice_City.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            //    }

            //}
            //else
            //{
            //    model.ListOffice_City.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            //}


            #endregion
            #region Get Servic types

            var temp_list_servicetypes = _repositoryTbl_ServicesTypes.Table.ToList();
            if (temp_list_servicetypes != null && temp_list_servicetypes.Count > 0)
            {
                model.ListServiceTypes.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0" });
                foreach (var itemservicetypes in temp_list_servicetypes)
                {
                    model.ListServiceTypes.Add(new SelectListItem { Text = itemservicetypes.Name, Value = itemservicetypes.Id.ToString() });
                }
            }
            else
            {
                model.ListServiceTypes.Add(new SelectListItem { Text = "داده ای وجود ندراد", Value = "0" });
            }
            #endregion
            #region Transportaion Types

            //var temp_list_TransportationTypes = _repositoryTransportationTypes.Table.ToList();
            //if (temp_list_TransportationTypes != null && temp_list_TransportationTypes.Count > 0)
            //{
            //    model.ListTransportationTypes.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
            //    foreach (var itemsTransportationTypes in temp_list_TransportationTypes)
            //    {
            //        model.ListTransportationTypes.Add(new SelectListItem { Text = itemsTransportationTypes.Name, Value = itemsTransportationTypes.Id.ToString() });
            //    }
            //}
            //else
            //{
            //    model.ListTransportationTypes.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
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

            return View("/Plugins/Misc.ShippingSolutions/Views/ServiceProviders/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(Tbl_ServicesProviders model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //if (ModelState.IsValid)
            //{
            #region Check Duplicate
            Tbl_ServicesProviders duplicate = _repositoryServicesProviders.Table.Where(p => p.ServicesProviderName == model.ServicesProviderName).FirstOrDefault();
            if (duplicate != null)
            {
                ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Duplicate"));
                return View("/Plugins/Misc.ShippingSolutions/Views/ServiceProviders/Create.cshtml");

            }
            #endregion
            Tbl_ServicesProviders Provider = new Tbl_ServicesProviders();
            Provider.ServicesProviderName = _CategoryService.GetCategoryById(model.CategoryId).Name;//model.ServicesProviderName
            Provider.AgentName = model.AgentName;
            Provider.ServiceTypeId = model.ServiceTypeId;
            Provider.IsActive = true;
            Provider.CategoryId = model.CategoryId;
            Provider.advancefreight = model.advancefreight;
            Provider.cod = model.cod;
            Provider.freightforward = model.freightforward;
            Provider.MaxOrder = model.MaxOrder;
            Provider.MaxTimeDeliver = model.MaxTimeDeliver;
            Provider.MaxWeight = model.MaxWeight;
            Provider.MinWeight = model.MinWeight;
            Provider.MaxbillingamountCOD = model.MaxbillingamountCOD;
            Provider.Maxheight = model.Maxheight;
            Provider.Maxlength = model.Maxlength;
            Provider.Maxwidth = model.Maxwidth;
            Provider.IsPishtaz = model.IsPishtaz;
            Provider.IsSefareshi = model.IsSefareshi;
            Provider.IsVIje = model.IsVIje;
            Provider.IsNromal = model.IsNromal;
            Provider.IsDroonOstani = model.IsDroonOstani;
            Provider.IsAdjoining = model.IsAdjoining;
            Provider.IsNotAdjacent = model.IsNotAdjacent;
            Provider.IsHeavyTransport = model.IsHeavyTransport;
            Provider.IsForeign = model.IsForeign;
            Provider.IsInCity = model.IsInCity;
            Provider.IsTwoStep = model.IsTwoStep;
            Provider.HasHagheMaghar = model.HasHagheMaghar;
            Provider.DateInsert = DateTime.Now;
            Provider.IdUserInsert = _workContext.CurrentCustomer.Id;
            _repositoryServicesProviders.Insert(Provider);


            //activity log
            _customerActivityService.InsertActivity("AddNewProvider", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"), Provider.ServicesProviderName);

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = Provider.Id });
            }
            return RedirectToAction("List");
            //}


            //return View("/Plugins/Misc.ShippingSolutions/Views/ServiceProviders/Create.cshtml",model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Provider = _repositoryServicesProviders.GetById(id);
            if (Provider == null || Provider.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            var model = new Tbl_ServicesProviders();
            model = Provider;

            #region  get list category
            var categories = SelectListHelper.GetCategoryList(_CategoryService, _cacheManager, true);
            if (categories.Count > 0)
            {
                model.ListCategory.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0" });
                foreach (var c in categories)
                {

                    model.ListCategory.Add(c);
                }
            }
            else
            {
                model.ListCategory.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
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
            #region Get Servic types new**

            var temp_list_servicetypes = _repositoryTbl_ServicesTypes.Table.ToList();
            if (temp_list_servicetypes != null && temp_list_servicetypes.Count > 0)
            {
                foreach (var itemservicetypes in temp_list_servicetypes)
                {
                    model.ListServiceTypes.Add(new SelectListItem { Text = itemservicetypes.Name, Value = itemservicetypes.Id.ToString() });
                }
            }
            else
            {
                model.ListServiceTypes.Add(new SelectListItem { Text = "داده ای وجود ندراد", Value = "0" });
            }
            #endregion
            #region Get Servic types old

            //var temp_list_servicetypes = _repositoryTbl_ServicesTypes.Table.ToList();
            //if (temp_list_servicetypes != null && temp_list_servicetypes.Count > 0)
            //{
            //    model.ServiceTypeId = temp_list_servicetypes.Select(d => d.Id).ToList();

            //    model.ListServiceTypes.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0" });
            //    foreach (var itemservicetypes in temp_list_servicetypes)
            //    {
            //        model.ListServiceTypes.Add(new SelectListItem { Text = itemservicetypes.Name, Value = itemservicetypes.Id.ToString() });
            //    }
            //}
            //else
            //{
            //    model.ListServiceTypes.Add(new SelectListItem { Text = "داده ای وجود ندراد", Value = "0" });
            //}
            #endregion
            #region Transportaion Types

            //var temp_list_TransportationTypes = _repositoryTransportationTypes.Table.ToList();
            //if (temp_list_TransportationTypes != null && temp_list_TransportationTypes.Count > 0)
            //{
            //    model.ListTransportationTypes.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
            //    foreach (var itemsTransportationTypes in temp_list_TransportationTypes)
            //    {
            //        model.ListTransportationTypes.Add(new SelectListItem { Text = itemsTransportationTypes.Name, Value = itemsTransportationTypes.Id.ToString() });
            //    }
            //}
            //else
            //{
            //    model.ListTransportationTypes.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
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

            return View("/Plugins/Misc.ShippingSolutions/Views/ServiceProviders/Edit.cshtml", model);
        }

        [Area(AreaNames.Admin)]
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(Tbl_ServicesProviders model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Provider = _repositoryServicesProviders.GetById(model.Id);
            if (Provider == null || Provider.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            //update
            Provider.ServicesProviderName = _CategoryService.GetCategoryById(model.CategoryId).Name;// model.ServicesProviderName;
            Provider.CategoryId = model.CategoryId;
            Provider.AgentName = model.AgentName;
            Provider.ServiceTypeId = model.ServiceTypeId;
            Provider.advancefreight = model.advancefreight;
            Provider.cod = model.cod;
            Provider.freightforward = model.freightforward;
            Provider.MaxOrder = model.MaxOrder;
            Provider.MaxTimeDeliver = model.MaxTimeDeliver;
            Provider.MaxWeight = model.MaxWeight;
            Provider.MinWeight = model.MinWeight;
            Provider.MaxbillingamountCOD = model.MaxbillingamountCOD;
            Provider.Maxheight = model.Maxheight;
            Provider.Maxlength = model.Maxlength;
            Provider.Maxwidth = model.Maxwidth;
            Provider.IsPishtaz = model.IsPishtaz;
            Provider.IsSefareshi = model.IsSefareshi;
            Provider.IsVIje = model.IsVIje;
            Provider.IsNromal = model.IsNromal;
            Provider.IsDroonOstani = model.IsDroonOstani;
            Provider.IsAdjoining = model.IsAdjoining;
            Provider.IsNotAdjacent = model.IsNotAdjacent;
            Provider.IsHeavyTransport = model.IsHeavyTransport;
            Provider.IsForeign = model.IsForeign;
            Provider.IsInCity = model.IsInCity;
            Provider.IsTwoStep = model.IsTwoStep;
            Provider.HasHagheMaghar = model.HasHagheMaghar;
            Provider.DateUpdate = DateTime.Now;
            Provider.IdUserUpdate = _workContext.CurrentCustomer.Id;


            _repositoryServicesProviders.Update(Provider);
            #region 
            //if (model.TransportationTypeId.Count() > 0)
            //{
            //    foreach (var item in model.TransportationTypeId)
            //    {
            //        if (item > 0)
            //        {  //check duplicate
            //            Tbl_TransportationTypesProvider Duplicate = _repositoryTransportationTypesProvider.Table.Where(p => p.ProviderId == model.Id && p.TransportationTypeId == item).FirstOrDefault();
            //            if (Duplicate != null)
            //            {
            //                if (Duplicate.IsActive == false)
            //                {
            //                    Duplicate.IsActive = true;
            //                    Duplicate.DateUpdate = DateTime.Now;
            //                    Duplicate.IdUserUpdate = _workContext.CurrentCustomer.Id;
            //                    _repositoryTransportationTypesProvider.Update(Duplicate);
            //                }
            //                //ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Duplicate"));
            //                //return RedirectToAction("Edit", new { id = Provider.Id });

            //            }
            //            Tbl_TransportationTypesProvider temp = new Tbl_TransportationTypesProvider();
            //            temp.DateInsert = DateTime.Now;
            //            temp.IdUserInsert = _workContext.CurrentCustomer.Id;
            //            temp.IsActive = true;
            //            temp.ProviderId = model.Id;
            //            temp.TransportationTypeId = item;
            //            _repositoryTransportationTypesProvider.Insert(temp);


            //        }

            //    }

            //}
            #endregion
            if (model.StoreId.Count() > 0)
            {
                foreach (var item in model.StoreId)
                {
                    if (item > 0)
                    {  //check duplicate
                        Tbl_ProviderStores Duplicate = _repositoryProviderStores.Table.Where(p => p.ProviderId == model.Id && p.StoreId == item).FirstOrDefault();
                        if (Duplicate != null)
                        {
                            if (Duplicate.IsActive == false)
                            {
                                Duplicate.IsActive = true;
                                Duplicate.DateUpdate = DateTime.Now;
                                Duplicate.IdUserUpdate = _workContext.CurrentCustomer.Id;
                                _repositoryProviderStores.Update(Duplicate);
                            }
                            //ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Duplicate"));
                            //return RedirectToAction("Edit", new { id = Provider.Id });

                        }
                        Tbl_ProviderStores temp = new Tbl_ProviderStores();
                        temp.DateInsert = DateTime.Now;
                        temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                        temp.IsActive = true;
                        temp.ProviderId = model.Id;
                        temp.StoreId = item;
                        _repositoryProviderStores.Insert(temp);


                    }

                }
            }

            if (model.StateProvinceId.Count() > 0)
            {
                foreach (var item in model.StateProvinceId)
                {
                    if (item > 0)
                    {  //check duplicate
                        Tbl_Offices Duplicate = _repositoryTbl_Offices.Table.Where(p => p.ProviderId == model.Id && p.StateProvinceId == item).FirstOrDefault();
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
                        temp.ProviderId = model.Id;
                        temp.StateProvinceId = item;
                        temp.TypeOffice = false;
                        _repositoryTbl_Offices.Insert(temp);
                        //insert Tbl Time
                        string[] DayName = new[] { "Monday", "Tuesday", "Wednesday", "Thursday",
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
            _customerActivityService.InsertActivity("EditProvider", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"), Provider.ServicesProviderName);

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"));
            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = Provider.Id });
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
                return RedirectToAction("Edit", new { id = Office.CollectorId });
            var state_city = _StateProvinceService.GetStateProvinceById(Office.StateProvinceId);
            var NameProvider = _repositoryServicesProviders.GetById(Office.ProviderId);
            var model = new Tbl_Offices();
            model = Office;
            model.NameOffice = " " + state_city.Country.Name + " : " + state_city.Name + " : " + NameProvider.ServicesProviderName;

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
            //    //IdUser = _repositoryTbl_Collectors.Table.Where(p => p.Id == Office.CollectorId).Select(p => p.UserId).FirstOrDefault();
            //    //var ListAddressUser = _customerService.GetCustomerById(IdUser).Addresses.ToList();
            //    //if (ListAddressUser.Count > 0)
            //    //    AddressinIdCity_user = ListAddressUser.Where(p => p.StateProvinceId == Office.StateProvinceId).ToList();

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

       
        public virtual IActionResult EditPricingPolicy(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var ServicePrivider = _repositoryServicesProviders.GetById(id);
            if (ServicePrivider == null || ServicePrivider.IsActive == false)
            {
                //No category found with the specified id
                return RedirectToAction("List");
            }
            else
            {
                Tbl_PricingPolicy model = new Tbl_PricingPolicy();
                model.TypeUser = 3;
                model.Dealer_Customer_Id = 0;
                model.CountryId = 0;
                model.ProviderId = ServicePrivider.Id;
                model.NameUser =  " سرویس دهنده:" + ServicePrivider.ServicesProviderName;
                return View("/Plugins/Misc.ShippingSolutions/Views/PricingPolicy/Edit.cshtml", model);

            }
        }
        #endregion



    }
}
