using Newtonsoft.Json;

namespace UbiquoStub.Models.DTOs
{
    public record StubDto
    {
        //[JsonProperty("request")]
        public string ServiceName { get; set; }
        public RequestDto Request { get; set; }

        //[JsonProperty("response")]
        public ResponseDto Response { get; set; }
    }
}
