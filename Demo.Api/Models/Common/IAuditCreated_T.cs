namespace Demo.Api.Models.Common;

public interface IAuditCreated<T> : ITrackCreated
{
    T CreatedBy { get; set; }
}
