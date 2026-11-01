
using Cortex_Api.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Cortex_Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options)
        : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<AccountMetaData> AccountMetaDatas { get; set; }
        public DbSet<BreachResult> BreachResults { get; set; }
        public DbSet<SecurityEvent> SecurityEvents { get; set; }
        public DbSet<Auditlog> Auditlogs { get; set; }
        public DbSet<DevicePairingAttempt> DevicePairingAttempts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<Device>()
                .HasIndex(d => d.DeviceUid)
                .IsUnique();

           
            modelBuilder.Entity<User>()
                .HasOne(u => u.Device)
                .WithOne()
                .HasForeignKey<User>(u => u.DeviceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.DeviceId)
                .IsUnique()
                .HasFilter("[DeviceId] is not null");

            modelBuilder.Entity<User>()
                .HasMany(u => u.AccountMetaDatas)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AccountMetaData>()
                .HasMany(a => a.BreachResults)
                .WithOne(b => b.AccountMetaData)
                .HasForeignKey(b => b.AccountMetaDataId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<User>()
                .HasMany(u => u.Auditlogs)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasMany(u => u.SecurityEvents)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.SetNull);

        
            modelBuilder.Entity<Device>()
                .HasMany(d => d.Auditlogs)
                .WithOne(a => a.Device)
                .HasForeignKey(a => a.DeviceId)
                .OnDelete(DeleteBehavior.SetNull);

            
            modelBuilder.Entity<Device>()
                .HasMany(d => d.SecurityEvents)
                .WithOne(e => e.Device)
                .HasForeignKey(e => e.DeviceId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<DevicePairingAttempt>()
    .HasIndex(p => p.PairingId)
    .IsUnique();

            modelBuilder.Entity<DevicePairingAttempt>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DevicePairingAttempt>()
                .HasOne(p => p.Device)
                .WithMany()
                .HasForeignKey(p => p.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

