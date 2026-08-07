using System.Net;
using System.Net.Http.Json;
using AccountManagementMaui.Shared.Models.AccountCardSubGroupModels;
using AccountManagementMaui.Shared.Models.Common;
using AccountManagementMaui.Shared.Services.Common;

namespace AccountManagementMaui.Shared.Services.AccountCardSubGroupServices;

public class AccountCardSubGroupService : IAccountCardSubGroupService
{
    private readonly HttpClient _httpClient;

    public AccountCardSubGroupService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<AccountCardSubGroupListDto>> GetAllAsync(
        string? search = null,
        int? accountCardGroupId = null,
        CancellationToken cancellationToken = default)
    {
        var parameters = new List<string>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            parameters.Add(
                $"search={Uri.EscapeDataString(search.Trim())}");
        }

        if (accountCardGroupId.HasValue &&
            accountCardGroupId.Value > 0)
        {
            parameters.Add(
                $"accountCardGroupId={accountCardGroupId.Value}");
        }

        var requestUri =
            "api/accountcardsubgroups";

        if (parameters.Count > 0)
        {
            requestUri +=
                $"?{string.Join("&", parameters)}";
        }

        using var response =
            await _httpClient.GetAsync(
                requestUri,
                cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var items = await response.Content
            .ReadFromJsonAsync<List<AccountCardSubGroupListDto>>(
                cancellationToken: cancellationToken);

        return items ?? [];
    }

    public async Task<AccountCardSubGroupDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.GetAsync(
                $"api/accountcardsubgroups/{id}",
                cancellationToken);

        if (response.StatusCode ==
            HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        return await response.Content
            .ReadFromJsonAsync<AccountCardSubGroupDetailDto>(
                cancellationToken: cancellationToken);
    }

    public async Task<AccountCardSubGroupDetailDto> CreateAsync(
        CreateAccountCardSubGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.PostAsJsonAsync(
                "api/accountcardsubgroups",
                request,
                cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var item = await response.Content
            .ReadFromJsonAsync<AccountCardSubGroupDetailDto>(
                cancellationToken: cancellationToken);

        return item ?? throw new ApiException(
            "API geçerli bir hesap kart alt grubu bilgisi döndürmedi.",
            (int)response.StatusCode);
    }

    public async Task<AccountCardSubGroupDetailDto> UpdateAsync(
        int id,
        UpdateAccountCardSubGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.PutAsJsonAsync(
                $"api/accountcardsubgroups/{id}",
                request,
                cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var item = await response.Content
            .ReadFromJsonAsync<AccountCardSubGroupDetailDto>(
                cancellationToken: cancellationToken);

        return item ?? throw new ApiException(
            "API geçerli bir hesap kart alt grubu bilgisi döndürmedi.",
            (int)response.StatusCode);
    }

    public async Task DeleteAsync(
        int id,
        DeleteAccountCardSubGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        using var httpRequest =
            new HttpRequestMessage(
                HttpMethod.Delete,
                $"api/accountcardsubgroups/{id}")
            {
                Content = JsonContent.Create(request)
            };

        using var response =
            await _httpClient.SendAsync(
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

        var error =
            await TryReadErrorAsync(
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