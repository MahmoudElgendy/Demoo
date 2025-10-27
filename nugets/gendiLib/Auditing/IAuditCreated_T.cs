namespace gendiLib.Auditing;

public interface IAuditCreated<T> : ITrackCreated
{
    T CreatedBy { get; set; }
}
