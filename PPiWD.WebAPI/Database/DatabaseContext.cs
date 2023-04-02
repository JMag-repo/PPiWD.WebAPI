using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using PPiWD.WebAPI.Models;
using PPiWD.WebAPI.Models.Authentication;

namespace PPiWD.WebAPI.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        
    }
    
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }
}