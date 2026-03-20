namespace MVFC.Mediator.Tests.Models;

public sealed record TestCommand(string Value) : ICommand<TestResponse>;
