using System.Net.Http.Json;

using AccountManagementMaui.Shared.Models.Common;
using AccountManagementMaui.Shared.Models.VehicleLookupModels;
using AccountManagementMaui.Shared.Services.Common;

namespace AccountManagementMaui.Shared.Services.VehicleLookupServices
{
    public class VehicleLookupService
        : IVehicleLookupService
    {
        private readonly HttpClient _httpClient;


        public VehicleLookupService(
            HttpClient httpClient)
        {
            _httpClient =
                httpClient;
        }


        // =====================================================
        // DRIVERS
        // =====================================================

        public async Task<List<VehicleAccountLookupDto>>
            GetDriversAsync(
                string? search = null,
                int take = 30,
                CancellationToken cancellationToken = default)
        {
            var url =
                BuildSearchUrl(
                    "api/vehicle-lookups/drivers",
                    search,
                    take);


            using var response =
                await _httpClient.GetAsync(
                    url,
                    cancellationToken);


            await EnsureSuccessAsync(
                response,
                cancellationToken);


            return await response.Content
                       .ReadFromJsonAsync<
                           List<VehicleAccountLookupDto>>(
                           cancellationToken:
                               cancellationToken)
                   ?? [];
        }


        // =====================================================
        // ACCOUNTS
        // =====================================================

        public async Task<List<VehicleAccountLookupDto>>
            GetAccountsAsync(
                string? search = null,
                int take = 30,
                CancellationToken cancellationToken = default)
        {
            var url =
                BuildSearchUrl(
                    "api/vehicle-lookups/accounts",
                    search,
                    take);


            using var response =
                await _httpClient.GetAsync(
                    url,
                    cancellationToken);


            await EnsureSuccessAsync(
                response,
                cancellationToken);


            return await response.Content
                       .ReadFromJsonAsync<
                           List<VehicleAccountLookupDto>>(
                           cancellationToken:
                               cancellationToken)
                   ?? [];
        }


        // =====================================================
        // ACCOUNT DETAIL
        // =====================================================

        public async Task<VehicleAccountLookupDetailDto?>
            GetAccountByIdAsync(
                int id,
                CancellationToken cancellationToken = default)
        {
            using var response =
                await _httpClient.GetAsync(
                    $"api/vehicle-lookups/accounts/{id}",
                    cancellationToken);


            await EnsureSuccessAsync(
                response,
                cancellationToken);


            return await response.Content
                .ReadFromJsonAsync<
                    VehicleAccountLookupDetailDto>(
                    cancellationToken:
                        cancellationToken);
        }


        // =====================================================
        // CITIES
        // =====================================================

        public async Task<List<VehicleCityLookupDto>> GetCitiesAsync(
            CancellationToken cancellationToken = default)
        {
            using var response =
                await _httpClient.GetAsync(
                    "api/vehicle-lookups/cities",
                    cancellationToken);


            await EnsureSuccessAsync(
                response,
                cancellationToken);


            return await response.Content
                       .ReadFromJsonAsync<List<VehicleCityLookupDto>>(
                           cancellationToken:
                               cancellationToken)
                   ?? [];
        }


        // =====================================================
        // TAX OFFICES
        // =====================================================

        public async Task<List<VehicleTaxOfficeLookupDto>>
            GetTaxOfficesAsync(
                int? cityId = null,
                CancellationToken cancellationToken = default)
        {
            var url =
                "api/vehicle-lookups/tax-offices";


            if (cityId.HasValue &&
                cityId.Value > 0)
            {
                url +=
                    $"?cityId={cityId.Value}";
            }


            using var response =
                await _httpClient.GetAsync(
                    url,
                    cancellationToken);


            await EnsureSuccessAsync(
                response,
                cancellationToken);


            return await response.Content
                       .ReadFromJsonAsync<List<VehicleTaxOfficeLookupDto>>(
                           cancellationToken:
                               cancellationToken)
                   ?? [];
        }




        public async Task<List<VehicleAccountLookupDto>>
            GetInvoiceAccountsAsync(
            string? search = null,
            int take = 30,
            CancellationToken cancellationToken = default)
        {
            var url =
                BuildSearchUrl(
                    "api/vehicle-lookups/invoice-accounts",
                    search,
                    take);


            using var response =
                await _httpClient.GetAsync(
                    url,
                    cancellationToken);


            await EnsureSuccessAsync(
                response,
                cancellationToken);


            return await response.Content
                       .ReadFromJsonAsync<
                           List<VehicleAccountLookupDto>>(
                           cancellationToken:
                               cancellationToken)
                   ?? [];
        }

        // =====================================================
        // URL
        // =====================================================

        private static string BuildSearchUrl(
            string baseUrl,
            string? search,
            int take)
        {
            var query =
                new List<string>
                {
                    $"take={take}"
                };


            if (!string.IsNullOrWhiteSpace(
                search))
            {
                query.Add(
                    "search=" +
                    Uri.EscapeDataString(
                        search.Trim()));
            }


            return baseUrl +
                   "?" +
                   string.Join(
                       "&",
                       query);
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


            ApiErrorResponse? error =
                null;


            try
            {
                error =
                    await response.Content
                        .ReadFromJsonAsync<ApiErrorResponse>(
                            cancellationToken:
                                cancellationToken);
            }
            catch
            {
            }


            throw new ApiException(
                error?.GetErrorMessage()
                ??
                $"Hesap arama işlemi başarısız oldu. Durum kodu: {(int)response.StatusCode}",
                (int)response.StatusCode);
        }
    }
}