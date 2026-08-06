using AccountManagementMaui.Shared.Models.AccountCardTypeModels;

namespace AccountManagementMaui.Shared.Services.AccountCardTypeServices;

public interface IAccountCardTypeService
{
    Task<IReadOnlyList<AccountCardTypeListDto>> GetAllAsync(
        string? search = null,
        CancellationToken cancellationToken = default);

    Task<AccountCardTypeDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<AccountCardTypeDetailDto> CreateAsync(
        CreateAccountCardTypeRequest request,
        CancellationToken cancellationToken = default);

    Task<AccountCardTypeDetailDto> UpdateAsync(
        int id,
        UpdateAccountCardTypeRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int id,
        DeleteAccountCardTypeRequest request,
        CancellationToken cancellationToken = default);
}