namespace AccountManagementMaui.Api.Services.CurrentUser
{
    public interface ICurrentUserService
    {
        int? UserId { get; }

        bool IsAuthenticated { get; }
    }
}
