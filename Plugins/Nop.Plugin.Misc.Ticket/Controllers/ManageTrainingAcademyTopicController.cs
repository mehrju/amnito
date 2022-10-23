using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Topics;
using Nop.Data;
using Nop.Plugin.Misc.Ticket.Domain;
using Nop.Plugin.Misc.Ticket.Models.Grid;
using Nop.Plugin.Misc.Ticket.Models.Search;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Seo;
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

namespace Nop.Plugin.Misc.Ticket.Controllers
{
    public class ManageTrainingAcademyTopicController : BaseAdminController
    {
        //


        private readonly IRepository<Tbl_TrainingAcademyTopic> _repositoryTbl_TrainingAcademyTopic;

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
        public ManageTrainingAcademyTopicController
            (
        IHostingEnvironment hostingEnvironment,
        IRepository<Tbl_TrainingAcademyTopic> repositoryTbl_TrainingAcademyTopic,
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
            _repositoryTbl_TrainingAcademyTopic = repositoryTbl_TrainingAcademyTopic;
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

            var Model = new Search_TrainingAcademyTopic();
            #region stor
            var Stores = _storeService.GetAllStores(true).ToList();
            if (Stores.Count > 0)
            {
                Model.ListStore.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var s in Stores)
                {

                    Model.ListStore.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
                }
            }
            else
            {
                Model.ListStore.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            #endregion

            #region  topic
            var topics = _topicservice.GetAllTopics(0).ToList();
            if (topics.Count > 0)
            {
                Model.ListTopic.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var s in topics)
                {

                    Model.ListTopic.Add(new SelectListItem { Text = s.SystemName, Value = s.Id.ToString() });
                }
            }
            else
            {
                Model.ListTopic.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            #endregion

            return View("/Plugins/Misc.Ticket/Views/TrainingAcademyTopic/List.cshtml", Model);
        }
        public (string, List<int>) GetNameStorTopic(int Id)
        {
            string result = "";
            List<int> Storids = new List<int>();
            if (Id > 0)
            {
                Topic topic = _topicservice.GetTopicById(Id);
                if (topic.LimitedToStores == false)
                {
                    //all
                    result = "همه فروشگاهها";
                    var t = _storeService.GetAllStores(false).ToList();
                    if (t.Count > 0)
                    {
                        foreach (var item in t)
                        {
                            Storids.Add(item.Id);
                        }
                    }
                }
                else
                {

                }
                var listidstor = _storeMappingService.GetStoresIdsWithAccess(topic).ToList();
                if (listidstor.Count > 0)
                {
                    Storids = listidstor;
                    foreach (var item in listidstor)
                    {
                        result += _storeService.GetStoreById(item).Name;
                        if (listidstor.Count > 1)
                        {
                            result += ", ";
                        }

                    }
                }
            }
            return (result, Storids);
        }

        public string Urlp(int id)
        {
            Topic topic = _topicservice.GetTopicById(id);
            string s = Url.RouteUrl("Topic", new { SeName = topic.GetSeName() }, "http");
            return s;
        }
        [HttpPost]
        public virtual IActionResult TrainingTopicList(DataSourceRequest command, Search_TrainingAcademyTopic model)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();


