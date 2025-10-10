using Demo.Api.Contracts.Enums;

namespace Demo.Api.Contracts.Requests
{
    public class QualificationRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Level Level { get; set; }
    }
}
