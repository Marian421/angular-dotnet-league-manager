namespace backend.Mappers.Shared;

public interface INewMapper<TSource, TDestination>
{
    TDestination Map(TSource source);
}