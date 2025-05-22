using System;
using UbiquoStub.Abstractions.Utils;
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Utils;

public class StubConverter(IEntityToDtoConverter<RequestEntity, ReqDto> requestConverter,
    IEntityToDtoConverter<ResponseEntity, ResDto> responseConverter) : IEntityToDtoConverter<Stub, NewStubDto>
{
    public NewStubDto Convert(Stub toConvert)
    {
        NewStubDto dto = new NewStubDto(toConvert.Name, toConvert.TestName,
            toConvert.Order, toConvert.Host,
            requestConverter.Convert(toConvert.Request),
            responseConverter.Convert(toConvert.Response));
        return dto;
    }
}
