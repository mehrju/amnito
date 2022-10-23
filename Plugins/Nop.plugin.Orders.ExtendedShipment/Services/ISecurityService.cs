using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Nop.Web.Framework.Menu;
using static Nop.plugin.Orders.ExtendedShipment.Services.SecurityService;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface ISecurityService
    {
        string DecryptSepInput(string msisdn);
        bool Login(string username, string password,out string msg);
        bool HasAccessToMenu(SiteMapNode node);
        List<menuItemAccessList> getMenuAccessList();
        void InsertMenuItem(SiteMapNode node);
        string SignData(string DataToSign);
        bool VerifyData(string signedData, string originalData, VerifySignType type);
        string GetHashString(string inputString);
        bool NeedCheckActionAccess(string actionName, string controllerName, ICollection<string> FormKey, out int NeedCheckAccessItemId);
        bool HasAccessToAction(HttpContext context, string actionName, string controllerName);
        bool IsValidSecurityCode(string userName, string SecurtyCode);
        bool ValidateActivationCode(string Username, string ActivationCode);
        string SetActivationCode(string UserName, out string error);
        string GetActivationCoe();
        bool IsMaxActivationCodeExceed(string Username);
    }
}