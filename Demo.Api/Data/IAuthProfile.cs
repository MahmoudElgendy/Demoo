namespace Demo.Api.Data
{
    public interface IAuthProfile
    {
        Guid? TenantId { get; }

        Guid? UserId { get; }

        IEnumerable<string> Permissions { get; }

        Guid? ImpersonatorUserId { get; }

        Guid? ResourceId { get; }

        IEnumerable<Guid> RoleIds { get; set; }
    }
}
