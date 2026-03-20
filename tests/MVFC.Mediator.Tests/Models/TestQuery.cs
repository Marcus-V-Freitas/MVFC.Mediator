namespace MVFC.Mediator.Tests.Models;

public sealed record TestQuery(string Id) : IQuery<TestResponse>;
