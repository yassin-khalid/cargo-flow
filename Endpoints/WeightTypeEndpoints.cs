using CargoFlow.Data;
using CargoFlow.Domain;
using CargoFlow.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CargoFlow.Endpoints;

public static class WeightTypeEndpoints
{
   public static RouteGroupBuilder MapWeightTypeEndpoints(this RouteGroupBuilder group)
   {
      group.MapGet("/", GetWeightTypes);
      return group;
   }

   private static async Task<Ok<List<WeightTypeDto>>> GetWeightTypes(CargoFlowDbContext db)
   {
      return TypedResults.Ok(await db.WeightTypes.Select(w =>  new WeightTypeDto()
      {
         Id = w.Id,
         Name = w.Name
      }).ToListAsync());
   }
}