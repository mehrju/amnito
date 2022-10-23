using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Domain
{
    public class Tbl_AffiliateToCustomer : BaseEntity
    {
        public int CustomerId { get; set; }
        public int AffiliateId { get; set; }
        public int registerUserId { get; set; }
        public DateTime registerDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastDeActiveDate { get; set; }
        public int? LastDeActiveUserId { get; set; }
        public DateTime? LastActiveDate { get; set; }
        public int? LastActiveUser { get; set; }

    }
}
