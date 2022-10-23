using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.plugin.Orders.ExtendedShipment.Services.Security;
using Nop.Plugin.Misc.PrintedReports_Requirements.Service.Interface;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
//using Nop.Plugin.Misc.PostbarDashboard.Models;
using Nop.Services.Orders;
using Nop.Web.Controllers;
using Nop.Web.Framework.Mvc.Filters;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Nop.Plugin.Orders.MultiShipping.Controllers
{
	public class ShipitoHomeController : BasePublicController
	{
		private readonly OrderSettings _orderSettings;
		private readonly IOrderService _orderService;
		private readonly INewCheckout _newCheckout;
		private readonly IWorkContext _workContext;
		private readonly IApService _apService;
		private readonly IPdfService _pdfService;
		private readonly I_IndexPageService _I_IndexPageService;
		private readonly ICustomerService _customerService;
		private readonly IAuthenticationService _authenticationService;
		private readonly IEventPublisher _eventPublisher;
		private readonly IStoreContext _storeContext;
		private readonly INotificationService _notificationService;
		private readonly ISecurityService _securityService;
		private readonly IShipmentTrackingService _shipmentTrackingService;
		private readonly ISekehService _sekehService;
		private readonly IExtendedShipmentService _extendedShipmentService;

		public ShipitoHomeController(OrderSettings orderSettings, IPdfService pdfService, IOrderService orderService, INewCheckout newCheckout
			, IWorkContext workContext, IApService apService, I_IndexPageService I_IndexPageService
			, ICustomerService customerService, IAuthenticationService authenticationService
			, IEventPublisher eventPublisher
			, IStoreContext storeContext,
			INotificationService notificationService
			, ISecurityService securityService
			, ISekehService sekehService
			, IShipmentTrackingService shipmentTrackingService
			, IExtendedShipmentService extendedShipmentService)
		{
			_extendedShipmentService = extendedShipmentService;
			_securityService = securityService;
			_shipmentTrackingService = shipmentTrackingService;
			_eventPublisher = eventPublisher;
			_authenticationService = authenticationService;
			_apService = apService;
			_orderSettings = orderSettings;
			_pdfService = pdfService;
			_orderService = orderService;
			_I_IndexPageService = I_IndexPageService;
			_newCheckout = newCheckout;
			_workContext = workContext;
			_customerService = customerService;
			_storeContext = storeContext;
			_notificationService = notificationService;
			_sekehService = sekehService;
		}
		public IActionResult encryptuser()
		{
			try
			{
				if (!IsValidCustomer())
				{
					return Json(new { data = (AddressDetailes)null, success = false, message = "" });
				}
				if (!(_workContext.CurrentCustomer.IsInCustomerRole("mini-Administrators")
					|| _workContext.CurrentCustomer.IsInCustomerRole("ContractCustomers")))
				{
					return Json(new { data = (AddressDetailes)null, success = false, message = "شما دسترسی لازم برای استفاده از پست فروشگاهی را ندارید" });
				}

				var Result = _extendedShipmentService.getAddressData(_workContext.CurrentCustomer.Id);
				if (Result == null)
				{
					Result = new AddressDetailes();
					Result.token = _workContext.CurrentCustomer.Username;
				}
				Result.token = _sekehService.EncryptData(Result.token);
				return Json(new { data = Result, success = true });
			}
			catch (Exception ex)
			{
				LogException(ex);
				return Json(new { data = (AddressDetailes)null, success = false, message = "شما دسترسی لازم برای استفاده از پست فروشگاهی را ندارید" });
			}
		}

        public bool IsValidCustomer()
        {
            var customer = _workContext.CurrentCustomer;
            if (customer == null || !customer.IsRegistered() || customer.IsGuest() || customer.Username == null)
                return false;
            return true;
        }
        public IActionResult Login()
        {
            HttpContext.Session.SetString("ComeFrom", "App");
            if (!string.IsNullOrEmpty(HttpContext.Request.Query["username"].ToString()))
            {
                string username = HttpContext.Request.Query["username"].ToString();
                string password = HttpContext.Request.Query["password"].ToString();
                string msg = "";
                var lgoinreuslt = _securityService.Login(username, password, out msg);
                if (!lgoinreuslt)
                {
                    common.Log("خطا در زمان لاگین اپلیکشن", $"{username} :  {password} ===> {msg}");
                    return Unauthorized();
                }
            }
            return RedirectToAction("Index", "ShipitoHome");
        }
        // [Route("Sekeh/_Startup")]
        [Route("Index")]
        public IActionResult Index(string input)
        {
            if (_storeContext.CurrentStore.Id != 5)
            {
                return NotFound();
            }
			if (HttpContext.Request.Query.Count > 0)
			{
				string json = Newtonsoft.Json.JsonConvert.SerializeObject(HttpContext.Request.Query.Select(p => new { p.Key, p.Value }));
				common.Log("اطلاعات ورودی ایرانسل", json);
			}
			string _channel = HttpContext.Request.Query["channel"].ToString();
            if (!string.IsNullOrEmpty(_channel) && _channel.ToLower() == "myirancell")
            {
                string _token =HttpContext.Request.Query["token"].ToString();
                bool isAuthenticated = _apService.AuthenticateMyIrancell(_token);
            }

            var Model = new Nop.Plugin.Orders.MultiShipping.Models.vm_Index();
            Model.List_Item_RSS = new List<Item>();
            Model.vmServiceProvider_Index = _I_IndexPageService.GetServiceProvider();
            Model.vmSlidShow_Index = _I_IndexPageService.GetSlidshowAsync();
            //  }

				string submarket = "PostexNew";
			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/{submarket}/Index.cshtml", Model);

		}
		[HttpPost, HttpGet]
		public IActionResult hyperJetIndex(string mobile)
		{
			HttpContext.Session.SetString("ComeFrom", "HyperJet");
			_newCheckout.Log("Input from HyperJet", mobile ?? "");
			if (string.IsNullOrEmpty(mobile))
			{
				_newCheckout.Log("اطلاعات پست شده از هایپرچت نا معتبر می باشد", mobile ?? "فیلد اطلاعاتی خالی می باشد");
				return Content("در حال حاضر این سرویس در دسترس نمی باشد");

			}

			string msg = "";
			var customer = _apService.AuthHyperJet(mobile, out msg, 1146);
			if (customer == null)
			{
				_newCheckout.Log(msg, "");
				return RedirectToRoute("Login");
				//return Content(msg);
			}

			//درخواست اطلاعات کاربر


			if (!_apService.IsValidCustomer(customer))
			{
				_newCheckout.Log("خطا در زمان اعتبار سنجی کاربر هایپر جت", "");
				return RedirectToRoute("Login");
				// return Content("در حال حاضر امکان دسترسی به این سرویس برای شما مقدور نمی باشد. مجددا سعی کنید");
			}
			var Model = new Nop.Plugin.Orders.MultiShipping.Models.vm_Index();
			Model.List_Item_RSS = new List<Item>();
			try
			{
				#region rss
				//var rssFeed = new Uri("https://mag.postbar.ir/?call_custom_simple_rss=1");
				//var request = (HttpWebRequest)WebRequest.Create(rssFeed);
				//request.Method = "GET";
				//var response = (HttpWebResponse)request.GetResponse();
				//using (var reader = new StreamReader(response.GetResponseStream()))
				//{
				//    var feedContents = reader.ReadToEnd();
				//    var document = XDocument.Parse(feedContents);



				//    //====================================
				//    var posts = (from p in document.Descendants("item")
				//                 select new
				//                 {
				//                     Title = (p.Element("title") == null) ? "" : p.Element("title").Value,
				//                     Link = (p.Element("link") == null) ? "" : p.Element("link").Value,
				//                     Comments = (p.Element("comments") == null) ? "" : p.Element("comments").Value,
				//                     PubDate = (p.Element("pubDate") == null) ? "" : (p.Element("pubDate").Value),
				//                     Description = (p.Element("description") == null) ? "" : p.Element("description").Value,
				//                     Source = (p.Element("enclosure") == null) ? "" : (string)p.Element("enclosure").Attribute("url").Value

				//                 }).ToList();

				//    foreach (var post in posts)
				//    {
				//        Item temp = new Item();
				//        temp.Title = post.Title;
				//        temp.Link = post.Link;
				//        temp.Comments = post.Comments;
				//        temp.PubDate = post.PubDate;
				//        temp.Description = post.Description;
				//        temp.Source = post.Source;
				//        Model.List_Item_RSS.Add(temp);
				//    }
				//}
				#endregion

			}
			catch (Exception ex)
			{
				LogException(ex);
			}
			finally
			{
				Model.vmServiceProvider_Index = _I_IndexPageService.GetServiceProvider();
				Model.vmSlidShow_Index = _I_IndexPageService.GetSlidshowAsync();
			}
			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Index.cshtml", Model);
		}
		public IActionResult GetPDF(int orderId)
		{
			//a vendor should have access only to his products
			var vendorId = 0;
			if (_workContext.CurrentVendor != null)
			{
				vendorId = _workContext.CurrentVendor.Id;
			}

			var order = _orderService.GetOrderById(orderId);
			var orders = new List<Order>();
			orders.Add(order);
			byte[] bytes;
			using (var stream = new MemoryStream())
			{
				_pdfService.PrintOrdersToPdf(stream, orders, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? 0 : _workContext.WorkingLanguage.Id, vendorId);
				bytes = stream.ToArray();
			}
			//HttpContext.Response.Headers.Add("Set-Cookie", "fileDownload=true; path=/");
			return File(bytes, MimeTypes.ApplicationPdf, $"order_{order.Id}.pdf");
		}

		[PublicAntiForgery]
		[TokenAuthorize]
		[HttpGet]
		public IActionResult LoginApp(string UName)
		{
			if (string.IsNullOrEmpty(UName) || UName.Length != 11 || !CheckNumber(UName))
			{
				return Json(new { success = false, message = "نام کاربری وارد شده نامعتبر می باشد" });
			}
			var customer = _customerService.GetCustomerByUsername(UName);
			if (customer == null)
				return Json(new { success = false, message = "کاربر مورد نظر یافت نشد" });
			if (!customer.Active || customer.Deleted)
			{
				return Json(new { success = false, message = "کاربری مورد نظر مسدود می باشد لطفا با پشتیبانی تماس بگیرید" });
			}
			if (_workContext.CurrentCustomer.IsRegistered())
			{
				//Already registered customer. 
				_authenticationService.SignOut();

				//raise logged out event       
				_eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));

				//Save a new record
				//   _workContext.CurrentCustomer = _customerService.InsertGuestCustomer();
			}
			_authenticationService.SignIn(customer, true);
			return Json(new { success = true });
		}
		public bool CheckNumber(string strPhoneNumber)
		{
			var MatchPhoneNumberPattern = "(0|\\+98)?([ ]|-|[()]){0,2}9[1|2|3|4]([ ]|-|[()]){0,3}(?:[0-9]([ ]|-|[()]){0,2}){8}";
			return strPhoneNumber != null && Regex.IsMatch(strPhoneNumber, MatchPhoneNumberPattern);
		}

		public IActionResult GetLatestNotification(string cookieIds)
		{
			var notifIds = cookieIds?.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
			return Ok(_notificationService.GetLatestNotification(notifIds));
		}

		public IActionResult GetWightCategories()
		{
			var wiCats = _shipmentTrackingService.GetWightCategories();
			return Ok(wiCats.Select(p => new WeightCategoryModel() { Text = p.Text, Value = p.Text }).ToList());
		}
	}


}
