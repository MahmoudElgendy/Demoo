using Demo.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Runtime.InteropServices;

namespace Demo.Api.Mappings
{
    public class Person_Map : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.OwnsOne(h => h.Address, a =>
            {
                a.Property(x => x.Location).HasColumnType("geography");
            });
        }
    }
}
