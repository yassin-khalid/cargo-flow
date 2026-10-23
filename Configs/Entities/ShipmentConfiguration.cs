using CargoFlow.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoFlow.Configs.Entities;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder
            .HasMany(s => s.Offers)
            .WithOne(o => o.Shipment)
            .HasForeignKey(o => o.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(s => s.WinningOffer)
            .WithOne(o => o.Shipment)
            .HasForeignKey<Shipment>(s => s.WinningOfferId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(s => s.Customer)
            .WithMany(c => c.Shipments)
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(s => s.Pickup)
            .WithMany(a => a.PickupShipments)
            .HasForeignKey(s => s.PickupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(s => s.Destination)
            .WithMany(a => a.DestinationShipments)
            .HasForeignKey(s => s.DestinationId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}