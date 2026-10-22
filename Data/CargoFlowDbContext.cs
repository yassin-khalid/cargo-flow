using CargoFlow.Domain;
using Microsoft.EntityFrameworkCore;

namespace CargoFlow.Data;

public class CargoFlowDbContext(DbContextOptions<CargoFlowDbContext> options) : DbContext(options)
{
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<OfferType> OfferTypes => Set<OfferType>();
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<ShipmentStatus> ShipmentStatuses => Set<ShipmentStatus>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<WeightType> WeightTypes => Set<WeightType>();
    public DbSet<ShipmentStatusHistory> ShipmentStatusHistories => Set<ShipmentStatusHistory>();
}
