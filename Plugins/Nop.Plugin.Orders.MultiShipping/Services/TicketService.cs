using Nop.Core.Data;
using Nop.Plugin.Misc.Ticket.Domain;
using Nop.Plugin.Orders.MultiShipping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public class TicketService : ITicketService
    {
        private readonly IRepository<Tbl_Ticket> _repository_Ticket;

        public TicketService(IRepository<Tbl_Ticket> repository_Ticket )
        {
            _repository_Ticket = repository_Ticket;
        }

        public void Insert(TicketModel ticketModel)
        {
            var ticket = new Tbl_Ticket()
            {
                DateInsert = ticketModel.DateInsert,
                DateStaffAccept = ticketModel.DateStaffAccept,
                DateStaffLastAnswer = ticketModel.DateStaffLastAnswer,
                DateUpdate = ticketModel.DateUpdate,
                DepartmentId = ticketModel.DepartmentId,
                IdCategoryTicket = ticketModel.IdCategoryTicket,
                IdCustomer = ticketModel.IdCustomer,
                IdUserUpdate = ticketModel.IdUserUpdate,
                IsActive = ticketModel.IsActive,
                Issue = ticketModel.Issue,
                OrderCode = ticketModel.OrderCode,
                ProrityId = ticketModel.ProrityId,
                ShowCustomer = ticketModel.ShowCustomer,
                StaffIdAccept = ticketModel.StaffIdAccept,
                StaffIdLastAnswer = ticketModel.StaffIdLastAnswer,
                Status = ticketModel.Status,
                StoreId = ticketModel.StoreId,
                ticket_Details = ticketModel.ticket_Details.Select(p => new Tbl_Ticket_Detail()
                {
                    DateInsert = p.DateInsert,
                    Description = p.Description,
                    IdTicket = p.IdTicket,
                    StaffId = p.StaffId,
                    Type = p.Type,
                    UrlFile1 = p.UrlFile1,
                    UrlFile2 = p.UrlFile2,
                    UrlFile3 = p.UrlFile3
                }).ToList(),
                TrackingCode = ticketModel.TrackingCode,
                TypeTicket = ticketModel.TypeTicket
            };
            _repository_Ticket.Insert(ticket);
            ticketModel.Id = ticket.Id;
        }
    }
}
