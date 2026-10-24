using CargoFlow.Data;
using CargoFlow.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<CargoFlowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CargoFlow")));

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

var app = builder.Build();

var group = app.MapGroup("/api");

group
    .MapGroup("/weight-types")
    .MapWeightTypeEndpoints();

app.Run();
