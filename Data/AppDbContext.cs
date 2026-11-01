using Cortex_Api.Data.Models;
using Microsoft.EntityFrameworkCore;

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

            // Device: نفس الجهاز الفعلي ما يتسجلش مرتين
            modelBuilder.Entity<Device>()
                .HasIndex(d => d.DeviceUid)
                .IsUnique();

            // User (0..1) ←→ Device: الـ FK في Device، وكل جهاز له صاحب
            // الـ WithOne مع HasForeignKey بينشئ Unique Index على UserId تلقائيًا
            modelBuilder.Entity<Device>()
                .HasOne(d => d.user)
                .WithOne(u => u.device)
                .HasForeignKey<Device>(d => d.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // لما الجهاز يتمسح، الـ logs تفضل وعمود DeviceId فيها يبقى null
            modelBuilder.Entity<Auditlog>()
                .HasOne(a => a.device)
                .WithMany(d => d.Auditlogs)
                .HasForeignKey(a => a.DeviceId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<SecurityEvent>()
                .HasOne(s => s.device)
                .WithMany(d => d.SecurityEvents)
                .HasForeignKey(s => s.DeviceId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasMany(u => u.AccountMetaDatas)
                .WithOne(a => a.user)
                .HasForeignKey(a => a.UserId)
                .IsRequired();
        }
    }
}
