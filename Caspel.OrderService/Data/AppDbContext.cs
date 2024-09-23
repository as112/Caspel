using Caspel.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Caspel.OrderService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
    }
}
