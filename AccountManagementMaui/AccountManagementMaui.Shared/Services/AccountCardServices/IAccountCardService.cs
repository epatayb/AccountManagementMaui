using AccountManagementMaui.Shared.Models.AccountCardModels;

namespace AccountManagementMaui.Shared.Services.AccountCardServices;

public interface IAccountCardService
{
    Task<IReadOnlyList<AccountCardListDto>> GetAllAsync(
        string? search = null,
        int? accountCardTypeId = null,
        int? accountCardKindId = null,
        int? accountCardGroupId = null,
        int? accountCardSubGroupId = null,
        int? cityId = null,
        CancellationToken cancellationToken = default);

    Task<AccountCardDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<AccountCardDetailDto> CreateAsync(
        CreateAccountCardRequest request,
        CancellationToken cancellationToken = default);

    Task<AccountCardDetailDto> UpdateAsync(
        int id,
        UpdateAccountCardRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int id,
        DeleteAccountCardRequest request,
        CancellationToken cancellationToken = default);
}