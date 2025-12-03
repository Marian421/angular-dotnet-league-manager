namespace backend.Mappers.Shared;

public interface IMapper<TSource, TDestination>
{
    TDestination Map(TSource model);
}