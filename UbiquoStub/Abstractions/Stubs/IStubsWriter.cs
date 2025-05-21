using System;
using UbiquoStub.Models.DTOs;

namespace UbiquoStub.Abstractions.Stubs;

public interface IStubsWriter
{
    Task SetStubs(IEnumerable<StubDto> stubs);
}
