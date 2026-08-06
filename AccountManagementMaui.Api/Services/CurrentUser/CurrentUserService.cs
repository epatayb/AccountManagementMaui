using System.Security.Claims;

namespace AccountManagementMaui.Api.Services.CurrentUser
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var userIdValue = _httpContextAccessor
                    .HttpContext?
                    .User
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                return int.TryParse(userIdValue, out var userId)
                    ? userId
                    : null;
            }
        }

        public bool IsAuthenticated =>
            _httpContextAccessor
                .HttpContext?
                .User
                .Identity?
                .IsAuthenticated == true;
    }
}
