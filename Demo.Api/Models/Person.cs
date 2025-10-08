namespace Demo.Api.Models
{
    public class Person
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }

        public required List<Qualification> Qualifications { get; set; }
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
