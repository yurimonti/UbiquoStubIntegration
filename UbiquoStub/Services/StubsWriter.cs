using System.Text.Json;
using UbiquoStub.Abstractions.Stubs;
using UbiquoStub.Models.DTOs;

namespace UbiquoStub.Services
{
    public class StubsWriter : IStubsWriter
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }

        public async Task SetStubs(IEnumerable<StubDto> stubs)
        {
            string toWrite = JsonSerializer.Serialize(stubs, JsonSerializerOptions.Web);
            await File.WriteAllTextAsync(GetCompletePath(),toWrite);   
        }

        private string GetCompletePath() => Path.Combine(Directory.GetCurrentDirectory(), FilePath, FileName);
    }
}
