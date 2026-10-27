using System.Threading.Tasks.Dataflow;
using CargoFlow.Data;
using CargoFlow.Domain;
using CargoFlow.Dtos;
using CargoFlow.Helpers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CargoFlow.Endpoints;

public static class OfferEndpoints
{
    public static RouteGroupBuilder MapOfferEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("/{shipmentId:int}/offers", SubmitOffer)
            .WithName(EndpointRouteNames.GetShipmentOfferByIdRoute);
        group.MapGet("/{shipmentId:int}/offers/{id:int}", GetShipmentOffer);
        return group;
    }

    private static async Task<Results<Ok<OfferDto>, ProblemHttpResult>> GetShipmentOffer(int id, int shipmentId, CargoFlowDbContext db)
    {
        var isShipmentExists = await db.Shipments.AnyAsync(s => s.Id == shipmentId);
        if (!isShipmentExists)
        {
            return HttpResultProblems.NotFoundProblem("shipment_id", shipmentId);
        }

        return await db.Offers
                .Where(o => o.Id == id && o.ShipmentId == shipmentId)
                .Select(o => new OfferDto(o.Id, o.ShipmentId, o.CompanyId, o.SubmittedDate, o.Price, o.OfferStateId,
                    o.HasWon))
                .FirstOrDefaultAsync() switch
            {
                { } offer => TypedResults.Ok(offer),
                _ => HttpResultProblems.NotFoundProblem("offer_id", id)
            };
    }
    private static async Task<Results<CreatedAtRoute<OfferDto>, NotFound, ValidationProblem, ProblemHttpResult>> SubmitOffer(
       int shipmentId,
       SubmitOfferRequestBody body,
       CargoFlowDbContext db)
    {
        var isShipmentExists = await db.Shipments.AnyAsync(s => s.Id == shipmentId);
        if (!isShipmentExists)
        {
            return HttpResultProblems.NotFoundProblem("shipment_id", shipmentId);
        }

        var isCompanyExists = await db.Companies.AnyAsync(c => c.Id == body.CompanyId);
        if (!isCompanyExists)
        {
            return HttpResultProblems.NotFoundProblem("company_id", body.CompanyId);
        }

        var isNotTheFirstOffer =
           await db.Offers.AnyAsync(o => o.CompanyId == body.CompanyId && o.ShipmentId == shipmentId);
        if (isNotTheFirstOffer)
        {
            return HttpResultProblems.ValidationProblem("shipment_id",
                ["you've already a submitted an offer for this shipment"]);
        }

        var shipmentStatus = await db.Shipments
           .Where(s => s.Id == shipmentId)
           .Select(s => s.ShipmentStatus.Name)
           .FirstAsync();

        if (shipmentStatus is not "Open for Offers")
        {
            return HttpResultProblems.ValidationProblem("shipment_id", ["this shipment is not opened for offers"]);
        }

        var offerStateId = await db.OfferStates
            .Where(os => os.Name == "Submitted")
            .Select(os => os.Id)
            .FirstOrDefaultAsync();

        if (offerStateId == 0)
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError, 
                title: "Database Configuration Error", 
                detail: "Required system lookup data ('Submitted' state) is missing. Please ensure the database seeder has been executed.");
        }

        var offer = new Offer()
        {
            CompanyId = body.CompanyId,
            ShipmentId = shipmentId,
            OfferStateId = 1,
            Price = body.Price,
            SubmittedDate = DateTime.Now,
            HasWon = false
        };

        db.Offers.Add(offer);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ExceptionsHelpers.IsUniqueConstraintViolation(ex, "AK_Offers_Id_ShipmentId"))
        {
            return HttpResultProblems.ValidationProblem("shipment_id",
                ["you've already a submitted an offer for this shipment"]);
        }
        catch (DbUpdateException ex) when (ExceptionsHelpers.IsForeignKeyViolation(ex,
                                               "FK_Offers_OfferStates_OfferStateId"))
        {
            
        }

        catch (DbUpdateException ex) when (ExceptionsHelpers.IsForeignKeyViolation(ex, "FK_Offers_Companies_CompanyId"))
        {
            return HttpResultProblems.ValidationProblem("company_id", ["company_id not exists"]);
        }


        return TypedResults.CreatedAtRoute(
            routeName: EndpointRouteNames.GetShipmentOfferByIdRoute,
            value: new OfferDto(
                offer.Id, 
                offer.ShipmentId, 
                offer.CompanyId, 
                offer.SubmittedDate, 
                offer.Price,
                offer.OfferStateId, 
                offer.HasWon), 
            routeValues:  new {id = offer.Id});
    }
}
