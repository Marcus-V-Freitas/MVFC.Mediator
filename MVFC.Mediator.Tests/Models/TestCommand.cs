namespace MVFC.Mediator.Tests.Models;

public sealed record class TestCommand(string Value) : ICommand<TestResponse>;