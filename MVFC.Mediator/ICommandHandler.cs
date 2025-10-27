namespace MVFC.Mediator;

/// <summary>
/// Interface para manipuladores de comandos sem retorno
/// </summary>
/// <typeparam name="TCommand">Tipo do comando</typeparam>
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    ValueTask Handle(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface para manipuladores de comandos com retorno
/// </summary>
/// <typeparam name="TCommand">Tipo do comando</typeparam>
/// <typeparam name="TResponse">Tipo da resposta</typeparam>
public interface ICommandHandler<in TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    ValueTask<TResponse> Handle(TCommand command, CancellationToken cancellationToken = default);
}
