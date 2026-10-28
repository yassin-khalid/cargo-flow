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
        group.MapPost("/{shipmentId:int}/offers", SubmitOffer);
        group.MapPost("/{shipmentId:int}/offers/{offerId:int}/award", AwardOffer);
        group.MapGet("/{shipmentId:int}/offers/{id:int}", GetShipmentOffer)
            .WithName(EndpointRouteNames.GetShipmentOfferByIdRoute);
        group.MapGet("/{shipmentId:int}/offers", GetShipmentOffers);
        return group;
    }

    private static async Task<Results<Ok<List<OfferDto>>, ProblemHttpResult>> GetShipmentOffers(int shipmentId,
        CargoFlowDbContext db)
    {
        var isShipmentExists = await db.Shipments.AnyAsync(s => s.Id == shipmentId);
        if (!isShipmentExists)
        {
            return HttpResultProblems.NotFoundProblem("shipment_id", shipmentId);
        }

        return TypedResults.Ok(await db.Offers
            .Where(o => o.ShipmentId == shipmentId)
            .OrderByDescending(o => o.SubmittedDate)
            .Select(o =>
                new OfferDto(o.Id, o.ShipmentId, o.CompanyId, o.SubmittedDate, o.Price, o.OfferStateId, o.HasWon))
            .ToListAsync());
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
            return TypedResults.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Duplicate offer",
                detail: $"you've already a submitted an offer for this shipment #{shipmentId}");
        }

        var shipmentStatus = await db.Shipments
           .Where(s => s.Id == shipmentId)
           .Select(s => s.ShipmentStatus.Name)
           .FirstAsync();

        if (shipmentStatus is not "Open for Offers")
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Invalid offer",
                detail: $"shipment with id {shipmentId} is not opened for offers");
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
            OfferStateId = offerStateId,
            Price = body.Price,
            SubmittedDate = DateTime.UtcNow,
            HasWon = false
        };

        db.Offers.Add(offer);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ExceptionsHelpers.IsUniqueConstraintViolation(ex,
                                               "IX_Offers_ShipmentId_CompanyId"))
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Duplicate offer",
                detail: $"you've already a submitted an offer for this shipment #{shipmentId}");
        }
        catch (DbUpdateException ex) when (ExceptionsHelpers.IsForeignKeyViolation(ex,
                                               "FK_Offers_OfferStates_OfferStateId"))
        {
            
            return HttpResultProblems.ValidationProblem("offer_state_id", ["offer_state_id not exists"]);
        }

        catch (DbUpdateException ex) when (ExceptionsHelpers.IsForeignKeyViolation(ex, "FK_Offers_Companies_CompanyId"))
        {
            return HttpResultProblems.ValidationProblem("company_id", ["company_id not exists"]);
        }

        catch (DbUpdateException ex) when (ExceptionsHelpers.IsForeignKeyViolation(ex, "FK_Offers_Shipments_ShipmentId"))
        {
            return HttpResultProblems.ValidationProblem("shipment_id", ["shipment_id not exists"]);
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
            routeValues:  new
            {
                id = offer.Id, 
                shipmentId
            });
    }

    private static async Task<Results<NoContent, ProblemHttpResult>> AwardOffer(int shipmentId, int offerId, CargoFlowDbContext db)
    {
        var shipment = await db.Shipments.FirstOrDefaultAsync(s => s.Id == shipmentId);
        if (shipment is null)
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Shipment not found",
                detail: $"shipment #{shipmentId} not found"
            );
        }

        var openForOffersStatusId = await db.ShipmentStatuses
            .Where(ss => ss.Name == "Open for Offers")
            .Select(ss => ss.Id)
            .FirstOrDefaultAsync();

        if (openForOffersStatusId == 0)
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Database Configuration Error",
                detail: "Required system lookup data ('Open For Offers' status) is missing. Please ensure the database seeder has been executed.");
        }

        if (shipment.ShipmentStatusId != openForOffersStatusId)
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Shipment Status Issue",
                detail: $"Shipment #{shipmentId} is not open for offers"
           );
        }
        
        var offer = await db.Offers
            .FirstOrDefaultAsync(o => o.Id == offerId && o.ShipmentId == shipmentId);
        if (offer is null)
        {
            
            return TypedResults.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Offer not found",
                detail: $"Offer #{offerId} is either not exists or does not belong to shipment #{shipmentId}"
            );
        }

        var offerStateId = await db.OfferStates
            .Where(os => os.Name == "Accepted")
            .Select(os => os.Id)
            .FirstOrDefaultAsync();

        if (offerStateId == 0)
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Database Configuration Error",
                detail: "Required system lookup data ('Accepted' state) is missing. Please ensure the database seeder has been executed."
            );
        }

        var shipmentStatusId = await db.ShipmentStatuses
            .Where(ss => ss.Name == "Awarded")
            .Select(ss => ss.Id)
            .FirstOrDefaultAsync();

        if (shipmentStatusId == 0)
        {
            
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Database Configuration Error",
                detail: "Required system lookup data ('Awarded' status) is missing. Please ensure the database seeder has been executed."
            );
        }


        var rejectedOfferStateId = await db.OfferStates
            .Where(os => os.Name == "Rejected")
            .Select(os => os.Id)
            .FirstOrDefaultAsync();

        if (rejectedOfferStateId is 0)
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Database Configuration Error",
                detail: "Required system lookup data ('Rejected' state) is missing. Please ensure the database seeder has been executed."
            );
        }

        var otherOffers = await db.Offers
            .Where(o => o.Id != offerId && o.ShipmentId == shipmentId)
            .ToListAsync();

        foreach (var oo in otherOffers)
        {
            oo.OfferStateId = rejectedOfferStateId;
        }
            
        offer.OfferStateId = offerStateId;
        offer.HasWon = true;
        shipment.ShipmentStatusId = shipmentStatusId;

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ExceptionsHelpers
                                               .IsUniqueConstraintViolation(ex, "IX_Offers_ShipmentId"))
        {
            return TypedResults.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Invalid Operation",
                detail: $"shipment #{shipmentId} already has an accepted offer, can not accept offer #{offerId}");
        }

        return TypedResults.NoContent();
    }
    
}
