using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
    public class AppDbContext : DbContext {
        // Constructor that passes options to the base DbContext
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Define a DbSet for each table you want to create
        public DbSet<User> Users { get; set; }  // This will map to a "Users" table
    }
}