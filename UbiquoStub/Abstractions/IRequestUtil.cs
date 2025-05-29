using System;
using System.Net.Http.Headers;
using Microsoft.Extensions.Primitives;
using UbiquoStub.Models.DTOs;

namespace UbiquoStub.Abstractions;

public interface IRequestUtil
{
    string GetUriFromRequest(string path, QueryString query);

    bool RequestHeaderContainsHeadersDto(IHeaderDictionary requestHeaders,IDictionary<string , IEnumerable<string>>? headersDto);
    bool CompareRequestDto(RequestDto first, RequestDto second);
}
