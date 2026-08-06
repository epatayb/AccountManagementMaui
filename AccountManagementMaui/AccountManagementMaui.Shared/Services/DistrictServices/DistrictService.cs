using AccountManagementMaui.Shared.Models.Common;
using AccountManagementMaui.Shared.Services.Common;
using AccountManagementMaui.Shared.Models.DistrictModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagementMaui.Shared.Services.DistrictServices
{
    public class DistrictService : IDistrictService
    {
        private readonly HttpClient _httpClient;

        public DistrictService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<DistrictListDto>> GetAllAsync(
            string? search = null,
            int? cityId = null,
            CancellationToken cancellationToken = default)
        {
            var parameters = new List<string>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                parameters.Add(
                    $"search={Uri.EscapeDataString(search.Trim())}");
            }

            if (cityId.HasValue && cityId.Value > 0)
            {
                parameters.Add($"cityId={cityId.Value}");
            }

            var requestUri = "api/districts";

            if (parameters.Count > 0)
            {
                requestUri += $"?{string.Join("&", parameters)}";
            }

            using var response = await _httpClient.GetAsync(
                requestUri,
                cancellationToken);

            await EnsureSuccessAsync(response, cancellationToken);

            var districts = await response.Content
                .ReadFromJsonAsync<List<DistrictListDto>>(
                    cancellationToken: cancellationToken);

            return districts ?? [];
        }

        public async Task<DistrictDetailDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync(
                $"api/districts/{id}",
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            await EnsureSuccessAsync(response, cancellationToken);

            return await response.Content
                .ReadFromJsonAsync<DistrictDetailDto>(
                    cancellationToken: cancellationToken);
        }

        public async Task<DistrictDetailDto> CreateAsync(
            CreateDistrictRequest request,
            CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.PostAsJsonAsync(
                "api/districts",
                request,
                cancellationToken);

            await EnsureSuccessAsync(response, cancellationToken);

            var district = await response.Content
                .ReadFromJsonAsync<DistrictDetailDto>(
                    cancellationToken: cancellationToken);

            return district ?? throw new ApiException(
                "API geçerli bir ilçe bilgisi döndürmedi.",
                (int)response.StatusCode);
        }

        public async Task<DistrictDetailDto> UpdateAsync(
            int id,
            UpdateDistrictRequest request,
            CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.PutAsJsonAsync(
                $"api/districts/{id}",
                request,
                cancellationToken);

            await EnsureSuccessAsync(response, cancellationToken);

            var district = await response.Content
                .ReadFromJsonAsync<DistrictDetailDto>(
                    cancellationToken: cancellationToken);

            return district ?? throw new ApiException(
                "API geçerli bir ilçe bilgisi döndürmedi.",
                (int)response.StatusCode);
        }

        public async Task DeleteAsync(
            int id,
            DeleteDistrictRequest request,
            CancellationToken cancellationToken = default)
        {
            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Delete,
                $"api/districts/{id}")
            {
                Content = JsonContent.Create(request)
            };

            using var response = await _httpClient.SendAsync(
                httpRequest,
                cancellationToken);

            await EnsureSuccessAsync(response, cancellationToken);
        }

        private static async Task EnsureSuccessAsync(
            HttpResponseMessage response,
            CancellationToken cancellationToken)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var error = await TryReadErrorAsync(
                response,
                cancellationToken);

            throw new ApiException(
                error?.GetErrorMessage() ??
                $"API isteği başarısız oldu. Durum kodu: {(int)response.StatusCode}",
                (int)response.StatusCode);
        }

        private static async Task<ApiErrorResponse?> TryReadErrorAsync(
            HttpResponseMessage response,
            CancellationToken cancellationToken)
        {
            try
            {
                return await response.Content
                    .ReadFromJsonAsync<ApiErrorResponse>(
                        cancellationToken: cancellationToken);
            }
            catch
            {
                return null;
            }
        }
    }
}    