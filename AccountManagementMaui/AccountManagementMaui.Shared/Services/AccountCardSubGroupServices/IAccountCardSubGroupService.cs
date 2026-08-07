using AccountManagementMaui.Shared.Models.AccountCardSubGroupModels;

namespace AccountManagementMaui.Shared.Services.AccountCardSubGroupServices;

public interface IAccountCardSubGroupService
{
    Task<IReadOnlyList<AccountCardSubGroupListDto>> GetAllAsync(
        string? search = null,
        int? accountCardGroupId = null,
        CancellationToken cancellationToken = default);

    Task<AccountCardSubGroupDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<AccountCardSubGroupDetailDto> CreateAsync(
        CreateAccountCardSubGroupRequest request,
        CancellationToken cancellationToken = default);

    Task<AccountCardSubGroupDetailDto> UpdateAsync(
        int id,
        UpdateAccountCardSubGroupRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int id,
        DeleteAccountCardSubGroupRequest request,
        CancellationToken cancellationToken = default);
}