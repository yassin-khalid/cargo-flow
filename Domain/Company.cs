namespace CargoFlow.Domain;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string TaxID { get; set; }
    public List<Driver> Drivers { get; set; }
    public List<Offer> Offers { get; set; }
    public List<Vehicle> Vehicles { get; set; }
}
