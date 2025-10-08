using Demo.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Demo.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Person> Persons { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .OwnsMany(p => p.Qualifications, q =>
                {
                    q.ToJson();
                    q.Property(l=>l.Level)
                    .HasConversion<string>();
                });

        }
    }
}
