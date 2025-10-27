namespace MVFC.Mediator;

/// <summary>
/// Implementação principal do padrão Mediator, responsável por enviar comandos e coordenar sua execução.
/// </summary>
public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask Send<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        ArgumentNullException.ThrowIfNull(command);
        return VoidCommandDispatcher<TCommand>.Send(_serviceProvider, command, cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueTask<TResponse> Send<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResponse>
    {
        ArgumentNullException.ThrowIfNull(command);
        return CommandDispatcherWithResponse<TCommand, TResponse>.Send(_serviceProvider, command, cancellationToken);
    }
}