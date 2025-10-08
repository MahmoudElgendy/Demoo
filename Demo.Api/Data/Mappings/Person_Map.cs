using Demo.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Demo.Api.Data.Mappings
{
    public class Person_Map : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.OwnsMany(p => p.Qualifications, q =>
                  {
                      q.ToJson();
                      q.Property(l => l.Level)
                      .HasConversion<string>();
                  });
        }
    }
}
