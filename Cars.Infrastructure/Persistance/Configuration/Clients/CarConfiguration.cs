using Cars.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.Infrastructure.Persistance.Configuration.Client;

public class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Model)
            .HasColumnName(nameof(Car.Model))
            .HasMaxLength(20); // to samo dla kazdego pola
        
        builder.Property(x => x.Make)
            .HasColumnName(nameof(Car.Make))
            .HasMaxLength(20); // to samo dla kazdego pola
        
        builder.Property(x => x.VIN)
            .HasColumnName(nameof(Car.VIN))
            .HasMaxLength(17)
            .IsRequired(); // jak zrobic zeby moglo byc tylko 17 znakow i nic innego

        builder.HasIndex(x => x.VIN)
            .IsUnique();

        builder.Property(x => x.Year)
            .HasColumnName(nameof(Car.Year))
            .HasDefaultValue(0);
        
        builder.Property(x => x.Visits)
            .HasColumnName(nameof(Car.Visits))
            .HasDefaultValue(0);
        
        builder.Property(x => x.IsDeleted)
            .HasColumnName(nameof(Car.IsDeleted))
            .HasDefaultValue(false);

        builder.Property<int>("ClientId")
            .IsRequired();
    }
}
