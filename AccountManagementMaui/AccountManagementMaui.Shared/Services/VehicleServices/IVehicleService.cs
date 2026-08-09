using AccountManagementMaui.Shared.Models.VehicleModels;

namespace AccountManagementMaui.Shared.Services.VehicleServices
{
    public interface IVehicleService
    {
        Task<VehicleListResponse> GetAllAsync(
            string? search = null,
            int? vehicleTypeId = null,
            int? vehicleKindId = null,
            int? cityId = null,
            bool? isActive = null,
            string? sortBy = null,
            string? sortDirection = null,
            int page = 1,
            int pageSize = 25,
            CancellationToken cancellationToken = default);


        Task<VehicleDetailDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);


        Task<VehicleDetailDto> CreateAsync(
            CreateVehicleRequest request,
            CancellationToken cancellationToken = default);


        Task<VehicleDetailDto> UpdateAsync(
            int id,
            UpdateVehicleRequest request,
            CancellationToken cancellationToken = default);


        Task<VehicleDetailDto> ChangeStatusAsync(
            int id,
            ChangeVehicleStatusRequest request,
            CancellationToken cancellationToken = default);


        Task DeleteAsync(
            int id,
            DeleteVehicleRequest request,
            CancellationToken cancellationToken = default);
    }
}