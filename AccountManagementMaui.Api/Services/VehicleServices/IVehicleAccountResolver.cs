using AccountManagementMaui.Api.Entities;
using AccountManagementMaui.Api.Models.VehicleModels;

namespace AccountManagementMaui.Api.Services.VehicleServices
{
    public interface IVehicleAccountResolver
    {
        Task<AccountCard?> ResolveDriverAsync(
            int? selectedAccountId,
            VehicleAccountInputDto? input,
            CancellationToken cancellationToken = default);


        Task<AccountCard?> ResolveReferenceAsync(
            int? selectedAccountId,
            VehicleAccountInputDto? input,
            CancellationToken cancellationToken = default);


        Task<AccountCard?> ResolveLicenseAsync(
            int? selectedAccountId,
            VehicleAccountInputDto? input,
            CancellationToken cancellationToken = default);


        Task<AccountCard?> ResolveInvoiceAsync(
            int? selectedAccountId,
            VehicleAccountInputDto? input,
            CancellationToken cancellationToken = default);
    }
}