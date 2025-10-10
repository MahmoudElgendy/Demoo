using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace Demo.Api.Contracts.Requests;

public class AddressRequest
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


    public Location? Location { get; set; }
}
