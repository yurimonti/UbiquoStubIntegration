using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace UbiquoStub.Models.DTOs
{
    public record RequestDto
    {
        //[JsonProperty("method")]
        public string Method { get; set; }
        //[JsonProperty("url")]
        public string Uri { get; set; }
        public string Host { get; set; }
        //[JsonProperty("headers")]
        public IDictionary<string, IEnumerable<string>>? Headers { get; set; }
        //[JsonProperty("body")]
        public JsonNode? Body { get; set; }
    }
}