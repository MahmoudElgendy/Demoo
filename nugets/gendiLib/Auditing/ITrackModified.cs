namespace gendiLib.Auditing;

public interface ITrackModified
{
    DateTimeOffset ModifiedOn { get; set; }
}