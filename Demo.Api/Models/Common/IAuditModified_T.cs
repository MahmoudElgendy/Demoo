namespace Demo.Api.Models.Common;

public interface IAuditModified<T> : ITrackModified
{
    T ModifiedBy { get; set; }
}
