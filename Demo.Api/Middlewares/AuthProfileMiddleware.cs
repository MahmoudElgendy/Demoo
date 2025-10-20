using System.Security.Claims;

namespace Demo.Api.Middlewares
{
    public class AuthProfileMiddleware
    {
        private readonly RequestDelegate _next;
        private const string TenantHeader = "TenantId";
        private const string UserHeader = "UserId";
        public AuthProfileMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, CurrentUser currentUser)
        {
            // Read header values
            Guid? tenantId = null;
            Guid? userId = null;

            if (context.Request.Headers.TryGetValue(TenantHeader, out var tenantValues))
            {
                if (Guid.TryParse(tenantValues.FirstOrDefault(), out var t))
                    tenantId = t;
            }

            if (context.Request.Headers.TryGetValue(UserHeader, out var userValues))
            {
                if (Guid.TryParse(userValues.FirstOrDefault(), out var u))
                    userId = u;
            }

            // set CurrentUser
            currentUser.Set(userId, tenantId);

            if (userId.HasValue || tenantId.HasValue)
            {
                var claims = new List<Claim>();
                if (userId.HasValue)
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.ToString()!));

            }
            await _next(context);
        }
    }
}
