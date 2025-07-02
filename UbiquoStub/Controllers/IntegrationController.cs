using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using UbiquoStub.Abstractions;
using UbiquoStub.Abstractions.Repositories;
using UbiquoStub.Abstractions.Stubs;
using UbiquoStub.Abstractions.Utils;
using UbiquoStub.Exceptions;
using UbiquoStub.Models.DTOs;
using UbiquoStub.Models.DTOs.Errors;
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;
using UbiquoStub.Services;
using UbiquoStub.Utils;

namespace UbiquoStub.Controllers
{
    [Route("api/v2")]
    [ApiController]
    public class IntegrationController(IHttpContextAccessor accessor, IStubsSelector stubSelector,
        IResponseConverter responseConverter, IUnitOfWork unitOfWork,
        IResponseUtil responseUtil,
        IRequestUtil requestUtil, 
        IEntityToDtoConverter<Stub, NewStubDto> stubConverter, ILogger<IntegrationController> logger) : ControllerBase
    {
        private JsonNodeOptions _jsonNodeOptions = new JsonNodeOptions() { PropertyNameCaseInsensitive = true };

        private async Task<IResult> Integration(string sutName, string serviceName, string requestMethod,
        IHeaderDictionary requestHeaders, string uri, JsonNode? body)
        {
            var responsesComparator = new ResponsesComparator();
            // Get TestStub From the running test by using filter
            var testStub = body is not null ?
                await stubSelector.SelectStub(sutName, stub =>
                    stub.Name == serviceName && stub.Request.Method == requestMethod && stub.Request.Uri == uri
                    && requestUtil.RequestHeaderContainsHeadersDto(requestHeaders, stub.Request?.Headers)
                    && JsonNode.DeepEquals(stub.Request.Body, body)) :
                await stubSelector.SelectStub(sutName, stub => stub.Name == serviceName
                    && stub.Request.Method == requestMethod && stub.Request.Uri == uri
                    && requestUtil.RequestHeaderContainsHeadersDto(requestHeaders, stub.Request?.Headers));

            HttpResponseMessage expected = responseConverter.Deserialize(testStub.Response);
            using var client = new HttpClient()
            {
                BaseAddress = new Uri(testStub.Host)
            };
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Parse(testStub.Request.Method), testStub.Request.Uri);
            // Add body to request if exists
            if (body is not null) request.Content = new StringContent(body.ToString(),Encoding.UTF8,"application/json");
            logger.LogCritical($"request content is \n{body}");
            //Get actual response
            HttpResponseMessage actual = await client.SendAsync(request);
            ResDto actualResponse = await responseConverter.DeserializeResponseMessageToDto(actual);
            string result = await actual.Content.ReadAsStringAsync();
            JsonNode? serialization = (result == "" || result == null) ? null : JsonNode.Parse(result, _jsonNodeOptions);
            logger.LogCritical($"actual response body is {serialization}");
            bool error = false;
            try
            {
                await responsesComparator.CompareResponses(actual, expected);
            }
            catch (Exception)
            {
                error = true;
            }
            StubResult sr = new StubResult()
            {
                IsIntegration = true,
                Status = error is true ? TestStatus.FAILED : TestStatus.PASSED,
                Stub = testStub,
                StubDto = stubConverter.Convert(testStub),
                ActualResponse = actualResponse
            };
            await unitOfWork.StubResultRepository.Insert(sr);
            await unitOfWork.SaveAsync();
            responseUtil.AddHeaderToHttpContextResponse(HttpContext.Response, actual);
            //var deserialized = JsonConvert.DeserializeObject<>(result);
            var statusCode = (int)actual.StatusCode;
            logger.LogInformation($"the integration response is \n{new { body = serialization, status = statusCode }}");
            return Results.Json(serialization, statusCode: statusCode);
        }


