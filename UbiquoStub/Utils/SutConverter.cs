using System;
using UbiquoStub.Abstractions.Utils;
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Utils;

public class SutConverter(IStubConverter testConverter) : ISutConverter
{
    public SutDto EntityToDto(Sut toConvert)
    {
        var dto = new SutDto(toConvert.Name,toConvert.Stubs.Select(testConverter.EntityToDto));
        return dto;
    }

}
