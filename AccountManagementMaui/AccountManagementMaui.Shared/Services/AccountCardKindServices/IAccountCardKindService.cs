using AccountManagementMaui.Shared.Models.AccountCardKindModels;

namespace AccountManagementMaui.Shared.Services.AccountCardKindServices;

public interface IAccountCardKindService
{
    Task<IReadOnlyList<AccountCardKindListDto>> GetAllAsync(
        string? search = null,
        int? accountCardTypeId = null,
        CancellationToken cancellationToken = default);

    Task<AccountCardKindDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<AccountCardKindDetailDto> CreateAsync(
        CreateAccountCardKindRequest request,
        CancellationToken cancellationToken = default);

    Task<AccountCardKindDetailDto> UpdateAsync(
        int id,
        UpdateAccountCardKindRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int id,
        DeleteAccountCardKindRequest request,
        CancellationToken cancellationToken = default);
}