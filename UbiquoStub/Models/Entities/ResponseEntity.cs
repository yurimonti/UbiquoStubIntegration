using System;
using System.Text.Json.Nodes;

namespace UbiquoStub.Models.Entities;

public class ResponseEntity
{
    public long Id { get; set; }
    public int Status { get; set; }

    //[JsonProperty("headers")]
    public IDictionary<string, IEnumerable<string>>? Headers { get; set; }
    //[JsonProperty("body")]
    public JsonNode? Body { get; set; }
}