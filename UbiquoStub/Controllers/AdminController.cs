using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using UbiquoStub.Abstractions;
using UbiquoStub.Abstractions.Repositories;
using UbiquoStub.Abstractions.Services;
using UbiquoStub.Abstractions.Stubs;
using UbiquoStub.Abstractions.Utils;
using UbiquoStub.Models.DTOs;
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Controllers
{
    [Route("api/v2/admin/stubs")]
    [ApiController]
    public class AdminController(IStubsReader stubReader, IStubsWriter stubWriter,
    IRequestUtil requestUtil, IStubService stubService,
    IEntityToDtoConverter<Sut, SutDto> sutConverter, IUnitOfWork unitOfWork,
    IEntityToDtoConverter<StubResult, StubResultDto> stubResultConverter, ILogger<AdminController> logger) : ControllerBase
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
            var result = await stubService.GetSutsAsync(null, true);
            var dtos = result.Select(sutConverter.Convert);
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
                return Results.Ok(sutConverter.Convert(addedSut));
            }
            catch (Exception ex)
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
            try
            {
                await stubService.DeleteStubsByIds(body.sutId, body.ids);
                return Results.Ok("Stubs Deleted");
            } catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
            
           
        }

        [HttpGet("results")]
        [EndpointName("Admin Test Results")]
        [EndpointDescription("Get test results in the DB")]
        public async Task<IResult> GetTestResults()
        {
            try
            {
                var results = await unitOfWork.StubResultRepository.Get(null, null, "Stub.Response,Stub.Request");
                var resultsDto = results.Select(stubResultConverter.Convert);
                //return Results.Ok(results);
                return Results.Ok(resultsDto);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new ResponseMessageDto(ex.Message));
            }
        }

        [HttpDelete("results")]
        [EndpointName("Admin Test Results Deletion")]
        [EndpointDescription("Delete test results in the DB")]
        public async Task<IResult> DeleteTestResults()
        {
            try
            {
                var results = await unitOfWork.StubResultRepository.Get();
                foreach (var result in results)
                {
                    unitOfWork.StubResultRepository.Delete(result);
                }
                await unitOfWork.SaveAsync();
                return Results.Ok(new ResponseMessageDto($"test results are deleted successfully"));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new ResponseMessageDto(ex.Message));
            }
        }

        [HttpGet("results/file")]
        [EndpointName("Admin Download Test Results")]
        [EndpointDescription("Get test results file")]
        public async Task<IResult> DownloadTestResults()
        {
            try
            {
                var results = await unitOfWork.StubResultRepository.Get(null, null, "Stub.Response,Stub.Request");
                var resultsDtos = results.Select(stubResultConverter.Convert);
                string toWriteString = JsonSerializer.Serialize(resultsDtos, JsonSerializerOptions.Web);
                // Generate file path
                string fileName = $"data_{DateTime.UtcNow.Ticks}.json";
                string filePath = Path.Combine(Path.GetTempPath(), fileName);
                // Write to file
                await System.IO.File.WriteAllTextAsync(filePath, toWriteString);
                // Open the file stream
                Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                // Register deletion of the file after response is completed
                HttpContext.Response.OnCompleted(async () =>
                {
                    try
                    {
                        stream.Dispose(); // Close the stream
                        System.IO.File.Delete(filePath);
                        logger.LogInformation("Temporary file {FilePath} deleted successfully.", filePath);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to delete temporary file {FilePath}.", filePath);
                    }

                    await Task.CompletedTask;
                });

                // Return the file as a download
                return Results.File(stream, "application/json", fileName);

            } catch(Exception ex)
            {
                return Results.BadRequest(new ResponseMessageDto(ex.Message));
            }
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
