using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Plugins;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using Nop.Services.Media;
using Nop.Services.Vendors;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework;
using Nop.Services.Seo;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog;
using System.Web;
using System.IO;
using BS.Plugin.NopStation.MobileWebApi.Models.SliderImageModel;
using Nop.Web.Framework.Security;
using System.Globalization;
using BS.Plugin.NopStation.MobileWebApi.Models.NstSettingsModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class MobileWebApiConfigurationController : BasePluginController
    {
        #region Fields

        private const string IconFilePath = "Plugins/NopStation.MobileWebApi/Content/IconPackage";

        private readonly IWorkContext _workContext;
        private readonly IBsNopMobilePluginService _nopMobileService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IPictureService _pictureService;
        private readonly MobileWebApiSettings _webApiSettings;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly IVendorService _vendorService;
        private readonly IContentManagementTemplateService _contentManagementTemplateService;
        private readonly IContentManagementService _contentManagementService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly ICategoryIconService _categoryIconService;
        private readonly IBS_SliderService _bsSliderService;

        #endregion
   
        public MobileWebApiConfigurationController(IWorkContext workContext,
            IBsNopMobilePluginService nopMobileService,
            IWebHelper webHelper,
            IProductService productService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IPluginFinder pluginFinder,
            MobileWebApiSettings webApiSettings,
            ISettingService settingService,
            IPermissionService permissionService,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            IStoreService storeService,
            IVendorService vendorService,
            IPictureService pictureService,
            IContentManagementTemplateService contentManagementTemplateService,
            IContentManagementService contentManagementService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            ICategoryIconService categoryIconService,
            IBS_SliderService bsSliderService
            )
        {
            this._categoryService = categoryService;
            this._vendorService = vendorService;
            this._manufacturerService = manufacturerService;
            this._workContext = workContext;
            this._webHelper = webHelper;
            this._nopMobileService = nopMobileService;
            this._pluginFinder = pluginFinder;
            this._webApiSettings = webApiSettings;
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._permissionService = permissionService;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._pictureService = pictureService;
            this._productService = productService;
            this._contentManagementTemplateService = contentManagementTemplateService;
            this._contentManagementService = contentManagementService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._storeMappingService = storeMappingService;
            this._urlRecordService = urlRecordService;
            this._categoryIconService = categoryIconService;
            this._bsSliderService = bsSliderService;
        }


        #region Utilities

        [NonAction]
        protected virtual void PrepareTemplatesModel(TopicModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var templates = _contentManagementTemplateService.GetAllTopicTemplates();
            foreach (var template in templates)
            {
                model.AvailableTopicTemplates.Add(new SelectListItem
                {
                    Text = template.Name,
                    Value = template.Id.ToString()
                });
            }
        }

        [NonAction]
        protected virtual void UpdateLocales(BS_ContentManagement topic, TopicModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(topic,
                                                               x => x.Title,
                                                               localized.Title,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.Body,
                                                           localized.Body,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.MetaKeywords,
                                                           localized.MetaKeywords,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.MetaDescription,
                                                           localized.MetaDescription,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.MetaTitle,
                                                           localized.MetaTitle,
                                                           localized.LanguageId);

                //search engine name
                var seName = topic.ValidateSeName(localized.SeName, localized.Title, false);
                _urlRecordService.SaveSlug(topic, seName, localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void PrepareStoresMappingModel(TopicModel model, BS_ContentManagement topic, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            //model.AvailableStores = _storeService.GetAllStores().Select(s => s.ToModel())
            //    .ToList();
            if (!excludeProperties)
            {
                if (topic != null)
                {
                    model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(topic);
                }
            }
        }

        [NonAction]
        protected virtual void SaveStoreMappings(BS_ContentManagement topic, TopicModel model)
        {
            var existingStoreMappings = _storeMappingService.GetStoreMappings(topic);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds != null && model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(topic, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        [NonAction]
        private List<SelectListItem> GetSubCategories()
        {
            var categories = _categoryService.GetAllCategories();

            var parentCategories = categories.Where(w => w.ParentCategoryId == 0).ToList();

            var subCategories = categories.Join(parentCategories, p => p.ParentCategoryId, q => q.Id, (p, q) => new { p }).ToList();

            var model = subCategories.Select(s => new SelectListItem()
            {
                Text = s.p.Name,
                Value = s.p.Id.ToString()

            }).ToList();

            return model;
        }

        private List<CategoryNavigationModelApi> GetAllSubCategoryIcon()
        {
            List<CategoryNavigationModelApi> model = new List<CategoryNavigationModelApi>();

            var categoryIcons = _categoryIconService.GetAllCategoryIcons().ToList();

            var category = _categoryService.GetAllCategories();

            model = categoryIcons.Join(category, p => p.SubCategoryId, q => q.Id, (p, q) => new { p, catName = q.Name }).Select(s => new CategoryNavigationModelApi() { Id = s.p.SubCategoryId, Name = s.catName, Extension = s.p.Extension, IconPath = Path.Combine(_webHelper.GetStoreLocation() + (IconFilePath) + "/" + string.Format("{0}{1}", s.p.SubCategoryId, s.p.Extension) + "?id=" + s.p.Id) }).ToList();


            return model;
        }

        #endregion

        #region Method

        /*--------------------------------------------------Overview---------------------------------------------------*/


      
        public ActionResult Configure()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);
            var model = new ConfigureModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            model.AndroidAppStatus = BsNopMobileSettings.AndroidAppStatus;
            model.AppKey = BsNopMobileSettings.AppKey;
            model.AppName = BsNopMobileSettings.AppName;
            model.CreatedDate = BsNopMobileSettings.CreatedDate;
            model.DownloadUrl = BsNopMobileSettings.DownloadUrl;
            model.iOsAPPUDID = BsNopMobileSettings.iOsAPPUDID;
            model.MobilWebsiteURL = BsNopMobileSettings.MobilWebsiteURL;


            if (storeScope > 0)
            {
                model.AndroidAppStatus_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AndroidAppStatus, storeScope);
                model.AndroidAppStatus_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AndroidAppStatus, storeScope);
                model.AppKey_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppKey, storeScope);
                model.AppName_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppName, storeScope);
                model.CreatedDate_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.CreatedDate, storeScope);
                model.DownloadUrl_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.DownloadUrl, storeScope);
                model.iOsAPPUDID_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.iOsAPPUDID, storeScope);
                model.MobilWebsiteURL_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.MobilWebsiteURL, storeScope);
            }

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/Configure.cshtml", model);
            // return RedirectToAction("GeneralSetting");
        }

        [HttpPost]
        [AuthorizeAdmin]
        //[FormValueRequired("save")]
        public ActionResult Configure(ConfigureModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);

            BsNopMobileSettings.AndroidAppStatus = model.AndroidAppStatus;
            BsNopMobileSettings.AppKey = model.AppKey;
            BsNopMobileSettings.AppName = model.AppName;
            BsNopMobileSettings.CreatedDate = model.CreatedDate;
            BsNopMobileSettings.DownloadUrl = model.DownloadUrl;
            BsNopMobileSettings.iOsAPPUDID = model.iOsAPPUDID;
            BsNopMobileSettings.MobilWebsiteURL = model.MobilWebsiteURL;
            BsNopMobileSettings.BSMobSetVers++;


            if (model.AndroidAppStatus_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AndroidAppStatus, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AndroidAppStatus, storeScope);

            if (model.AppKey_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppKey, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppKey, storeScope);

            if (model.AppName_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppName, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppName, storeScope);

            if (model.CreatedDate_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.CreatedDate, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.CreatedDate, storeScope);

            if (model.DownloadUrl_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.DownloadUrl, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.DownloadUrl, storeScope);

            if (model.iOsAPPUDID_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.iOsAPPUDID, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.iOsAPPUDID, storeScope);

            if (model.MobilWebsiteURL_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.MobilWebsiteURL, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.MobilWebsiteURL, storeScope);


            _settingService.SaveSetting(BsNopMobileSettings, x => x.BSMobSetVers, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            //redisplay the form
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/Configure.cshtml", model);
        }

        /*--------------------------------------------------CategoryIcons---------------------------------------------------*/

        public ActionResult CategoryIcons()
        {
            var model = new CategoryIconsModel();

            model.AvailableSubCategories = GetSubCategories();

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/CategoryIcons.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        //[FormValueRequired("save")]
        public ActionResult CategoryIcons(IFormFileCollection files, CategoryIconsModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);

            if (files != null)
            {
                foreach (var file in files)
                {
                    // Some browsers send file names with full path. This needs to be stripped.
                    model.Extension = Path.GetExtension(file.FileName);
                    var fileName = string.Format("{0}{1}", model.SubCategoryId, model.Extension);
                    var physicalPath = Path.Combine(CommonHelper.MapPath("~/" + IconFilePath), fileName);

                    //var categoryIcon = model.ToEntity();
                    BS_CategoryIcons categoryIcon = new BS_CategoryIcons()
                    {
                        SubCategoryId = model.SubCategoryId,
                        Extension = model.Extension
                    };

                    BS_CategoryIcons bsCatIcon = _categoryIconService.GetIconExtentionByCategoryId(categoryIcon.SubCategoryId);

                    if (bsCatIcon == null)
                    {
                        _categoryIconService.InsertCategoryIcon(categoryIcon);
                    }
                    else
                    {
                        categoryIcon.Id = bsCatIcon.Id;
                        _categoryIconService.UpdateCategoryIcon(categoryIcon);
                    }
                    using (FileStream fs = System.IO.File.Create(physicalPath))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }
                   

                    model = new CategoryIconsModel();

                    model.AvailableSubCategories = GetSubCategories();
                    BsNopMobileSettings.BSMobSetVers++;
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.BSMobSetVers, storeScope, false);
                }
            }

            //redisplay the form
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/CategoryIcons.cshtml", model);
        }

        #region Grid Actions
        [HttpPost]
        public ActionResult SubCategoryList()
        {
            var gridModel = new DataSourceResult();
            gridModel.Data = GetAllSubCategoryIcon();

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult SubCategoryListDelete(CategoryNavigationModelApi model)
        {
            if (model != null)
            {
                //var categoryIcon = model.ToEntity();
                BS_CategoryIcons categoryIcon = _categoryIconService.GetIconExtentionByCategoryId(model.Id);

                _categoryIconService.DeleteCategoryIcon(categoryIcon);
            }

            return Json(null);
        }


        #endregion

        /*--------------------------------------------------ContentManagement---------------------------------------------------*/

        public ActionResult ContentManagement()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);
            var model = new ContentManagementModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            model.DefaultNopFlowSameAs = BsNopMobileSettings.DefaultNopFlowSameAs;


            //stores
            //model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            //foreach (var s in _storeService.GetAllStores())
            //    model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });



            if (storeScope > 0)
            {
                model.DefaultNopFlowSameAs_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.DefaultNopFlowSameAs, storeScope);

            }

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/ContentManagement.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [FormValueRequired("save")]
        public ActionResult ContentManagement(ContentManagementModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);

            BsNopMobileSettings.DefaultNopFlowSameAs = model.DefaultNopFlowSameAs;

            if (model.DefaultNopFlowSameAs_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.DefaultNopFlowSameAs, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.DefaultNopFlowSameAs, storeScope);

            BsNopMobileSettings.BSMobSetVers++;
            _settingService.SaveSetting(BsNopMobileSettings, x => x.BSMobSetVers, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            //redisplay the form
            return RedirectToAction("ContentManagement");
            //return View("~/Plugins/BS.Plugin.NopStation.MobileWebApi./Views/WebApi/ContentManagement.cshtml", model);
        }

        [HttpPost]
        public ActionResult ContentManagementList(DataSourceRequest command, ContentManagementModel model)
        {
            //var topicModels = _contentManagementService.GetAllTopics(model.SearchStoreId)
            //    .Select(x => x.ToModel())
            //    .ToList();


            var topicModels = _contentManagementService.GetAllTopics(model.SearchStoreId)
               .Select(x => new TopicModel
               {
                   AccessibleWhenStoreClosed = x.AccessibleWhenStoreClosed,
                   Body = x.Body,
                   DisplayOrder = x.DisplayOrder,
                   IncludeInFooterColumn1 = x.IncludeInFooterColumn1,
                   IncludeInFooterColumn2 = x.IncludeInFooterColumn2,
                   IncludeInFooterColumn3 = x.IncludeInFooterColumn3,
                   IncludeInSitemap = x.IncludeInSitemap,
                   IncludeInTopMenu = x.IncludeInTopMenu,
                   IsPasswordProtected = x.IsPasswordProtected,
                   LimitedToStores = x.LimitedToStores,
                   MetaDescription = x.MetaDescription,
                   MetaKeywords = x.MetaKeywords,
                   MetaTitle = x.MetaTitle,
                   Password = x.Password,
                   SystemName = x.SystemName,
                   Title = x.Title,
                   TopicTemplateId = x.TopicTemplateId,
                   Id = x.Id

               }).ToList();
            //little hack here:
            //we don't have paging supported for topic list page
            //now ensure that topic bodies are not returned. otherwise, we can get the following error:
            //"Error during serialization or deserialization using the JSON JavaScriptSerializer. The length of the string exceeds the value set on the maxJsonLength property. "
            foreach (var topic in topicModels)
            {
                topic.Body = "";
            }
            var gridModel = new DataSourceResult
            {
                Data = topicModels,
                Total = topicModels.Count
            };

            return Json(gridModel);
        }



        #region Create / Edit / Delete

        public IActionResult Create()
        {

            var model = new TopicModel();
            //templates
            PrepareTemplatesModel(model);
            //Stores
            PrepareStoresMappingModel(model, null, false);
            //locales
            AddLocales(_languageService, model.Locales);

            //default values
            model.DisplayOrder = 1;

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public IActionResult Create(TopicModel model, bool continueEditing)
        {

            if (ModelState.IsValid)
            {
                if (!model.IsPasswordProtected)
                {
                    model.Password = null;
                }
                BS_ContentManagement topic = new BS_ContentManagement()
                {
                    AccessibleWhenStoreClosed = model.AccessibleWhenStoreClosed,
                    Body = model.Body,
                    DisplayOrder = model.DisplayOrder,
                    IncludeInFooterColumn1 = model.IncludeInFooterColumn1,
                    IncludeInFooterColumn2 = model.IncludeInFooterColumn2,
                    IncludeInFooterColumn3 = model.IncludeInFooterColumn3,
                    IncludeInSitemap = model.IncludeInSitemap,
                    IncludeInTopMenu = model.IncludeInTopMenu,
                    IsPasswordProtected = model.IsPasswordProtected,
                    LimitedToStores = model.LimitedToStores,
                    MetaDescription = model.MetaDescription,
                    MetaKeywords = model.MetaKeywords,
                    MetaTitle = model.MetaTitle,
                    Password = model.Password,
                    SystemName = model.SystemName,
                    Title = model.Title,
                    TopicTemplateId = model.TopicTemplateId
                };

                _contentManagementService.InsertTopic(topic);
                //search engine name
                model.SeName = topic.ValidateSeName(model.SeName, topic.Title ?? topic.SystemName, true);
                _urlRecordService.SaveSlug(topic, model.SeName, 0);
                //Stores
                SaveStoreMappings(topic, model);
                //locales
                UpdateLocales(topic, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Topics.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = topic.Id }) : RedirectToAction("ContentManagement");
            }

            //If we got this far, something failed, redisplay form

            //templates
            PrepareTemplatesModel(model);
            //Stores
            PrepareStoresMappingModel(model, null, true);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var topic = _contentManagementService.GetTopicById(id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("ContentManagement");

            //var model = topic.ToModel();

            TopicModel model = new TopicModel()
            {
                AccessibleWhenStoreClosed = topic.AccessibleWhenStoreClosed,
                Body = topic.Body,
                DisplayOrder = topic.DisplayOrder,
                IncludeInFooterColumn1 = topic.IncludeInFooterColumn1,
                IncludeInFooterColumn2 = topic.IncludeInFooterColumn2,
                IncludeInFooterColumn3 = topic.IncludeInFooterColumn3,
                IncludeInSitemap = topic.IncludeInSitemap,
                IncludeInTopMenu = topic.IncludeInTopMenu,
                IsPasswordProtected = topic.IsPasswordProtected,
                LimitedToStores = topic.LimitedToStores,
                MetaDescription = topic.MetaDescription,
                MetaKeywords = topic.MetaKeywords,
                MetaTitle = topic.MetaTitle,
                Password = topic.Password,
                SystemName = topic.SystemName,
                Title = topic.Title,
                TopicTemplateId = topic.TopicTemplateId
            };
            model.Url = Url.RouteUrl("Topic", new { SeName = topic.GetSeName() }, "http");
            //templates
            PrepareTemplatesModel(model);
            //Store
            PrepareStoresMappingModel(model, topic, false);
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Title = topic.GetLocalized(x => x.Title, languageId, false, false);
                locale.Body = topic.GetLocalized(x => x.Body, languageId, false, false);
                locale.MetaKeywords = topic.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = topic.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = topic.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = topic.GetSeName(languageId, false, false);
            });

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(TopicModel model, bool continueEditing)
        {

            var topic = _contentManagementService.GetTopicById(model.Id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("ContentManagement");

            if (!model.IsPasswordProtected)
            {
                model.Password = null;
            }

            if (ModelState.IsValid)
            {
                //topic = model.ToEntity(topic);

                topic.AccessibleWhenStoreClosed = model.AccessibleWhenStoreClosed;
                topic.Body = model.Body;
                topic.DisplayOrder = model.DisplayOrder;
                topic.IncludeInFooterColumn1 = model.IncludeInFooterColumn1;
                topic.IncludeInFooterColumn2 = model.IncludeInFooterColumn2;
                topic.IncludeInFooterColumn3 = model.IncludeInFooterColumn3;
                topic.IncludeInSitemap = model.IncludeInSitemap;
                topic.IncludeInTopMenu = model.IncludeInTopMenu;
                topic.IsPasswordProtected = model.IsPasswordProtected;
                topic.LimitedToStores = model.LimitedToStores;
                topic.MetaDescription = model.MetaDescription;
                topic.MetaKeywords = model.MetaKeywords;
                topic.MetaTitle = model.MetaTitle;
                topic.Password = model.Password;
                topic.SystemName = model.SystemName;
                topic.Title = model.Title;
                topic.TopicTemplateId = model.TopicTemplateId;

                _contentManagementService.UpdateTopic(topic);
                //search engine name
                model.SeName = topic.ValidateSeName(model.SeName, topic.Title ?? topic.SystemName, true);
                _urlRecordService.SaveSlug(topic, model.SeName, 0);
                //Stores
                SaveStoreMappings(topic, model);
                //locales
                UpdateLocales(topic, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Topics.Updated"));

                if (continueEditing)
                {
                    return RedirectToAction("Edit", new { id = topic.Id });
                }
                return RedirectToAction("ContentManagement");
            }


            //If we got this far, something failed, redisplay form

            model.Url = Url.RouteUrl("Topic", new { SeName = topic.GetSeName() }, "http");
            //templates
            PrepareTemplatesModel(model);
            //Store
            PrepareStoresMappingModel(model, topic, true);
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {

            var topic = _contentManagementService.GetTopicById(id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("ContentManagement");

            _contentManagementService.DeleteTopic(topic);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Topics.Deleted"));
            return RedirectToAction("ContentManagement");
        }

        #endregion

        /*--------------------------------------------------MobilSetting---------------------------------------------------*/

        public ActionResult MobileWebSiteSetting()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);
            var model = new MobileSettingsModel();

            model.ActiveStoreScopeConfiguration = storeScope;
            model.ActivatePushNotification = BsNopMobileSettings.ActivatePushNotification;
            model.SandboxMode = BsNopMobileSettings.SandboxMode;
            model.GcmApiKey = BsNopMobileSettings.GcmApiKey;
            model.GoogleApiProjectNumber = BsNopMobileSettings.GoogleApiProjectNumber;
            model.UploudeIOSPEMFile = BsNopMobileSettings.UploudeIOSPEMFile;
            model.PEMPassword = BsNopMobileSettings.PEMPassword;
            model.AppNameOnGooglePlayStore = BsNopMobileSettings.AppNameOnGooglePlayStore;
            model.AppUrlOnGooglePlayStore = BsNopMobileSettings.AppUrlOnGooglePlayStore;
            model.AppNameOnAppleStore = BsNopMobileSettings.AppNameOnAppleStore;
            model.AppUrlonAppleStore = BsNopMobileSettings.AppUrlonAppleStore;
            model.AppDescription = BsNopMobileSettings.AppDescription;
            model.AppImage = BsNopMobileSettings.AppImage;
            model.AppLogo = BsNopMobileSettings.AppLogo;
            model.AppLogoAltText = BsNopMobileSettings.AppLogoAltText;


            if (storeScope > 0)
            {
                model.ActivatePushNotification_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.ActivatePushNotification, storeScope);
                model.SandboxMode_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.SandboxMode, storeScope);
                model.GoogleApiProjectNumber_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.GoogleApiProjectNumber, storeScope);
                model.UploudeIOSPEMFile_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.UploudeIOSPEMFile, storeScope);
                model.PEMPassword_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.PEMPassword, storeScope);
                model.AppNameOnGooglePlayStore_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppNameOnGooglePlayStore, storeScope);
                model.GcmApiKey_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.GcmApiKey, storeScope);
                model.AppUrlOnGooglePlayStore_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppUrlOnGooglePlayStore, storeScope);
                model.AppNameOnAppleStore_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppNameOnAppleStore, storeScope);
                model.AppUrlonAppleStore_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppUrlonAppleStore, storeScope);
                model.AppDescription_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppDescription, storeScope);
                model.AppImage_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppImage, storeScope);
                model.AppLogo_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppLogo, storeScope);
                model.AppLogoAltText_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppLogoAltText, storeScope);

            }

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/MobileWebSiteSetting.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        //[FormValueRequired("save")]
        public ActionResult MobileWebSiteSetting(MobileSettingsModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);

            BsNopMobileSettings.ActivatePushNotification = model.ActivatePushNotification;
            BsNopMobileSettings.SandboxMode = model.SandboxMode;
            BsNopMobileSettings.GcmApiKey = model.GcmApiKey;
            BsNopMobileSettings.GoogleApiProjectNumber = model.GoogleApiProjectNumber;
            BsNopMobileSettings.UploudeIOSPEMFile = model.UploudeIOSPEMFile;
            BsNopMobileSettings.PEMPassword = model.PEMPassword;
            BsNopMobileSettings.AppNameOnGooglePlayStore = model.AppNameOnGooglePlayStore;
            BsNopMobileSettings.AppUrlOnGooglePlayStore = model.AppUrlOnGooglePlayStore;
            BsNopMobileSettings.AppNameOnAppleStore = model.AppNameOnAppleStore;
            BsNopMobileSettings.AppUrlonAppleStore = model.AppUrlonAppleStore;
            BsNopMobileSettings.AppDescription = model.AppDescription;
            BsNopMobileSettings.AppImage = model.AppImage;
            BsNopMobileSettings.AppLogo = model.AppLogo;
            BsNopMobileSettings.AppLogoAltText = model.AppLogoAltText;

            BsNopMobileSettings.BSMobSetVers++;
            _settingService.SaveSetting(BsNopMobileSettings, x => x.BSMobSetVers, storeScope, false);

            if (model.ActivatePushNotification_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.ActivatePushNotification, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.ActivatePushNotification, storeScope);

            if (model.SandboxMode_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.SandboxMode, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.SandboxMode, storeScope);

            if (model.GcmApiKey_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.GcmApiKey, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.GcmApiKey, storeScope);

            if (model.GoogleApiProjectNumber_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.GoogleApiProjectNumber, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.GoogleApiProjectNumber, storeScope);

            if (model.UploudeIOSPEMFile_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.UploudeIOSPEMFile, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.UploudeIOSPEMFile, storeScope);

            if (model.PEMPassword_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.PEMPassword, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.PEMPassword, storeScope);

            if (model.AppNameOnGooglePlayStore_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppNameOnGooglePlayStore, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppNameOnGooglePlayStore, storeScope);

            if (model.AppUrlOnGooglePlayStore_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppUrlOnGooglePlayStore, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppUrlOnGooglePlayStore, storeScope);

            if (model.AppNameOnAppleStore_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppNameOnAppleStore, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppNameOnAppleStore, storeScope);

            if (model.AppUrlonAppleStore_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppUrlonAppleStore, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppUrlonAppleStore, storeScope);

            if (model.AppDescription_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppDescription, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppDescription, storeScope);

            if (model.AppImage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppImage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppImage, storeScope);

            if (model.AppLogo_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppLogo, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppLogo, storeScope);

            if (model.AppLogoAltText_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppLogoAltText, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppLogoAltText, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            //redisplay the form
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/MobileWebSiteSetting.cshtml", model);
        }

        /*--------------------------------------------------PushNotificationSetting---------------------------------------------------*/

        public ActionResult PushNotification()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);
            var model = new PushNotificationModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            model.PushNotificationHeading = BsNopMobileSettings.PushNotificationHeading;
            model.PushNotificationMessage = BsNopMobileSettings.PushNotificationMessage;

            if (storeScope > 0)
            {
                model.PushNotificationHeading_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.PushNotificationHeading, storeScope);
                model.PushNotificationMessage_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.PushNotificationMessage, storeScope);

            }

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/PushNotification.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        //[FormValueRequired("save")]
        public ActionResult PushNotification(PushNotificationModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);

            BsNopMobileSettings.PushNotificationHeading = model.PushNotificationHeading;
            BsNopMobileSettings.PushNotificationMessage = model.PushNotificationMessage;

            BsNopMobileSettings.BSMobSetVers++;
            _settingService.SaveSetting(BsNopMobileSettings, x => x.BSMobSetVers, storeScope, false);

            if (model.PushNotificationHeading_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.PushNotificationHeading, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.PushNotificationHeading, storeScope);

            if (model.PushNotificationMessage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.PushNotificationMessage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.PushNotificationMessage, storeScope);


            //now clear settings cache
            _settingService.ClearCache();

            //redisplay the form
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/PushNotification.cshtml", model);
        }

        public IActionResult Theme()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);
            var model = new ThemeSettingModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            model.HeaderBackgroundColor = BsNopMobileSettings.HeaderBackgroundColor;
            model.HeaderFontandIconColor = BsNopMobileSettings.HeaderFontandIconColor;
            model.HighlightedTextColor = BsNopMobileSettings.HighlightedTextColor;
            model.PrimaryTextColor = BsNopMobileSettings.PrimaryTextColor;
            model.SecondaryTextColor = BsNopMobileSettings.SecondaryTextColor;
            model.BackgroundColorofPrimaryButton = BsNopMobileSettings.BackgroundColorofPrimaryButton;
            model.TextColorofPrimaryButton = BsNopMobileSettings.TextColorofPrimaryButton;
            model.BackgroundColorofSecondaryButton = BsNopMobileSettings.BackgroundColorofSecondaryButton;

            if (storeScope > 0)
            {
                model.HeaderBackgroundColor_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.HeaderBackgroundColor, storeScope);
                model.HeaderFontandIconColor_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.HeaderFontandIconColor, storeScope);
                model.HighlightedTextColor_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.HighlightedTextColor, storeScope);
                model.PrimaryTextColor_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.PrimaryTextColor, storeScope);
                model.SecondaryTextColor_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.SecondaryTextColor, storeScope);
                model.BackgroundColorofPrimaryButton_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.BackgroundColorofPrimaryButton, storeScope);
                model.TextColorofPrimaryButton_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.TextColorofPrimaryButton, storeScope);
                model.BackgroundColorofSecondaryButton_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.BackgroundColorofSecondaryButton, storeScope);

            }

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/Theme.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        //[FormValueRequired("save")]
        public IActionResult Theme(ThemeSettingModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);


            BsNopMobileSettings.HeaderBackgroundColor = model.HeaderBackgroundColor;
            BsNopMobileSettings.HeaderFontandIconColor = model.HeaderFontandIconColor;
            BsNopMobileSettings.HighlightedTextColor = model.HighlightedTextColor;
            BsNopMobileSettings.PrimaryTextColor = model.PrimaryTextColor;
            BsNopMobileSettings.SecondaryTextColor = model.SecondaryTextColor;
            BsNopMobileSettings.BackgroundColorofPrimaryButton = model.BackgroundColorofPrimaryButton;
            BsNopMobileSettings.TextColorofPrimaryButton = model.TextColorofPrimaryButton;
            BsNopMobileSettings.BackgroundColorofSecondaryButton = model.BackgroundColorofSecondaryButton;

            BsNopMobileSettings.BSMobSetVers++;
            _settingService.SaveSetting(BsNopMobileSettings, x => x.BSMobSetVers, storeScope, false);

            if (model.HeaderBackgroundColor_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.HeaderBackgroundColor, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.HeaderBackgroundColor, storeScope);

            if (model.HeaderFontandIconColor_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.HeaderFontandIconColor, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.HeaderFontandIconColor, storeScope);

            if (model.HighlightedTextColor_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.HighlightedTextColor, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.HighlightedTextColor, storeScope);

            if (model.PrimaryTextColor_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.PrimaryTextColor, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.PrimaryTextColor, storeScope);

            if (model.SecondaryTextColor_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.SecondaryTextColor, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.SecondaryTextColor, storeScope);

            if (model.BackgroundColorofPrimaryButton_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.BackgroundColorofPrimaryButton, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.BackgroundColorofPrimaryButton, storeScope);

            if (model.TextColorofPrimaryButton_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.TextColorofPrimaryButton, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.TextColorofPrimaryButton, storeScope);

            if (model.BackgroundColorofSecondaryButton_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.BackgroundColorofSecondaryButton, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.BackgroundColorofSecondaryButton, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            //redisplay the form
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/Theme.cshtml", model);
        }

        public IActionResult BannerSlider()
        {
           
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);
            var model = new BannerSliderModel();
            model.Picture1Id = BsNopMobileSettings.Picture1Id;
            model.Text1 = BsNopMobileSettings.Text1;
            model.Link1 = BsNopMobileSettings.Link1;
            model.IsProduct1 = BsNopMobileSettings.IsProduct1;
            model.ProductOrCategory1 = BsNopMobileSettings.ProductOrCategoryId1;

            model.Picture2Id = BsNopMobileSettings.Picture2Id;
            model.Text2 = BsNopMobileSettings.Text2;
            model.Link2 = BsNopMobileSettings.Link2;
            model.IsProduct2 = BsNopMobileSettings.IsProduct2;
            model.ProductOrCategory2 = BsNopMobileSettings.ProductOrCategoryId2;

            model.Picture3Id = BsNopMobileSettings.Picture3Id;
            model.Text3 = BsNopMobileSettings.Text3;
            model.Link3 = BsNopMobileSettings.Link3;
            model.IsProduct3 = BsNopMobileSettings.IsProduct3;
            model.ProductOrCategory3 = BsNopMobileSettings.ProductOrCategoryId3;

            model.Picture4Id = BsNopMobileSettings.Picture4Id;
            model.Text4 = BsNopMobileSettings.Text4;
            model.Link4 = BsNopMobileSettings.Link4;
            model.IsProduct4 = BsNopMobileSettings.IsProduct4;
            model.ProductOrCategory4 = BsNopMobileSettings.ProductOrCategoryId4;

            model.Picture5Id = BsNopMobileSettings.Picture5Id;
            model.Text5 = BsNopMobileSettings.Text5;
            model.Link5 = BsNopMobileSettings.Link5;
            model.IsProduct5 = BsNopMobileSettings.IsProduct5;
            model.ProductOrCategory5 = BsNopMobileSettings.ProductOrCategoryId5;

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.Picture1Id_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Picture1Id,
                    storeScope);
                model.Text1_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Text1,
                    storeScope);
                model.Link1_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Link1,
                    storeScope);
                model.IsProduct1_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.IsProduct1,
                    storeScope);
                model.ProductOrCategory1_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings,
                    x => x.ProductOrCategoryId1, storeScope);

                model.Picture2Id_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Picture2Id,
                    storeScope);
                model.Text2_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Text2,
                    storeScope);
                model.Link2_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Link2,
                    storeScope);
                model.IsProduct2_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.IsProduct2,
                    storeScope);
                model.ProductOrCategory2_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings,
                    x => x.ProductOrCategoryId2, storeScope);

                model.Picture3Id_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Picture3Id,
                    storeScope);
                model.Text3_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Text3,
                    storeScope);
                model.Link3_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Link3,
                    storeScope);
                model.IsProduct3_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.IsProduct3,
                    storeScope);
                model.ProductOrCategory3_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings,
                    x => x.ProductOrCategoryId3, storeScope);

                model.Picture4Id_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Picture4Id,
                    storeScope);
                model.Text4_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Text4,
                    storeScope);
                model.Link4_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Link4,
                    storeScope);
                model.IsProduct4_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.IsProduct4,
                    storeScope);
                model.ProductOrCategory4_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings,
                    x => x.ProductOrCategoryId4, storeScope);

                model.Picture5Id_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Picture5Id,
                    storeScope);
                model.Text5_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Text5,
                    storeScope);
                model.Link5_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Link5,
                    storeScope);
                model.IsProduct5_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.IsProduct5,
                    storeScope);
                model.ProductOrCategory5_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings,
                    x => x.ProductOrCategoryId5, storeScope);
            }
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/BannerSlider.cshtml", model);
        }

        public IActionResult SliderImage()
        {
            SliderImageModel model = new SliderImageModel();

            foreach (ImageType type in Enum.GetValues(typeof(ImageType)))
            {
                model.ImageFor.Add(new SelectListItem { Text = type.ToString(), Value = ((int)type).ToString() });
            }
            
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/SliderImage.cshtml", model);
        }

        [HttpPost]
        public IActionResult SliderImageAdd(int pictureId,
         string overrideAltAttribute, string overrideTitleAttribute,
         string sliderActiveStartDate, string sliderActiveEndDate,
         string imageIdFor, bool isProduct, int productOrCategory,
         string shortDescription, int? campaignType)
        {
            CultureInfo ci = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            /*if (pictureId == 0)
                throw new ArgumentException();

            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");*/
            //var pp = _webHelper.GetCurrentIpAddress();
            BS_Slider objOfBsSlider = new BS_Slider();



            // Rojand Fix
            //DateTimeFormatInfo englishDateTime = new CultureInfo("en-US", false).DateTimeFormat;
            //DateTimeFormatInfo italianDateTime = new CultureInfo("it-IT", false).DateTimeFormat;
            //string sliderActiveStartDatestr = Convert.ToDateTime(sliderActiveStartDate, englishDateTime).ToString(italianDateTime.SortableDateTimePattern);
            //string sliderActiveEndDatestr = Convert.ToDateTime(sliderActiveEndDate, englishDateTime).ToString(italianDateTime.SortableDateTimePattern);

            DateTime dtstart = DateTime.Parse(sliderActiveStartDate, new CultureInfo("fa-IR"));
            DateTime dtend = DateTime.Parse(sliderActiveEndDate, new CultureInfo("fa-IR"));

            // DateTime dt2 = DateTime.Parse(dt.Year+"/"+dt.Month+"/"+dt.Day+" "+dt.Hour+":"+dt.Minute, new CultureInfo("en-US"));

            var start = dtstart.ToString(CultureInfo.GetCultureInfo("en-US"));

            var end = dtend.ToString(CultureInfo.GetCultureInfo("en-US"));


            var startdate = Convert.ToDateTime(start, CultureInfo.GetCultureInfo("en-US"));
            var enddate = Convert.ToDateTime(end, CultureInfo.GetCultureInfo("en-US"));


            DateTimeFormatInfo englishDateTime = new CultureInfo("en-US", false).DateTimeFormat;
            DateTimeFormatInfo italianDateTime = new CultureInfo("it-IT", false).DateTimeFormat;

          

            //string sliderActiveStartDatestr = Convert.ToDateTime(sliderActiveStartDate, englishDateTime).ToString(italianDateTime.SortableDateTimePattern);
            //string sliderActiveEndDatestr = Convert.ToDateTime(sliderActiveEndDate, englishDateTime).ToString(italianDateTime.SortableDateTimePattern);


            string sliderActiveStartDatestr = Convert.ToDateTime(start, englishDateTime).ToString(italianDateTime.SortableDateTimePattern);
            string sliderActiveEndDatestr = Convert.ToDateTime(end, englishDateTime).ToString(italianDateTime.SortableDateTimePattern);


            objOfBsSlider.PictureId = pictureId;
            objOfBsSlider.SliderActiveStartDate = String.IsNullOrEmpty(sliderActiveStartDate) ? (DateTime?)null : Convert.ToDateTime(sliderActiveStartDatestr);
            objOfBsSlider.SliderActiveEndDate = String.IsNullOrEmpty(sliderActiveStartDate) ? (DateTime?)null : Convert.ToDateTime(sliderActiveEndDatestr);
            objOfBsSlider.IsProduct = isProduct;
            objOfBsSlider.ProdOrCatId = productOrCategory;

            _bsSliderService.Insert(objOfBsSlider);

            return Json(new { Result = true });

        }

        [HttpPost]
        public ActionResult SliderImageList(DataSourceRequest command)
        {
            //a vendor should have access only to his products

            var SliderImages = _bsSliderService.GetBSSliderImages(command.Page - 1, command.PageSize);
            var sliderImagesModel = SliderImages
                .Select(x =>
                {
                    var picture = _pictureService.GetPictureById(x.PictureId);

                    /*if (picture == null)
                        throw new Exception("Picture cannot be loaded");*/

                    var m = new SliderImageModel
                    {
                        Id = x.Id,
                        PictureId = x.PictureId,
                        PictureUrl = _pictureService.GetPictureUrl(picture),
                        SliderActiveStartDateInGrid = x.SliderActiveStartDate,
                        SliderActiveEndDateInGrid = x.SliderActiveEndDate,
                        IsProduct = x.IsProduct,
                        ProductOrCategory = x.ProdOrCatId,
                        SliderActiveStartDateInGridString= x.SliderActiveStartDate.ToString(),
                        SliderActiveEndDateInGridString= x.SliderActiveEndDate.ToString()
                    };
                    return m;
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = sliderImagesModel,
                Total = SliderImages.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult SliderImageUpdate(SliderImageModel model)
        {
            BS_Slider sliderImage = _bsSliderService.GetBsSliderImageById(model.Id);
            if (sliderImage == null)
                throw new ArgumentException("No slider picture found with the specified id");

            //a vendor should have access only to his products

            sliderImage.SliderActiveStartDate = DateTime.Parse(model.SliderActiveStartDateInGridString.Substring(0, 21).Trim());
            sliderImage.SliderActiveEndDate = DateTime.Parse(model.SliderActiveEndDateInGridString.Substring(0, 21).Trim());
            sliderImage.IsProduct = model.IsProduct;
            sliderImage.ProdOrCatId = model.ProductOrCategory;
            _bsSliderService.Update(sliderImage);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult SliderImageDelete(int id)
        {
            var SliderImage = _bsSliderService.GetBsSliderImageById(id);
            if (SliderImage == null)
                throw new ArgumentException("No category picture found with the specified id");

            //a vendor should have access only to his products

            var pictureId = SliderImage.PictureId;

            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");
            _pictureService.DeletePicture(picture);
            _bsSliderService.Delete(id);
            return new NullJsonResult();
        }

        [HttpPost]
        [AuthorizeAdmin]
        //[FormValueRequired("save")]
        public IActionResult BannerSlider(BannerSliderModel model)
        {

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);
            if (ModelState.IsValid)
            {
                BsNopMobileSettings.Picture1Id = model.Picture1Id;
                BsNopMobileSettings.Text1 = model.Text1;
                BsNopMobileSettings.Link1 = model.Link1;
                BsNopMobileSettings.IsProduct1 = model.IsProduct1;
                BsNopMobileSettings.ProductOrCategoryId1 = model.ProductOrCategory1;

                BsNopMobileSettings.Picture2Id = model.Picture2Id;
                BsNopMobileSettings.Text2 = model.Text2;
                BsNopMobileSettings.Link2 = model.Link2;
                BsNopMobileSettings.IsProduct2 = model.IsProduct2;
                BsNopMobileSettings.ProductOrCategoryId2 = model.ProductOrCategory2;

                BsNopMobileSettings.Picture3Id = model.Picture3Id;
                BsNopMobileSettings.Text3 = model.Text3;
                BsNopMobileSettings.Link3 = model.Link3;
                BsNopMobileSettings.IsProduct3 = model.IsProduct3;
                BsNopMobileSettings.ProductOrCategoryId3 = model.ProductOrCategory3;

                BsNopMobileSettings.Picture4Id = model.Picture4Id;
                BsNopMobileSettings.Text4 = model.Text4;
                BsNopMobileSettings.Link4 = model.Link4;
                BsNopMobileSettings.IsProduct4 = model.IsProduct4;
                BsNopMobileSettings.ProductOrCategoryId4 = model.ProductOrCategory4;

                BsNopMobileSettings.Picture5Id = model.Picture5Id;
                BsNopMobileSettings.Text5 = model.Text5;
                BsNopMobileSettings.Link5 = model.Link5;
                BsNopMobileSettings.IsProduct5 = model.IsProduct5;
                BsNopMobileSettings.ProductOrCategoryId5 = model.ProductOrCategory5;

                BsNopMobileSettings.BSMobSetVers++;
                _settingService.SaveSetting(BsNopMobileSettings, x => x.BSMobSetVers, storeScope, false);

                if (model.Picture1Id_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Picture1Id, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Picture1Id, storeScope);

                if (model.Text1_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Text1, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Text1, storeScope);

                if (model.Link1_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Link1, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Link1, storeScope);

                if (model.IsProduct1_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.IsProduct1, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.IsProduct1, storeScope);

                if (model.ProductOrCategory1_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.ProductOrCategoryId1, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.ProductOrCategoryId1, storeScope);

                if (model.Picture2Id_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Picture2Id, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Picture2Id, storeScope);

                if (model.Text2_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Text2, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Text2, storeScope);

                if (model.Link2_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Link2, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Link2, storeScope);

                if (model.IsProduct2_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.IsProduct2, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.IsProduct2, storeScope);

                if (model.ProductOrCategory2_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.ProductOrCategoryId2, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.ProductOrCategoryId2, storeScope);

                if (model.Picture3Id_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Picture3Id, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Picture3Id, storeScope);

                if (model.Text3_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Text3, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Text3, storeScope);

                if (model.Link3_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Link3, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Link3, storeScope);

                if (model.IsProduct3_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.IsProduct3, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.IsProduct3, storeScope);

                if (model.ProductOrCategory3_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.ProductOrCategoryId3, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.ProductOrCategoryId3, storeScope);

                if (model.Picture4Id_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Picture4Id, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Picture4Id, storeScope);

                if (model.Text4_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Text4, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Text4, storeScope);

                if (model.Link4_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Link4, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Link4, storeScope);

                if (model.IsProduct4_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.IsProduct4, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.IsProduct4, storeScope);

                if (model.ProductOrCategory4_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.ProductOrCategoryId4, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.ProductOrCategoryId4, storeScope);

                if (model.Picture5Id_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Picture5Id, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Picture5Id, storeScope);

                if (model.Text5_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Text5, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Text5, storeScope);

                if (model.Link5_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Link5, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Link5, storeScope);

                if (model.IsProduct5_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.IsProduct5, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.IsProduct5, storeScope);

                if (model.ProductOrCategory5_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.ProductOrCategoryId5, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.ProductOrCategoryId5, storeScope);

                //now clear settings cache
                _settingService.ClearCache();
            }

            //redisplay the form

            //redisplay the form
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/BannerSlider.cshtml", model);
        }


        public IActionResult GeneralSetting()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);
            var model = new GeneralSettingModel();

            model.EnableBestseller = BsNopMobileSettings.EnableBestseller;
            model.EnableFeaturedProducts = BsNopMobileSettings.EnableFeaturedProducts;
            model.EnableNewProducts = BsNopMobileSettings.EnableNewProducts;

            if (storeScope > 0)
            {
                model.EnableBestseller_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.EnableBestseller, storeScope);
                model.EnableFeaturedProducts_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.EnableFeaturedProducts, storeScope);
                model.EnableNewProducts_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.EnableNewProducts, storeScope);
            }

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/GeneralSetting.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        //[FormValueRequired("save")]
        public IActionResult GeneralSetting(GeneralSettingModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);

            BsNopMobileSettings.EnableBestseller = model.EnableBestseller;
            BsNopMobileSettings.EnableFeaturedProducts = model.EnableFeaturedProducts;
            BsNopMobileSettings.EnableNewProducts = model.EnableNewProducts;

            BsNopMobileSettings.BSMobSetVers++;
            _settingService.SaveSetting(BsNopMobileSettings, x => x.BSMobSetVers, storeScope, false);

            if (model.EnableBestseller_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.EnableBestseller, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.EnableBestseller, storeScope);

            if (model.EnableFeaturedProducts_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.EnableFeaturedProducts, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.EnableFeaturedProducts, storeScope);

            if (model.EnableNewProducts_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.EnableNewProducts, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.EnableNewProducts, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            //redisplay the form
            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            //return GeneralSetting();
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/GeneralSetting.cshtml", model);
        }

        [HttpPost]
        public IActionResult FeaturedProductList(DataSourceRequest command)
        {
            var FeatureProducts = _nopMobileService.GetAllPluginFeatureProducts(command.Page - 1, command.PageSize);

            var productlist = _productService.GetProductsByIds(FeatureProducts.Select(x => x.ProductId).ToArray());

            var featuredProductsModel = productlist
                .Select(x => new GeneralSettingModel.AssociatedProductModel
                {
                    Id = x.Id,
                    ProductName = x.Name
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = featuredProductsModel,
                Total = featuredProductsModel.Count
            };
            return Json(gridModel);
        }

        [HttpPost]
        public IActionResult FeaturedProductDelete(int id)
        {
            var featuredProduct = _nopMobileService.GetPluginFeatureProductsById(id);
            if (featuredProduct == null)
                throw new ArgumentException("No associated product found with the specified id");

            _nopMobileService.DeleteFeatureProducts(featuredProduct);

            return new NullJsonResult();
        }

        public IActionResult FeaturedProductsAddPopup(string btnId)
        {
            var model = new GeneralSettingModel.AddAssociatedProductModel();
            //a vendor should have access only to his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = _categoryService.GetAllCategories(showHidden: true);
            foreach (var c in categories)
                model.AvailableCategories.Add(new SelectListItem { Text = c.GetFormattedBreadCrumb(categories), Value = c.Id.ToString() });

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var m in _manufacturerService.GetAllManufacturers(showHidden: true))
                model.AvailableManufacturers.Add(new SelectListItem { Text = m.Name, Value = m.Id.ToString() });

            //stores
            //model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            //foreach (var s in _storeService.GetAllStores())
            //    model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var v in _vendorService.GetAllVendors(showHidden: true))
                model.AvailableVendors.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            ViewBag.btnId = btnId;
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/FeaturedProductsAddPopup.cshtml", model);
        }

        [HttpPost]
        public IActionResult FeaturedProductsAddPopupList(DataSourceRequest command, GeneralSettingModel.AddAssociatedProductModel model)
        {

            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

            var products = _productService.SearchProducts(
                categoryIds: new List<int> { model.SearchCategoryId },
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true
                );

            var featuredProductsModel = products
               .Select(x => new GeneralSettingModel.AssociatedProductModel
               {
                   Id = x.Id,
                   ProductName = x.Name,
                   DisplayOrder = x.DisplayOrder,
                   Published = x.Published

               })
               .ToList();
            var gridModel = new DataSourceResult
            {
                Data = featuredProductsModel,
                Total = featuredProductsModel.Count
            };

           

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public IActionResult FeaturedProductsAddPopup(string btnId, string formId, GeneralSettingModel.AddAssociatedProductModel model)
        {
            if (model.SelectedProductIds != null)
            {
                var productidlist = _nopMobileService.GetAllPluginFeatureProducts().Where(x => model.SelectedProductIds.Contains(x.ProductId)).Select(x => x.ProductId);
                foreach (int id in model.SelectedProductIds)
                {
                    if (productidlist.Count() > 0)
                    {
                        if (!productidlist.Contains(id))
                        {
                            BS_FeaturedProducts FetureProduct = new BS_FeaturedProducts()
                            {
                                ProductId = id
                            };
                            _nopMobileService.InsertFeatureProducts(FetureProduct);
                        }
                    }
                    else
                    {
                        BS_FeaturedProducts FetureProduct = new BS_FeaturedProducts()
                        {
                            ProductId = id
                        };
                        _nopMobileService.InsertFeatureProducts(FetureProduct);
                    }

                }
            }
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/FeaturedProductsAddPopup.cshtml", model);
        }


        #region NSt Settings
        public IActionResult NopStationSecrateToken()
        {
            var model = new Nst_ConfigurationSettings();
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var nstSettings = _settingService.LoadSetting<NstSettingsModel>(storeScope);
            model.ActiveStoreScopeConfiguration = storeScope;
            model.NST_KEY = nstSettings.NST_KEY;
            model.NST_SECRET = nstSettings.NST_SECRET;
            model.EditMode = false;
            if (storeScope > 0)
            {
                model.NST_KEY_OverrideForStore = _settingService.SettingExists(nstSettings, x => x.NST_KEY, storeScope);
                model.NST_SECRET_OverrideForStore = _settingService.SettingExists(nstSettings, x => x.NST_SECRET, storeScope);

            }

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/NstSettings.cshtml", model);
        }

        public IActionResult NopStationSecrateTokenEdit()
        {
            var model = new Nst_ConfigurationSettings();
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var nstSettings = _settingService.LoadSetting<NstSettingsModel>(storeScope);
            model.ActiveStoreScopeConfiguration = storeScope;
            model.NST_KEY = nstSettings.NST_KEY;
            model.NST_SECRET = nstSettings.NST_SECRET;
            model.EditMode = true;
            if (storeScope > 0)
            {
                model.NST_KEY_OverrideForStore = _settingService.SettingExists(nstSettings, x => x.NST_KEY, storeScope);
                model.NST_SECRET_OverrideForStore = _settingService.SettingExists(nstSettings, x => x.NST_SECRET, storeScope);

            }

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/NstSettingsEdit.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        //[FormValueRequired("save")]
        public IActionResult NopStationSecrateTokenEdit(Nst_ConfigurationSettings model)
        {
            if (model.EditMode)
            {

                var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
                var nstSettings = _settingService.LoadSetting<NstSettingsModel>(storeScope);
                nstSettings.NST_KEY = model.NST_KEY;
                nstSettings.NST_SECRET = model.NST_SECRET;
                _settingService.SaveSettingOverridablePerStore(nstSettings, x => x.NST_KEY, model.NST_KEY_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(nstSettings, x => x.NST_SECRET, model.NST_SECRET_OverrideForStore, storeScope, false);
                //now clear settings cache
                _settingService.ClearCache();

            }
            return RedirectToAction("NopStationSecrateToken", "MobileWebApiConfiguration");
        }
        #endregion
        #endregion



    }

}
