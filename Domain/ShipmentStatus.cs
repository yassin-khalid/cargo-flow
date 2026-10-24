using System.ComponentModel.DataAnnotations;

namespace CargoFlow.Domain;

public class ShipmentStatus
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public List<ShipmentStatusHistory> ShipmentStatusHistories { get; set; } = [];
    public List<Shipment> Shipments { get; set; } = [];
}
