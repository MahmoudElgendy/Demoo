using Demo.Api.Models.Common.AddressSettings;
using Demo.Api.Models.Common.Auditing;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Demo.Api.Models
{
    public class Person : BaseModel<Guid>
    {
        public required string Name { get; set; }
        public Address Address { get; set; }

        public List<Qualification> Qualifications { get; set; }
    }

    public class Qualification
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Level Level { get; set; }
    }

    public enum Level
    {
        Beginner,
        Intermediate,
        Advanced,
        Expert
    }
}
