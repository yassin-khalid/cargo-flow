using CargoFlow.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();
builder.Services.AddDbContext<CargoFlowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CargoFlow")));

var app = builder.Build();

app.Run();
