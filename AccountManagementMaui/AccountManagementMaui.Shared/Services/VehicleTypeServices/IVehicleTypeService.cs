using AccountManagementMaui.Shared.Models.VehicleTypeModels;

namespace AccountManagementMaui.Shared.Services.VehicleTypeServices
{
    public interface IVehicleTypeService
    {
        Task<List<VehicleTypeListDto>> GetAllAsync(
            string? search = null,
            CancellationToken cancellationToken = default);

        Task<VehicleTypeDetailDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<VehicleTypeDetailDto> CreateAsync(
            CreateVehicleTypeRequest request,
            CancellationToken cancellationToken = default);

        Task<VehicleTypeDetailDto> UpdateAsync(
            int id,
            UpdateVehicleTypeRequest request,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            int id,
            DeleteVehicleTypeRequest request,
            CancellationToken cancellationToken = default);
    }
}