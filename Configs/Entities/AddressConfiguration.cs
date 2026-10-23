using CargoFlow.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoFlow.Configs.Entities;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder
            .HasMany(a => a.DestinationShipments)
            .WithOne(s => s.Destination)
            .HasForeignKey(s => s.DestinationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(a => a.PickupShipments)
            .WithOne(s => s.Pickup)
            .HasForeignKey(s => s.PickupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}