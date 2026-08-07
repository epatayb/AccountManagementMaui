using System.Net;
using System.Net.Http.Json;
using AccountManagementMaui.Shared.Models.AccountCardGroupModels;
using AccountManagementMaui.Shared.Models.Common;
using AccountManagementMaui.Shared.Services.Common;

namespace AccountManagementMaui.Shared.Services.AccountCardGroupServices;

public class AccountCardGroupService : IAccountCardGroupService
{
    private readonly HttpClient _httpClient;

    public AccountCardGroupService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<AccountCardGroupListDto>> GetAllAsync(
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var requestUri = "api/accountcardgroups";

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
            .ReadFromJsonAsync<List<AccountCardGroupListDto>>(
                cancellationToken: cancellationToken);

        return items ?? [];
    }

    public async Task<AccountCardGroupDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            $"api/accountcardgroups/{id}",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        return await response.Content
            .ReadFromJsonAsync<AccountCardGroupDetailDto>(
                cancellationToken: cancellationToken);
    }

    public async Task<AccountCardGroupDetailDto> CreateAsync(
        CreateAccountCardGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/accountcardgroups",
            request,
            cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var item = await response.Content
            .ReadFromJsonAsync<AccountCardGroupDetailDto>(
                cancellationToken: cancellationToken);

        return item ?? throw new ApiException(
            "API geçerli bir hesap kart grubu bilgisi döndürmedi.",
            (int)response.StatusCode);
    }

    public async Task<AccountCardGroupDetailDto> UpdateAsync(
        int id,
        UpdateAccountCardGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PutAsJsonAsync(
            $"api/accountcardgroups/{id}",
            request,
            cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var item = await response.Content
            .ReadFromJsonAsync<AccountCardGroupDetailDto>(
                cancellationToken: cancellationToken);

        return item ?? throw new ApiException(
            "API geçerli bir hesap kart grubu bilgisi döndürmedi.",
            (int)response.StatusCode);
    }

    public async Task DeleteAsync(
        int id,
        DeleteAccountCardGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Delete,
            $"api/accountcardgroups/{id}")
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