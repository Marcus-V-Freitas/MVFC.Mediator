namespace MVFC.Mediator.Tests.Models;

public sealed record TestVoidCommand(string Value) : ICommand;

public sealed class TestVoidCommandHandler : ICommandHandler<TestVoidCommand>
{
    private static int _callCount;

    public ValueTask Handle(TestVoidCommand command, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref _callCount);
        return ValueTask.CompletedTask;
    }

    public static int GetCount() => _callCount;

    public static void ResetCount() => _callCount = 0;
}
