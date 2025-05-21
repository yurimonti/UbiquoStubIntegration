using Microsoft.AspNetCore.Mvc;
using UbiquoStub.Abstractions.Repositories;
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Controllers
{
    [Route("api/v2/test")]
    [ApiController]
    public class TestController(IUnitOfWork unitOfWork) : ControllerBase
    {

        [HttpPost("test")]
        public async Task<IResult> Test([FromBody] AddStubDto body)
        {
            await unitOfWork.SutRepository.Insert(new Sut()
            {
                Name = body.sutName,
                Stubs = []
            });
            return Results.Ok();
        }
    }
}
