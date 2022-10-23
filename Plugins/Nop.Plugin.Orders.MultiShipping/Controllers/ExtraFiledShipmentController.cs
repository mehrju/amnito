using Microsoft.AspNetCore.Mvc;
using Nop.Core.Data;
using Nop.Plugin.Orders.MultiShipping.Models.Tbl_Extra_Status_Field_Shipment;
using Nop.Web.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Controllers
{
    public class ExtraFiledShipmentController : BasePublicController
    {
        private readonly IRepository<TblExtraFiledShipment> _repositoryTblExtraFiledShipment;
        public ExtraFiledShipmentController(IRepository<TblExtraFiledShipment> repositoryTblExtraFiledShipment)
        {
            _repositoryTblExtraFiledShipment = repositoryTblExtraFiledShipment;
        }
        [HttpGet]
        [AdminAntiForgery]
        [Area(AreaNames.Admin)]
        public virtual IActionResult GetStatus()
        {
            //1 مفقود شده 
            //2 غرامت خسارت
            //3 غرامت مفقودی
            //4 غرامت تاخیر
            //5 شکایت

            try
            {
                List<TblExtraFiledShipment> listAll = _repositoryTblExtraFiledShipment.Table.ToList();
                var ListAllShipingid = from m in listAll group m by m.ShippingId into g select new { ShippingId = g.Key };
                int gheramat_khesarat = 0;
                int gheramat_mafghodi = 0;
                int gheramat_takhir = 0;
                int shekayat = 0;
                List<TblExtraFiledShipment> gheramat_khesarat1 = new List<TblExtraFiledShipment>();
                List<TblExtraFiledShipment> gheramat_mafghodi1 = new List<TblExtraFiledShipment>();
                List<TblExtraFiledShipment> gheramat_takhir1 = new List<TblExtraFiledShipment>();
                List<TblExtraFiledShipment> shekayat1 = new List<TblExtraFiledShipment>();
                foreach (var item in ListAllShipingid)
                {
                    

                    gheramat_khesarat1 = listAll.Where(p => p.ShippingId == item.ShippingId && p.Type == 2).ToList();
                    gheramat_mafghodi1 = listAll.Where(p => p.ShippingId == item.ShippingId && p.Type == 3).ToList();
                    gheramat_takhir1 = listAll.Where(p => p.ShippingId == item.ShippingId && p.Type == 4).ToList();
                    shekayat1 = listAll.Where(p => p.ShippingId == item.ShippingId && p.Type == 5).ToList();

                    if (gheramat_khesarat1.Count > 0)
                    {
                        List<TblExtraFiledShipment> temp1 = gheramat_khesarat1.Where(p => p.value == 1).ToList();
                        List<TblExtraFiledShipment> temp2 = gheramat_khesarat1.Where(p => p.value == 2).ToList();
                        if (temp1.Count > 0 && temp2.Count == 0)
                        {
                            gheramat_khesarat++;
                        }
                    }

                    /////////
                    if (gheramat_mafghodi1.Count > 0)
                    {
                        List<TblExtraFiledShipment> temp1 = gheramat_mafghodi1.Where(p => p.value == 1).ToList();
                        List<TblExtraFiledShipment> temp2 = gheramat_mafghodi1.Where(p => p.value == 2).ToList();
                        if (temp1.Count > 0 && temp2.Count == 0)
                        {
                            gheramat_mafghodi++;
                        }
                    }
                    ////
                    if (gheramat_takhir1.Count > 0)
                    {
                        List<TblExtraFiledShipment> temp1 = gheramat_takhir1.Where(p => p.value == 1).ToList();
                        List<TblExtraFiledShipment> temp2 = gheramat_takhir1.Where(p => p.value == 2).ToList();
                        if (temp1.Count > 0 && temp2.Count == 0)
                        {
                            gheramat_takhir++;
                        }
                    }


                    ///
                    if (shekayat1.Count > 0)
                    {
                        List<TblExtraFiledShipment> temp1 = shekayat1.Where(p => p.value == 1).ToList();
                        List<TblExtraFiledShipment> temp2 = shekayat1.Where(p => p.value == 2).ToList();
                        if (temp1.Count > 0 && temp2.Count == 0)
                        {
                            shekayat++;
                        }
                    }

                }
                return Json(new { success = true, _gheramat_khesarat = gheramat_khesarat.ToString(), _gheramat_mafghodi = gheramat_mafghodi.ToString(), _gheramat_takhir = gheramat_takhir.ToString(), _shekayat = shekayat.ToString() });

            }
            catch (Exception ex)
            {
                 
                return Json(new { success = false, responseText = "خطا در دریافت اطلاعات نوتیفیکیشن غرامت" });
            }



        }
    }
}
