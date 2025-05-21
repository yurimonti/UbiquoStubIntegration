using System;
using UbiquoStub.Models.DTOs;

namespace UbiquoStub.Abstractions;

public interface IStubsReader
{
    Task<IEnumerable<StubDto>?> GetStubs();
}
