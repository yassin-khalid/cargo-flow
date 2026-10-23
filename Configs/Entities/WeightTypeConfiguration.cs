using CargoFlow.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoFlow.Configs.Entities;

public class WeightTypeConfiguration : IEntityTypeConfiguration<WeightType>
{
    public void Configure(EntityTypeBuilder<WeightType> builder)
    {
        var weightTypes = new List<WeightType>()
        {
            new() { Id = 1, Name = "Kilogram (kg)" },
            new() { Id = 2, Name = "Ton (t)" }
        };
        builder.HasData(weightTypes);
    }
}