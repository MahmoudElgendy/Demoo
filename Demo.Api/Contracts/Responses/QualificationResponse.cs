using Demo.Api.Contracts.Enums;

namespace Demo.Api.Contracts.Responses
{
    public class QualificationResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Level Level { get; set; }
    }
}
