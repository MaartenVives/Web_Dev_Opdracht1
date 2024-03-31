namespace opdracht_1.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime OrderPlace { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<OrderDetails> OrderDetails { get; set; } = null!; // Navigatie-eigenschap naar OrderDetails toevoegen
    }
}
