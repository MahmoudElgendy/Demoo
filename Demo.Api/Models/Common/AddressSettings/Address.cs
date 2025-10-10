using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Demo.Api.Models.Common.AddressSettings;

[Owned]
public class Address
{

    [MaxLength(46)]
    public string? AddressLine1 { get; set; }
    [MaxLength(46)]
    public string? AddressLine2 { get; set; }
    [MaxLength(50)]
    public string? City { get; set; }
    [MaxLength(50)]
    public string? County { get; set; }
    [MaxLength(3)]
    public string? RegionCode { get; set; }
    [MaxLength(16)]
    public string? PostalCode { get; set; }
    [MaxLength(3)]
    public string? CountryCode { get; set; } = "USA";
    public Point? Location { get; set; }

    [return: NotNullIfNotNull("request")]
    public static Address? FromRequest(AddressRequest? request)
    {
        if (request == null)
        {
            return null;
        }

        return new Address
        {
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            County = request.County,
            RegionCode = request.RegionCode,
            PostalCode = request.PostalCode,
            CountryCode = request.CountryCode,
            Location = request.Location
        };
    }

    [return: NotNullIfNotNull("response")]
    public static Address? FromResponse(AddressResponse? response)
    {
        if (response == null)
        {
            return null;
        }

        return new Address
        {
            AddressLine1 = response.AddressLine1,
            AddressLine2 = response.AddressLine2,
            City = response.City,
            County = response.County,
            RegionCode = response.RegionCode,
            PostalCode = response.PostalCode,
            CountryCode = response.CountryCode,
            Location = response.Location
        };
    }
    [return: NotNullIfNotNull(nameof(address))]
    public static AddressResponse? ToResponse( Address? address)
    {
        if (address == null)
        {
            return null;
        }
        return new AddressResponse
        {
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            City = address.City,
            County = address.County,
            RegionCode = address.RegionCode,
            PostalCode = address.PostalCode,
            CountryCode = address.CountryCode,
            Location = address.Location,
        };
    }

    [return: NotNullIfNotNull("request")]
    public static implicit operator Address?(AddressRequest? request)
    {
        return FromRequest(request);
    }

    [return: NotNullIfNotNull("response")]
    public static implicit operator Address?(AddressResponse? response)
    {
        return FromResponse(response);
    }

    [return: NotNullIfNotNull("address")]
    public static implicit operator AddressResponse?(Address? address)
    {
        return ToResponse(address);
    }
}