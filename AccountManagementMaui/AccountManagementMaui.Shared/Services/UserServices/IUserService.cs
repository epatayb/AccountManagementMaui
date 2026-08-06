using AccountManagementMaui.Shared.Models.UserModels;

namespace AccountManagementMaui.Shared.Services.UserServices;

public interface IUserService
{
    Task<IReadOnlyList<UserListDto>> GetAllAsync(
        string? search = null,
        CancellationToken cancellationToken = default);

    Task<UserDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<UserDetailDto> CreateAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default);

    Task<UserDetailDto> UpdateAsync(
        int id,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int id,
        DeleteUserRequest request,
        CancellationToken cancellationToken = default);
}