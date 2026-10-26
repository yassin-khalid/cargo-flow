using CargoFlow.Data;
using CargoFlow.Domain;
using CargoFlow.Dtos;
using CargoFlow.Helpers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CargoFlow.Endpoints;

public static class ShipmentEndpoints
{
    public static RouteGroupBuilder MapShipmentEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:int}", GetShipment)
            .WithName(EndpointRouteNames.GetShipmentByIdRoute);
        group.MapPost("/", CreateShipment);
        return group;
    }

    private static async Task<Results<Ok<ShipmentDto>, NotFound>> GetShipment(int id, CargoFlowDbContext db)
    {
        return await db.Shipments
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new ShipmentDto(s.Id, s.CustomerId, s.PickupId, s.DestinationId, s.Weight, s.WeightTypeId, s.ShipmentStatusId))
            .FirstOrDefaultAsync() switch
        {
            { } dto => TypedResults.Ok(dto),
            _ => TypedResults.NotFound()
        };
    }

    private static async Task<Results<CreatedAtRoute<ShipmentDto>, ProblemHttpResult>> CreateShipment(CargoFlowDbContext db, CreateShipmentDto dto)
    {
        var shipment = new Shipment()
        {
            CustomerId = dto.CustomerId,
            DestinationId = dto.DestinationId,
            PickupId = dto.PickupId,
            WeightTypeId = dto.WeightTypeId,
            ShipmentStatusId = 1,
            Weight = dto.Weight
        };

        db.Shipments.Add(shipment);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ExceptionsHelpers.IsForeignKeyViolation(ex, "FK_Shipments_Customers_CustomerId"))
        {
            return HttpResultProblems.ValidationProblem("No customer found", "Invalid CustomerId", "customer_id", ["customer_id not exists"]);
        }
        catch (DbUpdateException ex) when (ExceptionsHelpers.IsForeignKeyViolation(ex, "FK_Shipments_Addresses_PickupId"))
        {
            return HttpResultProblems.ValidationProblem("No pickup address found", "Invalid PickupId", "pickup_id", ["pickup_id not exists"]);
        }
        catch (DbUpdateException ex) when (ExceptionsHelpers.IsForeignKeyViolation(ex, "FK_Shipments_Addresses_DestinationId"))
        {
            return HttpResultProblems.ValidationProblem("No destination address found", "Invalid DestinationId", "destination_id", ["destination_id not exists"]);
        }
        catch (DbUpdateException ex) when (ExceptionsHelpers.IsForeignKeyViolation(ex, "FK_Shipments_WeightTypes_WeightTypeId"))
        {
            return HttpResultProblems.ValidationProblem("No weight type found", "Invalid WeightTypeId", "weight_type_id", ["weight_type_id not exists"]);
        }


        return TypedResults.CreatedAtRoute(value: new ShipmentDto(shipment.Id, shipment.CustomerId, shipment.PickupId, shipment.DestinationId, shipment.Weight, shipment.WeightTypeId, shipment.ShipmentStatusId), routeName: EndpointRouteNames.GetShipmentByIdRoute, routeValues: new { id = shipment.Id });
    }
}
