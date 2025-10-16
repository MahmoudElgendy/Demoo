
namespace Demo.Api.Middlewares
{
    public class CurrentUser : ICurrentUser
    {
        public Guid? UserId { get; private set; }

        public Guid? TenantId { get; private set; }

        internal void Set(Guid? userId, Guid? tenantId)
        {
            UserId = userId;
            TenantId = tenantId;
        }
    }
}
