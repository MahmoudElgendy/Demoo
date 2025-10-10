namespace Demo.Api.Models.Common.Auditing;

public interface IAuditModified<T> : ITrackModified
{
    T ModifiedBy { get; set; }
}
