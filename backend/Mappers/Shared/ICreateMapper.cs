namespace backend.Mappers.Shared;

public interface ICreateMapper<TModel, TCreateDto>
{
    TModel Map(TCreateDto model);
}