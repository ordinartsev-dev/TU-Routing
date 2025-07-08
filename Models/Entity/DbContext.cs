using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Backend.Models
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Place> Places { get; set; }
        public DbSet<Entrance> Entrances { get; set; }
        public DbSet<RoomEntity> Rooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Зв’язок Place -> Entrances (один-до-багатьох)
            modelBuilder.Entity<Place>()
                .HasMany(p => p.Entrances)
                .WithOne(e => e.Place)
                .HasForeignKey(e => e.PlaceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Для Geometry (Contour) – вказуємо тип просторових даних
            modelBuilder.Entity<Place>()
                .Property(p => p.Contour)
                .HasColumnType("geometry");

            // Для Latitude, Longitude – прості double
            modelBuilder.Entity<Entrance>()
                .Property(e => e.Latitude)
                .HasColumnType("double precision");

            modelBuilder.Entity<Entrance>()
                .Property(e => e.Longitude)
                .HasColumnType("double precision");
        }
    }
}