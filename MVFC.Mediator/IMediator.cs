namespace MVFC.Mediator;

/// <summary>
/// Interface principal do Mediator
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Envia um comando sem retorno
    /// </summary>
    ValueTask Send<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    /// <summary>
    /// Envia um comando com retorno
    /// </summary>
    ValueTask<TResponse> Send<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResponse>;
}