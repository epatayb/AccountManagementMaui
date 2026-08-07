using System.Net;
using System.Net.Http.Json;
using AccountManagementMaui.Shared.Models.AccountCardModels;
using AccountManagementMaui.Shared.Models.Common;
using AccountManagementMaui.Shared.Services.Common;

namespace AccountManagementMaui.Shared.Services.AccountCardServices;

public class AccountCardService : IAccountCardService
{
    private readonly HttpClient _httpClient;

    public AccountCardService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<IReadOnlyList<AccountCardListDto>> GetAllAsync(
        string? search = null,
        int? accountCardTypeId = null,
        int? accountCardKindId = null,
        int? accountCardGroupId = null,
        int? accountCardSubGroupId = null,
        int? cityId = null,
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


        if (accountCardKindId.HasValue &&
            accountCardKindId.Value > 0)
        {
            parameters.Add(
                $"accountCardKindId={accountCardKindId.Value}");
        }


        if (accountCardGroupId.HasValue &&
            accountCardGroupId.Value > 0)
        {
            parameters.Add(
                $"accountCardGroupId={accountCardGroupId.Value}");
        }


        if (accountCardSubGroupId.HasValue &&
            accountCardSubGroupId.Value > 0)
        {
            parameters.Add(
                $"accountCardSubGroupId={accountCardSubGroupId.Value}");
        }


        if (cityId.HasValue &&
            cityId.Value > 0)
        {
            parameters.Add(
                $"cityId={cityId.Value}");
        }


        var requestUri =
            "api/accountcards";


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


        var items =
            await response.Content
                .ReadFromJsonAsync<List<AccountCardListDto>>(
                    cancellationToken: cancellationToken);


        return items ?? [];
    }


    public async Task<AccountCardDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.GetAsync(
                $"api/accountcards/{id}",
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
            .ReadFromJsonAsync<AccountCardDetailDto>(
                cancellationToken: cancellationToken);
    }


    public async Task<AccountCardDetailDto> CreateAsync(
        CreateAccountCardRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.PostAsJsonAsync(
                "api/accountcards",
                request,
                cancellationToken);


        await EnsureSuccessAsync(
            response,
            cancellationToken);


        var item =
            await response.Content
                .ReadFromJsonAsync<AccountCardDetailDto>(
                    cancellationToken: cancellationToken);


        return item ??
            throw new ApiException(
                "API geçerli bir hesap kartı bilgisi döndürmedi.",
                (int)response.StatusCode);
    }


    public async Task<AccountCardDetailDto> UpdateAsync(
        int id,
        UpdateAccountCardRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.PutAsJsonAsync(
                $"api/accountcards/{id}",
                request,
                cancellationToken);


        await EnsureSuccessAsync(
            response,
            cancellationToken);


        var item =
            await response.Content
                .ReadFromJsonAsync<AccountCardDetailDto>(
                    cancellationToken: cancellationToken);


        return item ??
            throw new ApiException(
                "API geçerli bir hesap kartı bilgisi döndürmedi.",
                (int)response.StatusCode);
    }


    public async Task DeleteAsync(
        int id,
        DeleteAccountCardRequest request,
        CancellationToken cancellationToken = default)
    {
        using var httpRequest =
            new HttpRequestMessage(
                HttpMethod.Delete,
                $"api/accountcards/{id}")
            {
                Content =
                    JsonContent.Create(request)
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