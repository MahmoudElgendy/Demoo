namespace Demo.Api.Contracts.Auditing;

public interface IAuditCreated<T> : ITrackCreated
{
    T CreatedBy { get; set; }
}
