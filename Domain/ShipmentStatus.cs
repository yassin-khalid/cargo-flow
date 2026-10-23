namespace CargoFlow.Domain;

public class ShipmentStatus
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<ShipmentStatusHistory> ShipmentStatusHistories;
}
