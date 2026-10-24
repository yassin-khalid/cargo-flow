namespace CargoFlow.Domain;

public class Address
{
    public int Id { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public List<Shipment> PickupShipments { get; set; } = [];
    public List<Shipment> DestinationShipments { get; set; } = [];
}
