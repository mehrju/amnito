using Nop.Core;
using Nop.Core.Plugins;
using Nop.Plugin.Misc.Ticket.Data;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket
{
    public class TicketPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly TicketObjectContext _context;

        public TicketPlugin(ISettingService settingService, IWebHelper webHelper, TicketObjectContext context)
        {
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._context = context;
        }
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/Ticket/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {


            #region Set settings
            var settings = new TicketSettings
            {
                //pde
                //PDE_Authorization = "Basic QXBpQHBkZXhwLmNvbTpjYzM0NGRkYzgzN2FmMzNlZDA4NDE5OWJiZjBhZGNkMw==",

            };
            // _settingService.SaveSetting(settings);
            #endregion

            //Resource
            #region set Local Resource Update/Delete/Create/Duplicate/
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.MessageDisable", "عملیات غیرفعال سازی با موفقیت انجام شد");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.MessageDisable", "عملیات فعال سازی با موفقیت انجام شد");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Duplicate", "رکورد تکراری میباشد");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Create", "عملیات ثبت با موفقیت انجام شد");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.MessageUpdate", "عملیات ویرایش با موفقیت انجام شد");

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.BackToList", "برگشت به لیست");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.TabInfo", "مشخصات");

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Active", "فعال سازی");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Disable", "غیرفعالسازی");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Limitation", "محدودیت ها");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.PricingPolicy", "قیمت گذاری");

            #endregion

            #region Dep
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_Name", "نام دپارتمان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_IsActive", "وضعیت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_DateInsert", "تاریخ ثبت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_DateUpdate", "اخرین تاریخ ویرایش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_UserInsert", "کاربر ثبت کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_UserUpdate", "کاربر ویرایش کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_StoreName", "نام فروشگاه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_DepartmentTicket_Name", "نام دپارتمان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_DepartmentTicket_IsActive", "وضعیت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchStoreId", "فروشگاه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.DepartmentTicket_title", "لیست دپارتمان ها");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.DepartmentTicket_pagetitle", "عنوان بالای صفحه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.DepartmentTicket_new", "دپارتمان جدید");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.DepartmentTicket_update", "ویرایش دپارتمان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.NewDepName", "نام دپارتمان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.NewListStore", "انتخاب فروشگاه");





            #endregion

            #region Priority
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_ProirityTicket_Name", "نام اولویت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_ProirityTicket_IsActive", "وضعیت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_ProirityTicket_DateInsert", "تاریخ ثبت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_ProirityTicket_DateUpdate", "اخرین تاریخ ویرایش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_ProirityTicket_UserInsert", "کاربر ثبت کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_ProirityTicket_UserUpdate", "کاربر ویرایش کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_PriorityTicket_Name", "نام اولویت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_PriorityTicket_IsActive", "وضعیت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.PriorityTicket_title", "لیست اولویت ها");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.PriorityTicket_pagetitle", "عنوان بالای صفحه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.PriorityTicket_new", "اولویت جدید");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.PriorityTicket_update", "ویرایش اولویت");





            #endregion

            #region Staff Deps
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Page_Titel_List_StaffDepartmentTicket", "عنوان بالای صفحه کارمندان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Titel_List_StaffDepartmentTicket", "لیست کارمندان در دپارتمانها");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.New_StaffDepartmentTicket", "تخصیص کارمندان به دپارتمانها");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchDepartmentId", "انتخاب دپارتمان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.DepartmentTicketName", "دپارتمان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.NewStaffIdDepartment", "دپارتمان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.NewStaffUserId", "کارمند");

            #endregion

            #region Ticket
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchDepartmentId", "انتخاب دپارتمان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchUsername_Staff", "نام کارمند بازکننده تیکت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchUsername_customer", "نام مشتری");

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchIdStatus", "وضعیت تیکت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchOrderId", "کد سفارش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchTrackingCode", "کد پیگیری سفارش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchIssue", "موضوع تیکت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchPriorityId", "اولویت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchStoreId", "فروشگاه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_StoreName", "فروشگاه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.DepartmentTicketName", "دپارتمان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_Isuue", "موضوع");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_FullName", "نام مشتری");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_DepName", "نام دپارتمان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_PriorityName", "اولویت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_OrderId", "کد سفارش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_TrackingCode", "کد پیگیری سفارش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_Status", "اخرین وضعیت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_DateInsert", "تاریخ ثبت تیکت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_NameStaffOpen", "کارمند بازکننده تیکت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_DateOpen", "تاریخ بازشدن تیکت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_LastDateAnswer", "تاریخ اخرین جواب به تیکت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_NameStaffLastAnswer", "آخرین کارمند پاسخ دهنده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_LastRank", "آخرین امتیاز ");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_Ticket_IsActive", "وضعیت(فعال)");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Page_Titel_List_Ticket", "عنوان بالای صفحه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Titel_List_Ticket", "لیست تیکت ها");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Finished", "پایان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.AnswerTicket", "پاسخ به تیکت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchCategoryId", "دسته بندی تیکت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchTicketNumber", "شماره تیکت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_CategoryName", "دسته بندی تیکت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_TicketNumber", "شماره تیکت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchOstanOrginId", "استان فرستنده");

            
            #endregion


            #region  FAQCategory
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_FAQCategory_Name", "نام دسته بندی");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_FAQCategory_IsActive", "وضعیت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_FAQCategory_DateInsert", "تاریخ ثبت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_FAQCategory_DateUpdate", "اخرین تاریخ ویرایش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_FAQCategory_UserInsert", "کاربر ثبت کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_FAQCategory_UserUpdate", "کاربر ویرایش کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_FAQCategory_Name", "نام دسته بندی");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_FAQCategory_IsActive", "وضعیت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.FAQCategory_title", "لیست دسته بندی سوالات متداول");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.FAQCategory_pagetitle", "عنوان بالای صفحه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.FAQCategory_new", "دسته بندی جدید");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.FAQCategory_update", "ویرایش دسته بندی");





            #endregion


            #region  FAQCategory
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_FAQ_Question", "سوال");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_FAQ_Answer", "پاسخ");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_FAQ_CategoryName", "دسته بندی");

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_FAQ_IsActive", "وضعیت(قعال)");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_FAQ_DateInsert", "تاریخ ثبت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_FAQ_DateUpdate", "اخرین تاریخ ویرایش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_FAQ_UserInsert", "کاربر ثبت کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_FAQ_UserUpdate", "کاربر ویرایش کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_FAQ_Question", "سوال");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_FAQ_Answer", "پاسخ");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchFAQCategoryId", "دسته بندی");

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_FAQ_IsActive", "وضعیت(فعال)");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.FAQ_title", "لیست  سوالات متداول");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.FAQ_pagetitle", "عنوان بالای صفحه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.FAQ_new", "سوال جدید");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.FAQ_update", "ویرایش سوال");


            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.FAQQuestion", "سوال");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.FAQAnswer", "پاسخ");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.FAQIdCategory", "انتخاب دسته بندی");




            #endregion


            #region Pattern Answer Ticket
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.PatternAnswer_title", "لیست پاسخ های متداول تیکت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.PatternAnswer_pagetitle", "عنوان بالای صفحه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.PatternAnswer_new", "پاسخ جدید");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.PatternAnswer_update", "ویرایش پاسخ");

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.TitlePatternAnswer", "عنوان پاسخ");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.DescriptipnPatternAnswer", "توضیحات");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_PatternAnswerTicket_Title", "عنوان پاسخ");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_PatternAnswerTicket_Descriptipn", "توضیحات");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_PatternAnswerTicket_IsActive", "وضعیت(فعال)");

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_PatternAnswerTicket_Title", "عنوان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_PatternAnswerTicket_Description", "توضیحات");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_PatternAnswerTicket_IsActive", "وضعیت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_PatternAnswerTicket_DateInsert", "تاریخ ثبت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_PatternAnswerTicket_DateUpdate", "اخرین تاریخ ویرایش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_PatternAnswerTicket_UserInsert", "کاربر ثبت کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_PatternAnswerTicket_UserUpdate", "اخرین کاربر ویرایش کننده");








            #endregion

            #region CategoryTicket

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.CategoryTicket_title", "لیست دسته بندی تیکت براساس دپارتمان ها");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.CategoryTicket_pagetitle", "عنوان بالای صفحه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.CategoryTicket_new", "دسته بندی جدید");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.CategoryTicket_update", "ویرایش دسته بندی");

            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_CategoryTicket_NameCategory", "نام دسته بندی");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_CategoryTicket_IsActive", "وضعیت(فعال)");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_CategoryTicket_DepartmentId", "دپارتمان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_CategoryTicket_UserUpdate", "کاربر ویرایش کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_CategoryTicket_UserInsert", "کاربر ثبت کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_CategoryTicket_DateUpdate", "تاریخ آخرین ویرایش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_CategoryTicket_DateInsert", "تاریخ ثبت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_CategoryTicket_IsActive", "وضعیت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_CategoryTicket_CategoryName", "نام دسته بندی");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_CategoryTicket_DepartmentName", "دپارتمان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.NameCategoryTicket", "نام دسته بندی");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.NewCategoryTicketListDepartments", "دپارتمان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.DepartmentIdeCategoryTicket", "دپارتمان");



            #endregion




            #region TrainingAcademyTopic
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_TrainingAcademyTopic_Title", "عنوان صفحه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_TrainingAcademyTopic_Description", "توضیحات صفحه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_TrainingAcademyTopic_SystemName", "نام سیستم صفحه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_TrainingAcademyTopic_IsActive", "وضعیت(فعال)");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_TrainingAcademyTopic_StoreId", "فروشگاه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_TrainingAcademyTopic_TopicId", "صفحه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.TrainingAcademyTopic_title", "لیست صفحات منتشر شده در قسمت آموزش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.TrainingAcademyTopic_pagetitle", "عنوان بالای صفحه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.TrainingAcademyTopic_new", "صفحه جدید");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.TrainingAcademyTopic_update", "ویرایش صفحه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.IdTopic", "انتخاب صفحه(تاپیک)");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_TrainingAcademyTopic_UrlImage", "عکس");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_TrainingAcademyTopic_Title", "عنوان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_TrainingAcademyTopic_SystemName", "نام سیستم");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_TrainingAcademyTopic_StoresName", "نام فروشکاهها");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_TrainingAcademyTopic_IsActive", "وضعیت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_TrainingAcademyTopic_DateInsert", "تاریخ ثبت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_TrainingAcademyTopic_DateUpdate", "تاریخ آخرین ویرایش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_TrainingAcademyTopic_UserInsert", "کاربر ثبت کننده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_TrainingAcademyTopic_UserUpdate", "کاربر ویرایش کننده");
            #endregion

            #region Damages
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Titel_List_Damages", "لیست درخواست های خسارت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchDamagesNumber", "شماره درخواست");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchNameGoods", "نام کالا");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchNameBerand", "نام برند");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_Damages_IsDeleted", "وضعیت(حذف شده)");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchTrackingCodeDamages", "شماره رهگیری سفارش");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchIdStatusDamages", "وضعیت درخواست");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Damages_TrackingCode", "شماره رهگیری");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Damages_StoreName", "نام فروشگاه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Damages_FullName", "نام مشتری");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Damages_DamagesNumber", "شماره درخواست");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Damages_NameGoods", "نام کالا");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Damages_Status", "وضعیت درخواست");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.AcceptedFinishedDamagesPayment", "تایید،پرداخت،پایان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.disapprovalFinishedDamagesPayment", "عدم تایید،پایان");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.AnswerDamages", "پاسخ درخواست");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Damages_NameStaffLastAnswer", "آخرین کارمند پاسخ دهنده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Damages_LastDateAnswer", "تاریخ آخرین پاسخ");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Damages_DateOpen", "تاریخ باز شدن درخواست");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Damages_NameStaff", "نام کارمند");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Damages_DateInsert", "تاریخ ثبت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Damages_IsActive", "وضعیت(فعال)");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.PatternAnswer_pagetitleDamages", "عنان بالای صفحه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.PatternAnswer_titleDamages", "لیست پاسخ های متداول درخواست های خسارت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.TitlePatternAnswerDamages", "عنوان پاسخ");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.DescriptipnPatternAnswerDamages", "توضیحات");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Page_Titel_List_Damages", "لیست درخواست های خسارت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchTrackingCodeDamages", "کد رهگیری");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchIdStatusDamages", "وضعیت");
            #endregion


            #region Request COD
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchRequestCODNumber", "شماره درخواست");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchIdStatusRequestCOD", "وضعیت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_RequestCOD_StoreName", "نام فروشگاه");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_RequestCOD_FullName", "نام و نام خانوادگی");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_RequestCOD_Status", "وضعیت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_RequestCOD_DateInsert", "تاریخ ثبت");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_RequestCOD_NameStaff", "نام کارمند");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_RequestCOD_DateOpen", "تاریخ باز شدن درخواست");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_RequestCOD_LastDateAnswer", "تاریخ آخرین پاسخ");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_RequestCOD_NameStaffLastAnswer", "آخرین کارمند پاسخ دهنده");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_RequestCOD_RequestCODNumber", "شماره درخواست");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.Page_Titel_List_RequestCOD", "لیست درخواست های فعال سازی حساب COD");
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.AnswerRequestCOD", "پاسخ به درخواست");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.", "");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.", "");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.", "");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.", "");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.", "");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.", "");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.", "");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.", "");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.", "");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.", "");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.", "");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.", "");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.", "");
            //this.AddOrUpdatePluginLocaleResource("Nop.Plugin.Misc.Ticket.", ""); 
            #endregion



            base.Install();
            _context.Install();
            PluginManager.MarkPluginAsInstalled(this.PluginDescriptor.SystemName);
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            //اگر میخواهی به روت اد بشه نیازی به نود تنظیمات نیست
            //  rootNode?.ChildNodes.Add(RequestCODNode);
            SiteMapNode siteMapNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Configuration");

            #region  تیکت
            SiteMapNode TicketNode = new SiteMapNode
            {
                SystemName = "TicketNode",
                Title = "تیکت و سولات متداول",
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
            siteMapNode?.ChildNodes.Add(TicketNode);
            SiteMapNode itemListTicket = new SiteMapNode
            {
                SystemName = "TicketNode.itemListTicket",
                Title = "لیست تیکت ها",
                ControllerName = "ManageTicket",
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
            TicketNode?.ChildNodes.Add(itemListTicket);

            SiteMapNode itemDepartment = new SiteMapNode
            {
                SystemName = "TicketNode.itemDepartment",
                Title = "مدیریت دپارتمانها",
                ControllerName = "ManageDepartmentTicket",
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
            TicketNode?.ChildNodes.Add(itemDepartment);

            SiteMapNode itemPriority = new SiteMapNode
            {
                SystemName = "TicketNode.itemPriority",
                Title = "مدیریت اولویت ها",
                ControllerName = "ManagePriorityTicket",
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
            TicketNode?.ChildNodes.Add(itemPriority);

            SiteMapNode itemStaff = new SiteMapNode
            {
                SystemName = "TicketNode.itemPriority",
                Title = "مدیریت کارمندان",
                ControllerName = "ManageStaffDepartmentTicket",
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
            TicketNode?.ChildNodes.Add(itemStaff);

            SiteMapNode itemPatternAnswer = new SiteMapNode
            {
                SystemName = "TicketNode.itemPatternAnswer",
                Title = "مدیریت الگوهای پاسخ تیکت",
                ControllerName = "ManagePatternAnswerTicket",
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
            TicketNode?.ChildNodes.Add(itemPatternAnswer);

            SiteMapNode itemCategoryTicket = new SiteMapNode
            {
                SystemName = "TicketNode.itemCategoryTicket",
                Title = "مدیریت دسته بندی تیکت",
                ControllerName = "ManageCategoryTicket",
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
            TicketNode?.ChildNodes.Add(itemCategoryTicket);


            SiteMapNode itemFAQ = new SiteMapNode
            {
                SystemName = "TicketNode.itemPriority",
                Title = "مدیریت سوالات متداول",
                ControllerName = "ManageFAQ",
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
            TicketNode?.ChildNodes.Add(itemFAQ);
            #endregion

            #region Training Topic
            SiteMapNode TrainingTopicNode = new SiteMapNode
            {
                SystemName = "TrainingTopicNode",
                Title = "آکادمی آموزش",
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
            siteMapNode?.ChildNodes.Add(TrainingTopicNode);
            SiteMapNode Topic = new SiteMapNode
            {
                SystemName = "TrainingTopicNode.Topic",
                Title = "مدیریت صفحات آموزشی",
                ControllerName = "ManageTrainingAcademyTopic",
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
            TrainingTopicNode?.ChildNodes.Add(Topic);

            #endregion

            #region damages
            SiteMapNode damagesNode = new SiteMapNode
            {
                SystemName = "damagesNode",
                Title = "درخواست خسارت",
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
            siteMapNode?.ChildNodes.Add(damagesNode);
            SiteMapNode itemListDamages = new SiteMapNode
            {
                SystemName = "TicketNode.itemListDamages",
                Title = "لیست درخواست های خسارت",
                ControllerName = "ManageDamages",
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
            damagesNode?.ChildNodes.Add(itemListDamages);
            SiteMapNode itemPatternAnswerDamages = new SiteMapNode
            {
                SystemName = "TicketNode.itemPatternAnswerDamages",
                Title = "مدیریت الگوهای پاسخ درخواست های خسارت",
                ControllerName = "ManagePatternAnswerDamages",
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
            damagesNode?.ChildNodes.Add(itemPatternAnswerDamages);

            #endregion

            #region RequestCOD
            SiteMapNode RequestCODNode = new SiteMapNode
            {
                SystemName = "RequestCODNode",
                Title = "درخواست فعال سازی حساب پس کرایه",
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
            siteMapNode?.ChildNodes.Add(RequestCODNode);
            SiteMapNode itemListRequest = new SiteMapNode
            {
                SystemName = "RequestCODNode.itemListRequest",
                Title = "لیست درخواست های فعال سازی",
                ControllerName = "ManageRequestCOD",
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
            RequestCODNode?.ChildNodes.Add(itemListRequest);
            SiteMapNode itemPatternAnswerRequest = new SiteMapNode
            {
                SystemName = "RequestCODNode.itemPatternAnswerRequest",
                Title = "مدیریت الگوهای پاسخ درخواست های فعال سازی",
                ControllerName = "ManagePatternAnswerRequestCOD",
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
            RequestCODNode?.ChildNodes.Add(itemPatternAnswerRequest);

            #endregion
            //TopMenuModel.TopicModel
        }


        //private CustomerNavigationModel GetCustomerNavigation(int selectedTabId = 0)
        //{
        //    var model = new CustomerNavigationModel();

        //    model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
        //    {
        //        RouteName = "CustomerInfo",
        //        Title ="Ahmad",
        //        Tab = CustomerNavigationEnum.Info,
        //        ItemClass = "customer-info"
        //    });
        //    return model;
        //}
        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {

            _settingService.DeleteSetting<TicketSettings>();
            #region Delete Plugin LocaleResource servic
            // this.DeletePluginLocaleResource("Nop.Plugin.Misc.ShippingSolutions.PDE_Authorization");

            #endregion
            #region set Local Resource Update/Delete/Create/Duplicate/
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.MessageDisable");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.MessageDisable");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Duplicate");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Create");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.MessageUpdate");

            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.BackToList");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.TabInfo");

            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Active");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Disable");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Limitation");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.PricingPolicy");

            #endregion

            #region Dep
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_Name");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_IsActive");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_DateInsert");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_DateUpdate");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_UserInsert");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_UserUpdate");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_DepartmentTicket_StoreName");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_DepartmentTicket_Name");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_DepartmentTicket_IsActive");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchStoreId");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.DepartmentTicket_title");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.DepartmentTicket_pagetitle");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.DepartmentTicket_new");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.DepartmentTicket_update");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.DepName");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.ListStore");





            #endregion

            #region Priority
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_PriorityTicket_Name");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_PriorityTicket_IsActive");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_PriorityTicket_DateInsert");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_PriorityTicket_DateUpdate");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_PriorityTicket_UserInsert");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_PriorityTicket_UserUpdate");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_PriorityTicket_Name");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_PriorityTicket_IsActive");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.PriorityTicket_title");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.PriorityTicket_pagetitle");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.PriorityTicket_new");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.PriorityTicket_update");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.DepName");





            #endregion

            #region Staff Deps
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Page_Titel_List_StaffDepartmentTicket");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Titel_List_StaffDepartmentTicket");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.New_StaffDepartmentTicket");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchDepartmentId");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.DepartmentTicketName");

            #endregion

            #region Ticket
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchDepartmentId");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchUsername_Staff");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchIdStatus");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchOrderId");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchTrackingCode");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchIssue");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchPriorityId");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.SearchStoreId"); ;
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_StoreName");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.DepartmentTicketName");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_Isuue");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_FullName");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_DepName");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_PriorityName");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_OrderId");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_TrackingCode");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_Status");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_DateInsert");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_NameStaffOpen");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_DateOpen");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_LastDateAnswer");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_NameStaffLastAnswer");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Grid_Ticket_LastRank");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Search_Ticket_IsActive");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Page_Titel_List_Ticket");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Titel_List_Ticket");
            this.DeletePluginLocaleResource("Nop.Plugin.Misc.Ticket.Finished");



            #endregion
            PluginManager.MarkPluginAsUninstalled(this.PluginDescriptor.SystemName);
            _context.Uninstall();
            base.Uninstall();
        }
    }
}
