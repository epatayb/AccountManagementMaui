using System.Net.Http.Json;
using AccountManagementMaui.Shared.Models.Common;
using AccountManagementMaui.Shared.Models.VehicleKindModels;
using AccountManagementMaui.Shared.Services.Common;

namespace AccountManagementMaui.Shared.Services.VehicleKindServices;

public class VehicleKindService : IVehicleKindService
{
    private readonly HttpClient _httpClient;

    public VehicleKindService(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<List<VehicleKindListDto>> GetAllAsync(
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var url = "api/vehiclekinds";

        if (!string.IsNullOrWhiteSpace(search))
        {
            url +=
                "?search=" +
                Uri.EscapeDataString(search.Trim());
        }

        using var response =
            await _httpClient.GetAsync(
                url,
                cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        return await response.Content
                   .ReadFromJsonAsync<List<VehicleKindListDto>>(
                       cancellationToken: cancellationToken)
               ?? [];
    }


    public async Task<VehicleKindDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.GetAsync(
                $"api/vehiclekinds/{id}",
                cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        return await response.Content
            .ReadFromJsonAsync<VehicleKindDetailDto>(
                cancellationToken: cancellationToken);
    }


    public async Task<VehicleKindDetailDto> CreateAsync(
        CreateVehicleKindRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.PostAsJsonAsync(
                "api/vehiclekinds",
                request,
                cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var result =
            await response.Content
                .ReadFromJsonAsync<VehicleKindDetailDto>(
                    cancellationToken: cancellationToken);

        if (result is null)
        {
            throw new ApiException(
                "Araç türü oluşturuldu ancak API yanıtı alınamadı.",
                (int)response.StatusCode);
        }

        return result;
    }


    public async Task<VehicleKindDetailDto> UpdateAsync(
        int id,
        UpdateVehicleKindRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.PutAsJsonAsync(
                $"api/vehiclekinds/{id}",
                request,
                cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var result =
            await response.Content
                .ReadFromJsonAsync<VehicleKindDetailDto>(
                    cancellationToken: cancellationToken);

        if (result is null)
        {
            throw new ApiException(
                "Araç türü güncellendi ancak API yanıtı alınamadı.",
                (int)response.StatusCode);
        }

        return result;
    }


    public async Task DeleteAsync(
        int id,
        DeleteVehicleKindRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message =
            new HttpRequestMessage(
                HttpMethod.Delete,
                $"api/vehiclekinds/{id}")
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


    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        ApiErrorResponse? error = null;

        try
        {
            error =
                await response.Content
                    .ReadFromJsonAsync<ApiErrorResponse>(
                        cancellationToken: cancellationToken);
        }
        catch
        {
            // API hata cevabı beklenen JSON formatında değilse
            // varsayılan mesaj kullanılacak.
        }

        throw new ApiException(
            error?.GetErrorMessage()
            ?? "Araç türü işlemi sırasında hata oluştu.",
            (int)response.StatusCode);
    }
}