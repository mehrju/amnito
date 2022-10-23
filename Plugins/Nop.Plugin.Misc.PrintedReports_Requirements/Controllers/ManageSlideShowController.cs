using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class ManageSlideShowController : BaseAdminController
    {
        private readonly IRepository<Tbl_Carousel_slideshow> _repositoryTbl_Carousel_slideshow;

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

        public ManageSlideShowController
            (
              IHostingEnvironment hostingEnvironment,
        IRepository<Tbl_Carousel_slideshow> repositoryTbl_Carousel_slideshow,
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
            _repositoryTbl_Carousel_slideshow = repositoryTbl_Carousel_slideshow;
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

            var Model = new Search_SlideShow();
            #region get Stores
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
            #endregion
            return View("/Plugins/Misc.PrintedReports_Requirements/Views/SlideShow/List.cshtml", Model);
        }

        [HttpPost]
        public virtual IActionResult SlideShowList(DataSourceRequest command, Search_SlideShow model)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();


            List<Tbl_Carousel_slideshow> Providers = new List<Tbl_Carousel_slideshow>();
            var gridModel = new DataSourceResult();
            var Final_Deps = new List<Grid_SlideShow>();
            try
            {
                Providers = _repositoryTbl_Carousel_slideshow.Table.ToList();


                if (Providers.Count > 0)
                {

                    Final_Deps = (from q in Providers
                                  select new Grid_SlideShow
                                  {
                                      Id = q.Id,
                                      Grid_SlideShow_UrlImage = "/ImageSlideShowDashboard/" + q.UrlImage,
                                      Grid_SlideShow_UrlPage = q.UrlPage,
                                      Grid_SlideShow_Title = q.Title_Carousel_slideshow,
                                      Grid_SlideShow_Dis = q.Discrition_Carousel_slideshow,
                                      Grid_SlideShow_IsActive = q.IsActive,
                                      Grid_SlideShow_IsVideo = q.IsVideo,
                                      Grid_SlideShow_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                      Grid_SlideShow_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                      Grid_SlideShow_DateInsert = q.DateInsert,
                                      Grid_SlideShow_DateUpdate = q.DateUpdate,
                                      Grid_SlideShow_NameStore = GetNameStore(q.LimitedStore)
                                  }).ToList();
                    if (model.Search_SlideShow_ActiveSearch == true)
                    {

                        #region Search
                        if (!string.IsNullOrEmpty(model.Search_SlideShow_Title))
                        {
                            Final_Deps = Final_Deps.Where(p => p.Grid_SlideShow_Title.Contains(model.Search_SlideShow_Title)).ToList();
                        }

                        Final_Deps = Final_Deps.Where(p => p.Grid_SlideShow_IsActive == model.Search_SlideShow_IsActive).ToList();
                        Final_Deps = Final_Deps.Where(p => p.Grid_SlideShow_IsVideo == model.Search_SlideShow_IsVideo).ToList();
                        if (model.Search_StoreId > 0)
                        {
                            Final_Deps = Final_Deps.Where(p => p.Grid_SlideShow_NameStore.Contains(_storeService.GetStoreById(model.Search_StoreId).Name)).ToList();

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

        private string GetNameStore(string limitedStore)
        {
            string result = "";
            string[] strores = limitedStore.Split(',');
            if (strores.Count() > 0)
            {
                foreach (var item in strores)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        result += _storeService.GetStoreById(Convert.ToInt32(item)).Name + ",";
                    }
                }
            }
            return result;
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
                Tbl_Carousel_slideshow Tar = _repositoryTbl_Carousel_slideshow.Table.Where(p => p.Id == id).FirstOrDefault();

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
                    _repositoryTbl_Carousel_slideshow.Update(Tar);
                }
                //activity log
                _customerActivityService.InsertActivity("DisableslideshowDashboard", _localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.MessageDisable"), Tar.Title_Carousel_slideshow.ToString());
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
                        Tbl_Carousel_slideshow TrainingTopic = _repositoryTbl_Carousel_slideshow.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (TrainingTopic != null)
                        {

                            TrainingTopic.IsActive = false;
                            TrainingTopic.DateUpdate = DateTime.Now;
                            TrainingTopic.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_Carousel_slideshow.Update(TrainingTopic);
                        }
                    }
                }
                _customerActivityService.InsertActivity("Disableslideshow", _localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.MessageDisable"));
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
                Tbl_Carousel_slideshow TrainingTopic = _repositoryTbl_Carousel_slideshow.Table.Where(p => p.Id == id).FirstOrDefault();

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
                    _repositoryTbl_Carousel_slideshow.Update(TrainingTopic);
                }
                //activity log
                _customerActivityService.InsertActivity("ActiveslideshowDashboard", _localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.MessageActive"), TrainingTopic.Title_Carousel_slideshow.ToString());
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
                        Tbl_Carousel_slideshow TrainingTopic = _repositoryTbl_Carousel_slideshow.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (TrainingTopic != null)
                        {

                            TrainingTopic.IsActive = true;
                            TrainingTopic.DateUpdate = DateTime.Now;
                            TrainingTopic.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_Carousel_slideshow.Update(TrainingTopic);
                        }
                    }
                }
                _customerActivityService.InsertActivity("ActiveslideshowDashboard", _localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.MessageActive"));
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
            var model = new Tbl_Carousel_slideshow();
            //model.IsVideo = false;
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
            return View("/Plugins/Misc.PrintedReports_Requirements/Views/SlideShow/Create.cshtml", model);
        }



        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var TrainingTopic = _repositoryTbl_Carousel_slideshow.GetById(id);
            if (TrainingTopic == null || TrainingTopic.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");



            var model = TrainingTopic;
            model.UrlImage = "ImageSlideShowDashboard/" + model.UrlImage;
            model.UrlImageMobile = "ImageSlideShowDashboard/" + model.UrlImageMobile;
            model.IsVideo = TrainingTopic.IsVideo;
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
            string[] strores = TrainingTopic.LimitedStore.Split(',');
            if (Stores.Count > 0)
            {
                model.StoreId = new List<int>();
                foreach (var item in strores)
                {
                    if (!string.IsNullOrEmpty(item))
                    {

                        model.StoreId.Add(Convert.ToInt32(item));
                    }

                }
            }
            return View("/Plugins/Misc.PrintedReports_Requirements/Views/SlideShow/Edit.cshtml", model);
        }



        #endregion


        [HttpPost]
        [AdminAntiForgery(true)]
        public IActionResult SlideShowDashboard(string Title, string Dis, string UrlPage, int Id, bool IsVideo, int TimeInterval, string _Store, DateTime _DS, DateTime _DE)
        {
            if (!_workContext.CurrentCustomer.IsAdmin())
                return AccessDeniedView();


            var files = Request.Form.Files;
            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "ImageSlideShowDashboard");
            var filename = "";

            var filename1 = "";
            var filename2 = "";
            var filenamevideo = "";
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
                                    format = ".jpg";
                                    //if (oldfilename.Contains(".jpg"))
                                    //{

                                    //}
                                    //if (oldfilename.Contains(".png"))
                                    //{
                                    //    format = ".png";
                                    //}
                                    filename = number.Next(1, 999999999).ToString() + format;
                                    if (oldfilename.Contains("Des"))
                                    {
                                        filename1 = filename;

                                    }
                                    if (oldfilename.Contains("Mobile"))
                                    {
                                        filename2 = filename;

                                    }
                                    if (oldfilename.Contains("video"))
                                    {
                                        filenamevideo = number.Next(1, 999999999).ToString() + ".mp4";
                                        filename = filenamevideo;

                                    }
                                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageSlideShowDashboard", filename);
                                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageSlideShowDashboard")))
                                    {
                                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageSlideShowDashboard"));
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

                    Tbl_Carousel_slideshow TrainingTopic = new Tbl_Carousel_slideshow();
                    TrainingTopic.Title_Carousel_slideshow = Title;
                    if (IsVideo)
                    {
                        TrainingTopic.UrlPage = @"ImageSlideShowDashboard\" + filenamevideo;

                    }
                    else
                    {
                        TrainingTopic.UrlPage = UrlPage;
                        TrainingTopic.Discrition_Carousel_slideshow = Dis;

                    }
                    TrainingTopic.IsVideo = IsVideo;
                    TrainingTopic.TimeInterval = TimeInterval;
                    TrainingTopic.DateStart = _DS;
                    TrainingTopic.DateExpire = _DE;
                    TrainingTopic.LimitedStore = _Store;
                    if (filename1 != "")
                    {
                        TrainingTopic.UrlImage = filename1;
                    }
                    if (filename2 != "")
                    {
                        TrainingTopic.UrlImageMobile = filename2;
                    }
                    TrainingTopic.IsActive = true;
                    TrainingTopic.DateInsert = DateTime.Now;
                    TrainingTopic.IdUserInsert = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_Carousel_slideshow.Insert(TrainingTopic);

                    //activity log
                    _customerActivityService.InsertActivity("AddNewSlideShowDashboard", _localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.Create"), TrainingTopic.Title_Carousel_slideshow.ToString());
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
                Tbl_Carousel_slideshow TrainingTopic1 = _repositoryTbl_Carousel_slideshow.Table.Where(p => p.Id == Id).FirstOrDefault();

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
                                    format = ".jpg";
                                    //if (oldfilename.Contains(".jpg"))
                                    //{

                                    //}
                                    //if (oldfilename.Contains(".png"))
                                    //{
                                    //    format = ".png";
                                    //}
                                    filename = number.Next(1, 999999999).ToString() + format;
                                    if (oldfilename.Contains("Des"))
                                    {
                                        filename1 = filename;

                                    }
                                    if (oldfilename.Contains("Mobile"))
                                    {
                                        filename2 = filename;

                                    }
                                    if (oldfilename.Contains("video"))
                                    {
                                        filenamevideo = number.Next(1, 999999999).ToString() + ".mp4";
                                        filename = filenamevideo;

                                    }
                                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageSlideShowDashboard", filename);
                                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageSlideShowDashboard")))
                                    {
                                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageSlideShowDashboard"));
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
                    if (TrainingTopic1.UrlImage != null && filename1 != null)
                    {
                        var oldfile = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageSlideShowDashboard", TrainingTopic1.UrlImage);
                        if (System.IO.File.Exists(oldfile))
                        {
                            System.IO.File.Delete(oldfile);
                        }
                    }
                    if (TrainingTopic1.IsVideo && filenamevideo != null)
                    {
                        var oldfile = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageSlideShowDashboard", TrainingTopic1.UrlPage);
                        if (System.IO.File.Exists(oldfile))
                        {
                            System.IO.File.Delete(oldfile);
                        }
                    }
                    if (TrainingTopic1.UrlImageMobile != null && filename2 != null)
                    {
                        var oldfile = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImageSlideShowDashboard", TrainingTopic1.UrlImageMobile);
                        if (System.IO.File.Exists(oldfile))
                        {
                            System.IO.File.Delete(oldfile);
                        }
                    }
                    #endregion

                    TrainingTopic1.Title_Carousel_slideshow = Title;
                    TrainingTopic1.IsVideo = IsVideo;
                    TrainingTopic1.TimeInterval = TimeInterval;
                    TrainingTopic1.LimitedStore = _Store;
                    if (IsVideo)
                    {
                        TrainingTopic1.UrlPage = @"ImageSlideShowDashboard\" + filenamevideo;
                        TrainingTopic1.Discrition_Carousel_slideshow = "";
                        TrainingTopic1.UrlImage = "";
                        TrainingTopic1.UrlImageMobile = "";

                    }
                    else
                    {
                        TrainingTopic1.UrlPage = UrlPage;
                        TrainingTopic1.Discrition_Carousel_slideshow = Dis;
                    }
                    if (filename1 != "")
                    {
                        TrainingTopic1.UrlImage = filename1;
                    }
                    if (filename2 != "")
                    {
                        TrainingTopic1.UrlImageMobile = filename2;
                    }
                    TrainingTopic1.DateUpdate = DateTime.Now;
                    TrainingTopic1.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    TrainingTopic1.DateExpire = _DE;
                    TrainingTopic1.DateStart = _DS;
                    _repositoryTbl_Carousel_slideshow.Update(TrainingTopic1);
                    _customerActivityService.InsertActivity("EditSlidShowDashboard", _localizationService.GetResource("Nop.Plugin.Misc.PostbarDashboard.MessageUpdate"), TrainingTopic1.Id.ToString());
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
