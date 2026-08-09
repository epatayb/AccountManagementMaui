using System.Net.Http.Json;
using AccountManagementMaui.Shared.Models.Common;
using AccountManagementMaui.Shared.Models.VehicleTypeModels;
using AccountManagementMaui.Shared.Services.Common;

namespace AccountManagementMaui.Shared.Services.VehicleTypeServices;

public class VehicleTypeService : IVehicleTypeService
{
    private readonly HttpClient _httpClient;

    public VehicleTypeService(
        HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<List<VehicleTypeListDto>> GetAllAsync(
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var url = "api/vehicletypes";

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
                   .ReadFromJsonAsync<List<VehicleTypeListDto>>(
                       cancellationToken: cancellationToken)
               ?? [];
    }


    public async Task<VehicleTypeDetailDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.GetAsync(
                $"api/vehicletypes/{id}",
                cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        return await response.Content
            .ReadFromJsonAsync<VehicleTypeDetailDto>(
                cancellationToken: cancellationToken);
    }


    public async Task<VehicleTypeDetailDto> CreateAsync(
        CreateVehicleTypeRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.PostAsJsonAsync(
                "api/vehicletypes",
                request,
                cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var result =
            await response.Content
                .ReadFromJsonAsync<VehicleTypeDetailDto>(
                    cancellationToken: cancellationToken);

        if (result is null)
        {
            throw new ApiException(
                "Araç tipi oluşturuldu ancak API yanıtı alınamadı.",
                (int)response.StatusCode);
        }

        return result;
    }


    public async Task<VehicleTypeDetailDto> UpdateAsync(
        int id,
        UpdateVehicleTypeRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response =
            await _httpClient.PutAsJsonAsync(
                $"api/vehicletypes/{id}",
                request,
                cancellationToken);

        await EnsureSuccessAsync(
            response,
            cancellationToken);

        var result =
            await response.Content
                .ReadFromJsonAsync<VehicleTypeDetailDto>(
                    cancellationToken: cancellationToken);

        if (result is null)
        {
            throw new ApiException(
                "Araç tipi güncellendi ancak API yanıtı alınamadı.",
                (int)response.StatusCode);
        }

        return result;
    }


    public async Task DeleteAsync(
        int id,
        DeleteVehicleTypeRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message =
            new HttpRequestMessage(
                HttpMethod.Delete,
                $"api/vehicletypes/{id}")
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
            ?? "Araç tipi işlemi sırasında hata oluştu.",
            (int)response.StatusCode);
    }
}