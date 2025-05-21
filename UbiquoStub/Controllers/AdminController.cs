using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using UbiquoStub.Abstractions;
using UbiquoStub.Abstractions.Services;
using UbiquoStub.Abstractions.Stubs;
using UbiquoStub.Abstractions.Utils;
using UbiquoStub.Models.DTOs;
using UbiquoStub.Models.DTOs.Stubs;

namespace UbiquoStub.Controllers
{
    [Route("api/v2/admin/stubs")]
    [ApiController]
    public class AdminController(IStubsReader stubReader, IStubsWriter stubWriter, 
    IRequestUtil requestUtil, IStubService stubService,
    ISutConverter sutConverter, ILogger<AdminController> logger) : ControllerBase
    {
        // [HttpGet]
        // [EndpointName("Admin Get Stubs")]
        // [EndpointDescription("Get all stubs in the default file json")]
        // public async Task<IResult> GetStubs()
        // {
        //     var result = await stubReader.GetStubs();
        //     return Results.Ok(result);
        //}

        [HttpGet("sut")]
        [EndpointName("Admin Get Stubs For a SUT")]
        [EndpointDescription("Get all stubs in the database for a sut")]
        public async Task<IResult> GetSutStubs()
        {
            var result = await stubService.GetSutsAsync(null,true);
            var dtos = result.Select(sutConverter.EntityToDto);
            return Results.Ok(dtos);
        }

        [HttpPost("sut")]
        [EndpointName("Admin Stubs upload to database")]
        [EndpointDescription("Add new stub in a the DB, if the request stub is not already exist")]
        public async Task<IResult> PostTestSuite([FromBody] AddStubDto body)
        {
            try
            {
                var addedSut = await stubService.AddStub(body.sutName, body.stubs);
                return Results.Ok(addedSut);
            } catch(Exception ex)
            {
                return Results.BadRequest(new ResponseMessageDto(ex.Message));
            }
        }
        public record DeleteBody(long sutId, long[] ids);

        [HttpDelete("sut")]
        [EndpointName("Admin Stubs deletion to database")]
        [EndpointDescription("Delete all stubs in the database")]
        public async Task<IResult> DeleteStubs([FromBody] DeleteBody body)
        {
            await stubService.DeleteStubsByIds(body.sutId, body.ids);
            return Results.Ok("Stubs Deleted");
        }

        // [HttpPost]
        // [EndpointName("Admin Stubs upload")]
        // [EndpointDescription("Add new stub, if the request stub is not already in the default file json")]
        // public async Task<IResult> PostStub([FromBody] StubDto body)
        // {
        //     IEnumerable<StubDto>? fileJson = await stubReader.GetStubs();
        //     IEnumerable<StubDto> toWrite;
        //     var stub = fileJson.FirstOrDefault(s => requestUtil.CompareRequestDto(s.Request,body.Request));
        //     if (stub is null) toWrite = fileJson?.Append(body);
        //     else
        //     {
        //         logger.LogWarning($"new stub request is {JsonSerializer.Serialize(body.Request)}");
        //         logger.LogWarning($"the present stub request is {JsonSerializer.Serialize(stub?.Request)}");
        //         //TODO: write new stub instead leaving the old one
        //         stub = body;
        //         toWrite = fileJson;
        //     }
        //     //var toWrite = fileJson?.Append(body);
        //     logger.LogInformation($"The writing will be:\n{toWrite}");
        //     await stubWriter.SetStubs(toWrite!);
        //     return Results.Ok(JsonSerializer.Serialize(toWrite));
        // }

        // [HttpDelete]
        // [EndpointName("Admin Stubs deletion")]
        // [EndpointDescription("Delete all stubs in the default file json")]
        // public async Task<IResult> DeleteStubs()
        // {
        //     IEnumerable<StubDto>? fileJson = await stubReader.GetStubs();
        //     await stubWriter.SetStubs([]);
        //     return Results.Ok("Stubs Deleted");
        // }
    }
}
