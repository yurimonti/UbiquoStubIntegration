using System;

namespace UbiquoStub.Models.Entities;

public class Test
{
    public long Id { get; set; }
    public string TestName { get; set; }
    public IEnumerable<Stub> Stubs { get; set; }
}
