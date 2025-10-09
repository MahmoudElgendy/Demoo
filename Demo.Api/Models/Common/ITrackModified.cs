namespace Demo.Api.Models.Common;

public interface ITrackModified
{
    DateTimeOffset ModifiedOn { get; set; }
}