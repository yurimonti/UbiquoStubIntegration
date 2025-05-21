using System;
using Microsoft.Extensions.Primitives;
using UbiquoStub.Abstractions.Utils;
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Utils;

public class ResponseUtil : IResponseUtil
{
    public void AddHeaderToHttpContextResponse(HttpResponse response, HttpResponseMessage from)
    {
        if (from.Headers is not null)
        {
            foreach (var header in from.Headers)
            {
                var name = header.Key;
                var values = header.Value.ToArray();
                if (name is not "Transfer-Encoding")
                {
                    // Content-Type must go on the content object
                    if (name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                    {
                        // overwrite whatever the StringContent constructor set, if present
                        if (values?.Any() == true)
                            response.ContentType = values.First();
                    }
                    response.Headers[name] = new StringValues(values); ; 
                }
                
            }
        }
    }
    
    public void AddHeaderToHttpContextResponse(HttpResponse response, ResponseEntity from)
    {
        if (from.Headers is not null)
            {
                foreach (var header in from.Headers)
                {
                    var name = header.Key;
                    var values = header.Value;
                    // Content-Type must go on the content object
                    if (name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                    {
                        // overwrite whatever the StringContent constructor set, if present
                        if (values?.Any() == true)
                            response.ContentType = values.First();
                    }
                    response.Headers[name] = (StringValues)values!;
                }
            }
    } 
}
