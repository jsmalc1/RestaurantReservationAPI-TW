using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Models;

namespace RestaurantAPI.Data
{
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RestaurantTable> Tables { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<SpecialService> SpecialServices { get; set; }
        public DbSet<ReservationService> ReservationServices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // user i role m-n
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // res i ss m-n
            modelBuilder.Entity<ReservationService>()
                .HasKey(rs => new { rs.ReservationId, rs.SpecialServiceId });

            modelBuilder.Entity<ReservationService>()
                .HasOne(rs => rs.Reservation)
                .WithMany(r => r.ReservationServices)
                .HasForeignKey(rs => rs.ReservationId);

            modelBuilder.Entity<ReservationService>()
                .HasOne(rs => rs.SpecialService)
                .WithMany(ss => ss.ReservationServices)
                .HasForeignKey(rs => rs.SpecialServiceId);

            // disable kaskadno brisanje
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Table)
                .WithMany(t => t.Reservations)
                .HasForeignKey(r => r.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SpecialService>()
                .Property(s => s.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Korisnik" }
            );
        }
    }
}