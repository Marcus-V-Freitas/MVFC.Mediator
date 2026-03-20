namespace MVFC.Mediator.Tests.Models;

public sealed class TestNotificationHandler : INotificationHandler<TestNotification>
{
    public static int CallCount;

    public ValueTask Handle(TestNotification notification, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref CallCount);
        return ValueTask.CompletedTask;
    }
}

public sealed class TestNotificationHandlerB : INotificationHandler<TestNotification>
{
    public static int CallCount;

    public ValueTask Handle(TestNotification notification, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref CallCount);
        return ValueTask.CompletedTask;
    }
}
public sealed class TestNotificationHandlerC : INotificationHandler<TestNotification>
{
    public static int CallCount;

    public ValueTask Handle(TestNotification notification, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref CallCount);
        return ValueTask.CompletedTask;
    }
}
