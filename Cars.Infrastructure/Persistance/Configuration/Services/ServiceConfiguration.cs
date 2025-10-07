using Cars.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cars.Infrastructure.Persistance.Configuration.Services;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services", "Cars");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ServiceName)
            .HasColumnName(nameof(Service.ServiceName))
            .HasMaxLength(50)
            .IsRequired(); // to samo dla kazdego pola

        builder.Property(x => x.ServiceDescription)
            .HasColumnName(nameof(Service.ServiceDescription))
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnName(nameof(Service.Price))
            .HasPrecision(10, 2)
            .IsRequired()
            .HasDefaultValue(0.00m);

        builder.Property(x => x.ServiceDate)
            .HasColumnName(nameof(Service.ServiceDate))
            .IsRequired();

        builder.Property(x => x.CreatedOn)
            .HasColumnName("CreatedOn")
            .IsRequired();

        builder.Property(x => x.ModifiedOn)
            .HasColumnName("ModifiedOn")
            .IsRequired();

        
    }
}
