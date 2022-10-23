using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
    public class AddRequestCODModel
    {
        public string Fname { get; set; }
        public string Lname { get; set; }
        public string Phone { get; set; }
        public string PostalCode { get; set; }
        public string NationalCode { get; set; }
        public string ManagerNationalIDSerial { get; set; }
        public string ManagerBirthDate { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string AccountNumber { get; set; }
        public string AccountIBAN { get; set; }
        public string AccountBranchName { get; set; }
        public string Address { get; set; }
        //public string UserName { get; set; }
        public int CustomerId { get; set; }
        //آی دی از جدول CountryCodeModel
        public int Country { get; set; }
        //آی دی از جدول StateCodeModel
        public int State { get; set; }
        public string PaymentMethod { get; set; }
        public bool isFromApp { get; set; }

    }
}
