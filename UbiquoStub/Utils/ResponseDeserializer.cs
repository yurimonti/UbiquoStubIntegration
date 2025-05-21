using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using UbiquoStub.Abstractions.Utils;
using UbiquoStub.Models.DTOs;
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Utils
{
    public class ResponseDeserializer : IResponseConverter
    {
        public HttpResponseMessage Deserialize(ResponseDto toDeserialize)
        {
            HttpResponseMessage response = new HttpResponseMessage()
            {
                StatusCode = (HttpStatusCode)toDeserialize.Status,
                Content = new StringContent(JsonSerializer.Serialize(toDeserialize.Body)),
            };
            if (toDeserialize.Headers is not null) AddHeaderToResponseFromDto(response, toDeserialize?.Headers);
            return response;
        }

        public HttpResponseMessage Deserialize(ResponseEntity toDeserialize)
        {
            HttpResponseMessage response = new HttpResponseMessage()
            {
                StatusCode = (HttpStatusCode)toDeserialize.Status,
                Content = new StringContent(JsonSerializer.Serialize(toDeserialize.Body)),
            };
            if (toDeserialize.Headers is not null) AddHeaderToResponseFromDto(response, toDeserialize?.Headers);
            return response;
        }

        public async Task<ResDto> DeserializeResponseMessageToDto(HttpResponseMessage responseMessage)
        {
            var statusCode = (int)responseMessage.StatusCode;
            var bodyString = await responseMessage.Content.ReadAsStringAsync();
            var body = bodyString is not null && bodyString is not ""
                ? JsonNode.Parse(bodyString, new JsonNodeOptions() { PropertyNameCaseInsensitive = true })
                : null;
            ResDto toReturn = new ResDto()
            {
                Body = body,
                Status = statusCode,
            };
            if (responseMessage.Headers.Any() || responseMessage.Headers is not null)
                AddHeaderToResponseDtoFromResponseMessage(toReturn, responseMessage.Headers);
            return toReturn;
        }

        public ResponseEntity DtoToEntity(ResDto toConvert)
        {
            ResponseEntity request = new()
            {
                Body = toConvert.Body,
                Status = toConvert.Status,
                Headers = toConvert.Headers
            };
            return request;
        }

        public ResDto EntityToDto(ResponseEntity toConvert)
        {
            ResDto response = new ResDto(toConvert.Status, toConvert.Body, toConvert.Headers);
            return response;
        }
        
        //PRIVATE METHODS

        private void AddHeaderToResponseDtoFromResponseMessage(ResDto response, HttpResponseHeaders headers)
        {
            string[] keys = ["Date", "Server", "Transfer-Encoding"];
            response.Headers = new Dictionary<string, IEnumerable<string>>();
            foreach (var header in headers)
            {
                if (!keys.Contains(header.Key)) response.Headers.Add(header.Key, header.Value);
            }
            if (!response.Headers.Any()) response.Headers = null;
        }
        
        private void AddHeaderToResponseFromDto(HttpResponseMessage response, IDictionary<string, IEnumerable<string>> headerDto)
        {
            foreach (var header in headerDto)
            {
                response.Headers.Add(header.Key, header.Value);
            }
        }

    }
}
