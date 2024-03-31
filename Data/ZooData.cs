using Microsoft.EntityFrameworkCore;
using opdracht_1.Models;

namespace opdracht_1.Data
{
    public class ZooData : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Tickets> Tickets { get; set; } = null!;
        public DbSet<OrderDetails> OrderDetails { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!; // Nieuw toegevoegd

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ZooData;Integrated Security=True;");
        }
    }
}