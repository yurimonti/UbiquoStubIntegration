using System;
using UbiquoStub.Abstractions.Utils;
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Utils;

public class StubResultConverter(IEntityToDtoConverter<ResponseEntity, ResDto> responseConverter) : IEntityToDtoConverter<StubResult, StubResultDto>
{
    public StubResultDto Convert(StubResult entityToConvert)
    {
        var dto = new StubResultDto(entityToConvert.IsIntegration,entityToConvert.Status.ToString(),
            responseConverter.Convert(entityToConvert.Stub.Response),entityToConvert.ActualResponse);
        return dto;
    }
}
