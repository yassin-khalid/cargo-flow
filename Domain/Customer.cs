namespace CargoFlow.Domain;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string NationalId { get; set; }
    public List<Shipment> Shipments { get; set; }
}
