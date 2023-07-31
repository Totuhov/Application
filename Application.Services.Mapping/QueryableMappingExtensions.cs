
namespace Application.Services.Mapping;

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using AutoMapper.QueryableExtensions;

/// <summary>
/// This whole project is made from Nikolay Kostov https://github.com/NikolayIT and add an AutoMapper to the solution
/// </summary>
[ExcludeFromCodeCoverage]
public static class QueryableMappingExtensions
{
    public static IQueryable<TDestination> To<TDestination>(
        this IQueryable source,
        params Expression<Func<TDestination, object>>[] membersToExpand)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return source.ProjectTo(AutoMapperConfig.MapperInstance.ConfigurationProvider, null, membersToExpand);
    }

    public static IQueryable<TDestination> To<TDestination>(
        this IQueryable source,
        object parameters)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return source.ProjectTo<TDestination>(AutoMapperConfig.MapperInstance.ConfigurationProvider, parameters);
    }
}
