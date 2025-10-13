namespace Demo.Api.Contracts.Auditing;

public interface ITrackModified
{
    DateTimeOffset ModifiedOn { get; set; }
}