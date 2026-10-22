namespace CargoFlow.Domain;

public class Offer
{
    public int Id { get; set; }
    public int ShipmentId { get; set; }
    public Shipment Shipment { get; set; }
    public int CompanyId { get; set; }
    public Company Company { get; set; }
    public DateTime SubmittedDate { get; set; }
    public decimal Price { get; set; }
    public OfferType Type { get; set; }
}
