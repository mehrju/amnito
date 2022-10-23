using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Data
{
    public class Tbl_TrainingAcademyTopicMAP : EntityTypeConfiguration<Domain.Tbl_TrainingAcademyTopic>
    {
        public Tbl_TrainingAcademyTopicMAP()
        {
            ToTable("Tbl_TrainingAcademyTopic");

            HasKey(m => m.Id);
            Property(m => m.IdTopic);
            Property(m => m.UrlImage);
            Property(m => m.IsActive);
            Property(m => m.DateInsert);
            Property(m => m.DateUpdate).IsOptional();
            Property(m => m.IdUserInsert);
            Property(m => m.IdUserUpdate).IsOptional();
        }
    }
}
