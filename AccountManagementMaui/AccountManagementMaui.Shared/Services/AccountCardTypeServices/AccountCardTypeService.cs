using System.Net;
using System.Net.Http.Json;
using AccountManagementMaui.Shared.Models.AccountCardTypeModels;
using AccountManagementMaui.Shared.Models.Common;
using AccountManagementMaui.Shared.Services.Common;

namespace AccountManagementMaui.Shared.Services.AccountCardTypeServices;

public class AccountCardTypeService : IAccountCardTypeService
{
    private readonly HttpClient _httpClient;

    public AccountCardTypeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<AccountCardTypeListDto>> GetAllAsync(
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var requestUri = "api/accountcardtypes";

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

        var items = await response.Content
            .ReadFromJsonAsync<List<AccountCardTypeListDto>>(
                cancellationToken: cancellationToken);

        return items ?? [];
    }

    public async Task<AccountCardTypeDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"api/accountcardtypes/{id}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        return await response.Content
            .ReadFromJsonAsync<AccountCardTypeDetailDto>(
                cancellationToken: cancellationToken);
    }

    public async Task<AccountCardTypeDetailDto> CreateAsync(
        CreateAccountCardTypeRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/accountcardtypes",
            request,
            cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var item = await response.Content
            .ReadFromJsonAsync<AccountCardTypeDetailDto>(
                cancellationToken: cancellationToken);

        return item ?? throw new ApiException(
            "API geçerli bir kart tipi bilgisi döndürmedi.",
            (int)response.StatusCode);
    }

    public async Task<AccountCardTypeDetailDto> UpdateAsync(
        int id,
        UpdateAccountCardTypeRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PutAsJsonAsync(
            $"api/accountcardtypes/{id}",
            request,
            cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var item = await response.Content
            .ReadFromJsonAsync<AccountCardTypeDetailDto>(
                cancellationToken: cancellationToken);

        return item ?? throw new ApiException(
            "API geçerli bir kart tipi bilgisi döndürmedi.",
            (int)response.StatusCode);
    }

    public async Task DeleteAsync(
        int id,
        DeleteAccountCardTypeRequest request,
        CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"api/accountcardtypes/{id}")
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