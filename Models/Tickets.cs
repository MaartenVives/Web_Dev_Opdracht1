using System.ComponentModel.DataAnnotations.Schema;

namespace opdracht_1.Models
{
    public class Tickets
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        [Column(TypeName ="decimal(6, 2")]
        public decimal Price { get; set; }
    }
}
