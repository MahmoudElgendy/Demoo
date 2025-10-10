namespace Demo.Api.Models.Common.AddressSettings
{
    public class AddressResponse
    {
        public string? AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        public string? City { get; set; }

        public string? County { get; set; }

        public string? RegionCode { get; set; }

        public string? PostalCode { get; set; }

        public string? CountryCode { get; set; } = "USA";

        public Location? Location { get; set; }
    }
}
