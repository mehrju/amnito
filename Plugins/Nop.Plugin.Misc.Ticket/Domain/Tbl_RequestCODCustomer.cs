using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Domain
{
    public class Tbl_RequestCODCustomer : BaseEntity
    {
        [ScaffoldColumn(false)]
        public string Fname { get; set; }
        [ScaffoldColumn(false)]
        public string Lname { get; set; }
        [ScaffoldColumn(false)]
        public int IdCustomer { get; set; }
        [ScaffoldColumn(false)]
        public string Shaba { get; set; }
        [ScaffoldColumn(false)]
        public string NatinolCode { get; set; }
        [ScaffoldColumn(false)]
        public int StoreId { get; set; }
        [ScaffoldColumn(false)]
        public string BusinessType{get;set;}
        [ScaffoldColumn(false)]
        public string Address { get; set; }

        [ScaffoldColumn(false)]
        public bool IsActive { get; set; }
        //0 در صف انتظار   
        //1 در حال بررسی
        //2 عدم تایید
        //3 تایید
        [ScaffoldColumn(false)]
        public int Status { get; set; }


        [ScaffoldColumn(false)]
        public string UrlFile { get; set; }
        [ScaffoldColumn(false)]
        public string DesAnswerStaff { get; set; }




        [ScaffoldColumn(false)]
        public string Username { get; set; }
        [ScaffoldColumn(false)]
        public string CodePosti { get; set; }

        [ScaffoldColumn(false)]
        public DateTime DateInsert { get; set; }
        [ScaffoldColumn(false)]
        public int? StaffIdAccept { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? DateStaffAccept { get; set; }
        [ScaffoldColumn(false)]
        public int? StaffIdLastAnswer { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? DateStaffLastAnswer { get; set; }
        [ScaffoldColumn(false)]
        public int? IdUserUpdate { get; set; }
        [ScaffoldColumn(false)]
        public DateTime? DateUpdate { get; set; }


        [ScaffoldColumn(false)]
        public int IdTicket { get; set; }
        public int OrderId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public string Phone { get; set; }
        public string ManagerNationalIDSerial { get; set; }
        public string ManagerBirthDate { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string WebSiteURL { get; set; }
        public string AccountNumber { get; set; }
        public string AccountBranchName { get; set; }
    }
}
