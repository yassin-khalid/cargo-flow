using CargoFlow.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoFlow.Configs.Entities;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder
            .HasOne(v => v.Company)
            .WithMany(c => c.Vehicles)
            .HasForeignKey(v => v.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(v => v.WeightType)
            .WithMany(w => w.Vehicles)
            .HasForeignKey(v => v.WeightTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}