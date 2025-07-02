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
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;
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
builder.Services.AddSingleton<IEntityToDtoConverter<RequestEntity, ReqDto>, RequestDeserializer>();
builder.Services.AddSingleton<IEntityToDtoConverter<ResponseEntity, ResDto>, ResponseDeserializer>();
builder.Services.AddSingleton<IEntityToDtoConverter<Sut, SutDto>, SutConverter>();
builder.Services.AddSingleton<IEntityToDtoConverter<StubResult, StubResultDto>,StubResultConverter>();
builder.Services.AddSingleton<IEntityToDtoConverter<Stub, NewStubDto>, StubConverter>();

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

var serializerOptions = JsonSerializerOptions.Web;
var jsonNodeOptions = new JsonNodeOptions() { PropertyNameCaseInsensitive = true };

Func<string, QueryString, string> GetUriFromRequest = (path, query) => string.Concat("/", path, query.ToString());

app.Run();


record JsonFileDto(string FileName, TestSuite Suite);
//record JsonStubsDto(string FileName, StubDto Stub);

record InputDto(string Type, string Value);
record OutputDto(string Type, object Value);
record MockBehaviorDto(InputDto[] Input, OutputDto Output, string ServiceName, string ServiceApi);
record IntegrationResponse(bool IsPassed, string Actual, string Expected);