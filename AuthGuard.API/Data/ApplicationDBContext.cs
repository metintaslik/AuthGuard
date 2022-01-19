using AuthGuard.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthGuard.API.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(x => x.ID)
                .IsRequired()
                .HasColumnType("integer")
                .ValueGeneratedOnAdd();

                entity.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(25)
                .HasColumnType("text")
                .IsUnicode(true);

                entity.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(25)
                .HasColumnType("text")
                .IsUnicode(true);

                entity.Property(x => x.Gender)
                .IsRequired()
                .HasColumnType("integer")
                .HasComment("0: I Don't Want to Specify\n 1: Male\n 2: Female");

                entity.Property(x => x.Age)
                .IsRequired()
                .HasColumnType("integer");

                entity.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true)
                .HasColumnType("boolean");
            });
        }

        public DbSet<User> Users { get; set; } = null!;
    }
}