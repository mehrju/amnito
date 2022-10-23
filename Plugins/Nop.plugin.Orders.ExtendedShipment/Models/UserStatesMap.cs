using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class UserStatesMap: EntityTypeConfiguration<UserStetesModel>
    {
        public UserStatesMap()
        {
            ToTable("Tb_UserStates");
            HasKey(p => p.Id);
            Property(p => p.CustomerId);
            Property(p => p.StateId);
        }
    }
}
