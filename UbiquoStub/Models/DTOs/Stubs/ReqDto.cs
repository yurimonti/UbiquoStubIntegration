using System.Text.Json.Nodes;

namespace UbiquoStub.Models.DTOs.Stubs;

public record ReqDto(string uri, string method, JsonNode? body,IDictionary<string, IEnumerable<string>>? headers);