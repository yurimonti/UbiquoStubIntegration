using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using UbiquoStub.Abstractions;
using UbiquoStub.Utils;
using UbiquoStub.Abstractions.Repositories;
using UbiquoStub.Abstractions.Services;
using UbiquoStub.Abstractions.Stubs;
using UbiquoStub.Abstractions.Utils;
using UbiquoStub.Exceptions;
using UbiquoStub.Models.DTOs;
using UbiquoStub.Models.Entities;
using UbiquoStub.Models.DTOs.Stubs;

namespace UbiquoStub.Controllers
{
    [Route("api/v2")]
    [ApiController]
    public class StubController(IHttpContextAccessor accessor, IStubsSelector stubSelector,
        IRequestUtil requestUtil, IStubService stubService,
        IResponseUtil responseUtil,
        IEntityToDtoConverter<Stub, NewStubDto> stubConverter,
        IUnitOfWork unitOfWork, ILogger<StubController> logger) : ControllerBase
    {

        private async Task<IResult> Stubbing(string sutName, string serviceName, string requestMethod,
        IHeaderDictionary requestHeaders, string uri, JsonNode? body = null)
        {
            var testStub = body is null ?
                await stubSelector.SelectStub(sutName, stub => stub.Name == serviceName
                    && stub.Request.Method == requestMethod && stub.Request.Uri == uri
                    && requestUtil.RequestHeaderContainsHeadersDto(requestHeaders, stub.Request?.Headers)) :
                await stubSelector.SelectStub(sutName, stub => stub.Name == serviceName
                    && stub.Request.Method == requestMethod && stub.Request.Uri == uri
                    // I don't remember why i need to compare with a Subset...
                    && BodyUtil.GptIsJsonSubset(stub.Request.Body, body)
                    && requestUtil.RequestHeaderContainsHeadersDto(requestHeaders, stub.Request?.Headers));
            StubResult sr = new StubResult()
            {
                IsIntegration = false,
                Status = TestStatus.PASSED,
                Stub = testStub,
                StubDto = stubConverter.Convert(testStub),
                ActualResponse = null
            };
            await unitOfWork.StubResultRepository.Insert(sr);
            await unitOfWork.SaveAsync();
            //TODO: insert here
            responseUtil.AddHeaderToHttpContextResponse(HttpContext.Response, testStub.Response);
            return Results.Json(testStub.Response.Body, statusCode: testStub.Response.Status);
        }

        [HttpGet("{sutName}/{serviceName}/stubs/{*path}")]
        [EndpointName("Ubiquo GET Mock From Stubs V2")]
        [EndpointDescription("Get the 'GET' response associated to the sent request; these information are retrieved from database")]
        public async Task<IResult> GetResponseStub(string sutName, string serviceName, string path)
        {
            string requestMethod = accessor.HttpContext.Request.Method;
            var requestHeaders = accessor.HttpContext.Request.Headers;
            var uri = requestUtil.GetUriFromRequest(path, accessor.HttpContext.Request.QueryString);
            try
            {
                IResult result = await Stubbing(sutName, serviceName, requestMethod, requestHeaders, uri, null);
                return result;
            }
            catch (NoStubInSUTException ex)
            {
                return Results.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        [HttpPost("{sutName}/{serviceName}/stubs/{*path}")]
        [EndpointName("Ubiquo POST Mock From Stubs V2")]
        [EndpointDescription("Get the 'POST' response associated to the sent request; these information are retrieved from database")]
        public async Task<IResult> PostResponseStub(string sutName, string serviceName,
            string? path = null, [FromBody] JsonNode? body = null)
        {
            string requestMethod = accessor.HttpContext.Request.Method;
            var requestHeaders = accessor.HttpContext.Request.Headers;
            var uri = requestUtil.GetUriFromRequest(path, accessor.HttpContext.Request.QueryString);
            try
            {
                IResult result = await Stubbing(sutName, serviceName, requestMethod, requestHeaders, uri, body);
                return result;
            }
            catch (NoStubInSUTException ex)
            {
                return Results.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        [HttpPut("{sutName}/{serviceName}/stubs/{*path}")]
        [EndpointName("Ubiquo PUT Mock From Stubs V2")]
        [EndpointDescription("Get the 'PUT' response associated to the sent request; these information are retrieved from database")]
        public async Task<IResult> PutResponseStub(string sutName, string serviceName, 
            string? path = null, [FromBody] JsonNode? body = null)
        {
            string requestMethod = accessor.HttpContext.Request.Method;
            var requestHeaders = accessor.HttpContext.Request.Headers;
            var uri = requestUtil.GetUriFromRequest(path, accessor.HttpContext.Request.QueryString);
            try
            {
                IResult result = await Stubbing(sutName, serviceName, requestMethod, requestHeaders, uri, body);
                return result;
            }
            catch (NoStubInSUTException ex)
            {
                return Results.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
        // [HttpGet("{serviceName}/fdgsdfg/stubs/{testName}/{order}/{*path}")]
        // [EndpointName("Ubiquo GET Mock From Stubs V2")]
        // [EndpointDescription("Get the 'GET' response associated to the sent request; these information are retrieved from database")]
        // public async Task<IResult> GetResponseStub(string serviceName, string testName, int order, string path)
        // {
        //     string requestMethod = accessor.HttpContext.Request.Method;
        //     var requestHeaders = accessor.HttpContext.Request.Headers;
        //     var uri = requestUtil.GetUriFromRequest(path, accessor.HttpContext.Request.QueryString);
        //     try
        //     {
        //         var testStub = await stubSelector.SelectStub(stub => stub.Name == serviceName 
        //             && stub.TestName == testName && stub.Order == order
        //             && stub.Request.Method == requestMethod && stub.Request.Uri == uri 
        //             && requestUtil.RequestHeaderContainsHeadersDto(requestHeaders, stub.Request?.Headers));
        //         logger.LogCritical($"test stub reponse body is : \n {testStub.Response.Body}");
        //         StubResult sr = new StubResult()
        //         {
        //             IsIntegration = false,
        //             Status = TestStatus.PASSED,
        //             Stub = testStub,
        //             ActualResponse = null
        //         };
        //         await unitOfWork.StubResultRepository.Insert(sr);
        //         await unitOfWork.SaveAsync();
        //         // StubDto? st = await stubSelector.SelectStub(stub => 
        //         // stub.ServiceName == serviceName && stub.Request.Method == requestMethod 
        //         // && stub.Request.Uri == uri && requestUtil.RequestHeaderContainsHeadersDto(requestHeaders,stub.Request?.Headers));
        //         //if (st is null) return Results.BadRequest($"Stub File does not contain a stub for this request Uri: {uri} or method {requestMethod}");
        //         // var reqSer = new RequestDeserializer();
        //         // var req = reqSer.Deserialize(st.Request);
        //         // logger.LogCritical($"the request is \n {JsonSerializer.Serialize(req,JsonSerializerOptions.Web)}");
        //         //logger.LogCritical($"the request header is \n {JsonSerializer.Serialize(requestHeaders,JsonSerializerOptions.Web)}");
        //         // logger.LogCritical($"the stub header is \n {JsonSerializer.Serialize(st.Request.Headers,JsonSerializerOptions.Web)}");
        //         if (testStub.Response.Headers is not null)
        //         {
        //             foreach (var header in testStub.Response.Headers)
        //             {
        //                 var name = header.Key;
        //                 var values = header.Value;
        //                 //TODO: check this code
        //                 // Content-Type must go on the content object
        //                 if (name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
        //                 {
        //                     // overwrite whatever the StringContent constructor set, if present
        //                     if (values?.Any() == true)
        //                         HttpContext.Response.ContentType = values.First();
        //                 }
        //                 HttpContext.Response.Headers[name] = (StringValues)values!;
        //             }
        //         }
        //         return Results.Json(testStub.Response.Body, statusCode: testStub.Response.Status);
        //     }
        //     catch (Exception ex)
        //     {
        //         return Results.BadRequest(ex.Message);
        //     }
        // }

        // [HttpPost("{serviceName}/stubs/{*path}")]
        // [EndpointName("Ubiquo POST Mock From Stubs V2")]
        // [EndpointDescription("Get the 'POST' response associated to the sent request; these information are retrieved from stubs file")]
        // public async Task<IResult> PostResponseStub(string serviceName, string path, [FromBody] JsonNode? body)
        // {
        //     string requestMethod = accessor.HttpContext.Request.Method;
        //     var uri = requestUtil.GetUriFromRequest(path, accessor.HttpContext.Request.QueryString);
        //     try
        //     {
        //         StubDto? st = await stubSelector.SelectStub(stub =>
        //             stub.ServiceName == serviceName && stub.Request.Uri == uri && stub.Request.Method == requestMethod && BodyUtil.GptIsJsonSubset(stub.Request.Body, body));
        //         if (st is null) return Results.BadRequest($"Stub File does not contain a stub for this request Uri: {uri} or body {body}");
        //         return Results.Json(st.Response.Body, statusCode: st.Response.Status);
        //     }
        //     catch (Exception ex)
        //     {
        //         return Results.BadRequest(ex.Message);
        //     }
        // }

        // [HttpPut("{serviceName}/stubs/{*path}")]
        // [EndpointName("Ubiquo PUT Mock From Stubs V2")]
        // [EndpointDescription("Get the 'PUT' response associated to the sent request; these information are retrieved from stubs file")]
        // public async Task<IResult> PutResponseStub(string serviceName, string path, [FromBody] JsonNode? body)
        // {
        //     string requestMethod = accessor.HttpContext.Request.Method;
        //     var uri = requestUtil.GetUriFromRequest(path, accessor.HttpContext.Request.QueryString);
        //     try
        //     {
        //         StubDto? st = await stubSelector.SelectStub(stub =>
        //             stub.ServiceName == serviceName && stub.Request.Uri == uri && stub.Request.Method == requestMethod && BodyUtil.GptIsJsonSubset(stub.Request.Body, body));
        //         if (st is null) return Results.BadRequest($"Stub File does not contain a stub for this request Uri: {uri} or body {body}");
        //         return Results.Json(st.Response.Body, statusCode: st.Response.Status);
        //     }
        //     catch (Exception ex)
        //     {
        //         return Results.BadRequest(ex.Message);
        //     }
        // }
        //TODO: add Delete and Patch requests handlers

    }
}
