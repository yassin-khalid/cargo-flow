namespace CargoFlow.Domain;

public class Vehicle
{
    public int Id { get; set; }
    public string InspectionId { get; set; }
    public string InsuranceId { get; set; }
    public DateOnly InspectionIdExpiryDate { get; set; }
    public DateOnly InsuranceIdExpiryDate { get; set; }
}