        [HttpGet("{sutName}/{serviceName}/integration/{*path}")]
        [EndpointName("Ubiquo GET Integration V2")]
        public async Task<IResult> GetIntegration(string sutName, string serviceName,
            string? path = null)
        {
            string requestMethod = HttpContext.Request.Method;
            var requestHeaders = HttpContext.Request.Headers;
            var uri = requestUtil.GetUriFromRequest(path, HttpContext.Request.QueryString);
            try
            {
                IResult result = await Integration(sutName, serviceName, requestMethod, requestHeaders, uri, null);
                return result;
            }
            catch (NoSutFoundException ex)
            {
                return Results.NotFound(ex.Message);
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

        [HttpPost("{sutName}/{serviceName}/integration/{*path}")]
        [EndpointName("Ubiquo POST Integration V2")]
        public async Task<IResult> PostIntegration(string sutName, string serviceName, 
            string? path = null, [FromBody] JsonNode? body = null)
        {
            string requestMethod = HttpContext.Request.Method;
            var requestHeaders = HttpContext.Request.Headers;
            var uri = requestUtil.GetUriFromRequest(path, HttpContext.Request.QueryString);
            try
            {
                IResult result = await Integration(sutName, serviceName, requestMethod, requestHeaders, uri, body);
                return result;
            }
            catch (NoSutFoundException ex)
            {
                return Results.NotFound(ex.Message);
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

        [HttpDelete("{sutName}/{serviceName}/integration/{*path}")]
        [EndpointName("Ubiquo DELETE Integration V2")]
        public async Task<IResult> DeleteResponseIntegration(string sutName, string serviceName, 
            string? path = null, [FromBody] JsonNode? body = null)
        {
            string requestMethod = accessor.HttpContext.Request.Method;
            var requestHeaders = accessor.HttpContext.Request.Headers;
            var uri = requestUtil.GetUriFromRequest(path, accessor.HttpContext.Request.QueryString);
            try
            {
                IResult result = await Integration(sutName, serviceName, requestMethod, requestHeaders, uri, body);
                return result;
            }
            catch (NoSutFoundException ex)
            {
                return Results.NotFound(ex.Message);
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

        [HttpPatch("{sutName}/{serviceName}/integration/{*path}")]
        [EndpointName("Ubiquo PATCH Integration V2")]
        public async Task<IResult> PatchResponseIntegration(string sutName, string serviceName, 
            string? path = null, [FromBody] JsonNode? body = null)
        {
            string requestMethod = accessor.HttpContext.Request.Method;
            var requestHeaders = accessor.HttpContext.Request.Headers;
            var uri = requestUtil.GetUriFromRequest(path, accessor.HttpContext.Request.QueryString);
            try
            {
                IResult result = await Integration(sutName, serviceName, requestMethod, requestHeaders, uri, body);
                return result;
            }
            catch (NoSutFoundException ex)
            {
                return Results.NotFound(ex.Message);
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

        [HttpPut("{sutName}/{serviceName}/integration/{*path}")]
        [EndpointName("Ubiquo PUT Integration V2")]
        public async Task<IResult> PutResponseIntegration(string sutName, string serviceName, 
            string? path = null, [FromBody] JsonNode? body = null)
        {
            string requestMethod = accessor.HttpContext.Request.Method;
            var requestHeaders = accessor.HttpContext.Request.Headers;
            var uri = requestUtil.GetUriFromRequest(path, accessor.HttpContext.Request.QueryString);
            try
            {
                IResult result = await Integration(sutName, serviceName, requestMethod, requestHeaders, uri, body);
                return result;
            }
            catch (NoSutFoundException ex)
            {
                return Results.NotFound(ex.Message);
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

        // [HttpGet("{serviceName}/boh/integration/{*path}")]
        // [EndpointName("Ubiquo GET Integration V2")]
        // public async Task<IResult> GetResponseIntegration(string serviceName, string path)
        // {
        //     string requestMethod = accessor.HttpContext.Request.Method;
        //     var responsesComparator = new ResponsesComparator();
        //     var uri = requestUtil.GetUriFromRequest(path, accessor.HttpContext.Request.QueryString);
        //     try
        //     {
        //         StubDto? st = await stubSelector.SelectStub(stub => stub.ServiceName == serviceName && stub.Request.Method == requestMethod && stub.Request.Uri == uri);
        //         if (st is null) return Results.BadRequest($"Stub File does not contain a stub for this request Uri: {path} or method {requestMethod}");
        //         HttpResponseMessage expected = responseConverter.Deserialize(st.Response);
        //         using var client = new HttpClient()
        //         {
        //             BaseAddress = new Uri(st.Request.Host)
        //         };
        //         HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Parse(st.Request.Method), st.Request.Uri);
        //         HttpResponseMessage actual = await client.SendAsync(request);
        //         string result = await actual.Content.ReadAsStringAsync();
        //         JsonNode? serialization = JsonNode.Parse(result, _jsonNodeOptions);
        //         try
        //         {
        //             await responsesComparator.CompareResponses(actual, expected);
        //         }
        //         catch (Exception ex)
        //         {
        //             ComparisonError error = new ComparisonError()
        //             {
        //                 Actual = serialization!,
        //                 Expected = st.Response.Body!
        //             };
        //             logger.LogCritical(JsonSerializer.Serialize(error, JsonSerializerOptions.Web));
        //             //TODO: set if a bad request is preferred instead logging integration error only
        //             //return Results.BadRequest(error);
        //         }
        //         //var deserialized = JsonConvert.DeserializeObject<>(result);
        //         var statusCode = (int)actual.StatusCode;
        //         logger.LogInformation($"the GET integration response is \n{new {body=serialization, status=statusCode}}");
        //         return Results.Json(serialization, statusCode: statusCode);
        //     }
        //     catch (Exception ex)
        //     {
        //         return Results.BadRequest(ex.Message);
        //     }
        // }

        // [HttpGet("{serviceName}/integration/{*path}")]
        // [EndpointName("Ubiquo GET Integration V2")]
        // public async Task<IResult> GetResponseIntegration(string serviceName, string path)
        // {
        //     string requestMethod = accessor.HttpContext.Request.Method;
        //     var responsesComparator = new ResponsesComparator();
        //     var responseDeserializer = new ResponseDeserializer();
        //     var uri = requestUtil.GetUriFromRequest(path, accessor.HttpContext.Request.QueryString);
        //     try
        //     {
        //         StubDto? st = await stubSelector.SelectStub(stub => stub.ServiceName == serviceName && stub.Request.Method == requestMethod && stub.Request.Uri == uri);
        //         if (st is null) return Results.BadRequest($"Stub File does not contain a stub for this request Uri: {path} or method {requestMethod}");
        //         HttpResponseMessage expected = responseDeserializer.Deserialize(st.Response);
        //         using var client = new HttpClient()
        //         {
        //             BaseAddress = new Uri(st.Request.Host)
        //         };
        //         HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Parse(st.Request.Method), st.Request.Uri);
        //         HttpResponseMessage actual = await client.SendAsync(request);
        //         string result = await actual.Content.ReadAsStringAsync();
        //         JsonNode? serialization = JsonNode.Parse(result, _jsonNodeOptions);
        //         try
        //         {
        //             await responsesComparator.CompareResponses(actual, expected);
        //         }
        //         catch (Exception ex)
        //         {
        //             ComparisonError error = new ComparisonError()
        //             {
        //                 Actual = serialization!,
        //                 Expected = st.Response.Body!
        //             };
        //             logger.LogCritical(JsonSerializer.Serialize(error, JsonSerializerOptions.Web));
        //             //TODO: set if a bad request is preferred instead logging integration error only
        //             //return Results.BadRequest(error);
        //         }
        //         //var deserialized = JsonConvert.DeserializeObject<>(result);
        //         var statusCode = (int)actual.StatusCode;
        //         logger.LogInformation($"the GET integration response is \n{new {body=serialization, status=statusCode}}");
        //         return Results.Json(serialization, statusCode: statusCode);
        //     }
        //     catch (Exception ex)
        //     {
        //         return Results.BadRequest(ex.Message);
        //     }
        // }

        // [HttpPost("{serviceName}/integration/{*path}")]
        // [EndpointName("Ubiquo POST Integration V2")]
        // public async Task<IResult> PostResponseIntegration(string serviceName, string path, [FromBody] JsonNode? body = null)
        // {
        //     string requestMethod = accessor.HttpContext.Request.Method;
        //     var responsesComparator = new ResponsesComparator();
        //     var responseDeserializer = new ResponseDeserializer();
        //     var uri = requestUtil.GetUriFromRequest(path, accessor.HttpContext.Request.QueryString);
        //     try
        //     {
        //         StubDto? st = await stubSelector.SelectStub(stub =>
        //             stub.ServiceName == serviceName && stub.Request.Method == requestMethod && stub.Request.Uri == uri && JsonNode.DeepEquals(stub.Request.Body, body));
        //         if (st is null) return Results.BadRequest($"Stub File does not contain a stub for this request Uri: {uri} or body {body}");
        //         HttpResponseMessage expected = responseDeserializer.Deserialize(st.Response);
        //         using var client = new HttpClient()
        //         {
        //             BaseAddress = new Uri(st.Request.Host)
        //         };
        //         HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Parse(st.Request.Method), st.Request.Uri)
        //         {
        //             Content = body is not null ? new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json") : null
        //         };
        //         HttpResponseMessage actual = await client.SendAsync(request);
        //         string result = await actual.Content.ReadAsStringAsync();
        //         logger.LogInformation($"the actual response content is {result}");
        //         JsonNode? serialization = JsonNode.Parse(result, _jsonNodeOptions);
        //         logger.LogInformation($"the actual response body is {serialization}");
        //         try
        //         {
        //             await responsesComparator.CompareResponses(actual, expected);
        //         }
        //         catch (Exception)
        //         {
        //             return Results.BadRequest(new ComparisonError()
        //             {
        //                 Actual = serialization!,
        //                 Expected = st.Response.Body!
        //             });
        //         }
        //         //var deserialized = JsonConvert.DeserializeObject<>(result);
        //         return Results.Json(serialization, statusCode: ((int)actual.StatusCode));
        //     }
        //     catch (Exception ex)
        //     {
        //         return Results.BadRequest(ex.Message);
        //     }
        // }
    }
}
