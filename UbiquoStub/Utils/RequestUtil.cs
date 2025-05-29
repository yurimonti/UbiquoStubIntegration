using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Primitives;
using UbiquoStub.Abstractions;
using UbiquoStub.Models.DTOs;

namespace UbiquoStub.Utils;

public class RequestUtil : IRequestUtil
{
    public bool CompareRequestDto(RequestDto first, RequestDto second)
    {
        return JsonSerializer.Serialize(first) == JsonSerializer.Serialize(second);
    }

    public string GetUriFromRequest(string path, QueryString query)
    {
        return string.Concat("/", path, query.ToString());
    }

    public bool RequestHeaderContainsHeadersDto(IHeaderDictionary requestHeaders, IDictionary<string, IEnumerable<string>>? headersDto)
    {
        if(headersDto is null) return true;
        foreach (var rec in headersDto)
        {
            bool recordIsPresent = requestHeaders.TryGetValue(rec.Key, out StringValues values);
            bool result = recordIsPresent && rec.Value.Order().SequenceEqual(values.Order());
            if (!result) return false;
        }
        return true;
    }
}
