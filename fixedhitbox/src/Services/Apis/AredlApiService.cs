using System.Net.Http.Json;
using fixedhitbox.Dtos.Aredl;
using fixedhitbox.Services.Interfaces;
using fixedhitbox.Services.Results;
using System.Text.Json;
using fixedhitbox.Dtos;

namespace fixedhitbox.Services.Apis;

public sealed class AredlApiService(HttpClient httpClient) : IAredlApiService
{

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    public async Task<AredlApiResult<AredlProfileResponse>> GetProfileAsync(
        ulong discordId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await httpClient.GetAsync(
                $"https://api.aredl.net/v2/api/aredl/profile/{discordId}",
                cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return AredlApiResult<AredlProfileResponse>.NotFound();

            if (!response.IsSuccessStatusCode)
                return AredlApiResult<AredlProfileResponse>.ConnectionError(
                    $"Fail while attempting to connect to AREDL: {response.StatusCode}.");
            
            var profile = await response.Content.ReadFromJsonAsync<AredlProfileResponse>(
                JsonOptions, cancellationToken);
            
            if (profile is null)
                return AredlApiResult<AredlProfileResponse>.InvalidResponse(
                    "AREDL Api sent an invalid or empty response.");
            
            if (!AredlProfileMapper.TryNormalize(profile, out _, out var error))
                return AredlApiResult<AredlProfileResponse>.InvalidResponse(error);

            if (string.IsNullOrWhiteSpace(profile.Username))
                return AredlApiResult<AredlProfileResponse>.NotFound(
                    "AREDL Api returned an object with an empty username.");

            return AredlApiResult<AredlProfileResponse>.Success(profile);

        }
        catch (OperationCanceledException ex)
        {
            return AredlApiResult<AredlProfileResponse>.OperationCanceled(ex.Message);
        }
        catch (JsonException ex)
        {
            return AredlApiResult<AredlProfileResponse>.InvalidResponse(ex.Message);
        }
        catch (Exception ex)
        {
            return AredlApiResult<AredlProfileResponse>.UnexpectedError(ex.Message);
        }
    }
}