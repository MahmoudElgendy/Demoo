namespace gendiLib.Auditing;

public interface IAuditModified<T> : ITrackModified
{
    T ModifiedBy { get; set; }
}
