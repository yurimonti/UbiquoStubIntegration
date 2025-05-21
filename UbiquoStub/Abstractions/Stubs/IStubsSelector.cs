using System;
using UbiquoStub.Models.DTOs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Abstractions.Stubs;

public interface IStubsSelector
{
    Task<StubDto?> SelectStub(string serviceName, string path, HttpContext context);
    Task<StubDto?> SelectStub(Func<StubDto, bool> predicate);
    Task<Stub> SelectStub(Func<Stub, bool> predicate);
}
