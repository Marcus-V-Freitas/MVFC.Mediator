namespace MVFC.Mediator.Tests.Models;

public sealed class TestQueryHandler : IQueryHandler<TestQuery, TestResponse>
{
    public ValueTask<TestResponse> Handle(TestQuery query, CancellationToken cancellationToken = default) =>
        ValueTask.FromResult(new TestResponse($"Query: {query.Id}"));
}
