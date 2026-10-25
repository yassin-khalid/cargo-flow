using CargoFlow.Data;
using CargoFlow.Domain;
using CargoFlow.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CargoFlow.Endpoints;

public static class CustomerEndpoints
{
    public static RouteGroupBuilder MapCustomerEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateCustomer);
        group.MapGet("/{id}", GetCustomer);
        return group;
    }

    private static async Task<Results<Created<CustomerDto>, BadRequest<string>>> CreateCustomer(
        CreateCustomerRequestDto customer,
        CargoFlowDbContext db)
    {
        var isNationalIdExists = await db.Customers
            .AsNoTracking()
            .AnyAsync(c => c.NationalId == customer.NationalId);

        if (isNationalIdExists)
        {
            return TypedResults.BadRequest("NationalId is taken");
        }

        var cust = new Customer()
        {
            Name = customer.Name,
            NationalId = customer.NationalId
        };

        db.Customers.Add(cust);
        await db.SaveChangesAsync();
        return TypedResults.Created($"/api/customers/{cust.Id}",
            new CustomerDto(cust.Id, cust.Name, cust.NationalId));
    }

    private static async Task<Results<Ok<CustomerDto>, NotFound>> GetCustomer(int id, CargoFlowDbContext db)
    {
        return await db.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id) switch
        {
            { } customer => TypedResults.Ok(new CustomerDto(customer.Id, customer.Name, customer.NationalId)),
            _ => TypedResults.NotFound()
        };
    }
}
