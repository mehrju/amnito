using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class TicketModel
    {
        public TicketModel()
        {
            ticket_Details = new List<TicketDetailModel>();
        }

        public int Id { get; set; }
        public int IdCustomer { get; set; }

        
        public bool TypeTicket { get; set; } 

        public int? OrderCode { get; set; }

        public string TrackingCode { get; set; }
        public int StoreId { get; set; }
        public int ProrityId { get; set; }

        public int DepartmentId { get; set; }
        
        public int IdCategoryTicket { get; set; }
        
        public string Issue { get; set; }
        public bool IsActive { get; set; }
        public int Status { get; set; }
        
        public DateTime DateInsert { get; set; }
        public int? StaffIdAccept { get; set; }
        
        public DateTime? DateStaffAccept { get; set; }
        
        public int? StaffIdLastAnswer { get; set; }
        
        public DateTime? DateStaffLastAnswer { get; set; }
        
        public int? IdUserUpdate { get; set; }
        
        public DateTime? DateUpdate { get; set; }
        
        public bool ShowCustomer { get; set; }

        public List<TicketDetailModel> ticket_Details { get; set; }
    }
}
