using System;
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Abstractions.Utils;

public interface ITestConverter
{
    TestDto EntityToDto(Test toConvert);
}
