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
        ArgumentNullException.ThrowIfNull(assemblies);

        var assembliesToScan = assemblies.Length == 0
            ? [Assembly.GetCallingAssembly()]
            : assemblies;

        return AddMediatorInternal(services, filter: null, assembliesToScan);
    }

    /// <summary>
    /// Adiciona o Mediator e registra automaticamente apenas os manipuladores de comandos que atendam ao critério do filtro.
    /// </summary>
    /// <param name="services">A coleção de serviços para registro das dependências.</param>
    /// <param name="filter">Filtro opcional para registrar apenas os tipos que satisfaçam a condição.</param>
    /// <param name="assemblies">Assemblies a serem escaneados em busca de handlers. Se nenhum for informado, utiliza o assembly de chamada.</param>
    /// <returns>A própria coleção de serviços, para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Lançada se <paramref name="services"/> for nulo.</exception>
    public static IServiceCollection AddMediator(this IServiceCollection services, Func<Type, bool>? filter, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assemblies);

        var assembliesToScan = assemblies.Length == 0
            ? [Assembly.GetCallingAssembly()]
            : assemblies;

        return AddMediatorInternal(services, filter, assembliesToScan);
    }

    /// <summary>
    /// Adiciona o Mediator e registra explicitamente apenas os manipuladores de comandos informados.
    /// Esta abordagem é otimizada e evita a varredura completa de assemblies via reflection.
    /// </summary>
    /// <param name="services">A coleção de serviços para registro das dependências.</param>
    /// <param name="handlerTypes">Os tipos explícitos dos manipuladores a serem registrados.</param>
    /// <returns>A própria coleção de serviços, para encadeamento.</returns>
    /// <exception cref="ArgumentNullException">Lançada se <paramref name="services"/> ou <paramref name="handlerTypes"/> forem nulos.</exception>
    public static IServiceCollection AddMediatorSpecificHandlers(this IServiceCollection services, params Type[] handlerTypes)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(handlerTypes);

        services.AddScoped<IMediator, Mediator>();
        RegisterHandlers(services, handlerTypes, filter: null);

        return services;
    }

    private static IServiceCollection AddMediatorInternal(IServiceCollection services, Func<Type, bool>? filter, Assembly[] assembliesToScan)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembliesToScan);

        services.AddScoped<IMediator, Mediator>();

        foreach (var assembly in assembliesToScan)
        {
            RegisterHandlers(services, assembly.GetTypes(), filter);
        }

        return services;
    }

    /// <summary>
    /// Motor centralizado que processa e registra os manipuladores de comandos (handlers) na coleção de serviços.
    /// </summary>
    /// <param name="services">A coleção de serviços para registro das dependências.</param>
    /// <param name="typesToScan">Coleção de tipos a ser processada em busca de interfaces de manipulação.</param>
    /// <param name="filter">Filtro opcional para aplicar na lista de tipos.</param>
    private static void RegisterHandlers(IServiceCollection services, IEnumerable<Type> typesToScan, Func<Type, bool>? filter)
    {
        var validTypes = typesToScan
            .Where(t => t is { IsClass: true, IsAbstract: false });

        if (filter != null)
        {
            validTypes = validTypes.Where(filter);
        }

        var voidHandlerTypes = validTypes
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                .Select(i => new { Interface = i, Implementation = t }));

        foreach (var handler in voidHandlerTypes)
        {
            services.AddScoped(handler.Interface, handler.Implementation);
        }

        var responseHandlerTypes = validTypes
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))
                .Select(i => new { Interface = i, Implementation = t }));

        foreach (var handler in responseHandlerTypes)
        {
            services.AddScoped(handler.Interface, handler.Implementation);
        }
    }
}
