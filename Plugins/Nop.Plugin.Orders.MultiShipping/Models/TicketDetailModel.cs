using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class TicketDetailModel
    {

       
        public int IdTicket { get; set; }


        /// <summary>
        /// 0 custoer  1 staff
        /// </summary>
        public bool Type { get; set; }


       
        public int? StaffId { get; set; }

       
        public DateTime DateInsert { get; set; }

       
        public string Description { get; set; }

       
        public string UrlFile1 { get; set; }
       
        public string UrlFile2 { get; set; }
       
        public string UrlFile3 { get; set; }
    }
}
