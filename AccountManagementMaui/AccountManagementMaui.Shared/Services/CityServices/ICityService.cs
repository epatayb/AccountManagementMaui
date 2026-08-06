using AccountManagementMaui.Shared.Models.CityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagementMaui.Shared.Services.CityServices
{
    public interface ICityService
    {
        Task<IReadOnlyList<CityListDto>> GetAllAsync(
        string? search = null,
        CancellationToken cancellationToken = default);

        Task<CityDetailDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<CityDetailDto> CreateAsync(
            CreateCityRequest request,
            CancellationToken cancellationToken = default);

        Task<CityDetailDto> UpdateAsync(
            int id,
            UpdateCityRequest request,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            int id,
            DeleteCityRequest request,
            CancellationToken cancellationToken = default);
    }
}
