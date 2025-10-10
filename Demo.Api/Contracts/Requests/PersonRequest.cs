using Demo.Api.Models;

namespace Demo.Api.Contracts.Requests
{
    public class PersonRequest
    {
        public required string Name { get; set; }
        public AddressRequest Address { get; set; }

        public List<QualificationRequest> Qualifications { get; set; }
    }
}
