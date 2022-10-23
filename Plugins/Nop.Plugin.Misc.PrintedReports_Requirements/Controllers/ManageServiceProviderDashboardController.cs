using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Plugin.Misc.PostbarDashboard.Models;
using Nop.Plugin.Misc.PrintedReports_Requirements.Models.Grid;
using Nop.Plugin.Misc.PrintedReports_Requirements.Models.Search;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PrintedReports_Requirements.Controllers
{
    public class ManageServiceProviderDashboardController : BaseAdminController
    {
        private readonly IRepository<Tbl_ServiceProviderDashboard> _repositoryTbl_ServiceProviderDashboard;

        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IStoreService _storeService;
        private readonly ITopicService _topicservice;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IHostingEnvironment _hostingEnvironment;
        public ManageServiceProviderDashboardController
            (
        IHostingEnvironment hostingEnvironment,
        IRepository<Tbl_ServiceProviderDashboard> repositoryTbl_ServiceProviderDashboard,
        IWorkContext workContext,
        IDbContext dbContext,
        IPermissionService permissionService,
        IStaticCacheManager cacheManager,
        ILocalizationService localizationService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IStoreService storeService,
        ITopicService topicservice,
        IStoreMappingService storeMappingService
            )
        {
            _hostingEnvironment = hostingEnvironment;
            _repositoryTbl_ServiceProviderDashboard = repositoryTbl_ServiceProviderDashboard;
            _workContext = workContext;
            _dbContext = dbContext;
            _permissionService = permissionService;
            _cacheManager = cacheManager;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _storeService = storeService;
            _topicservice = topicservice;
            _storeMappingService = storeMappingService;
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
            if (!_workContext.CurrentCustomer.IsAdmin())
                return AccessDeniedView();

            var Model = new Search_ServiceProviderDashboard();
            return View("/Plugins/Misc.PrintedReports_Requirements/Views/ServiceProviderDashboard/List.cshtml", Model);
        }

        [HttpPost]
        public virtual IActionResult ProviderList(DataSourceRequest command, Search_ServiceProviderDashboard model)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();


            List<Tbl_ServiceProviderDashboard> Providers = new List<Tbl_ServiceProviderDashboard>();
            var gridModel = new DataSourceResult();
            var Final_Deps = new List<Grid_ServiceProviderDashboard>();
            try
            {
                Providers = _repositoryTbl_ServiceProviderDashboard.Table.ToList();


                if (Providers.Count > 0)
                {

                    Final_Deps = (from q in Providers
                                  select new Grid_ServiceProviderDashboard
                                  {
                                      Id = q.Id,
                                      Grid_ServiceProviderDashboard_UrlImage = "/ImageServiceProviderDashboard/" + q.UrlImage,
                                      Grid_ServiceProviderDashboard_UrlPage=q.UrlPageDiscreption,
                                      Grid_ServiceProviderDashboard_Title = q.TitleServiceProviderDashboard,
                                      Grid_ServiceProviderDashboard_IsActive = q.IsActive,
                                      Grid_ServiceProviderDashboard_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                      Grid_ServiceProviderDashboard_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                      Grid_ServiceProviderDashboard_DateInsert = q.DateInsert,
                                      Grid_ServiceProviderDashboard_DateUpdate = q.DateUpdate,
                                  }).ToList();
                    if (model.Search_ServiceProviderDashboard_ActiveSearch == true)
                    {

                        #region Search
                        if (!string.IsNullOrEmpty(model.Search_ServiceProviderDashboard_Title))
                        {
                            Final_Deps = Final_Deps.Where(p => p.Grid_ServiceProviderDashboard_Title.Contains(model.Search_ServiceProviderDashboard_Title)).ToList();
                        }

                        Final_Deps = Final_Deps.Where(p => p.Grid_ServiceProviderDashboard_IsActive == model.Search_ServiceProviderDashboard_IsActive).ToList();

                        #endregion
                    }
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
                    Data = Final_Deps,
                    Total = Final_Deps.Count,
                    Errors = ErrrorMassege,
                };

            }
            return Json(gridModel);

        }
        #endregion
        #region Diable & Active
        [HttpPost]
        public virtual IActionResult Disable(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_ServiceProviderDashboard Tar = _repositoryTbl_ServiceProviderDashboard.Table.Where(p => p.Id == id).FirstOrDefault();

                if (Tar == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                //a vendor should have access only to his products
                //if (_workContext.CurrentVendor != null && Providre.VendorId != _workContext.CurrentVendor.Id)
                //    return RedirectToAction("List");

                else
                {
                    Tar.IsActive = false;
                    Tar.DateUpdate = DateTime.Now;
                    Tar.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_ServiceProviderDashboard.Update(Tar);
                }
                //activity log
                _customerActivityService.InsertActivity("DisableServiceProviderDashboard", _localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.MessageDisable"), Tar.TitleServiceProviderDashboard.ToString());
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.MessageDisable"));
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
                        Tbl_ServiceProviderDashboard TrainingTopic = _repositoryTbl_ServiceProviderDashboard.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (TrainingTopic != null)
                        {

                            TrainingTopic.IsActive = false;
                            TrainingTopic.DateUpdate = DateTime.Now;
                            TrainingTopic.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_ServiceProviderDashboard.Update(TrainingTopic);
                        }
                    }
                }
                _customerActivityService.InsertActivity("DisableTrainingTopic", _localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.MessageDisable"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.MessageDisable"));
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
                Tbl_ServiceProviderDashboard TrainingTopic = _repositoryTbl_ServiceProviderDashboard.Table.Where(p => p.Id == id).FirstOrDefault();

                if (TrainingTopic == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                //a vendor should have access only to his products
                //if (_workContext.CurrentVendor != null && Providre.VendorId != _workContext.CurrentVendor.Id)
                //    return RedirectToAction("List");

                else
                {
                    TrainingTopic.IsActive = true;
                    TrainingTopic.DateUpdate = DateTime.Now;
                    TrainingTopic.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_ServiceProviderDashboard.Update(TrainingTopic);
                }
                //activity log
                _customerActivityService.InsertActivity("ActiveServiceProviderDashboard", _localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.MessageActive"), TrainingTopic.TitleServiceProviderDashboard.ToString());
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.MessageActive"));
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
                        Tbl_ServiceProviderDashboard TrainingTopic = _repositoryTbl_ServiceProviderDashboard.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (TrainingTopic != null)
                        {

                            TrainingTopic.IsActive = true;
                            TrainingTopic.DateUpdate = DateTime.Now;
                            TrainingTopic.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_ServiceProviderDashboard.Update(TrainingTopic);
                        }
                    }
                }
                _customerActivityService.InsertActivity("ActiveServiceProviderDashboard", _localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.MessageActive"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.MessageActive"));

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
            var model = new Tbl_ServiceProviderDashboard();

            return View("/Plugins/Misc.PrintedReports_Requirements/Views/ServiceProviderDashboard/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(Tbl_ServiceProviderDashboard model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //if (ModelState.IsValid)
            //{
            #region Check Duplicate
            Tbl_ServiceProviderDashboard TrainingTopic1 = _repositoryTbl_ServiceProviderDashboard.Table.Where(p => p.TitleServiceProviderDashboard == model.TitleServiceProviderDashboard && p.IsActive).FirstOrDefault();
            if (TrainingTopic1 != null)
            {
                ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.Duplicate"));
                return View("/Plugins/Misc.PrintedReports_Requirements/Views/ServiceProviderDashboard/Create.cshtml");

            }
            #endregion
            Tbl_ServiceProviderDashboard TrainingTopic = new Tbl_ServiceProviderDashboard();
            TrainingTopic.TitleServiceProviderDashboard = model.TitleServiceProviderDashboard;
            TrainingTopic.UrlPageDiscreption = model.UrlPageDiscreption;
            TrainingTopic.IsActive = true;
            TrainingTopic.DateInsert = DateTime.Now;
            TrainingTopic.IdUserInsert = _workContext.CurrentCustomer.Id;
            _repositoryTbl_ServiceProviderDashboard.Insert(TrainingTopic);

            //activity log
            _customerActivityService.InsertActivity("AddNewServiceProviderDashboard", _localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.Create"), TrainingTopic.TitleServiceProviderDashboard.ToString());

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.Create"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();
                return RedirectToAction("Edit", new { id = TrainingTopic.Id });
            }
            return RedirectToAction("List");
            //}


            //return View("/Plugins/Misc.ShippingSolutions/Views/ServiceTypes/Create.cshtml",model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var TrainingTopic = _repositoryTbl_ServiceProviderDashboard.GetById(id);
            if (TrainingTopic == null || TrainingTopic.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");



            var model = TrainingTopic;
            model.UrlImage = "/ImageServiceProviderDashboard/" + model.UrlImage;

            return View("/Plugins/Misc.PrintedReports_Requirements/Views/ServiceProviderDashboard/Edit.cshtml", model);
        }



        #endregion


        [HttpPost]
        [AdminAntiForgery(true)]
        public IActionResult ServiceProviderDashboard(string Title,string UrlPage, int Id)
        {
            if (!_workContext.CurrentCustomer.IsAdmin())
                return AccessDeniedView();


            var files = Request.Form.Files;
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "ImageServiceProviderDashboard");
            var filename = "";

            #region check params

            if (string.IsNullOrEmpty(Title))
            {
                return Json(new { error = true, Status = 110 });

            }
            #endregion

            if (Id == 0)
            {
                //new=insert
                //1-check file exits
                //2-insert file
                //3-insert to table
                try
                {
                    #region file
                    if (files != null && files.Count() > 0)
                    {
                        foreach (var item in files)
                        {
                            if (item != null)
                            {
                                if (files.First().Length > 3145728)
                                {
                                    return Json(new { error = true, Status = 113 });

                                }
                                else
                                {
                                    var number = new Random();
                                    string oldfilename = item.FileName;
                                    string format = "";
                                    if (oldfilename.Contains(".jpg"))
                                    {
                                        format = ".jpg";
                                    }
                                    if (oldfilename.Contains(".png"))
                                    {
                                        format = ".png";
                                    }
                                    filename = number.Next(1, 999999999).ToString() + format;
                                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageServiceProviderDashboard", filename);
                                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageServiceProviderDashboard")))
                                    {
                                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageServiceProviderDashboard"));
                                    }
                                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                                    {
                                        item.CopyTo(fileStream);
                                    }
                                }
                            }
                        }


                    }
                    else
                    {
                        return Json(new { error = true, Status = 112 });
                    }
                    #endregion

                    #region insert into table

                    Tbl_ServiceProviderDashboard TrainingTopic = new Tbl_ServiceProviderDashboard();
                    TrainingTopic.TitleServiceProviderDashboard = Title;
                    TrainingTopic.UrlPageDiscreption = UrlPage;
                    if (filename != "")
                    {
                        TrainingTopic.UrlImage = filename;
                    }
                    TrainingTopic.IsActive = true;
                    TrainingTopic.DateInsert = DateTime.Now;
                    TrainingTopic.IdUserInsert = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_ServiceProviderDashboard.Insert(TrainingTopic);

                    //activity log
                    _customerActivityService.InsertActivity("AddNewServiceProviderDashboard", _localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.Create"), TrainingTopic.TitleServiceProviderDashboard.ToString());
                    SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.Create"));
                    return Json(new { success = true });
                    #endregion
                }
                catch
                {
                    return Json(new { success = false });
                }
            }
            else
            {
                //update
                Tbl_ServiceProviderDashboard TrainingTopic1 = _repositoryTbl_ServiceProviderDashboard.Table.Where(p => p.Id == Id).FirstOrDefault();

                //update
                //1-check file exits
                //2-uplad file
                //3-update table
                try
                {
                    #region file
                    if (files != null && files.Count() > 0)
                    {
                        foreach (var item in files)
                        {
                            if (item != null)
                            {
                                if (files.First().Length > 3145728)
                                {
                                    return Json(new { error = true, Status = 113 });
                                }
                                else
                                {
                                    var number = new Random();
                                    string oldfilename = item.FileName;
                                    string format = "";
                                    if (oldfilename.Contains(".jpg"))
                                    {
                                        format = ".jpg";
                                    }
                                    if (oldfilename.Contains(".png"))
                                    {
                                        format = ".png";
                                    }
                                    filename = number.Next(1, 999999999).ToString() + format;
                                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageServiceProviderDashboard", filename);
                                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageServiceProviderDashboard")))
                                    {
                                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageServiceProviderDashboard"));
                                    }
                                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                                    {
                                        item.CopyTo(fileStream);
                                    }
                                }
                            }
                        }


                    }

                    #endregion

                    #region delete old file
                    if (TrainingTopic1.UrlImage != null)
                    {
                        var oldfile = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageServiceProviderDashboard", TrainingTopic1.UrlImage);
                        if (System.IO.File.Exists(oldfile))
                        {
                            System.IO.File.Delete(oldfile);
                        }
                    }
                    #endregion

                    TrainingTopic1.TitleServiceProviderDashboard = Title;
                    TrainingTopic1.UrlPageDiscreption = UrlPage;
                    if (filename != "")
                    {
                        TrainingTopic1.UrlImage = filename;
                    }
                    TrainingTopic1.DateUpdate = DateTime.Now;
                    TrainingTopic1.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_ServiceProviderDashboard.Update(TrainingTopic1);
                    _customerActivityService.InsertActivity("EditServiceProviderDashboard", _localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.MessageUpdate"), TrainingTopic1.Id.ToString());
                    SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.MessageUpdate"));
                    return Json(new { success = true });
                }
                catch
                {
                    return Json(new { success = false });
                }

            }

        }

    }
}
