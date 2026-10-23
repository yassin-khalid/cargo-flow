using CargoFlow.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoFlow.Configs.Entities;

public class OfferStateConfiguration : IEntityTypeConfiguration<OfferState>
{
    public void Configure(EntityTypeBuilder<OfferState> builder)
    {
        builder
            .HasMany(s => s.Offers)
            .WithOne(o => o.OfferState)
            .HasForeignKey(o => o.OfferStateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}