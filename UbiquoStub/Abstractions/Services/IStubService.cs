using System;
using System.Linq.Expressions;
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Abstractions.Services;

public interface IStubService
{
    Task<IEnumerable<Sut>> GetSutsAsync(Expression<Func<Sut, bool>> filter = null, bool? withRelations = null);
    Task<IEnumerable<Stub>> GetStubsAsync(Expression<Func<Stub, bool>> filter = null, bool? withRelations = null);
    Task<Sut> GetSutAsync(Expression<Func<Sut, bool>> filter = null, bool? withRelations = null);
    Task<SutDto> AddStub(string sutName, IEnumerable<NewStubDto> stubs);
    Task DeleteStubsByIds(long sutId, long[] ids);
}
