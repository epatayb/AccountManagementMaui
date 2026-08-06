using System.Net;
using System.Net.Http.Json;
using AccountManagementMaui.Shared.Models.Common;
using AccountManagementMaui.Shared.Models.TaxOfficeModels;
using AccountManagementMaui.Shared.Services.Common;

namespace AccountManagementMaui.Shared.Services.TaxOfficeServices;

public class TaxOfficeService : ITaxOfficeService
{
    private readonly HttpClient _httpClient;

    public TaxOfficeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<TaxOfficeListDto>> GetAllAsync(
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

        var requestUri = "api/taxoffices";

        if (parameters.Count > 0)
        {
            requestUri +=
                $"?{string.Join("&", parameters)}";
        }

        using var response = await _httpClient.GetAsync(
            requestUri,
            cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var taxOffices = await response.Content
            .ReadFromJsonAsync<List<TaxOfficeListDto>>(
                cancellationToken: cancellationToken);

        return taxOffices ?? [];
    }

    public async Task<TaxOfficeDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"api/taxoffices/{id}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        return await response.Content
            .ReadFromJsonAsync<TaxOfficeDetailDto>(
                cancellationToken: cancellationToken);
    }

    public async Task<TaxOfficeDetailDto> CreateAsync(
        CreateTaxOfficeRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.PostAsJsonAsync(
                "api/taxoffices",
                request,
                cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var taxOffice = await response.Content
            .ReadFromJsonAsync<TaxOfficeDetailDto>(
                cancellationToken: cancellationToken);

        return taxOffice ?? throw new ApiException(
            "API geçerli bir vergi dairesi bilgisi döndürmedi.",
            (int)response.StatusCode);
    }

    public async Task<TaxOfficeDetailDto> UpdateAsync(
        int id,
        UpdateTaxOfficeRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.PutAsJsonAsync(
                $"api/taxoffices/{id}",
                request,
                cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var taxOffice = await response.Content
            .ReadFromJsonAsync<TaxOfficeDetailDto>(
                cancellationToken: cancellationToken);

        return taxOffice ?? throw new ApiException(
            "API geçerli bir vergi dairesi bilgisi döndürmedi.",
            (int)response.StatusCode);
    }

    public async Task DeleteAsync(
        int id,
        DeleteTaxOfficeRequest request,
        CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"api/taxoffices/{id}")
        {
            Content = JsonContent.Create(request)
        };

        using var response = await _httpClient.SendAsync(
            httpRequest,
            cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);
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