using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace UbiquoStub.Models.DTOs
{
    public record ResponseDto
    {
        //[JsonProperty("status")]
        public int Status { get; set; }
        //[JsonProperty("headers")]
        public IDictionary<string, IEnumerable<string>>? Headers { get; set; }
        //[JsonProperty("body")]
        public JsonNode? Body { get; set; }
    }
}