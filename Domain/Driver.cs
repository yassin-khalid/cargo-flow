namespace CargoFlow.Domain;

public class Driver
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string Phone { get; set; }
    public int CompanyId { get; set; }
    public Company Company { get; set; }
}
