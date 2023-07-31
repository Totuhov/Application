
namespace Application.Web.Infrastructure.Extensions;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// This class is taken from one of the SoftUni Bulgaria Workshops with dozent Cristian Ivanov
/// and is used to configure service dependances in the application
/// </summary>
[ExcludeFromCodeCoverage]
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// This method registers all services with their interfaces and implementations of given assembly.
    /// The assembly is taken from the type of random service interface or implementation provided.
    /// </summary>
    /// <param name="serviceType"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void AddApplicationServices(this IServiceCollection services, Type serviceType)
    {
        Assembly? serviceAssembly = Assembly.GetAssembly(serviceType);
        if (serviceAssembly == null)
        {
            throw new InvalidOperationException("Invalid service type provided!");
        }

        Type[] implementationTypes = serviceAssembly
            .GetTypes()
            .Where(t => t.Name.EndsWith("Service") && !t.IsInterface)
            .ToArray();
        foreach (Type implementationType in implementationTypes)
        {
            Type? interfaceType = implementationType
                .GetInterface($"I{implementationType.Name}");
            if (interfaceType == null)
            {
                throw new InvalidOperationException(
                    $"No interface is provided for the service with name: {implementationType.Name}");
            }

            services.AddScoped(interfaceType, implementationType);
        }
    }       
}