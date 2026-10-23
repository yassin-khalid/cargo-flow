using CargoFlow.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoFlow.Configs.Entities;

public class OfferStateConfiguration : IEntityTypeConfiguration<OfferState>
{
    public void Configure(EntityTypeBuilder<OfferState> builder)
    {
        var offerStates = new List<OfferState>()
        {
            new() { Id = 1, Name = "Submitted" },
            new() { Id = 2, Name = "Under Review" },
            new() { Id = 3, Name = "Accepted" },
            new() { Id = 4, Name = "Rejected" },
            new() { Id = 5, Name = "Expired" },
            new() { Id = 6, Name = "Withdrawn" }
        };

        builder.HasData(offerStates);
        
        builder
            .HasMany(s => s.Offers)
            .WithOne(o => o.OfferState)
            .HasForeignKey(o => o.OfferStateId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}