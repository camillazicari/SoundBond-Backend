using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoundBond.Models;

namespace SoundBond.Data
{
    public class SoundBondDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public SoundBondDbContext(DbContextOptions<SoundBondDbContext> options) : base(options) { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<ApplicationRole> ApplicationRoles { get; set; }

        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }
        public DbSet<Generi> Generi { get; set; }
        public DbSet<Artisti> Artisti { get; set; }
        public DbSet<Brani> Brani { get; set; }
        public DbSet<Profilo> Profili { get; set; }
        public DbSet<Richiesta> Richieste { get; set; }
        public DbSet<Bonder> Bonders { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatCancellata> ChatCancellate { get; set; }
        public DbSet<Recensione> Recensioni { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUserRole>()
                .HasOne(ur => ur.ApplicationUser)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            builder.Entity<ApplicationUserRole>()
                .HasOne(ur => ur.ApplicationRole)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            builder.Entity<Generi>()
                .HasOne(g => g.User)
                .WithMany(u => u.Generi)
                .HasForeignKey(g => g.UserId);

            builder.Entity<Artisti>()
                .HasOne(a => a.User)
                .WithMany(u => u.Artisti)
                .HasForeignKey(a => a.UserId);

            builder.Entity<Brani>()
                .HasOne(b => b.User)
                .WithMany(u => u.Brani)
                .HasForeignKey(b => b.UserId);

            builder.Entity<Profilo>()
                .HasOne(p => p.User)
                .WithOne(u => u.Profilo)
                .HasForeignKey<Profilo>(p => p.UserId);

            builder.Entity<Bonder>()
                .HasIndex(b => new { b.UserId1, b.UserId2 })
                .IsUnique();

            builder.Entity<Bonder>()
                .HasOne(b => b.User1)
                .WithMany()
                .HasForeignKey(b => b.UserId1)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Bonder>()
                .HasOne(b => b.User2)
                .WithMany()
                .HasForeignKey(b => b.UserId2)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Richiesta>()
                .HasIndex(c => new { c.SenderId, c.ReceiverId })
                .IsUnique();

            builder.Entity<Richiesta>()
                .HasOne(r => r.Sender)
                .WithMany(u => u.RichiesteInviate)
                .HasForeignKey(r => r.SenderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Richiesta>()
                .HasOne(r => r.Receiver)
                .WithMany(u => u.RichiesteRicevute)
                .HasForeignKey(r => r.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Recensione>()
                .HasOne(r => r.ApplicationUser)
                .WithOne(u => u.Recensione)
                .HasForeignKey<Recensione>(r => r.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Recensione>()
                .HasIndex(r => r.ApplicationUserId)
                .IsUnique();
        }
    }
}
