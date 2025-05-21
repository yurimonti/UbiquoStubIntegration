using System.Text.Json.Nodes;

namespace UbiquoStub.Models.DTOs.Stubs;

public record ResDto
{
    public int Status { get; set; }
    public JsonNode? Body { get; set; }
    public IDictionary<string, IEnumerable<string>>? Headers { get; set; }

    public ResDto()
    {

    }

    public ResDto(int status, JsonNode? body, IDictionary<string, IEnumerable<string>>? headers) : base()
    {
        Status = status;
        Body = body;
        Headers = headers;
    }

}
