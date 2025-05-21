using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using UbiquoStub;
using UbiquoStub.Abstractions;
using UbiquoStub.Abstractions.Repositories;
using UbiquoStub.Abstractions.Services;
using UbiquoStub.Abstractions.Stubs;
using UbiquoStub.Abstractions.Utils;
using UbiquoStub.Data;
using UbiquoStub.Models;
using UbiquoStub.Options;
using UbiquoStub.Repositories;
using UbiquoStub.Services;
using UbiquoStub.Services.Stubs;
using UbiquoStub.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<ServicesOptions>(builder.Configuration.GetSection("Services"));

builder.Services.AddSingleton<IStubsReader, StubsReader>(sp =>
    {
        var sr = new StubsReader()
        {
            FileName = "prova.json",
            FilePath = "Files/stubs"
        };
        return sr;
    });
builder.Services.AddSingleton<IStubsWriter, StubsWriter>(sp =>
    {
        var sw = new StubsWriter()
        {
            FileName = "prova.json",
            FilePath = "Files/stubs"
        };
        return sw;
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStubsSelector, StubsSelector>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IStubService, StubService>();

builder.Services.AddSingleton<IRequestUtil, RequestUtil>();
builder.Services.AddSingleton<IResponseUtil, ResponseUtil>();
builder.Services.AddSingleton<IRequestConverter, RequestDeserializer>();
builder.Services.AddSingleton<IResponseConverter, ResponseDeserializer>();
builder.Services.AddSingleton<ISutConverter, SutConverter>();
//builder.Services.AddSingleton<ITestConverter, TestConverter>();
builder.Services.AddSingleton<IStubConverter, StubConverter>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();


var app = builder.Build();

app.MapControllers();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Swagger"));
}

app.UseHttpsRedirection();

// StubsReader sr = new StubsReader() { FileName = "stubs.json", FilePath = "Files/stubs" };
// StubsWriter sw = new StubsWriter() { FileName = "stubs.json", FilePath = "Files/stubs" };

var serializerOptions = JsonSerializerOptions.Web;
var jsonNodeOptions = new JsonNodeOptions() { PropertyNameCaseInsensitive = true };
// var stubSelector = new StubsSelector(sr);

Func<string, QueryString, string> GetUriFromRequest = (path, query) => string.Concat("/", path, query.ToString());

//V2
//GET STUBS
// app.MapGet("api/v2/{serviceName}/stubs/{*path}", async (string serviceName, string path, HttpContext context, IStubsSelector stubSelector, ILogger<Program> logger) =>
// {
//     string requestMethod = context.Request.Method;
//     var uri = GetUriFromRequest(path, context.Request.QueryString);
//     try
//     {
//         StubDto? st = await stubSelector.SelectStub(stub => stub.ServiceName == serviceName && stub.Request.Method == requestMethod && stub.Request.Uri == uri);
//         if (st is null) return Results.BadRequest($"Stub File does not contain a stub for this request Uri: {uri} or method {requestMethod}");
//         return Results.Json(st.Response.Body, statusCode: st.Response.Status);
//     }
//     catch (Exception ex)
//     {
//         return Results.BadRequest(ex.Message);
//     }
// }).WithName("Ubiquo GET Mock From Stubs V2");

//POST STUBS
// app.MapPost("api/v2/{serviceName}/stubs/{*path}", async (string serviceName, string path, [FromBody] JsonNode? body, HttpContext context, IStubsSelector stubSelector, ILogger<Program> logger) =>
// {
//     string requestMethod = context.Request.Method;
//     var uri = GetUriFromRequest(path, context.Request.QueryString);
//     try
//     {
//         StubDto? st = await stubSelector.SelectStub(stub =>
//             stub.ServiceName == serviceName && stub.Request.Uri == uri && stub.Request.Method == requestMethod && stub.Request.Body == body);
//         if (st is null) return Results.BadRequest($"Stub File does not contain a stub for this request Uri: {uri} or body {body}");
//         return Results.Json(st.Response.Body, statusCode: st.Response.Status);
//     }
//     catch (Exception ex)
//     {
//         return Results.BadRequest(ex.Message);
//     }
// }).WithName("Ubiquo POST Mock From Stubs V2");

// //PUT STUBS
// app.MapPut("api/v2/{serviceName}/stubs/{*path}", async (string serviceName, string path, [FromBody] JsonNode? body, HttpContext context, IStubsSelector stubSelector, ILogger<Program> logger) =>
// {
//     string requestMethod = context.Request.Method;
//     var uri = GetUriFromRequest(path, context.Request.QueryString);
//     try
//     {
//         StubDto? st = await stubSelector.SelectStub(stub =>
//             stub.ServiceName == serviceName && stub.Request.Uri == uri && stub.Request.Method == requestMethod && BohClass.GptIsJsonSubset(stub.Request.Body,body));
//         if (st is null) return Results.BadRequest($"Stub File does not contain a stub for this request Uri: {uri} or body {body}");
//         return Results.Json(st.Response.Body, statusCode: st.Response.Status);
//     }
//     catch (Exception ex)
//     {
//         return Results.BadRequest(ex.Message);
//     }
// }).WithName("Ubiquo PUT Mock From Stubs V2");

//GET Integration
// app.MapGet("api/v2/{serviceName}/integration/{*path}", async (string serviceName, string path, HttpContext context, IStubsSelector stubSelector, ILogger<Program> logger) =>
// {
//     string requestMethod = context.Request.Method;
//     var responsesComparator = new ResponsesComparator();
//     var responseDeserializer = new ResponseDeserializer();
//     var uri = GetUriFromRequest(path, context.Request.QueryString);
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
//         JsonNode? serialization = JsonNode.Parse(result, jsonNodeOptions);
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
//             logger.LogCritical(System.Text.Json.JsonSerializer.Serialize(error, serializerOptions));
//             return Results.BadRequest(error);
//         }
//         //var deserialized = JsonConvert.DeserializeObject<>(result);
//         return Results.Json(serialization, statusCode: ((int)actual.StatusCode));
//     }
//     catch (Exception ex)
//     {
//         return Results.BadRequest(ex.Message);
//     }
// }).WithName("Ubiquo GET Integration From Stubs V2");

//POST Integration
// app.MapPost("api/v2/{serviceName}/integration/{*path}", async (string serviceName, string path, HttpContext context, IStubsSelector stubSelector, ILogger<Program> logger,
//         [FromBody] JsonNode? body = null) =>
// {
//     string requestMethod = context.Request.Method;
//     var responsesComparator = new ResponsesComparator();
//     var responseDeserializer = new ResponseDeserializer();
//     var uri = GetUriFromRequest(path, context.Request.QueryString);
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
//         HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Parse(st.Request.Method), st.Request.Uri){
//             Content = body is not null ? new StringContent(body.ToJsonString(), Encoding.UTF8, "application/json") : null
//         };
//         HttpResponseMessage actual = await client.SendAsync(request);
//         string result = await actual.Content.ReadAsStringAsync();
//         logger.LogInformation($"the actual response content is {result}");
//         JsonNode? serialization = JsonNode.Parse(result, jsonNodeOptions);
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
// }).WithName("Ubiquo POST Integration From Stubs V2");


// app.MapGet("api/v2/admin/stubs", async (IStubsReader sr) =>
// {
//     var result = await sr.GetStubs();
//     return Results.Ok(result);
// }).WithName("Admin Get Stubs");

// app.MapPost("api/v2/admin/stubs", async ([FromBody] StubDto body, IStubsReader sr, IStubsWriter sw, ILogger<Program> logger) =>
// {

//     IEnumerable<StubDto>? fileJson = await sr.GetStubs();
//     IEnumerable<StubDto> toWrite;
//     var stub = fileJson.FirstOrDefault(s => s.Request == body.Request);
//     if (stub is null) toWrite = fileJson?.Append(body);
//     else
//     {
//         stub = body;
//         toWrite = fileJson;
//     }
//     ;
//     //var toWrite = fileJson?.Append(body);
//     logger.LogInformation($"The writing will be:\n{toWrite}");
//     await sw.SetStubs(toWrite!);
//     return Results.Ok(System.Text.Json.JsonSerializer.Serialize(toWrite));
// }).WithName("Admin Stubs upload");

// app.MapDelete("api/v2/admin/stubs", async (IStubsReader sr, IStubsWriter sw, ILogger<Program> logger) =>
// {

//     IEnumerable<StubDto>? fileJson = await sr.GetStubs();
//     await sw.SetStubs([]);

//     return Results.Ok("Stubs Deleted");
// }).WithName("Admin Stubs deletion");

//V1
// app.MapGet("ubiquo/{*path}", async (string path, HttpContext context, ILogger<Program> logger) =>
// {
//     using var client = new HttpClient()
//     {
//         BaseAddress = new Uri("http://localhost:8080")
//     };
//     var query = context.Request.QueryString.ToUriComponent();
//     var api = string.Concat("/", path, query);
//     var method = context.Request.Method;
//     var serviceUri = new Uri("http://localhost:8080" + api);
//     var request = new HttpRequestMessage(HttpMethod.Parse(method), serviceUri);
//     var res = await client.SendAsync(request);
//     var result = await res.Content.ReadAsStringAsync();
//     var serialization = JsonNode.Parse(result);
//     //var deserialized = JsonConvert.DeserializeObject<>(result);
//     return Results.Ok(serialization);
//     //var response = await client.GetAsync($"http://localhost:8080{path}");
// });

//TODO: to implement
// app.MapPost("ubiquo/{*path}", async (string path, HttpContext context, ILogger<Program> logger)=>{

// });
// app.MapDelete("ubiquo/{*path}", async (string path, HttpContext context, ILogger<Program> logger)=>{

// });
// app.MapPatch("ubiquo/{*path}", async (string path, HttpContext context, ILogger<Program> logger)=>{

// });
// app.MapPut("ubiquo/{*path}", async (string path, HttpContext context, ILogger<Program> logger)=>{

// });

// app.MapGet("/api/file", async () =>
// {
//     string json = await File.ReadAllTextAsync(FilePath("Files", "test-suite.json"));
//     var result = JsonConvert.DeserializeObject<TestSuite>(json);
//     return Results.Ok(result);
// }).WithName("File");

// app.MapPost("/api/file", async ([FromBody] JsonFileDto body) =>
// {
//     var result = JsonConvert.SerializeObject(body.Suite);
//     await File.WriteAllTextAsync(FilePath("Files", $"{body.FileName}.json"), result);
//     return Results.Ok(result);
// }).WithName("File Upload");

// app.MapPost("/api", async (IOptionsMonitor<ServicesOptions> monitor, [FromBody] MockBehaviorDto mockRequest, ILogger<Program> logger) =>
// {
//     var serviceName = mockRequest.ServiceName;
//     logger.LogInformation($"service Name is {serviceName}");
//     ServiceOption? service = monitor.CurrentValue.Values.FirstOrDefault(option => option.Name.Equals(serviceName));
//     logger.LogInformation($"service is {service}");
//     if (service is null) return Results.BadRequest($"The service {serviceName} does not exist");
//     var output = await SendRequest(ComposeUrlService(service) + mockRequest.ServiceApi);
//     logger.LogInformation($"real output is returned from Service: {output}");
//     var serializedMockOutput = JsonConvert.SerializeObject(mockRequest.Output.Value);

//     var assertion = output.Equals(mockRequest.Output.Value);
//     return Results.Ok(new IntegrationResponse(IsPassed: assertion, Actual: output, Expected: serializedMockOutput));
// })
// .WithName("api");

app.Run();

// string FilePath(string directory, string fileName) => Path.Combine(Directory.GetCurrentDirectory(), directory, fileName);

// string ComposeUrlService(ServiceOption service) => $"{service.Protocol}://{service.Host}:{service.Port}";

// async Task<string> SendRequest(string url)
// {
//     HttpClient client = new HttpClient();
//     var res = await client.GetAsync(url);
//     var result = await res.Content.ReadAsStringAsync();
//     return result;
// }

record JsonFileDto(string FileName, TestSuite Suite);
//record JsonStubsDto(string FileName, StubDto Stub);

record InputDto(string Type, string Value);
record OutputDto(string Type, object Value);
record MockBehaviorDto(InputDto[] Input, OutputDto Output, string ServiceName, string ServiceApi);
record IntegrationResponse(bool IsPassed, string Actual, string Expected);