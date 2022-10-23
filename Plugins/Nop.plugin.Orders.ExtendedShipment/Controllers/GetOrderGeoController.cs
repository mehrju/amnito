using Microsoft.AspNetCore.Mvc;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{
    public class GetOrderGeoController : BaseAdminController
    {
        private readonly IDbContext _dbContext;
        public GetOrderGeoController(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Area(AreaNames.Admin)]
        [AuthorizeAdmin]
        public IActionResult GetBillingGeo([FromQuery]int OrderId)
        {
            string Query = $@"select A.Lat, A.Long from Tbl_Address_LatLong As A
                                inner join [Order] As O
                                on A.AddressId = O.BillingAddressId
                                where O.Id = {OrderId} ";
            var data = _dbContext.SqlQuery<GeoModel>(Query, new object[0]).FirstOrDefault();
            if(data == null || data.Lat == 0 || data.Long == 0)
            {
                data= new GeoModel();
                data.Lat = 35.6892;
                data.Long = 51.3890;
            }
            return Json(data);
        }
    }
}
