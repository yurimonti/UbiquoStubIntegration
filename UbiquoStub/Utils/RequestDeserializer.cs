using UbiquoStub.Abstractions.Utils;
using UbiquoStub.Models.DTOs;
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Utils;

public class RequestDeserializer : IEntityToDtoConverter<RequestEntity, ReqDto>, IRequestConverter
{
    public HttpRequestMessage Deserialize(RequestDto toDeserialize)
    {
        HttpRequestMessage request = new HttpRequestMessage()
        {
            Content = toDeserialize?.Body != null ? new StringContent(toDeserialize?.Body?.ToJsonString()) : null,
            Method = HttpMethod.Parse(toDeserialize.Method),
            RequestUri = new Uri(toDeserialize.Uri),
        };
        foreach (var header in toDeserialize.Headers)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToList());
        }
        return request;
        // HttpResponseMessage response = new HttpResponseMessage()
        // {
        //     StatusCode = (HttpStatusCode)toDeserialize.Status,
        //     Content = new StringContent(JsonSerializer.Serialize(toDeserialize.Body))
        // };
        // return response;
    }

    public RequestEntity DtoToEntity(ReqDto toConvert)
    {
        RequestEntity request = new()
        {
            Body = toConvert.body,
            Method = toConvert.method,
            Uri = toConvert.uri,
            Headers = toConvert.headers
        };
        return request;
    }

    public ReqDto Convert(RequestEntity toConvert)
    {
        ReqDto request = new ReqDto(toConvert.Uri,toConvert.Method,toConvert.Body,toConvert.Headers);
        return request;
    }

}
