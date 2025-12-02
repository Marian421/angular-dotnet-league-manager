namespace backend.Mappers.Shared;

public interface IUpdateMapper<TModel, TUpdateDto>
{
    void Update(TUpdateDto dto, TModel model)
    {
        
    }
}