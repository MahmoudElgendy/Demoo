namespace Demo.Api.Contracts.Auditing;

public interface IAuditModified<T> : ITrackModified
{
    T ModifiedBy { get; set; }
}
