using System.Net;
using Microsoft.EntityFrameworkCore;
using Ip2GeoApi.Entities;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using NpgsqlTypes;

namespace Ip2GeoApi.Services;
public class Ip2GeoService
{
    private readonly Ip2GeoContext _context;
    private readonly ILogger<Ip2GeoService> _logger;
    public Ip2GeoService(Ip2GeoContext context, ILogger<Ip2GeoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public void CheckAndSeedCountriesTable()
    {
        var recordsCount = _context.Countries.Count();
        _logger.LogInformation($"Table has {recordsCount} records");
        if (recordsCount == 0)
        {
            InsertCountriesFromCsv();
        }
    }

    public async Task<string> GetCountryCodeByIpAddress(string ipAddressString)
    {
        ipAddressString = ipAddressString.Trim();
        _logger.LogInformation($"Getting country code for ${ipAddressString}");

        _logger.LogDebug($"Converting string ${ipAddressString} to ip address");
        IPAddress? ipAddress;
        if (!IPAddress.TryParse(ipAddressString, out ipAddress))
        {
            _logger.LogWarning($"Ip address {ipAddressString} is invalid");
            return "";
        }

        var countryCode = await _context.Countries
            .Where(x => x.IpRange.Contains(ipAddress))
            .Select(x => x.CountryCode)
            .FirstOrDefaultAsync();
        return countryCode ?? "";
    }

    private void InsertCountriesFromCsv()
    {
        _logger.LogInformation("Reading ip range - country values from csv file");
        var records = new List<Country>();
        var recordsCounter = 0;
        var BatchSize = 100000;

        using (var reader = new StreamReader("Resources\\dbip-country-lite.csv"))
        using (var csv = new CsvReader(reader,
            new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false }
            ))
        {

            while (csv.Read())
            {
                recordsCounter++;
                var startIp = IPAddress.Parse(csv.GetField<string>(0)!);
                var endIp = IPAddress.Parse(csv.GetField<string>(1)!);
                var country = csv.GetField<string>(2)!;

                records.Add(new Country
                {
                    Id = recordsCounter,
                    IpRange = new NpgsqlRange<IPAddress>(startIp, endIp),
                    CountryCode = country
                });

                if (recordsCounter % BatchSize == 0)
                {
                    _logger.LogInformation($"Inserting batch of {BatchSize} records");
                    _context.Countries.AddRange(records);
                    _context.SaveChanges();
                    records = new List<Country>();
                }
            }

            if (records.Count > 0)
            {
                _logger.LogInformation($"Inserting batch of {records.Count} records");
                _context.Countries.AddRange(records);
                _context.SaveChanges();
                _logger.LogInformation($"Inserted {recordsCounter} records in total");
            }
        }
    }
}
