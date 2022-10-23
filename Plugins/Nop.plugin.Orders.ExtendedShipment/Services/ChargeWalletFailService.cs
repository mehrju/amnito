using Newtonsoft.Json;
using Nop.Core.Data;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class ChargeWalletFailService : IChargeWalletFailService
    {
        private readonly IRepository<Tbl_ChargeWalletFails> _repository_TblChargeWalletFails;

        public ChargeWalletFailService(IRepository<Tbl_ChargeWalletFails> repository_TblChargeWalletFails)
        {
            _repository_TblChargeWalletFails = repository_TblChargeWalletFails;
        }

        public void InsertFailedLog(Exception ex,object detail, string message)
        {
            _repository_TblChargeWalletFails.Insert(new Tbl_ChargeWalletFails()
            {
                CreateDate = DateTime.Now,
                Data = JsonConvert.SerializeObject(detail),
                Message = message,
                Exception = ex.ToString()
            });
        }
    }
}
