using UbiquoStub.Abstractions;
using UbiquoStub.Abstractions.Services;
using UbiquoStub.Abstractions.Stubs;
using UbiquoStub.Exceptions;
using UbiquoStub.Models.DTOs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Services.Stubs
{
    public class StubsSelector(IStubsReader stubReader,IStubService stubService) : IStubsSelector
    {

        public async Task<StubDto?> SelectStub(string serviceName, string path, HttpContext context)
        {
            string requestMethod = context.Request.Method;
            IEnumerable<StubDto>? stubsFromFile = await stubReader.GetStubs();
            if (stubsFromFile is null) throw new Exception($"Stub File does not exist");
            //string requestMethod = context.Request.Method;
            StubDto? selectedStub = stubsFromFile.Where(stub => stub.ServiceName == serviceName && stub.Request.Method == requestMethod && stub.Request.Uri == "/" + path).FirstOrDefault();
            return selectedStub;
        }

        public async Task<StubDto?> SelectStub(Func<StubDto, bool> predicate)
        {
            IEnumerable<StubDto>? stubsFromFile = await stubReader.GetStubs();
            if (stubsFromFile is null) throw new Exception($"Stub File does not exist");
            //string requestMethod = context.Request.Method;
            StubDto? selectedStub = stubsFromFile.Where(predicate).FirstOrDefault();
            return selectedStub;
        }

        public async Task<Stub> SelectStub(string sutName, Func<Stub, bool> predicate)
        {
            Sut sut = await stubService.GetSutAsync(s => s.Name == sutName, true);
            var stubs = sut.Stubs;
            var stub = stubs.FirstOrDefault(predicate);
            if (stub is null) throw new NoStubInSUTException("Stub in SUT does not exist");
            return stub;
        }
    }
}
