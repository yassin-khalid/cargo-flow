using CargoFlow.Data;
using CargoFlow.Domain;
using CargoFlow.Dtos;
using CargoFlow.Helpers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CargoFlow.Endpoints;

public static class CustomerEndpoints
{
    public static RouteGroupBuilder MapCustomerEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/", CreateCustomer);
        group.MapGet("/{id:int}", GetCustomer)
            .WithName(EndpointRouteNames.GetCustomerByIdRoute);
        return group;
    }

    private static async Task<Results<CreatedAtRoute<CustomerDto>, ValidationProblem>> CreateCustomer(
        CreateCustomerRequestDto customer,
        CargoFlowDbContext db)
    {
        var cust = new Customer()
        {
            Name = customer.Name,
            NationalId = customer.NationalId
        };

        db.Customers.Add(cust);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ExceptionsHelpers.IsNationalIdUniqueConstraintViolation(ex))
        {
            return TypedResults.ValidationProblem(errors: new Dictionary<string, string[]>
            {
                ["NationalId"] = ["NationalId is taken"]
            });
        }

        return TypedResults.CreatedAtRoute(
            routeName: EndpointRouteNames.GetCustomerByIdRoute,
            routeValues: new
            {
                id = cust.Id
            },
            value: new CustomerDto(cust.Id, cust.Name, cust.NationalId)
        );
    }

    private static async Task<Results<Ok<CustomerDto>, NotFound>> GetCustomer(int id, CargoFlowDbContext db)
    {
        return await db.Customers
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CustomerDto(c.Id, c.Name, c.NationalId))
            .FirstOrDefaultAsync() switch
        {
            { } customer => TypedResults.Ok(customer),
            _ => TypedResults.NotFound()
        };
    }
}
