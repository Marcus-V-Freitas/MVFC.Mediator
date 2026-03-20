namespace MVFC.Mediator;

/// <summary>
/// Interface para handlers de queries.
/// </summary>
/// <typeparam name="TQuery">Tipo da query</typeparam>
/// <typeparam name="TResponse">Tipo da resposta</typeparam>
public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    public ValueTask<TResponse> Handle(TQuery query, CancellationToken cancellationToken = default);
}
