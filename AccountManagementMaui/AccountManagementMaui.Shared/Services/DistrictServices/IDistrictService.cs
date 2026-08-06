using AccountManagementMaui.Shared.Models.DistrictModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagementMaui.Shared.Services.DistrictServices
{
    public interface IDistrictService
    {
        Task<IReadOnlyList<DistrictListDto>> GetAllAsync(
       string? search = null,
       int? cityId = null,
       CancellationToken cancellationToken = default);

        Task<DistrictDetailDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<DistrictDetailDto> CreateAsync(
            CreateDistrictRequest request,
            CancellationToken cancellationToken = default);

        Task<DistrictDetailDto> UpdateAsync(
            int id,
            UpdateDistrictRequest request,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            int id,
            DeleteDistrictRequest request,
            CancellationToken cancellationToken = default);
    }
}
