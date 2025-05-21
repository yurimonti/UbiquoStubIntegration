using System;

namespace UbiquoStub.Models.Entities;

public class Sut
{
    public long Id { get; set; }
    public string Name { get; set; }

    public IEnumerable<Stub> Stubs { get; set; }
}