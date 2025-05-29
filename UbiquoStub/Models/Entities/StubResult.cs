using System;
using System.Text.Json.Nodes;
using UbiquoStub.Models.DTOs.Stubs;

namespace UbiquoStub.Models.Entities;

public class StubResult
{
    public long Id { get; set; }
    public Stub Stub { get; set; }
    public NewStubDto StubDto { get; set; }
    public bool IsIntegration { get; set; }
    public TestStatus Status { get; set; }
    public ResDto? ActualResponse { get; set; }
}
