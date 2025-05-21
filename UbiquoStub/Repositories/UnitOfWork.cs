using System;
using UbiquoStub.Abstractions.Repositories;
using UbiquoStub.Data;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;


    public IRepository<RequestEntity> RequestRepository {get;}

    public IRepository<ResponseEntity> ResponseRepository {get;}

    public IRepository<Stub> StubRepository {get;}

    //public IRepository<Test> TestRepository {get;}

    public IRepository<Sut> SutRepository {get;}

    public IRepository<StubResult> StubResultRepository {get;}

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        RequestRepository = new Repository<RequestEntity>(context);
        ResponseRepository = new Repository<ResponseEntity>(context);
        StubRepository = new Repository<Stub>(context);
        //TestRepository = new Repository<Test>(context);
        SutRepository = new Repository<Sut>(context);
        StubResultRepository = new Repository<StubResult>(context);
    }

    public void Dispose()=> _context.Dispose();

    public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
    
}