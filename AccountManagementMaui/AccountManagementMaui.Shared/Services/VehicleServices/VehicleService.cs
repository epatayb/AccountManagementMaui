using System.Net.Http.Json;

using AccountManagementMaui.Shared.Models.Common;
using AccountManagementMaui.Shared.Models.VehicleModels;
using AccountManagementMaui.Shared.Services.Common;

namespace AccountManagementMaui.Shared.Services.VehicleServices
{
    public class VehicleService : IVehicleService
    {
        private readonly HttpClient _httpClient;


        public VehicleService(
            HttpClient httpClient)
        {
            _httpClient =
                httpClient;
        }


        // =====================================================
        // LIST
        // =====================================================

        public async Task<VehicleListResponse> GetAllAsync(
            string? search = null,
            int? vehicleTypeId = null,
            int? vehicleKindId = null,
            int? cityId = null,
            bool? isActive = null,
            string? sortBy = null,
            string? sortDirection = null,
            int page = 1,
            int pageSize = 25,
            CancellationToken cancellationToken = default)
        {
            var queryParameters =
                new List<string>
                {
                    $"page={page}",
                    $"pageSize={pageSize}"
                };


            if (!string.IsNullOrWhiteSpace(search))
            {
                queryParameters.Add(
                    "search=" +
                    Uri.EscapeDataString(
                        search.Trim()));
            }


            if (vehicleTypeId.HasValue &&
                vehicleTypeId.Value > 0)
            {
                queryParameters.Add(
                    $"vehicleTypeId={vehicleTypeId.Value}");
            }


            if (vehicleKindId.HasValue &&
                vehicleKindId.Value > 0)
            {
                queryParameters.Add(
                    $"vehicleKindId={vehicleKindId.Value}");
            }


            if (cityId.HasValue &&
                cityId.Value > 0)
            {
                queryParameters.Add(
                    $"cityId={cityId.Value}");
            }


            if (isActive.HasValue)
            {
                queryParameters.Add(
                    $"isActive={isActive.Value.ToString().ToLowerInvariant()}");
            }


            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                queryParameters.Add(
                    "sortBy=" +
                    Uri.EscapeDataString(
                        sortBy.Trim()));
            }


            if (!string.IsNullOrWhiteSpace(sortDirection))
            {
                queryParameters.Add(
                    "sortDirection=" +
                    Uri.EscapeDataString(
                        sortDirection.Trim()));
            }


            var url =
                "api/vehicles?" +
                string.Join(
                    "&",
                    queryParameters);


            using var response =
                await _httpClient.GetAsync(
                    url,
                    cancellationToken);


            await EnsureSuccessAsync(
                response,
                cancellationToken);


            return await response.Content
                       .ReadFromJsonAsync<VehicleListResponse>(
                           cancellationToken:
                               cancellationToken)
                   ?? new VehicleListResponse
                   {
                       Page = page,
                       PageSize = pageSize
                   };
        }


        // =====================================================
        // DETAIL
        // =====================================================

        public async Task<VehicleDetailDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            using var response =
                await _httpClient.GetAsync(
                    $"api/vehicles/{id}",
                    cancellationToken);


            await EnsureSuccessAsync(
                response,
                cancellationToken);


            return await response.Content
                .ReadFromJsonAsync<VehicleDetailDto>(
                    cancellationToken:
                        cancellationToken);
        }


        // =====================================================
        // CREATE
        // =====================================================

        public async Task<VehicleDetailDto> CreateAsync(
            CreateVehicleRequest request,
            CancellationToken cancellationToken = default)
        {
            using var response =
                await _httpClient.PostAsJsonAsync(
                    "api/vehicles",
                    request,
                    cancellationToken);


            await EnsureSuccessAsync(
                response,
                cancellationToken);


            return await ReadVehicleAsync(
                response,
                cancellationToken);
        }


        // =====================================================
        // UPDATE
        // =====================================================

        public async Task<VehicleDetailDto> UpdateAsync(
            int id,
            UpdateVehicleRequest request,
            CancellationToken cancellationToken = default)
        {
            using var response =
                await _httpClient.PutAsJsonAsync(
                    $"api/vehicles/{id}",
                    request,
                    cancellationToken);


            await EnsureSuccessAsync(
                response,
                cancellationToken);


            return await ReadVehicleAsync(
                response,
                cancellationToken);
        }


        // =====================================================
        // STATUS
        // =====================================================

        public async Task<VehicleDetailDto> ChangeStatusAsync(
            int id,
            ChangeVehicleStatusRequest request,
            CancellationToken cancellationToken = default)
        {
            using var message =
                new HttpRequestMessage(
                    HttpMethod.Patch,
                    $"api/vehicles/{id}/status")
                {
                    Content =
                        JsonContent.Create(request)
                };


            using var response =
                await _httpClient.SendAsync(
                    message,
                    cancellationToken);


            await EnsureSuccessAsync(
                response,
                cancellationToken);


            return await ReadVehicleAsync(
                response,
                cancellationToken);
        }


        // =====================================================
        // DELETE
        // =====================================================

        public async Task DeleteAsync(
            int id,
            DeleteVehicleRequest request,
            CancellationToken cancellationToken = default)
        {
            using var message =
                new HttpRequestMessage(
                    HttpMethod.Delete,
                    $"api/vehicles/{id}")
                {
                    Content =
                        JsonContent.Create(request)
                };


            using var response =
                await _httpClient.SendAsync(
                    message,
                    cancellationToken);


            await EnsureSuccessAsync(
                response,
                cancellationToken);
        }

        // =====================================================
        // RESPONSE
        // =====================================================

        private static async Task<VehicleDetailDto>
            ReadVehicleAsync(
                HttpResponseMessage response,
                CancellationToken cancellationToken)
        {
            var item =
                await response.Content
                    .ReadFromJsonAsync<VehicleDetailDto>(
                        cancellationToken:
                            cancellationToken);


            if (item is null)
            {
                throw new ApiException(
                    "Araç bilgileri API yanıtından okunamadı.",
                    (int)response.StatusCode);
            }


            return item;
        }


        // =====================================================
        // ERROR
        // =====================================================

        private static async Task EnsureSuccessAsync(
            HttpResponseMessage response,
            CancellationToken cancellationToken)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }


            var error =
                await TryReadErrorAsync(
                    response,
                    cancellationToken);


            throw new ApiException(
                error?.GetErrorMessage()
                ?? $"Araç işlemi başarısız oldu. Durum kodu: {(int)response.StatusCode}",
                (int)response.StatusCode);
        }


        private static async Task<ApiErrorResponse?>
            TryReadErrorAsync(
                HttpResponseMessage response,
                CancellationToken cancellationToken)
        {
            try
            {
                return await response.Content
                    .ReadFromJsonAsync<ApiErrorResponse>(
                        cancellationToken:
                            cancellationToken);
            }
            catch
            {
                return null;
            }
        }
    }
}