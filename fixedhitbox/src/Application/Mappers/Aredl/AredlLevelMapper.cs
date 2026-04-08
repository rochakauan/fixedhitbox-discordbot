using fixedhitbox.Application.DTOs;
using fixedhitbox.Domain.Entities;
using fixedhitbox.Infrastructure.External.Aredl.DTOs;
using fixedhitbox.Shared.Results.Mappers;

namespace fixedhitbox.Application.Mappers.Aredl;

public static class AredlLevelMapper
{
    public static MapperResult<AredlLevelDto> Map(AredllevelResponse? level)
    {
        if (level is null)
            return MapperResult<AredlLevelDto>.Fail("Level is null.");

        if (string.IsNullOrWhiteSpace(level.Name))
            return MapperResult<AredlLevelDto>.Fail("Level.Name is missing.");

        if (level.LevelId is null)
            return MapperResult<AredlLevelDto>.Fail("Level.Id is missing.");

        if (level.Position is null)
            return MapperResult<AredlLevelDto>.Fail("Level.Position is missing.");

        if (level.Points is null)
            return MapperResult<AredlLevelDto>.Fail("Level.Points is missing.");

        if (level.TwoPlayer is null)
            return MapperResult<AredlLevelDto>.Fail("Level.TwoPlayer is missing.");

        if (level.Legacy is null)
            return MapperResult<AredlLevelDto>.Fail("Level.Legacy is missing.");

        return MapperResult<AredlLevelDto>.Ok(new AredlLevelDto(
            level.Id,
            level.Name,
            level.LevelId.Value,
            level.TwoPlayer.Value,
            level.Position.Value,
            level.Points.Value,
            level.Legacy.Value
            ));
    }
}