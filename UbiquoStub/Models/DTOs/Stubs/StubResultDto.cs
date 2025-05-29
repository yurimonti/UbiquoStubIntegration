namespace UbiquoStub.Models.DTOs.Stubs;

public record StubResultDto(bool isIntegration, string status, ReqDto request, ResDto expectedResponse, ResDto? actualResponse);
