using System.Net;
using Ip2GeoApi.Entities;
using Ip2GeoApi.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning)
    .CreateLogger();

Log.Information("Starting Ip2GeoApi");

builder.Host.UseSerilog();

// Add database connectivity
builder.Services.AddDbContext<Ip2GeoContext>(
    options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultDatabaseConnection"),
    options => options.MapRange<IPAddress>("inetrange"))
);

builder.Services.AddTransient<Ip2GeoService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetService<Ip2GeoService>();
    context!.CheckAndSeedCountriesTable();
}

app.MapGet("ip/{ipAddress}",
    async (string ipAddress, Ip2GeoService ip2GeoService) =>
{
    var countryCode = await ip2GeoService.GetCountryCodeByIpAddress(ipAddress);
    if (countryCode.Length == 0)
    {
        return Results.NotFound("");
    }

    return Results.Ok(countryCode);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
