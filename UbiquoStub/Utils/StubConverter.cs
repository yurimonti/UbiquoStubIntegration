using System;
using UbiquoStub.Abstractions.Utils;
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Utils;

public class StubConverter(IRequestConverter requestConverter,
    IResponseConverter responseConverter) : IStubConverter
{
    public NewStubDto EntityToDto(Stub toConvert)
    {
        NewStubDto dto = new NewStubDto(toConvert.Name, toConvert.TestName,
            toConvert.Order, toConvert.Host,
            requestConverter.EntityToDto(toConvert.Request),
            responseConverter.EntityToDto(toConvert.Response));
        return dto;
    }
}
