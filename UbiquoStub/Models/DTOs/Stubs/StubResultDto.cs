namespace UbiquoStub.Models.DTOs.Stubs;

public record StubResultDto(bool isIntegration, string status, ResDto expectedResponse, ResDto? actualResponse);
