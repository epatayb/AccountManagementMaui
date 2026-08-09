using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManagementMaui.Shared.Models.VehicleKindModels;

namespace AccountManagementMaui.Shared.Services.VehicleKindServices
{
    public interface IVehicleKindService
    {
        Task<List<VehicleKindListDto>> GetAllAsync(
            string? search = null,
            CancellationToken cancellationToken = default);

        Task<VehicleKindDetailDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<VehicleKindDetailDto> CreateAsync(
            CreateVehicleKindRequest request,
            CancellationToken cancellationToken = default);

        Task<VehicleKindDetailDto> UpdateAsync(
            int id,
            UpdateVehicleKindRequest request,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            int id,
            DeleteVehicleKindRequest request,
            CancellationToken cancellationToken = default);
    }
}
