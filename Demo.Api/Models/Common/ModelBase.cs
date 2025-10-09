namespace Demo.Api.Models.Common;
public class BaseModel<T> : IBaseIdModel<T>, IAuditCreated<Guid>, IAuditModified<Guid>
{
    public required T Id { get; set; }
    public bool IsDeleted { get; set; }
    public Guid TenantId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public Guid ModifiedBy { get; set; }
    public DateTimeOffset ModifiedOn { get; set; }
}
