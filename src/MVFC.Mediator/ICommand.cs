namespace MVFC.Mediator;

/// <summary>
/// Interface base para comandos sem retorno
/// </summary>
public interface ICommand;

/// <summary>
/// Interface base para comandos com retorno
/// </summary>
/// <typeparam name="TResponse">Tipo de resposta do comando</typeparam>
public interface ICommand<out TResponse>;