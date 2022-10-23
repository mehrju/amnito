using System.Data;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Plugins;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.Services.Common;
using Nop.Web.Framework.Menu;
using System.Linq;
using System.Net;
using System.ServiceModel;
using Nop.plugin.Orders.ExtendedShipment.PostTracking;
using Nop.Services.Logging;
using Nop.Core.Domain.Logging;
using Nop.Core.Infrastructure;
using System.ServiceModel.Security;
using System.IO;

namespace Nop.plugin.Product.ExtendedShipment
{
    /// <summary>
    /// 
    /// </summary>
    public class ExtendedShipmentPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        public static string postTrackingUrl = "https://Services.post.ir/tntsearch/getinfo.asmx";
        private readonly IWebHelper _webHelper;
        private readonly ShipmentAppointmenObjectContext _context;
        public ExtendedShipmentPlugin(IWebHelper webHelper,
            ShipmentAppointmenObjectContext context)
        {
            this._webHelper = webHelper;
            this._context = context;
        }
        public override void Install()
        {
            base.Install();
            _context.Install();
            PluginManager.MarkPluginAsInstalled(this.PluginDescriptor.SystemName);
        }
        public override void Uninstall()
        {
            PluginManager.MarkPluginAsUninstalled(this.PluginDescriptor.SystemName);
            _context.Uninstall();
            base.Uninstall();
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/ExtendedOrderSetting/Configure";
        }
        public void ManageSiteMap(SiteMapNode rootNode)
        {

            var node = rootNode.ChildNodes.FirstOrDefault(p => p.SystemName == "Content Management").ChildNodes.FirstOrDefault(p => p.SystemName == "Polls");
            node.ControllerName = "ManageComment";
            node.ActionName = "List";

            SiteMapNode siteMapNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Configuration");
            SiteMapNode item = new SiteMapNode
            {
                SystemName = "Product.ExtendedShipment",
                Title = "تنظیم ارسال محموله",
                ControllerName = "ExtendedOrderSetting",
                ActionName = "Configure",
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
            siteMapNode?.ChildNodes.Add(item);

            SiteMapNode item2 = new SiteMapNode
            {
                SystemName = "Product.NotifConfigruration",
                Title = "تنظیمات اطلاع رسانی",
                ControllerName = "NotifConfig",
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
            siteMapNode?.ChildNodes.Add(item2);


            SiteMapNode item20 = new SiteMapNode
            {
                SystemName = "Setting.gatewayShop",
                Title = "لیست فروشگاه های گیت وی",
                ControllerName = "gatewayShop",
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
            siteMapNode?.ChildNodes.Add(item20);


            SiteMapNode siteMapNode1 = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Sales");
            SiteMapNode item6 = new SiteMapNode
            {
                SystemName = "Product.QualityControl",
                Title = "کنترل کیفیت",
                ControllerName = "QualityControl",
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
            siteMapNode1?.ChildNodes.Add(item6);

            SiteMapNode item1 = new SiteMapNode
            {
                SystemName = "Product.CODShipment",
                Title = "محموله پس کرایه",
                ControllerName = "CodShipmentList",
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
            siteMapNode1?.ChildNodes.Add(item1);


            SiteMapNode item5 = new SiteMapNode
            {
                SystemName = "Product.ForeignOrder",
                Title = "محموله خارجی",
                ControllerName = "ForeignOrder",
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
            siteMapNode1?.ChildNodes.Add(item5);

            SiteMapNode item17 = new SiteMapNode
            {
                SystemName = "Product.PhoneOrder",
                Title = "ثبت سفارش تلفنی",
                ControllerName = "ManagePhoneOrder",
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
            siteMapNode1?.ChildNodes.Add(item17);

            SiteMapNode item33 = new SiteMapNode
            {
                SystemName = "Product.PhoneOrderList",
                Title = "لیست سفارشات تلفنی",
                ControllerName = "ManagePhoneOrder",
                ActionName = "List",
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
            siteMapNode1?.ChildNodes.Add(item33);


            SiteMapNode item7 = new SiteMapNode
            {
                SystemName = "Product.NewShipmentList",
                Title = "محموله جدید",
                ControllerName = "Order",
                ActionName = "NewShipmentList",
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
            siteMapNode1?.ChildNodes.Add(item7);

            SiteMapNode item18 = new SiteMapNode
            {
                SystemName = "Product.Shipments",
                Title = "لیست مرسولات",
                ControllerName = "Shipments",
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
            siteMapNode1?.ChildNodes.Add(item18);


            SiteMapNode siteMapNode3 = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Promotions");
            SiteMapNode item4 = new SiteMapNode
            {
                SystemName = "Product.AgentAmountRule",
                Title = "قوانین پرداخت به نماینده",
                ControllerName = "AgentAmountRule",
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
            siteMapNode3?.ChildNodes.Add(item4);

            SiteMapNode siteMapNode2 = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Customers");
            SiteMapNode item3 = new SiteMapNode
            {
                SystemName = "Customer.UserStates",
                Title = "تعریف شهر های کاربر",
                ControllerName = "UserStates",
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
            siteMapNode2?.ChildNodes.Add(item3);

            SiteMapNode item9 = new SiteMapNode
            {
                SystemName = "Customer.RewardPointsHistoryCashout",
                Title = "تسویه مالی",
                ControllerName = "ExtendedCustomer",
                ActionName = "RewardPointsHistoryCashout",
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

            siteMapNode2?.ChildNodes.Add(item9);
            SiteMapNode item16 = new SiteMapNode
            {
                SystemName = "Customer.SettlementInfo",
                Title = "لیست تسویه های مالی",
                ControllerName = "SettlementInfo",
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

            siteMapNode2?.ChildNodes.Add(item16);

            SiteMapNode item10 = new SiteMapNode
            {
                SystemName = "Customer.DashboardMessaging",
                Title = "اطلاع رسانی به کاربران سایت",
                ControllerName = "ExtendedCustomer",
                ActionName = "DashboardMessaging",
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
            siteMapNode2?.ChildNodes.Add(item10);

            SiteMapNode item11 = new SiteMapNode
            {
                SystemName = "Customer.RequestFactor",
                Title = "درخواست فاکتور مشتری",
                ControllerName = "ExtendedCustomer",
                ActionName = "RequestFactor",
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
            siteMapNode2?.ChildNodes.Add(item11);

            SiteMapNode item19 = new SiteMapNode
            {
                SystemName = "Customer.AgentConfig",
                Title = "تنطیمات نماینده/جمع آور",
                ControllerName = "ExtendedCustomer",
                ActionName = "AgentConfig",
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
            siteMapNode2?.ChildNodes.Add(item19);

            SiteMapNode siteMapNode4 = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Catalog");
            SiteMapNode item12 = new SiteMapNode
            {
                SystemName = "Products.EventCategories",
                Title = "وضعیت نوع سفارشات",
                ControllerName = "Order",
                ActionName = "EventCategory",
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
            siteMapNode4?.ChildNodes.Add(item12);
        }

        public static DataSet GetPostTrackingResult1(ExtendedShipmentSetting setting, string trackingNumber, out string strOutError)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var basicHttpbinding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                basicHttpbinding.Name = "get_alltrack_V2";
                basicHttpbinding.Security.Mode = BasicHttpSecurityMode.Transport;
                basicHttpbinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                basicHttpbinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                // ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                var endpointAddress = new EndpointAddress(ExtendedShipmentPlugin.postTrackingUrl);
                var proxyClient = new Service_rahgiriSoapClient(basicHttpbinding, endpointAddress);
                strOutError = "";

                var dataSet = proxyClient.get_alltrack_V2(setting.PostUserName,
                    setting.PostPassword, trackingNumber, out strOutError);
                proxyClient.Close();
                return dataSet;
            }
            catch (System.Exception ex)
            {
                strOutError = "خطا در زمان دریافت اطلاعات رهگیری" + "------" + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : "");
                Log("خطا در زمان دریافت اطلاعات رهگیری", strOutError);
                return null;
            }
        }
        public static void Log(string header, string Message)
        {
            var logger =
                (DefaultLogger)EngineContext.Current
                    .Resolve<ILogger>();
            logger.InsertLog(LogLevel.Information, header, Message, null);
        }
        public static DataSet GetPostTrackingResult(ExtendedShipmentSetting setting, string trackingNumber, out string strOutError)
        {
            try
            {
                if (string.IsNullOrEmpty(trackingNumber))
                {
                    strOutError = "کد رهگیری وارد شده نا معتبر می باشد";
                    return null;
                }
                string uri = $"https://Services.postex.ir/api/PostTracking?trackingNumber={trackingNumber}&Hash=0";
                //string uri = $"http://localhost:5000/api/PostTracking?trackingNumber={trackingNumber}&Hash=0";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                httpWebRequest.Method = "GET";
                string result = "";
                using (var response = httpWebRequest.GetResponse() as HttpWebResponse)
                {
                    if (httpWebRequest.HaveResponse && response != null)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            result = reader.ReadToEnd();

                        }
                        if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(result))
                        {
                            DataSet ds = new DataSet();
                            StringReader sr = new StringReader(result);
                            ds.ReadXml(sr);
                            strOutError = "";
                            return ds;
                        }
                        else
                        {
                            strOutError = "اطلاعاتی از سرویس رهگیری دریافت نشد";
                            return null;
                        }
                    }
                    else
                    {
                        strOutError = "اطلاعاتی از سرویس رهگیری دریافت نشد";
                        return null;
                    }
                }
            }
            catch (System.Exception ex)
            {
                strOutError = "خطا در زمان دریافت اطلاعات رهگیری" + "------" + ex.Message + (ex.InnerException != null ? ex.InnerException.Message : "");
                Log("خطا در زمان دریافت اطلاعات رهگیری", strOutError);
                return null;
            }
        }
    }

}

