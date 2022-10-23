using Nop.Core;
using Nop.Core.Data;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public partial class CategoryIconService : ICategoryIconService
    {
        #region Fields

        private readonly IRepository<BS_CategoryIcons> _categoryIconRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor
                
        public CategoryIconService(IRepository<BS_CategoryIcons> categoryIconRepository,
            IEventPublisher eventPublisher)
        {
            this._categoryIconRepository = categoryIconRepository;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Methods
                
        public IList<BS_CategoryIcons> GetAllCategoryIcons(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from ci in _categoryIconRepository.Table                        
                        select ci;

            var categoryIcons = query.ToList();

            return categoryIcons;
        }

        public BS_CategoryIcons GetIconExtentionByCategoryId(int CategoryId)
        {
            if (CategoryId == 0)
                return null;

            var query = from ci in _categoryIconRepository.Table
                        where ci.SubCategoryId == CategoryId
                        select ci;

            BS_CategoryIcons catIcon = new BS_CategoryIcons();

            if(query != null)
            {
                catIcon = query.FirstOrDefault();
            }

            return catIcon;
        }

        public void InsertCategoryIcon(BS_CategoryIcons CategoryIcons)
        {
            if (CategoryIcons == null)
                throw new ArgumentNullException("CategoryIcons");

            _categoryIconRepository.Insert(CategoryIcons);

            //event notification
            _eventPublisher.EntityInserted(CategoryIcons);
        }

        public void UpdateCategoryIcon(BS_CategoryIcons CategoryIcons)
        {
            if (CategoryIcons == null)
                throw new ArgumentNullException("CategoryIcons");

            _categoryIconRepository.Update(CategoryIcons);

            //event notification
            _eventPublisher.EntityUpdated(CategoryIcons);
        }

        public void DeleteCategoryIcon(BS_CategoryIcons CategoryIcons)
        {
            if (CategoryIcons == null)
                throw new ArgumentNullException("CategoryIcons");

            _categoryIconRepository.Delete(CategoryIcons);

            //event notification
            _eventPublisher.EntityDeleted(CategoryIcons);
        }

        #endregion

    }
}
