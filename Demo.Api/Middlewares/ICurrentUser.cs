namespace Demo.Api.Middlewares
{
    public interface ICurrentUser
    {
        Guid? UserId { get; }
        Guid? TenantId { get; }
        bool HasTenant => TenantId.HasValue;
        bool HasUser => UserId.HasValue;
    }
}
