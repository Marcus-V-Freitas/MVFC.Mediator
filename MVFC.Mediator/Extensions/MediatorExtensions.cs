namespace MVFC.Mediator.Extensions;

/// <summary>
/// Métodos de extensão para registro e configuração do Mediator na injeção de dependência.
/// </summary>
public static class MediatorExtensions
{
    /// <summary>
    /// Adiciona o Mediator e registra automaticamente todos os manipuladores de comandos encontrados nos assemblies informados.
    /// </summary>
    /// <param name="services">A coleção de serviços para registro das dependências.</param>
    /// <param name="assemblies">Assemblies a serem escaneados em busca de handlers. Se nenhum for informado, utiliza o assembly de chamada.</param>
    /// <returns>A própria coleção de serviços, para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Lançada se <paramref name="services"/> for nulo.</exception>
    public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IMediator, Mediator>();

        var assembliesToScan = assemblies.Length == 0
            ? [Assembly.GetCallingAssembly()]
            : assemblies;

        foreach (var assembly in assembliesToScan)
        {
            RegisterHandlers(services, assembly);
        }

        return services;
    }

    /// <summary>
    /// Registra todos os manipuladores de comandos (handlers) encontrados no assembly informado.
    /// </summary>
    /// <param name="services">A coleção de serviços para registro das dependências.</param>
    /// <param name="assembly">Assembly a ser escaneado em busca de handlers.</param>
    private static void RegisterHandlers(IServiceCollection services, Assembly assembly)
    {
        var voidHandlerTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                .Select(i => new { Interface = i, Implementation = t }));

        foreach (var handler in voidHandlerTypes)
        {
            services.AddScoped(handler.Interface, handler.Implementation);
        }

        var responseHandlerTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))
                .Select(i => new { Interface = i, Implementation = t }));

        foreach (var handler in responseHandlerTypes)
        {
            services.AddScoped(handler.Interface, handler.Implementation);
        }
    }
}