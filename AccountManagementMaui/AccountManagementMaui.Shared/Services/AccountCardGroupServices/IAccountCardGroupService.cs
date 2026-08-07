using AccountManagementMaui.Shared.Models.AccountCardGroupModels;

namespace AccountManagementMaui.Shared.Services.AccountCardGroupServices;

public interface IAccountCardGroupService
{
    Task<IReadOnlyList<AccountCardGroupListDto>> GetAllAsync(
        string? search = null,
        CancellationToken cancellationToken = default);

    Task<AccountCardGroupDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<AccountCardGroupDetailDto> CreateAsync(
        CreateAccountCardGroupRequest request,
        CancellationToken cancellationToken = default);

    Task<AccountCardGroupDetailDto> UpdateAsync(
        int id,
        UpdateAccountCardGroupRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, DeleteAccountCardGroupRequest request,CancellationToken cancellationToken = default);
}
