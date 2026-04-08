using fixedhitbox.Application.DTOs;
using fixedhitbox.Domain.Entities;
using fixedhitbox.Infrastructure.External.Aredl.DTOs;
using fixedhitbox.Shared.Results.Mappers;

namespace fixedhitbox.Application.Mappers.Aredl;

public static class AredlRecordMapper
{
    public static MapperResult<AredlRecordDto> Map(AredlRecordResponse? record)
    {
        if (record is null)
            return MapperResult<AredlRecordDto>.Fail("Record is null.");

        if (record.Mobile is null)
            return MapperResult<AredlRecordDto>.Fail("Record.Mobile is missing");

        if (string.IsNullOrWhiteSpace(record.VideoUrl))
            return MapperResult<AredlRecordDto>.Fail("Record.VideoUrl is missing.");

        if (record.IsVerification is null)
            return MapperResult<AredlRecordDto>.Fail("Record.IsVerification is missing.");

        if (record.CreatedAt is null)
            return MapperResult<AredlRecordDto>.Fail("Record.CreatedAt is missing.");

        var levelResult = AredlLevelMapper.Map(record.Level);
        if (!levelResult.Success)
            return MapperResult<AredlRecordDto>
            .Fail(levelResult.Error ?? "AredlLevelMapper could not map an AredlLevelResponse.");

        if (levelResult.Value is null)
            return MapperResult<AredlRecordDto>
            .Fail("AredlLevelMapper returned a null Value.");

        return MapperResult<AredlRecordDto>.Ok(new AredlRecordDto(
            record.Id,
            record.Mobile.Value,
            record.VideoUrl,
            record.IsVerification.Value,
            record.HideVideo!.Value,
            record.CreatedAt.Value,
            record.UpdatedAt,
            levelResult.Value
        ));
    }
}