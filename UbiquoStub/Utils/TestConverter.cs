using System;
using UbiquoStub.Abstractions.Utils;
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Utils;

public class TestConverter(IEntityToDtoConverter<Stub, NewStubDto> stubConverter) : IEntityToDtoConverter<Test, TestDto>
{
    public TestDto Convert(Test toConvert)
    {
        TestDto dto = new TestDto(toConvert.TestName,
            toConvert.Stubs.Select(stubConverter.Convert));
        return dto;
    }
}
