namespace MVFC.Mediator.Dispatcher;

/// <summary>
/// Dispatcher interno para comandos com retorno, responsável por validação e execução otimizada.
/// </summary>
internal static class CommandDispatcherWithResponse<TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    private static Func<IServiceProvider, TCommand, CancellationToken, ValueTask<TResponse>>? _invoker;

    /// <summary>
    /// Envia o comando para validação e execução, utilizando despache otimizado quando possível.
    /// </summary>
    /// <param name="sp">Provedor de serviços para resolução de dependências.</param>
    /// <param name="cmd">Comando a ser processado.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Resultado da execução do comando.</returns>
    /// <exception cref="ValidationException">Lançada se a validação do comando falhar.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<TResponse> Send(IServiceProvider sp, TCommand cmd, CancellationToken ct)
    {
        var validators = sp.GetServices<IValidator<TCommand>>();

        using var enumerator = validators.GetEnumerator();

        if (enumerator.MoveNext())
        {
            var firstValidator = enumerator.Current;

            if (!enumerator.MoveNext())
            {
                var result = await firstValidator.ValidateAsync(cmd!, ct).ConfigureAwait(false);
                if (!result.IsValid)
                    throw new ValidationException(result.Errors);
            }
            else
            {
                await ValidateMultiple(firstValidator, enumerator, cmd!, ct).ConfigureAwait(false);
            }
        }

        // Dispatch otimizado
        var invoker = Volatile.Read(ref _invoker);

        if (invoker != null)
            return await invoker(sp, cmd, ct).ConfigureAwait(false);

        return await DispatchSlow(sp, cmd, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Valida o comando utilizando múltiplos validadores, agregando todas as falhas encontradas.
    /// </summary>
    /// <param name="first">Primeiro validador encontrado.</param>
    /// <param name="remaining">Enumerador dos validadores restantes.</param>
    /// <param name="command">Comando a ser validado.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Uma tarefa que representa a operação de validação.</returns>
    /// <exception cref="ValidationException">Lançada se houver falhas de validação.</exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static async ValueTask ValidateMultiple(
        IValidator<TCommand> first,
        IEnumerator<IValidator<TCommand>> remaining,
        TCommand command,
        CancellationToken ct)
    {
        var validatorsList = new List<IValidator<TCommand>>(4) { first };

        do
        {
            validatorsList.Add(remaining.Current);
        }
        while (remaining.MoveNext());

        var count = validatorsList.Count;
        var tasks = new Task<ValidationResult>[count];

        for (int i = 0; i < count; i++)
        {
            tasks[i] = validatorsList[i].ValidateAsync(command, ct);
        }

        var results = new ValidationResult[count];
        for (int i = 0; i < count; i++)
        {
            results[i] = await tasks[i].ConfigureAwait(false);
        }

        List<ValidationFailure>? failures = null;

        for (int i = 0; i < count; i++)
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

    /// <summary>
    /// Executa o despacho do comando de forma não otimizada e inicializa o invoker para futuras execuções.
    /// </summary>
    /// <param name="sp">Provedor de serviços para resolução de dependências.</param>
    /// <param name="cmd">Comando a ser processado.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Resultado da execução do comando.</returns>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static ValueTask<TResponse> DispatchSlow(IServiceProvider sp, TCommand cmd, CancellationToken ct)
    {
        var handler = sp.GetRequiredService<ICommandHandler<TCommand, TResponse>>();

        static ValueTask<TResponse> newInvoker(IServiceProvider serviceProvider, TCommand command, CancellationToken cancellationToken)
        {
            var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResponse>>();
            return handler.Handle(command, cancellationToken);
        }

        Interlocked.CompareExchange(ref _invoker, newInvoker, null);
        return handler.Handle(cmd, ct);
    }
}