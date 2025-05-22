using System;
using UbiquoStub.Models.DTOs;
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Abstractions.Utils;

public interface IResponseConverter
{
    HttpResponseMessage Deserialize(ResponseDto toDeserialize);
    HttpResponseMessage Deserialize(ResponseEntity toDeserialize);

    Task<ResDto> DeserializeResponseMessageToDto(HttpResponseMessage responseMessage);

    ResponseEntity DtoToEntity(ResDto toConvert);

    // ResDto EntityToDto(ResponseEntity toConvert);
}
