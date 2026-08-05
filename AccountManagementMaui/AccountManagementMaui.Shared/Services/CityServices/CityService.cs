using System.Net;
using System.Net.Http.Json;
using AccountManagementMaui.Shared.Models.CityModels;
using AccountManagementMaui.Shared.Models.Common;
using AccountManagementMaui.Shared.Services.Common;

namespace AccountManagementMaui.Shared.Services.CityServices;

public class CityService : ICityService
{
    private readonly HttpClient _httpClient;

    public CityService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<CityListDto>> GetAllAsync(
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var requestUri = "api/cities";

        if (!string.IsNullOrWhiteSpace(search))
        {
            requestUri +=
                $"?search={Uri.EscapeDataString(search.Trim())}";
        }

        using var response = await _httpClient.GetAsync(
            requestUri,
            cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var cities = await response.Content
            .ReadFromJsonAsync<List<CityListDto>>(
                cancellationToken: cancellationToken);

        return cities ?? [];
    }

    public async Task<CityDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"api/cities/{id}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        return await response.Content
            .ReadFromJsonAsync<CityDetailDto>(
                cancellationToken: cancellationToken);
    }

    public async Task<CityDetailDto> CreateAsync(
        CreateCityRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/cities",
            request,
            cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var city = await response.Content
            .ReadFromJsonAsync<CityDetailDto>(
                cancellationToken: cancellationToken);

        return city ?? throw new ApiException(
            "API geçerli bir il bilgisi döndürmedi.",
            (int)response.StatusCode);
    }

    public async Task<CityDetailDto> UpdateAsync(
        int id,
        UpdateCityRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PutAsJsonAsync(
            $"api/cities/{id}",
            request,
            cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var city = await response.Content
            .ReadFromJsonAsync<CityDetailDto>(
                cancellationToken: cancellationToken);

        return city ?? throw new ApiException(
            "API geçerli bir il bilgisi döndürmedi.",
            (int)response.StatusCode);
    }

    public async Task DeleteAsync(
        int id,
        DeleteCityRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        request ??= new DeleteCityRequest();

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"api/cities/{id}")
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
            error?.Message ??
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