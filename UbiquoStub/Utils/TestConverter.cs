using System;
using UbiquoStub.Abstractions.Utils;
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Utils;

public class TestConverter(IStubConverter stubConverter) : ITestConverter
{
    public TestDto EntityToDto(Test toConvert)
    {
        TestDto dto = new TestDto(toConvert.TestName,
            toConvert.Stubs.Select(stubConverter.EntityToDto));
        return dto;
    }
}
