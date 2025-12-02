namespace backend.Mappers.Shared;

public interface IGetMapper<TModel, TGetDto>
{
    TGetDto Map(TModel model);
}