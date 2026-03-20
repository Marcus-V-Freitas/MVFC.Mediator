namespace MVFC.Mediator;

/// <summary>
/// Interface base para queries com retorno.
/// </summary>
/// <typeparam name="TResponse">Tipo da resposta</typeparam>
public interface IQuery<out TResponse>;