using System;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Abstractions.Utils;

public interface IResponseUtil
{
    void AddHeaderToHttpContextResponse(HttpResponse response, HttpResponseMessage from);
    void AddHeaderToHttpContextResponse(HttpResponse response, ResponseEntity from);

}
