namespace Demo.Api.Contracts.Auditing;

public interface ITrackCreated
{
    DateTimeOffset CreatedOn { get; set; }
}