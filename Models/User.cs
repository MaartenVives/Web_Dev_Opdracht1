namespace opdracht_1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsApproved { get; set; }  // Goedkeuringsstatus toegevoegd
        public RoleType Role { get; set; } // Nieuw toegevoegd voor de rol van de gebruiker

        public ICollection<Order> Orders { get; set; } = null!;// Aangepast van Order naar Orders
    }
    public enum RoleType
    {
        Gebruiker,
        Worker,
        Admin
    }
}