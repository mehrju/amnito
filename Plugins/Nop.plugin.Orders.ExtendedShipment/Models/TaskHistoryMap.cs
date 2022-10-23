using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class TaskHistoryMap : EntityTypeConfiguration<TaskHistoryModel>
    {
        public TaskHistoryMap()
        {
            ToTable("Tb_TaskHistory");
            HasKey(p => p.Id);
            Property(p => p.Name);
            Property(p => p.ExecDate);
            Property(p=> p.value);
        }
    }
}
