using System.Net;
using System.Net.Http.Json;
using AccountManagementMaui.Shared.Models.AccountCardKindModels;
using AccountManagementMaui.Shared.Models.Common;
using AccountManagementMaui.Shared.Services.Common;

namespace AccountManagementMaui.Shared.Services.AccountCardKindServices;

public class AccountCardKindService : IAccountCardKindService
{
    private readonly HttpClient _httpClient;

    public AccountCardKindService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<AccountCardKindListDto>> GetAllAsync(
        string? search = null,
        int? accountCardTypeId = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new List<string>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            parameters.Add(
                $"search={Uri.EscapeDataString(search.Trim())}");
        }

        if (accountCardTypeId.HasValue &&
            accountCardTypeId.Value > 0)
        {
            parameters.Add(
                $"accountCardTypeId={accountCardTypeId.Value}");
        }

        var requestUri = "api/accountcardkinds";

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

        var items = await response.Content
            .ReadFromJsonAsync<List<AccountCardKindListDto>>(
                cancellationToken: cancellationToken);

        return items ?? [];
    }

    public async Task<AccountCardKindDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"api/accountcardkinds/{id}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        return await response.Content
            .ReadFromJsonAsync<AccountCardKindDetailDto>(
                cancellationToken: cancellationToken);
    }

    public async Task<AccountCardKindDetailDto> CreateAsync(
        CreateAccountCardKindRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/accountcardkinds",
            request,
            cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var item = await response.Content
            .ReadFromJsonAsync<AccountCardKindDetailDto>(
                cancellationToken: cancellationToken);

        return item ?? throw new ApiException(
            "API geçerli bir kart türü bilgisi döndürmedi.",
            (int)response.StatusCode);
    }

    public async Task<AccountCardKindDetailDto> UpdateAsync(
        int id,
        UpdateAccountCardKindRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PutAsJsonAsync(
            $"api/accountcardkinds/{id}",
            request,
            cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var item = await response.Content
            .ReadFromJsonAsync<AccountCardKindDetailDto>(
                cancellationToken: cancellationToken);

        return item ?? throw new ApiException(
            "API geçerli bir kart türü bilgisi döndürmedi.",
            (int)response.StatusCode);
    }

    public async Task DeleteAsync(
        int id,
        DeleteAccountCardKindRequest request,
        CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"api/accountcardkinds/{id}")
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