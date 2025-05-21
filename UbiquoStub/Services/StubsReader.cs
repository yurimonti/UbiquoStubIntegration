using System.Text.Json;
using System.Text.Json.Nodes;
using UbiquoStub.Abstractions;
using UbiquoStub.Models.DTOs;

namespace UbiquoStub.Services
{
    public class StubsReader : IStubsReader
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }

        public async Task<IEnumerable<StubDto>?> GetStubs()
        {
            string json = await File.ReadAllTextAsync(GetCompletePath());
            IEnumerable<StubDto>? result = json.Length == 0 ? [] : JsonSerializer.Deserialize<IEnumerable<StubDto>?>(json,JsonSerializerOptions.Web);
            return result;
        }

        private string GetCompletePath() => Path.Combine(Directory.GetCurrentDirectory(), FilePath, FileName);
    }
}
