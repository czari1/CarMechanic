using Cars.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.Infrastructure.Persistance.Configuration.Clients;

public class ClientConfiguration : IEntityTypeConfiguration<Domain.Entities.Client>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Client> builder)
    {
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
            .HasMaxLength(40);

        builder.Property(x => x.Name)
            .HasColumnName(nameof(Domain.Entities.Client.Name))
            .HasMaxLength(40);

        builder.Property(x => x.PhoneNumber)
            .HasColumnName(nameof(Domain.Entities.Client.PhoneNumber))
            .HasMaxLength(15);
    }

}
