namespace MVFC.Mediator.Dispatcher;

internal static class NotificationDispatcher<TNotification> where TNotification : INotification
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask Publish(IServiceProvider sp, TNotification notification, CancellationToken ct)
    {
        var handlers = sp.GetServices<INotificationHandler<TNotification>>();

        using var enumerator = handlers.GetEnumerator();

        if (!enumerator.MoveNext())
            return;

        var first = enumerator.Current;

        if (!enumerator.MoveNext())
        {
            await first.Handle(notification, ct).ConfigureAwait(false);
            return;
        }

        await PublishToMultiple(first, enumerator, notification, ct).ConfigureAwait(false);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static async ValueTask PublishToMultiple(
        INotificationHandler<TNotification> first,
        IEnumerator<INotificationHandler<TNotification>> remaining,
        TNotification notification,
        CancellationToken ct)
    {
        var handlersList = new List<INotificationHandler<TNotification>>(4) { first };

        do { handlersList.Add(remaining.Current); }
        while (remaining.MoveNext());

        var count = handlersList.Count;

        for (var i = 0; i < count; i++)
            await handlersList[i].Handle(notification, ct).ConfigureAwait(false);
    }
}
