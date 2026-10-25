using CargoFlow.Data;
using CargoFlow.Domain;
using CargoFlow.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CargoFlow.Endpoints;

public static class CustomerEndpoints
{
    public static RouteGroupBuilder MapCustomerEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateCustomer);
        return group;
    }

    private static async Task<Created<CustomerDto>> CreateCustomer(
        CreateCustomerRequestDto customer,
        CargoFlowDbContext db)
    {
        Console.WriteLine($"ttttttttttttttttttttttttttttttt: {customer}");
        var cust = new Customer()
        {
            Name = customer.Name,
            NationalId = customer.NationalId
        };
        
        await db.Customers.AddAsync(cust);
        await db.SaveChangesAsync();
        return TypedResults.Created($"/{cust.Id}", 
            new CustomerDto(cust.Id, cust.Name, cust.NationalId));
    }
}