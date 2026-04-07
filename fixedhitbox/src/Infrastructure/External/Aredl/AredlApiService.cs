using System.Net.Http.Json;
using System.Text.Json;
using fixedhitbox.Application.DTOs;
using fixedhitbox.Application.Interfaces;
using fixedhitbox.Application.Mappers.Aredl;
using fixedhitbox.Infrastructure.External.Aredl.DTOs;
using fixedhitbox.Shared.Results.Api;

namespace fixedhitbox.Infrastructure.External.Aredl;

public sealed class AredlApiService(HttpClient httpClient) : IAredlApiService
{

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    public async Task<AredlApiResult<PendingAredlLinkDto>> GetProfileAsync(
        ulong discordId,
        CancellationToken cancellationToken = default)
    { //This method should not return a PendingDto, but a ProfileDto.
        //The application decides what to do with the profile.
        //Need to fix it.
        
        try
        {
            using var response = await httpClient.GetAsync(
                $"v2/api/aredl/profile/{discordId}",
                cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return AredlApiResult<PendingAredlLinkDto>.NotFound();

            if (!response.IsSuccessStatusCode)
                return AredlApiResult<PendingAredlLinkDto>.ConnectionError(
                    $"Fail while attempting to connect to AREDL: {response.StatusCode}.");
            
            var responseDto = await response.Content.ReadFromJsonAsync<AredlProfileResponse>(
                JsonOptions, cancellationToken);
            
            if (responseDto is null)
                return AredlApiResult<PendingAredlLinkDto>.InvalidResponse(
                    "AREDL Api sent an invalid or empty response.");
            
            var map = AredlProfileMapper.Map(responseDto);
            
            if (!map.Success)
                return AredlApiResult<PendingAredlLinkDto>
                    .InvalidResponse(map.Error ?? "AredlProfileMapper could not map a profile in AredlApiService.");

            if (string.IsNullOrWhiteSpace(map.Value!.Username))
                return AredlApiResult<PendingAredlLinkDto>.NotFound(
                    "AREDL Api returned an object with an empty username.");

            return AredlApiResult<PendingAredlLinkDto>.Success(map.Value);

        }
        catch (TaskCanceledException ex)
        {
            return AredlApiResult<PendingAredlLinkDto>.Timeout(ex.Message);
        }
        catch (JsonException ex)
        {
            return AredlApiResult<PendingAredlLinkDto>.InvalidResponse(ex.Message);
        }
        catch (Exception ex)
        {
            return AredlApiResult<PendingAredlLinkDto>.UnexpectedError(ex.Message);
        }
    }
}