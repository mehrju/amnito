using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Data;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.Services.Directory;
using NUglify.Helpers;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class UserStatesService : IUserStatesService
    {
        private readonly IRepository<UserStetesModel> _repositoryUserStetes;
        private readonly IDbContext _dbContext;
        private readonly IStateProvinceService _stateProvinceService;
        public UserStatesService(IRepository<UserStetesModel> repositoryUserStetes
            , IDbContext dbContext, IStateProvinceService stateProvinceService)
        {
            _repositoryUserStetes = repositoryUserStetes;
            _dbContext = dbContext;
            _stateProvinceService = stateProvinceService;
        }
        public bool Insert(int customerId, List<int> states, int countryId)
        {
            if (countryId == 0)
                return false;
            var tb = _repositoryUserStetes.Table;
            var _states = _stateProvinceService.GetStateProvincesByCountryId(countryId).Select(p => p.Id).ToList(); ;
            var ForDeleteLst_states = tb.Where(p => p.CustomerId == customerId && _states.Contains(p.StateId)).ToList();
            if (ForDeleteLst_states.Any())
            {
                foreach (var item in ForDeleteLst_states)
                {
                    _repositoryUserStetes.Delete(item);
                }
            }

            List<UserStetesModel> LstUserState = new List<UserStetesModel>();
            foreach (var item in states)
            {
                LstUserState.Add(new UserStetesModel()
                {
                    CustomerId = customerId,
                    StateId = item
                });
            }
            if (LstUserState.Any())
                _repositoryUserStetes.Insert(LstUserState);
            return true;
        }
        public List<int> getUserStates(int customerId)
        {
            return _repositoryUserStetes.Table.Where(p => p.CustomerId == customerId).Select(p => p.StateId).ToList();
        }

        public int GetIdUser(int CountryId)
        {
            return _repositoryUserStetes.Table.Where(p => p.StateId == CountryId).Select(p => p.CustomerId).FirstOrDefault();
        }
    }
}
