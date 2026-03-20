namespace MVFC.Mediator.Tests.Models;

public sealed record TestVoidCommand(string Value) : ICommand;

public sealed class TestVoidCommandHandler : ICommandHandler<TestVoidCommand>
{
    public static int CallCount;

    public ValueTask Handle(TestVoidCommand command, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref CallCount);
        return ValueTask.CompletedTask;
    }
}
