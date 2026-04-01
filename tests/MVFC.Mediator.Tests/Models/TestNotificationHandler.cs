namespace MVFC.Mediator.Tests.Models;

public sealed class TestNotificationHandler : INotificationHandler<TestNotification>
{
    private static int _callCount;

    public ValueTask Handle(TestNotification notification, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref _callCount);
        return ValueTask.CompletedTask;
    }

    public static int GetCount() => _callCount;

    public static void ResetCount() => _callCount = 0;
}

public sealed class TestNotificationHandlerB : INotificationHandler<TestNotification>
{
    private static int _callCount;

    public ValueTask Handle(TestNotification notification, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref _callCount);
        return ValueTask.CompletedTask;
    }

    public static int GetCount() => _callCount;

    public static void ResetCount() => _callCount = 0;
}

public sealed class TestNotificationHandlerC : INotificationHandler<TestNotification>
{
    private static int _callCount;

    public ValueTask Handle(TestNotification notification, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref _callCount);
        return ValueTask.CompletedTask;
    }

    public static int GetCount() => _callCount;

    public static void ResetCount() => _callCount = 0;
}
