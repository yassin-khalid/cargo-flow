using CargoFlow.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoFlow.Configs.Entities;

public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder
            .HasOne(o => o.Shipment)
            .WithMany(s => s.Offers)
            .HasForeignKey(o => o.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(o => o.Company)
            .WithMany(c => c.Offers)
            .HasForeignKey(o => o.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}