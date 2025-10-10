namespace Demo.Api.Models.Common.Auditing;

public interface ITrackModified
{
    DateTimeOffset ModifiedOn { get; set; }
}