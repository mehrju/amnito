using Nop.Core.Data;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class CartonService : ICartonService
    {
        private readonly IProductService _productService;
        private readonly IRepository<Tbl_CartonInfo> _repository_TblCartonInfo;

        public CartonService(IProductService productService,
            IRepository<Tbl_CartonInfo> repository_TblCartonInfo)
        {
            _productService = productService;
            _repository_TblCartonInfo = repository_TblCartonInfo;
        }

        public int CalculateSize9Price(decimal length, decimal width, decimal height)
        {
            var cartonProduct = _productService.GetProductById(10430);
            var sizeAttr = cartonProduct.ProductAttributeMappings.FirstOrDefault(p => p.ProductAttribute.Name.Contains("سایز"));
            var size9Attr = sizeAttr.ProductAttributeValues.FirstOrDefault(p => p.Name.Contains("سایز 9"));
            var cost = size9Attr.Cost;
            int size9Lenght = 55, size9Width = 45, size9Height = 35;
            //محیط ها
            var size9Environment = (size9Lenght * size9Width * 2) + (size9Width * size9Height * 2) + (size9Height * size9Lenght * 2);
            var entredSizeEnvironment = (length * width * 2) + (width * height * 2) + (height * length * 2);
            var ratio = (double)entredSizeEnvironment / (double)size9Environment;
            return (int)Math.Floor((int)cost * ratio);
        }

        public string GetRequiredCartonBySize(decimal length, decimal width, decimal height)
        {
            var carton = _repository_TblCartonInfo.Table
                .Where(p => p.Length >= length && p.Width >= width && p.Height >= height).OrderBy(p => p.Name).FirstOrDefault();
            if(carton != null)
            {
                return carton.Name;
            }
            return null;
        }
        public Tbl_CartonInfo getcartonInfoBySizeName(string name)
        {
            var carton = _repository_TblCartonInfo.Table
               .Where(p => p.Name ==name).FirstOrDefault();
            if (carton != null)
            {
                return carton;
            }
            return null;
        }

    }
}
