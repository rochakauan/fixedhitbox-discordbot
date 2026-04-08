using fixedhitbox.Application.DTOs;
using fixedhitbox.Domain.Domain_DTOs;
using fixedhitbox.Domain.Entities;
using fixedhitbox.Infrastructure.External.Aredl.DTOs;
using fixedhitbox.Shared;
using fixedhitbox.Shared.Results.Mappers;

namespace fixedhitbox.Application.Mappers.Aredl;

public static class AredlProfileMapper
{

    public static MapperResult<PendingAredlLinkDto> Map(AredlProfileResponse? response)
    {
        if (response == null)
            return MapperResult<PendingAredlLinkDto>.Fail("Response is null");

        if (string.IsNullOrWhiteSpace(response.Username))
            return MapperResult<PendingAredlLinkDto>.Fail("Username is missing.");
        if (response.DiscordId is null)
            return MapperResult<PendingAredlLinkDto>.Fail("DiscordId is missing.");
        if (response.CreatedInAredlAt is null)
            return MapperResult<PendingAredlLinkDto>.Fail("CreatedInAredlAt is missing.");
        
        var records = new List<AredlRecordDto>();
        var recordsAvailable = true;
        
        if (response.Records is not null)
        {
            foreach (var record in response.Records)
            {
                var result = AredlRecordMapper.Map(record);
                
                if (!result.Success)
                {
                    recordsAvailable = false;
                    continue;
                }

                records.Add(result.Value!);
            }
        } else recordsAvailable = false;

        var dto = new PendingAredlLinkDto
        {
            DiscordId = response.DiscordId.Value,
            Username = response.Username,
            GlobalName = response.GlobalName ?? string.Empty,
            Description = response.Description,
            Id = response.Id,
            Country = response.Country,
            CreatedAt = response.CreatedInAredlAt.Value,
            Records = records,
            RecordsAvailable = recordsAvailable
        };

        return MapperResult<PendingAredlLinkDto>.Ok(dto);
    }

    public static MapperResult<PendingAredlLinkDto> MapToPendingDto(AredlProfileDto validDto)
    {
        var pendingDto = new PendingAredlLinkDto
        {
            DiscordId = validDto.DiscordId,
            Username = validDto.Username,
            GlobalName = validDto.GlobalName,
            Description = validDto.Description,
            Id = validDto.Id,
            Country = validDto.Country,
            CreatedAt = validDto.CreatedInAredlAt,
            Records = validDto.Records
        };
        
        if (validDto.Records is not null)
            pendingDto.RecordsAvailable = true;

        return MapperResult<PendingAredlLinkDto>.Ok(pendingDto);
    }

    public static MapperResult<AredlProfileDto> MapFromPendingDto(PendingAredlLinkDto dto)
        => MapperResult<AredlProfileDto>.Ok(new AredlProfileDto(
            dto.Id,
            dto.Username,
            dto.GlobalName,
            dto.DiscordId,
            dto.Description,
            dto.Country,
            dto.CreatedAt,
            dto.Records));

    public static MapperResult<PendingAredlLinkDto> MapFromEntity(LinkedUser linkedUser)
        => MapperResult<PendingAredlLinkDto>.Ok(new PendingAredlLinkDto
        {
            DiscordId = linkedUser.DiscordId,
            Username = linkedUser.Username,
            GlobalName = linkedUser.GlobalName,
            Description = linkedUser.Description,
            Id = linkedUser.AredlUserId,
            Country = linkedUser.Country,
            CreatedAt = linkedUser.CreatedInAredlAt
        });
}