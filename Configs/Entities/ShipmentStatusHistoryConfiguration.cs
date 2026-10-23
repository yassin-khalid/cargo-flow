using CargoFlow.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoFlow.Configs.Entities;

public class ShipmentStatusHistoryConfiguration : IEntityTypeConfiguration<ShipmentStatusHistory>
{
    public void Configure(EntityTypeBuilder<ShipmentStatusHistory> builder)
    {
        builder
            .HasOne(h => h.Shipment)
            .WithMany(s => s.ShipmentStatusHistories)
            .HasForeignKey(h => h.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(h => h.ShipmentStatus)
            .WithMany(s => s.ShipmentStatusHistories)
            .HasForeignKey(h => h.ShipmentStatusId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}