using Demo.Api.Models;

namespace Demo.Api.Contracts.Responses
{
    public class PersonResponse
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public AddressResponse Address { get; set; }

        public List<QualificationResponse> Qualifications { get; set; }
    }
}
