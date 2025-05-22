using System;
using UbiquoStub.Abstractions.Utils;
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Utils;

public class SutConverter(IEntityToDtoConverter<Stub,NewStubDto> stubConverter) : IEntityToDtoConverter<Sut,SutDto>
{
    public SutDto Convert(Sut toConvert)
    {
        var dto = new SutDto(toConvert.Name,toConvert.Stubs.Select(stubConverter.Convert));
        return dto;
    }

}
