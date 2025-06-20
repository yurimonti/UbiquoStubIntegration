using System;
using System.Linq.Expressions;
using System.Text.Json;
using UbiquoStub.Abstractions.Repositories;
using UbiquoStub.Abstractions.Services;
using UbiquoStub.Abstractions.Utils;
using UbiquoStub.Exceptions;
using UbiquoStub.Models.DTOs.Stubs;
using UbiquoStub.Models.Entities;

namespace UbiquoStub.Services;

public class StubService(
    IUnitOfWork unitOfWork,
    IRequestConverter requestConverter,
    IResponseConverter responseConverter,
    IEntityToDtoConverter<RequestEntity, ReqDto> requestDtoConverter,
    IEntityToDtoConverter<ResponseEntity, ResDto> responseDtoConverter,
    ILogger<StubService> logger) : IStubService
{

    public async Task<Sut> GetSutAsync(Expression<Func<Sut,bool>> filter = null, bool? withRelations = null) {
        string includePath = withRelations is true ? "Stubs,Stubs.Request,Stubs.Response" : "";
        var results = await unitOfWork.SutRepository.Get(filter, null,
            includePath);
        return results.Any() ? results.First() : throw new NoSutFoundException("No Service Under Test found");
    }

    public async Task<IEnumerable<Stub>> GetStubsAsync(Expression<Func<Stub,bool>> filter = null, bool? withRelations = null)
    {
        string includePath = withRelations is true ? "Request,Response" : "";
        var results = await unitOfWork.StubRepository.Get(filter, null,
            includePath);
        return results;
    }

    public async Task<IEnumerable<Sut>> GetSutsAsync(Expression<Func<Sut, bool>> filter = null, bool? withRelations = null)
    {
        string includePath = withRelations is true ? "Stubs,Stubs.Request,Stubs.Response" : "";
        var results = await unitOfWork.SutRepository.Get(filter, null,
            includePath);
        return results;
    }
    public async Task<Sut> AddStub(string sutName, IEnumerable<NewStubDto> stubs)
    {
        var suts = await GetSutsAsync(s => s.Name == sutName, true);
        bool sutExists = suts.Count() != 0;
        Sut sut = sutExists ? suts.First() : new Sut() { Name = sutName, Stubs = [] };
        IList<Stub> stubList = sut.Stubs?.ToList() ?? new List<Stub>();
        foreach (NewStubDto stub in stubs)
        {
            Stub stubToAdd = new Stub()
            {
                Host = stub.host,
                Name = stub.name,
                TestName = stub.testName,
                Order = stub.order,
                Request = requestConverter.DtoToEntity(stub.request),
                Response = responseConverter.DtoToEntity(stub.response),
            };
            if (!SutAlreadyContainsAStub(stubList, stubToAdd)) stubList.Add(stubToAdd);
            else OverrideStub(stubList, stubToAdd);
            //if(!stubList.Select(s => JsonSerializer.Serialize(s)).Contains(JsonSerializer.Serialize(stubToAdd)))
            //    stubList.Add(stubToAdd);
        }
        sut.Stubs = stubList;
        if (sutExists) unitOfWork.SutRepository.Update(sut);
        else await unitOfWork.SutRepository.Insert(sut);
        await unitOfWork.SaveAsync();
        return sut;
    }

    public async Task DeleteStubsByIds(long sutId, long[] ids)
    {
        Sut? sut = await GetSutAsync(s => s.Id == sutId, true);
        bool idsEmpty = ids.Count() == 0;
        if (idsEmpty) await unitOfWork.SutRepository.Delete(sutId);
        else
        {
            foreach (long id in ids)
            {
                if (sut.Stubs.Select(s => s.Id).Contains(id))
                    await unitOfWork.StubRepository.Delete(id);
            }
            unitOfWork.SutRepository.Update(sut);
        }
        await unitOfWork.SaveAsync();
    }

    private Stub? StubToOverride(IList<Stub> stubList, Stub toCheck)
    {
        return stubList.FirstOrDefault(s =>
                    s.Host == toCheck.Host && s.Name == toCheck.Name &&
                    s.TestName == toCheck.TestName &&
                    requestDtoConverter.Convert(s.Request) == requestDtoConverter.Convert(toCheck.Request));
    }

    private bool SutAlreadyContainsAStub(IList<Stub> stubList, Stub toCheck)
    {
        bool isInList = StubToOverride(stubList,toCheck) is not null;
        return isInList;
    }

    private void OverrideStub(IList<Stub> stubList, Stub overrideWith)
    {
        Stub? toOverride = StubToOverride(stubList, overrideWith);
        if(toOverride is not null)
        {
            int toOverrideIndex = stubList.IndexOf(toOverride);
            if (toOverrideIndex > -1)
            {
                stubList[toOverrideIndex] = overrideWith;
            }
        }
        
    }
}