            List<Tbl_TrainingAcademyTopic> TrainingTopics = new List<Tbl_TrainingAcademyTopic>();
            var gridModel = new DataSourceResult();
            var Final_Deps = new List<Grid_TrainingAcademyTopic>();
            try
            {
                TrainingTopics = _repositoryTbl_TrainingAcademyTopic.Table.ToList();


                if (TrainingTopics.Count > 0)
                {
                    //var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageTrainingTopic");
                    Final_Deps = (from q in TrainingTopics
                                  select new Grid_TrainingAcademyTopic
                                  {
                                      Id = q.Id,
                                      IdTopic = q.IdTopic,
                                      UrlPage = Urlp(q.IdTopic),
                                      Grid_TrainingAcademyTopic_UrlImage = "/ImageTrainingTopic/" + q.UrlImage,
                                      Grid_TrainingAcademyTopic_Title = _topicservice.GetTopicById(q.IdTopic).Title,
                                      Grid_TrainingAcademyTopic_Description = _topicservice.GetTopicById(q.IdTopic).Body,
                                      Grid_TrainingAcademyTopic_SystemName = _topicservice.GetTopicById(q.IdTopic).SystemName,
                                      Grid_TrainingAcademyTopic_StoresName = GetNameStorTopic(q.IdTopic).Item1,
                                      stores = GetNameStorTopic(q.IdTopic).Item2,
                                      Grid_TrainingAcademyTopic_IsActive = q.IsActive,
                                      Grid_TrainingAcademyTopic_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                      Grid_TrainingAcademyTopic_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                      Grid_TrainingAcademyTopic_DateInsert = q.DateInsert,
                                      Grid_TrainingAcademyTopic_DateUpdate = q.DateUpdate,
                                  }).ToList();
                    if (model.Search_TrainingAcademyTopic_ActiveSearch == true)
                    {

                        #region Search
                        if (!string.IsNullOrEmpty(model.Search_TrainingAcademyTopic_Title))
                        {
                            Final_Deps = Final_Deps.Where(p => p.Grid_TrainingAcademyTopic_Title.Contains(model.Search_TrainingAcademyTopic_Title)).ToList();
                        }
                        if (!string.IsNullOrEmpty(model.Search_TrainingAcademyTopic_Description))
                        {
                            Final_Deps = Final_Deps.Where(p => p.Grid_TrainingAcademyTopic_Description.Contains(model.Search_TrainingAcademyTopic_Description)).ToList();
                        }
                        if (!string.IsNullOrEmpty(model.Search_TrainingAcademyTopic_SystemName))
                        {
                            Final_Deps = Final_Deps.Where(p => p.Grid_TrainingAcademyTopic_SystemName.Contains(model.Search_TrainingAcademyTopic_SystemName)).ToList();
                        }
                        TrainingTopics = TrainingTopics.Where(p => p.IsActive == model.Search_TrainingAcademyTopic_IsActive).ToList();
                        if (model.Search_TrainingAcademyTopic_TopicId > 0)
                        {
                            Final_Deps = Final_Deps.Where(p => p.IdTopic == (model.Search_TrainingAcademyTopic_TopicId)).ToList();

                        }
                        if (model.Search_TrainingAcademyTopic_StoreId > 0)
                        {
                            Final_Deps = Final_Deps.Where(p => p.stores.Contains(model.Search_TrainingAcademyTopic_TopicId)).ToList();

                        }
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
                Tbl_TrainingAcademyTopic Tar = _repositoryTbl_TrainingAcademyTopic.Table.Where(p => p.Id == id).FirstOrDefault();

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
                    _repositoryTbl_TrainingAcademyTopic.Update(Tar);
                }
                //activity log
                _customerActivityService.InsertActivity("DisableTrainingTopic", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDisable"), Tar.Id.ToString());
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDisable"));
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
                        Tbl_TrainingAcademyTopic TrainingTopic = _repositoryTbl_TrainingAcademyTopic.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (TrainingTopic != null)
                        {

                            TrainingTopic.IsActive = false;
                            TrainingTopic.DateUpdate = DateTime.Now;
                            TrainingTopic.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_TrainingAcademyTopic.Update(TrainingTopic);
                        }
                    }
                }
                _customerActivityService.InsertActivity("DisableTrainingTopic", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDisable"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDisable"));
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
                Tbl_TrainingAcademyTopic TrainingTopic = _repositoryTbl_TrainingAcademyTopic.Table.Where(p => p.Id == id).FirstOrDefault();

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
                    _repositoryTbl_TrainingAcademyTopic.Update(TrainingTopic);
                }
                //activity log
                _customerActivityService.InsertActivity("ActiveTrainingTopic", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"), TrainingTopic.Id.ToString());
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"));
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
                        Tbl_TrainingAcademyTopic TrainingTopic = _repositoryTbl_TrainingAcademyTopic.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (TrainingTopic != null)
                        {

                            TrainingTopic.IsActive = true;
                            TrainingTopic.DateUpdate = DateTime.Now;
                            TrainingTopic.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_TrainingAcademyTopic.Update(TrainingTopic);
                        }
                    }
                }
                _customerActivityService.InsertActivity("ActiveTrainingTopic", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"));

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
            var model = new Tbl_TrainingAcademyTopic();
            #region  topic
            var topics = _topicservice.GetAllTopics(0).ToList();
            if (topics.Count > 0)
            {
                model.ListTopic.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var s in topics)
                {

                    model.ListTopic.Add(new SelectListItem { Text = s.SystemName, Value = s.Id.ToString() });
                }
            }
            else
            {
                model.ListTopic.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            #endregion

            return View("/Plugins/Misc.Ticket/Views/TrainingAcademyTopic/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(Tbl_TrainingAcademyTopic model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //if (ModelState.IsValid)
            //{
            #region Check Duplicate
            Tbl_TrainingAcademyTopic TrainingTopic1 = _repositoryTbl_TrainingAcademyTopic.Table.Where(p => p.IdTopic == model.IdTopic && p.IsActive).FirstOrDefault();
            if (TrainingTopic1 != null)
            {
                ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.Duplicate"));
                return View("/Plugins/Misc.Ticket/Views/TrainingAcademyTopic/Create.cshtml");

            }
            #endregion
            Tbl_TrainingAcademyTopic TrainingTopic = new Tbl_TrainingAcademyTopic();
            TrainingTopic.IdTopic = model.IdTopic;
            TrainingTopic.IsActive = true;
            TrainingTopic.DateInsert = DateTime.Now;
            TrainingTopic.IdUserInsert = _workContext.CurrentCustomer.Id;
            _repositoryTbl_TrainingAcademyTopic.Insert(TrainingTopic);

            //activity log
            _customerActivityService.InsertActivity("AddNewTrainingTopic", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.Create"), TrainingTopic.Id.ToString());

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.Create"));

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

            var TrainingTopic = _repositoryTbl_TrainingAcademyTopic.GetById(id);
            if (TrainingTopic == null || TrainingTopic.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");



            var model = TrainingTopic;
            model.NameTopic4Edit = _topicservice.GetTopicById(TrainingTopic.IdTopic).SystemName;
            model.UrlImage = "/ImageTrainingTopic/" + model.UrlImage;
            #region  topic
            var topics = _topicservice.GetAllTopics(0).ToList();
            if (topics.Count > 0)
            {
                model.ListTopic.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var s in topics)
                {

                    model.ListTopic.Add(new SelectListItem { Text = s.SystemName, Value = s.Id.ToString() });
                }
            }
            else
            {
                model.ListTopic.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            #endregion
            return View("/Plugins/Misc.Ticket/Views/TrainingAcademyTopic/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(Tbl_TrainingAcademyTopic model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var TrainingTopic = _repositoryTbl_TrainingAcademyTopic.GetById(model.Id);
            if (TrainingTopic == null || TrainingTopic.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            //update
            TrainingTopic.IdTopic = model.IdTopic;
            TrainingTopic.DateUpdate = DateTime.Now;
            TrainingTopic.IdUserUpdate = _workContext.CurrentCustomer.Id;
            _repositoryTbl_TrainingAcademyTopic.Update(TrainingTopic);
            //activity log
            _customerActivityService.InsertActivity("EditTrainingTopic", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageUpdate"), TrainingTopic.Id.ToString());

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageUpdate"));
            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = TrainingTopic.Id });
            }
            return RedirectToAction("List");
        }

        #endregion


        [HttpPost]
        [AdminAntiForgery(true)]
        public IActionResult TrainingTopic(int IdTopic, int Id)
        {
            if (!_workContext.CurrentCustomer.IsAdmin())
                return Challenge();//login


            var files = Request.Form.Files;
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "ImageTrainingTopic");
            var filename = "";

            #region check params

            if (IdTopic == 0)
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
                                    filename = number.Next(1, 999999999).ToString() + format;
                                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageTrainingTopic", filename);
                                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageTrainingTopic")))
                                    {
                                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageTrainingTopic"));
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

                    Tbl_TrainingAcademyTopic TrainingTopic = new Tbl_TrainingAcademyTopic();
                    TrainingTopic.IdTopic = IdTopic;
                    if (filename != "")
                    {

                        TrainingTopic.UrlImage = filename;
                    }
                    TrainingTopic.IsActive = true;
                    TrainingTopic.DateInsert = DateTime.Now;
                    TrainingTopic.IdUserInsert = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_TrainingAcademyTopic.Insert(TrainingTopic);

                    //activity log
                    _customerActivityService.InsertActivity("AddNewTrainingTopic", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.Create"), TrainingTopic.Id.ToString());
                    SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.Create"));
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
                Tbl_TrainingAcademyTopic TrainingTopic1 = _repositoryTbl_TrainingAcademyTopic.Table.Where(p => p.Id == Id).FirstOrDefault();
                if (TrainingTopic1.IdTopic != IdTopic)
                {
                    Tbl_TrainingAcademyTopic duplicate = _repositoryTbl_TrainingAcademyTopic.Table.Where(p => p.IdTopic == IdTopic).FirstOrDefault();
                    if (duplicate != null)
                    {

                        return Json(new { error = true, Status = 111 });
                    }

                }
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
                                    filename = number.Next(1, 999999999).ToString() + format;
                                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageTrainingTopic", filename);
                                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageTrainingTopic")))
                                    {
                                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageTrainingTopic"));
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
                        var oldfile = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageTrainingTopic", TrainingTopic1.UrlImage);
                        if (System.IO.File.Exists(oldfile))
                        {
                            System.IO.File.Delete(oldfile);
                        }
                    }
                    #endregion

                    TrainingTopic1.IdTopic = IdTopic;
                    if (filename != "")
                    {
                        TrainingTopic1.UrlImage = filename;
                    }
                    TrainingTopic1.DateUpdate = DateTime.Now;
                    TrainingTopic1.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_TrainingAcademyTopic.Update(TrainingTopic1);
                    _customerActivityService.InsertActivity("EditTrainingTopic", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageUpdate"), TrainingTopic1.Id.ToString());
                    SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageUpdate"));
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
