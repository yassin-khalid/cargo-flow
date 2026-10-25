using System.ComponentModel.DataAnnotations;

namespace CargoFlow.Dtos;

    public record CustomerDto(
        int Id, 
        string Name,
        string NationalId
        );

    public record CreateCustomerRequestDto(
        [Required]
        [MaxLength(25, ErrorMessage = "Name must have 25 characters maximum")]
        string Name, 
        [Required]
        [MaxLength(10, ErrorMessage = "NationalId must have 10 characters maximum")]
        string NationalId
        );
