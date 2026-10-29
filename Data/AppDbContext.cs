using Cortex_Api.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Cortex_Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<AccountMetaData> AccountMetaDatas { get; set; }
        public DbSet<BreachResult> BreachResults { get; set; }
        public DbSet<SecurityEvent> SecurityEvents { get; set; }
        public DbSet<Auditlog> Auditlogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<Device>().HasIndex(e=>e.DeviceUid).IsUnique();
            modelBuilder.Entity<User>()
                                   .HasOne(u => u.device)
                                   .WithOne()
                                   .HasForeignKey<User>(u => u.DeviceId)
                                   .IsRequired(false);

            modelBuilder.Entity<User>()
                                 .HasIndex(u => u.DeviceId)
                                 .IsUnique()
                                 .HasFilter("[DeviceId] IS NOT NULL");
            modelBuilder.Entity<User>()
    .HasMany(u => u.AccountMetaDatas)
    .WithOne(a => a.user)
    .HasForeignKey(a => a.UserId)
    .IsRequired();
        }
    }

}
