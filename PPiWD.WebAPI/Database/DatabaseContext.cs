using Microsoft.EntityFrameworkCore;
using PPiWD.WebAPI.Models.Authentication;
using PPiWD.WebAPI.Models.Measurements;

namespace PPiWD.WebAPI.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<Measurement> Measurements { get; set; }
    public DbSet<SensorData> SensorDatas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Measurement>()
            .HasOne<User>(s => s.User)
            .WithMany(s => s.Measurements)
            .HasForeignKey(x => x.UserId);

        modelBuilder.Entity<Measurement>()
            .HasMany<SensorData>(x => x.SensorDatas)
            .WithOne(x => x.Measurement)
            .HasForeignKey(x => x.MeasurementId);
    }
}