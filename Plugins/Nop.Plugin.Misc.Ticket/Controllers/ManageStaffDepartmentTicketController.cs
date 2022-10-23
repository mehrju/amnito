using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Plugin.Misc.Ticket.Domain;
using Nop.Plugin.Misc.Ticket.Models.Grid;
using Nop.Plugin.Misc.Ticket.Models.Search;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Controllers
{
    [AdminAntiForgery]
    public class ManageStaffDepartmentTicketController : BaseAdminController
    {
        private readonly IRepository<Tbl_Ticket_Department> _repositoryTbl_Ticket_Department;
        private readonly IRepository<Tbl_Ticket_Staff_Department> _repositoryTbl_Ticket_Staff_Department;

        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IStoreService _storeService;

        public ManageStaffDepartmentTicketController
            (
          IRepository<Tbl_Ticket_Department> repositoryTbl_Ticket_Department,
        IRepository<Tbl_Ticket_Staff_Department> repositoryTbl_Ticket_Staff_Department,
        IWorkContext workContext,
        IDbContext dbContext,
        IPermissionService permissionService,
        IStaticCacheManager cacheManager,
        ILocalizationService localizationService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IStoreService storeService
            )
        {
            _repositoryTbl_Ticket_Department = repositoryTbl_Ticket_Department;
            _repositoryTbl_Ticket_Staff_Department = repositoryTbl_Ticket_Staff_Department;
            _workContext = workContext;
            _dbContext = dbContext;
            _permissionService = permissionService;
            _cacheManager = cacheManager;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _storeService = storeService;
        }

        #region index&list
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }
        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Model = new Search_Customer();
            var defaultRoleIds = new List<int> { _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered).Id };

            var allRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var role in allRoles)
            {
                Model.AvailableCustomerRoles.Add(new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Id.ToString(),
                    Selected = defaultRoleIds.Any(x => x == role.Id)
                });
            }

            var Deps = _repositoryTbl_Ticket_Department.Table.Where(p => p.IsActive == true).ToList();
            if (Deps.Count > 0)
            {
                Model.ListDepartmentTicket.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var s in Deps)
                {

                    Model.ListDepartmentTicket.Add(new SelectListItem { Text = (s.Name + "-" + (s.StoreId == 0 ? "همه فروشگاهها" : _storeService.GetStoreById(s.StoreId).Name)), Value = s.Id.ToString() });
                }
            }
            else
            {
                Model.ListDepartmentTicket.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }


            return View("/Plugins/Misc.Ticket/Views/StaffDepartmentTicket/List.cshtml", Model);
        }
        [HttpPost]
        public virtual IActionResult StaffList(DataSourceRequest command, Search_Customer model, int[] searchCustomerRoleIds)
        {
            String ErrrorMassege = "";
            //we use own own binder for searchCustomerRoleIds property 
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();
            var gridModel = new DataSourceResult();
            var Final_Staff = new List<Grid_Customer>();

            List<Tbl_Ticket_Staff_Department> tbl_Staff = _repositoryTbl_Ticket_Staff_Department.Table.OrderByDescending(p=>p.Id).ToList();//.GroupBy(p => p.UserId).Select(g => g.OrderByDescending(c => c.Id).FirstOrDefault())



           



            try
            {
                if (tbl_Staff.Count > 0)
                {
                    if (model.ActiveSearch == true)
                    {
                        if (model.SearchDepartmentId > 0)
                        {
                            tbl_Staff = tbl_Staff.Where(p => p.IdDepartment == model.SearchDepartmentId).ToList();
                            Final_Staff = (from a in tbl_Staff
                                           select new Grid_Customer
                                           {
                                               Id = a.Id,
                                               Email = _customerService.GetCustomerById(a.UserId).IsRegistered() ? _customerService.GetCustomerById(a.UserId).Email : _localizationService.GetResource("Admin.Customers.Guest"),
                                               Username = _customerService.GetCustomerById(a.UserId).Username,
                                               DepartmentTicketName = (_repositoryTbl_Ticket_Department.GetById(a.IdDepartment).Name + "-" + (_repositoryTbl_Ticket_Department.GetById(a.IdDepartment).StoreId==0?"همه فروشگاهها":_storeService.GetStoreById(_repositoryTbl_Ticket_Department.GetById(a.IdDepartment).StoreId).Name)),
                                               FullName = _customerService.GetCustomerById(a.UserId).GetFullName(),
                                               CustomerRoleNames = GetCustomerRolesNames(a.Id),
                                               Grid_Staff_IsActive = a.IsActive,
                                               Grid_DepartmentTicket_UserInsert = _customerService.GetCustomerById(a.IdUserInsert).GetFullName(),
                                               Grid_DepartmentTicket_UserUpdate = a.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)a.IdUserUpdate).GetFullName(),
                                               Grid_DepartmentTicket_DateInsert = a.DateInsert,
                                               Grid_DepartmentTicket_DateUpdate = a.DateUpdate,
                                           }).ToList();
                        }
                        if(searchCustomerRoleIds.Count()>0|| model.SearchEmail!=null|| model.SearchEmail != null|| model.SearchUsername!=null || model.SearchFirstName!=null|| model.SearchLastName!=null)
                        {
                            var search_Customer = _customerService.GetAllCustomers(
                              customerRoleIds: searchCustomerRoleIds,
                              email: model.SearchEmail,
                              username: model.SearchUsername,
                              firstName: model.SearchFirstName,
                              lastName: model.SearchLastName);

                            Final_Staff = (from a in tbl_Staff
                                           join c in search_Customer on a.UserId equals c.Id
                                           select new Grid_Customer
                                           {
                                               Id = a.Id,
                                               DepartmentTicketName = (_repositoryTbl_Ticket_Department.GetById(a.IdDepartment).Name + "-" + (_repositoryTbl_Ticket_Department.GetById(a.IdDepartment).StoreId == 0 ? "همه فروشگاهها" : _storeService.GetStoreById(_repositoryTbl_Ticket_Department.GetById(a.IdDepartment).StoreId).Name)),
                                               Email = c.IsRegistered() ? c.Email : _localizationService.GetResource("Admin.Customers.Guest"),
                                               Username = c.Username,
                                               FullName = c.GetFullName(),
                                               Grid_Staff_IsActive = a.IsActive,
                                               Grid_DepartmentTicket_UserInsert = _customerService.GetCustomerById(a.IdUserInsert).GetFullName(),
                                               Grid_DepartmentTicket_UserUpdate = a.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)a.IdUserUpdate).GetFullName(),
                                               Grid_DepartmentTicket_DateInsert = a.DateInsert,
                                               Grid_DepartmentTicket_DateUpdate = a.DateUpdate,

                                           }).ToList();
                        }
                        
                    }

                    else
                    {
                        Final_Staff = (from a in tbl_Staff
                                       select new Grid_Customer
                                       {
                                           Id = a.Id,
                                           Email =_customerService.GetCustomerById(a.UserId).IsRegistered() ? _customerService.GetCustomerById(a.UserId).Email : _localizationService.GetResource("Admin.Customers.Guest"),
                                           Username = _customerService.GetCustomerById(a.UserId).Username,
                                           DepartmentTicketName = (_repositoryTbl_Ticket_Department.GetById(a.IdDepartment).Name + "-" + (_repositoryTbl_Ticket_Department.GetById(a.IdDepartment).StoreId == 0 ? "همه فروشگاهها" : _storeService.GetStoreById(_repositoryTbl_Ticket_Department.GetById(a.IdDepartment).StoreId).Name)),
                                           FullName = _customerService.GetCustomerById(a.UserId).GetFullName(),
                                           CustomerRoleNames = GetCustomerRolesNames(a.UserId),
                                           Grid_Staff_IsActive = a.IsActive,
                                           Grid_DepartmentTicket_UserInsert = _customerService.GetCustomerById(a.IdUserInsert).GetFullName(),
                                           Grid_DepartmentTicket_UserUpdate = a.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)a.IdUserUpdate).GetFullName(),
                                           Grid_DepartmentTicket_DateInsert = a.DateInsert,
                                           Grid_DepartmentTicket_DateUpdate = a.DateUpdate,
                                       }).ToList();

                    }
                }
                else
                {
                    ErrrorMassege = "No Data Exist";
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
                    Data = Final_Staff,
                    Total = Final_Staff.Count,
                    Errors = ErrrorMassege
                };

            }

            return Json(gridModel);
        }

        protected virtual string GetCustomerRolesNames(int Id, string separator = ",")
        {
            Customer c = _customerService.GetCustomerById(Id);
            IList<CustomerRole> customerRoles = c.CustomerRoles.ToList();
            var sb = new StringBuilder();
            for (var i = 0; i < customerRoles.Count; i++)
            {
                sb.Append(customerRoles[i].Name);
                if (i != customerRoles.Count - 1)
                {
                    sb.Append(separator);
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

      
        #endregion

        #region Diables & active 
        [HttpPost]
        public virtual IActionResult DisableStaffInDepartment(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_Ticket_Staff_Department temp = _repositoryTbl_Ticket_Staff_Department.Table.Where(p => p.Id == id).FirstOrDefault();

                if (temp != null)
                {

                    temp.IsActive = false;
                    temp.DateUpdate = DateTime.Now;
                    temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_Ticket_Staff_Department.Update(temp);

                }

                //activity log
                _customerActivityService.InsertActivity("DisableStaffDepartmentTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDelete"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDelete"));
            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return Json(new { Result = true });

        }

        [HttpPost]
        public virtual IActionResult ActiveStaffInDepartment(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_Ticket_Staff_Department temp = _repositoryTbl_Ticket_Staff_Department.Table.Where(p => p.Id == id).FirstOrDefault();

                if (temp != null)
                {

                    temp.IsActive = true;
                    temp.DateUpdate = DateTime.Now;
                    temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_Ticket_Staff_Department.Update(temp);

                }

                //activity log
                _customerActivityService.InsertActivity("ActiveStaffDepartmentTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"));

            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return Json(new { Result = true });

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

        #region Create / Edit 

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            var model = new Tbl_Ticket_Staff_Department();

            #region  get list Deps
            var Deps = _repositoryTbl_Ticket_Department.Table.Where(p => p.IsActive == true).ToList();
            if (Deps.Count > 0)
            {
                model.ListDepartments.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var s in Deps)
                {

                    model.ListDepartments.Add(new SelectListItem { Text =( s.Name+"-"+(s.StoreId==0?"همه فروشگاهها":_storeService.GetStoreById(s.StoreId).Name)), Value = s.Id.ToString() });
                }
            }
            else
            {
                model.ListDepartments.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            #endregion
            return View("/Plugins/Misc.Ticket/Views/StaffDepartmentTicket/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(Tbl_Ticket_Staff_Department model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            if (model.IdDepartment > 0)
            {
                  //check duplicate
                        Tbl_Ticket_Staff_Department Duplicate = _repositoryTbl_Ticket_Staff_Department.Table.Where(p => p.UserId == model.UserId && p.IdDepartment == model.IdDepartment).FirstOrDefault();
                        if (Duplicate != null)
                        {
                            if (Duplicate.IsActive == false)
                            {
                                Duplicate.IsActive = true;
                                Duplicate.DateUpdate = DateTime.Now;
                                Duplicate.IdUserUpdate = _workContext.CurrentCustomer.Id;
                                _repositoryTbl_Ticket_Staff_Department.Update(Duplicate);
                            }
                            //ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Duplicate"));
                            //return RedirectToAction("Edit", new { id = Provider.Id });

                        }
                        Tbl_Ticket_Staff_Department temp = new Tbl_Ticket_Staff_Department();
                        temp.DateInsert = DateTime.Now;
                        temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                        temp.IsActive = true;
                        temp.UserId = model.UserId;
                        temp.IdDepartment = model.IdDepartment;
                        _repositoryTbl_Ticket_Staff_Department.Insert(temp);

            }


            //activity log
            _customerActivityService.InsertActivity("AddNewStaffInDeparmentTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.Create"), model.IdDepartment.ToString());

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.Create"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = model.Id });
            }
            return RedirectToAction("List");
            //}


            //return View("/Plugins/Misc.ShippingSolutions/Views/TransportationTypes/Create.cshtml",model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var StaffDep = _repositoryTbl_Ticket_Staff_Department.GetById(id);
            if (StaffDep == null || StaffDep.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");
            var model = new Tbl_Ticket_Staff_Department();
            model = StaffDep;
            #region user
            if (StaffDep.UserId > 0)
            {
                var Users = _customerService.GetCustomerById(StaffDep.UserId);
                if (Users != null)
                {


                    model.ListUsers.Add(new SelectListItem { Text = Users.GetFullName(), Value = Users.Id.ToString(), Selected = true });

                    //model.NameCustomer = "" + Users.GetFullName();
                }
                else
                {
                    model.ListUsers.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
                }
            }



            #endregion
            #region  get list Deps
            var Deps = _repositoryTbl_Ticket_Department.Table.Where(p => p.IsActive == true).ToList();
            if (Deps.Count > 0)
            {
                model.ListDepartments.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var s in Deps)
                {

                    model.ListDepartments.Add(new SelectListItem { Text =( s.Name + "-" +(s.StoreId==0?"همه فروشگاهها": _storeService.GetStoreById(s.StoreId).Name)), Value = s.Id.ToString() });
                }
            }
            else
            {
                model.ListDepartments.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            #endregion
            return View("/Plugins/Misc.Ticket/Views/StaffDepartmentTicket/Edit.cshtml", model);
        }


        #endregion




    }
}
