using Demo.Api.Constants;

namespace Demo.Api.Data
{
    public class Car
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public CarMake Make { get; set; }
    }
}
