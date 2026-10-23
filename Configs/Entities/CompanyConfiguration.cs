using CargoFlow.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CargoFlow.Configs.Entities;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder
            .HasMany(c => c.Offers)
            .WithOne(o => o.Company)
            .HasForeignKey(o => o.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(c => c.Drivers)
            .WithOne(d => d.Company)
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(c => c.Vehicles)
            .WithOne(v => v.Company)
            .HasForeignKey(v => v.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.Name).IsUnique();
        builder.HasIndex(c => c.TaxID).IsUnique();

        builder.Property(c => c.Name).HasMaxLength(40).IsRequired();
        builder.Property(c => c.TaxID).HasMaxLength(10).IsRequired();
    }
}