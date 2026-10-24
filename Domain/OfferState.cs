using System.ComponentModel.DataAnnotations;

namespace CargoFlow.Domain;

public class OfferState
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public List<Offer> Offers { get; set; } = [];
}
