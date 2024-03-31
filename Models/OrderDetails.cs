using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace opdracht_1.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public int TicketId { get; set; }
        public Order Order { get; set; } = null!;
        public Tickets Ticket { get; set; } = null!;

    }
}