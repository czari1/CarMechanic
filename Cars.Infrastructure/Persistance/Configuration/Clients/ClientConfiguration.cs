using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cars.Infrastructure.Persistance.Configuration.Clients;

public class ClientConfiguration : IEntityTypeConfiguration<Domain.Entities.Client>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Client> builder)
    {
        builder.ToTable("Clients", "Cars");

        builder.HasKey(x => x.Id);

        builder.Navigation(x => x.Cars)
            .AutoInclude();

        builder.HasMany(x => x.Cars)
          .WithOne()
          .HasForeignKey("ClientId")
          .IsRequired()
          .OnDelete(DeleteBehavior.Cascade);

        //Reszta kolumn
        builder.Property(x => x.Surname)
            .HasColumnName(nameof(Domain.Entities.Client.Surname))
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName(nameof(Domain.Entities.Client.Name))
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(x => x.PhoneNumber)
            .HasColumnName(nameof(Domain.Entities.Client.PhoneNumber))
            .HasMaxLength(15)
            .IsRequired(false);

        builder.Property(x => x.CreatedOn)
            .HasColumnName("CreatedOn")
            .IsRequired();

        builder.Property(x => x.ModifiedOn)
            .HasColumnName("ModifiedOn")
            .IsRequired();
    }
}
