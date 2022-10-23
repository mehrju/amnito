using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using Nop.Core;
using Nop.Core.Data;
using Nop.Services.Events;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
   public partial class BS_SliderService : IBS_SliderService
    {
        #region Field
        private readonly IRepository<BS_Slider> _bsSliderRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctr

        public BS_SliderService(IRepository<BS_Slider> bsSliderRepository, IEventPublisher eventPublisher)
        {
            this._bsSliderRepository = bsSliderRepository;
            this._eventPublisher = eventPublisher;

        }

        #endregion

        #region Methods

        public void Delete(int Id)
        {
            //item.Deleted = true;

            var query = from c in _bsSliderRepository.Table
                        where c.Id == Id
                        select c;

            var sliderImages = query.ToList();
            foreach (var sliderImage in sliderImages)
            {
                _bsSliderRepository.Delete(sliderImage);
            }
        }

        public void Insert(BS_Slider item)
        {
            _bsSliderRepository.Insert(item);
        }

        public BS_Slider GetBsSliderImageById(int Id)
        {
            return _bsSliderRepository.GetById(Id);
        }


        public IPagedList<BS_Slider> GetBSSliderImages(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = _bsSliderRepository.Table; 
             query = from c in _bsSliderRepository.Table
                        select c;


            query = query.OrderBy(b => b.SliderActiveStartDate);

            var bsSliderImages = new PagedList<BS_Slider>(query, pageIndex, pageSize);
            return bsSliderImages;
        }


        public List<BS_Slider> GetBSSliderImagesByDate()
        {
            var bsSliderImages =
                _bsSliderRepository.Table.ToList()
                    //.Where(c => DateTime.Now.Date.IsBetween(c.SliderActiveStartDate.Value, c.SliderActiveEndDate.Value))
                    .ToList();
            //var bsSliderImages =
            //    _bsSliderRepository.Table.ToList()
            //        .Where(c => DateTime.Now.Date.IsBetween(c.SliderActiveStartDate.Value, c.SliderActiveEndDate.Value))
            //        .ToList();

            return bsSliderImages;
        }

        #endregion




        public void Update(BS_Slider item)
        {
            _bsSliderRepository.Update(item);
        }
    }
}
