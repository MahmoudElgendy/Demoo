namespace Demo.Api.Models.Common.Auditing;

public interface ITrackCreated
{
    DateTimeOffset CreatedOn { get; set; }
}