using System;
using System.Text.Json.Nodes;

namespace UbiquoStub.Models.Entities;

public class RequestEntity
{
    public long Id { get; set; }
    public string Method { get; set; }
    //[JsonProperty("url")]
    public string Uri { get; set; }
    //[JsonProperty("headers")]
    public IDictionary<string, IEnumerable<string>>? Headers { get; set; }
    //[JsonProperty("body")]
    public JsonNode? Body { get; set; }
}
