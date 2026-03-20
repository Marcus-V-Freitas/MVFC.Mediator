namespace MVFC.Mediator.Dispatcher;

internal static class QueryDispatcher<TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    private static Func<IServiceProvider, TQuery, CancellationToken, ValueTask<TResponse>>? _invoker;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<TResponse> Send(IServiceProvider sp, TQuery query, CancellationToken ct)
    {
        var validators = sp.GetServices<IValidator<TQuery>>();

        using var enumerator = validators.GetEnumerator();

        if (enumerator.MoveNext())
        {
            var firstValidator = enumerator.Current;

            if (!enumerator.MoveNext())
            {
                var result = await firstValidator.ValidateAsync(query!, ct).ConfigureAwait(false);
                if (!result.IsValid)
                    throw new ValidationException(result.Errors);
            }
            else
            {
                await ValidateMultiple(firstValidator, enumerator, query!, ct).ConfigureAwait(false);
            }
        }

        var invoker = Volatile.Read(ref _invoker);

        return invoker != null ?
            await invoker(sp, query, ct).ConfigureAwait(false) :
            await DispatchSlow(sp, query, ct).ConfigureAwait(false);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static async ValueTask ValidateMultiple(
        IValidator<TQuery> first,
        IEnumerator<IValidator<TQuery>> remaining,
        TQuery query,
        CancellationToken ct)
    {
        var validatorsList = new List<IValidator<TQuery>>(4) { first };

        do { validatorsList.Add(remaining.Current); }
        while (remaining.MoveNext());

        var count = validatorsList.Count;
        var tasks = new Task<ValidationResult>[count];

        for (var i = 0; i < count; i++)
            tasks[i] = validatorsList[i].ValidateAsync(query, ct);

        var results = new ValidationResult[count];
        for (var i = 0; i < count; i++)
            results[i] = await tasks[i].ConfigureAwait(false);

        List<ValidationFailure>? failures = null;

        for (var i = 0; i < count; i++)
        {
            var errors = results[i].Errors;
            if (errors.Count > 0)
            {
                failures ??= new List<ValidationFailure>(errors.Count);
                failures.AddRange(errors);
            }
        }

        if (failures?.Count > 0)
            throw new ValidationException(failures);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static ValueTask<TResponse> DispatchSlow(IServiceProvider sp, TQuery query, CancellationToken ct)
    {
        var handler = sp.GetRequiredService<IQueryHandler<TQuery, TResponse>>();

        static ValueTask<TResponse> NewInvoker(IServiceProvider serviceProvider, TQuery q, CancellationToken cancellationToken)
        {
            var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResponse>>();
            return handler.Handle(q, cancellationToken);
        }

        Interlocked.CompareExchange(ref _invoker, NewInvoker, null);
        return handler.Handle(query, ct);
    }
}
