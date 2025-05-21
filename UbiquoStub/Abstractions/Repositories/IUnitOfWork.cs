using System;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Abstractions.Repositories;

public interface IUnitOfWork : IDisposable
{
    IRepository<RequestEntity> RequestRepository { get; }
    IRepository<ResponseEntity> ResponseRepository { get; }
    IRepository<Stub> StubRepository { get; }
    //IRepository<Test> TestRepository { get; }
    IRepository<Sut> SutRepository { get; }
    IRepository<StubResult> StubResultRepository { get; }
    Task<int> SaveAsync();

}
