namespace MVFC.Mediator.Tests.Models;

public sealed class TestCommandHandler : ICommandHandler<TestCommand, TestResponse>
{
    public ValueTask<TestResponse> Handle(TestCommand command, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(new TestResponse($"Processed: {command.Value}"));
    }
}