using Nop.Core;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public interface ICategoryIconService
    {
        IList<BS_CategoryIcons> GetAllCategoryIcons(int pageIndex = 0, int pageSize = int.MaxValue);

        BS_CategoryIcons GetIconExtentionByCategoryId(int CategoryId);

        void InsertCategoryIcon(BS_CategoryIcons CategoryIcons);

        void UpdateCategoryIcon(BS_CategoryIcons CategoryIcons);

        void DeleteCategoryIcon(BS_CategoryIcons CategoryIcons);
    }
}
