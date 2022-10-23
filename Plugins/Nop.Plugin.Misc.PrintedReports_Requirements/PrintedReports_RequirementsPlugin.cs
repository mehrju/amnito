using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PrintedReports_Requirements
{
    public class PrintedReports_RequirementsPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        public PrintedReports_RequirementsPlugin(ISettingService settingService, IWebHelper webHelper)
        {
            this._settingService = settingService;
            this._webHelper = webHelper;

        }


        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/PrintedReports_Requirements/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            #region set Local Resource Update/Delete/Create/Duplicate/
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.MessageStateVerifyNotOk", "عملیات بررسی آواتار(عدم تایید عکس) با موفقیت انجام شد");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.MessageStateVerifyOk", "عملیات بررسی آواتار (تایید عکس) با موفقیت انجام شد");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Duplicate", "رکورد تکراری میباشد");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Create", "عملیات ثبت با موفقیت انجام شد");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.MessageUpdate", "عملیات ویرایش با موفقیت انجام شد");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.BackToList", "برگشت به لیست");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.TabInfo", "مشخصات");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Active", "فعال سازی");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Disable", "غیرفعالسازی");

            #endregion

            #region set Local Resource Page Index Customer
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Page_Titel_List_Customer", "بررسی آواتار کاربران");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.btn_Ok", "تایید عکس(انتخاب شده)");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.btn_Not_Ok", "عدم تایید عکس(انتخاب شده)");

            #region set Local Resource Search Customer
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.AllUser", "همه کاربران");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.UserNotOkAvatar", "کاربرانی که آواتار آنها تایید نشده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.UserOkAvatar", "کاربرانی که آواتار آنها تایید شده");

            #endregion

            #region set Local Resource grid Customer
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.GridAvatarUrl", "عکس آواتار");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.StateVerify", "وضعیت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.DateVerify", "تاریخ بررسی");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.NameUserVerify", "کاربر بررسی کننده");

            #endregion
            #endregion


            #region set Local Resource Search Customer
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.AllUser", "همه کاربران");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.UserNotOkAvatar", "کاربرانی که آواتار آنها تایید نشده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.UserOkAvatar", "کاربرانی که آواتار آنها تایید شده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.StateVerify", "وضعیت بررسی");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.DateVerify", "تاریخ بررسی");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.NameUserVerify", "کاربر بررسی کننده");



            
            
            

            #endregion



            #region ServiceProviderDashboard
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Search_ServiceProviderDashboard_Title", "عنوان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Search_ServiceProviderDashboard_IsActive", "وضعیت(فعال)");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.ServiceProviderDashboard_title", "لیست سرویس دهنده های  صفحه Home");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.ServiceProviderDashboard_pagetitle", "عنوان بالای صفحه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.ServiceProviderDashboard_new", "سرویس دهنده جدید");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.ServiceProviderDashboard_update", "ویرایش سرویس دهنده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_ServiceProviderDashboard_UrlImage", "عکس");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_ServiceProviderDashboard_Title", "عنوان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_ServiceProviderDashboard_IsActive", "وضعیت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_ServiceProviderDashboard_DateInsert", "تاریخ ثبت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_ServiceProviderDashboard_DateUpdate", "تاریخ آخرین ویرایش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_ServiceProviderDashboard_UserInsert", "کاربر ثبت کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_ServiceProviderDashboard_UserUpdate", "کاربر ویرایش کننده");
            #endregion



            #region SlideShow
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Search_SlideShow_Title", "عنوان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Search_SlideShow_IsActive", "وضعیت(فعال)");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.SlideShow_title", "لیست اسلایدشوها");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.SlideShow_pagetitle", "عنوان بالای صفحه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.SlideShow_new", "اسلایدشو جدید");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.SlideShow_update", "ویرایش اسلایدشو");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_UrlImage", "عکس");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_Title", "عنوان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_IsActive", "وضعیت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_DateInsert", "تاریخ ثبت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_DateUpdate", "تاریخ آخرین ویرایش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_UserInsert", "کاربر ثبت کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_UserUpdate", "کاربر ویرایش کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_Dis", "توضیحات");

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Search_SlideShow_IsVideo", "وضعیت ویدئو");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.Grid_SlideShow_IsVideo", "وضعیت ویدئو");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.IsVideo", "وضعیت ویدئو");

            #endregion




            base.Install();
            PluginManager.MarkPluginAsInstalled(this.PluginDescriptor.SystemName);
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {

            SiteMapNode siteMapNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Configuration");

            SiteMapNode itemProvider = new SiteMapNode
            {
                SystemName = "Customer.ManageAvatarCustomer",
                Title = "مدیریت آواتار کاربران",
                ControllerName = "ManageAvatarCustomer",
                ActionName = "Index",
                Visible = true,
                IconClass = "fa fa-dot-circle-o",
                RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary
                {
                    {
                        "area",
                        "admin"
                    }
                }
            };
            siteMapNode?.ChildNodes.Add(itemProvider);


            #region ManageDashboard
            //SiteMapNode ManageDashboard = new SiteMapNode
            //{
            //    SystemName = "ManageDashboard",
            //    Title = "مدیریت آیتم های صفحه اول",
            //    Visible = true,
            //    IconClass = "fa fa-dot-circle-o",
            //    RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary
            //    {
            //        {
            //            "area",
            //            "admin"
            //        }
            //    }
            //};
            //siteMapNode?.ChildNodes.Add(ManageDashboard);
            SiteMapNode _PricingPolicy = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Sale.PricingPolicy");
            if (_PricingPolicy == null)
            {
                _PricingPolicy = new SiteMapNode
                {
                    SystemName = "Sale.PricingPolicy",
                    Title = "سیاست های مالی",
                    Visible = true,
                    IconClass = "fa fa-dot-circle-o",
                };
                siteMapNode?.ChildNodes.Add(_PricingPolicy);
            }
            SiteMapNode ServiceProviderDashboard = new SiteMapNode
            {
                SystemName = "ManageDashboard.ServiceProviderDashboard",
                Title = "مدیریت سرویس دهنده صفحه اول",
                ControllerName = "ManageServiceProviderDashboard",
                ActionName = "Index",
                Visible = true,
                IconClass = "fa fa-dot-circle-o",
                RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary
                {
                    {
                        "area",
                        "admin"
                    }
                }
            };
            _PricingPolicy?.ChildNodes.Add(ServiceProviderDashboard);

            #endregion
            #region slidshow

            SiteMapNode slidshow = new SiteMapNode
            {
                SystemName = "ManageDashboard.slidshow",
                Title = "مدیریت اسلایدشو",
                ControllerName = "ManageSlideShow",
                ActionName = "Index",
                Visible = true,
                IconClass = "fa fa-dot-circle-o",
                RouteValues = new Microsoft.AspNetCore.Routing.RouteValueDictionary
                {
                    {
                        "area",
                        "admin"
                    }
                }
            };
            siteMapNode?.ChildNodes.Add(slidshow);
            #endregion
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<PrintedReports_RequirementsSettings>();
            #region Delete Local Resource Search Customer
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.AllUser");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.UserNotOkAvatar");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.PrintedReports_Requirements.UserOkAvatar");

            #endregion

            PluginManager.MarkPluginAsUninstalled(this.PluginDescriptor.SystemName);

            base.Uninstall();
        }


       
    }
}
