using AccountManagementMaui.Shared.Models.VehicleLookupModels;

namespace AccountManagementMaui.Shared.Services.VehicleLookupServices
{
    public interface IVehicleLookupService
    {
        Task<List<VehicleAccountLookupDto>> GetDriversAsync(
            string? search = null,
            int take = 30,
            CancellationToken cancellationToken = default);


        Task<List<VehicleAccountLookupDto>> GetAccountsAsync(
            string? search = null,
            int take = 30,
            CancellationToken cancellationToken = default);


        Task<VehicleAccountLookupDetailDto?> GetAccountByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<List<VehicleCityLookupDto>> GetCitiesAsync(
            CancellationToken cancellationToken = default);


        Task<List<VehicleTaxOfficeLookupDto>> GetTaxOfficesAsync(
            int? cityId = null,
            CancellationToken cancellationToken = default);

        Task<List<VehicleAccountLookupDto>>
            GetInvoiceAccountsAsync(
            string? search = null,
            int take = 30,
            CancellationToken cancellationToken = default);
    }
}