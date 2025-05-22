using System;
using UbiquoStub.Models.DTOs;
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Abstractions.Utils;

public interface IRequestConverter
{
    HttpRequestMessage Deserialize(RequestDto toDeserialize);

    RequestEntity DtoToEntity(ReqDto toConvert);

}
