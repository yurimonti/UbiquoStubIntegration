using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Pre-convention model configuration goes here
        configurationBuilder.Properties<TestStatus>().HaveConversion<string>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        var serializerOptions = JsonSerializerOptions.Web;
        var jsonNodeOptions = new JsonNodeOptions() { PropertyNameCaseInsensitive = true };

        var jsonNodeConverter = new ValueConverter<JsonNode, string>(
            j => j.ToJsonString(serializerOptions),
            j => JsonNode.Parse(j, jsonNodeOptions, default)!
        );

        var resDtoConverter = new ValueConverter<ResDto, string>(
            r => JsonSerializer.Serialize(r, serializerOptions),
            r => JsonSerializer.Deserialize<ResDto>(r, serializerOptions)!
        );

        var dictConverter = new ValueConverter<IDictionary<string, IEnumerable<string>>, string>(
            v => JsonSerializer.Serialize(v, serializerOptions),
            v => JsonSerializer.Deserialize<IDictionary<string, IEnumerable<string>>>(v, serializerOptions)!
        );
        //RequestEntity binding
        modelBuilder.Entity<RequestEntity>(entity =>
        {
            entity.Property(p => p.Body).HasConversion(jsonNodeConverter).HasColumnType("jsonb");
            entity.Property(p => p.Headers).HasConversion(dictConverter).HasColumnType("jsonb");
        });
        //ResponseEntity binding
        modelBuilder.Entity<ResponseEntity>(entity =>
        {
            entity.Property(p => p.Body).HasConversion(jsonNodeConverter).HasColumnType("jsonb");
            entity.Property(p => p.Headers).HasConversion(dictConverter).HasColumnType("jsonb");
        });
        //Stub binding
        modelBuilder.Entity<Stub>(entity =>
        {
            entity.HasOne(t => t.Request);
            entity.HasOne(t => t.Response);
        });
        //Test binding
        // modelBuilder.Entity<Test>(entity =>
        // {

        //     entity.HasIndex(t => t.TestName).IsUnique();
        //     entity.HasMany(t => t.Stubs).WithOne().OnDelete(DeleteBehavior.Cascade);
        // });
        //SUT binding
        modelBuilder.Entity<Sut>(entity =>
        {
            entity.HasIndex(t => t.Name).IsUnique();
            entity.HasMany(t => t.Stubs).WithOne().OnDelete(DeleteBehavior.Cascade);
        });
        //StubResult binding
        modelBuilder.Entity<StubResult>(entity =>
        {
            entity.HasOne(i => i.Stub);
            entity.Property(p => p.ActualResponse).HasConversion(resDtoConverter).HasColumnType("jsonb");
        });
    }
}