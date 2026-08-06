using AccountManagementMaui.Shared.Models.TaxOfficeModels;

namespace AccountManagementMaui.Shared.Services.TaxOfficeServices;

public interface ITaxOfficeService
{
    Task<IReadOnlyList<TaxOfficeListDto>> GetAllAsync(
        string? search = null,
        int? cityId = null,
        CancellationToken cancellationToken = default);

    Task<TaxOfficeDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<TaxOfficeDetailDto> CreateAsync(
        CreateTaxOfficeRequest request,
        CancellationToken cancellationToken = default);

    Task<TaxOfficeDetailDto> UpdateAsync(
        int id,
        UpdateTaxOfficeRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int id,
        DeleteTaxOfficeRequest request,
        CancellationToken cancellationToken = default);
}