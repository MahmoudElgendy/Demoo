namespace Demo.Api.Models.Common.Auditing;

public interface IAuditCreated<T> : ITrackCreated
{
    T CreatedBy { get; set; }
}
