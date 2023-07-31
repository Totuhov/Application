
namespace Application.Services.Mapping;

using AutoMapper;

/// <summary>
/// This whole project is made from Nikolay Kostov https://github.com/NikolayIT and add an AutoMapper to the solution
/// </summary>
public interface IHaveCustomMappings
{
    void CreateMappings(IProfileExpression configuration);
}
