using System;

namespace UbiquoStub.Abstractions.Utils;

public interface IEntityToDtoConverter<TEntity, TDto> where TEntity : class where TDto : class
{
    TDto Convert(TEntity entityToConvert);
}
