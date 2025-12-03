namespace backend.Mappers.Shared;

public interface IExistingMapper<TSource, TDestination>
{
    void Map(TSource source, TDestination destination);
}