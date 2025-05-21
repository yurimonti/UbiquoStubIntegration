using System.Text.Json.Nodes;

namespace UbiquoStub.Models.DTOs.Errors
{
    public record ComparisonError
    {
        public JsonNode Actual { get; set; }
        public JsonNode Expected { get; set; }
    }
}
