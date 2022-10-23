using Nop.Plugin.Orders.MultiShipping.Models;

namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public interface ITicketService
    {
        void Insert(TicketModel ticketModel);
    }
}