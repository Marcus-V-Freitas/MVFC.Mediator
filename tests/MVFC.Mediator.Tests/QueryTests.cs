namespace MVFC.Mediator.Tests;

public sealed class QueryTests
{
    private static ServiceProvider BuildProvider(bool withValidator = false, bool multipleValidators = false)
    {
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddTransient<IQueryHandler<TestQuery, TestResponse>, TestQueryHandler>();

        if (withValidator)
        {
            services.AddSingleton<IValidator<TestQuery>, TestQueryValidator>();
            if (multipleValidators)
            {
                services.AddSingleton<IValidator<TestQuery>, TestQueryValidatorB>();
            }
        }

        return services.BuildServiceProvider();
    }


    [Fact]
    public async Task Query_ShouldReturnExpectedResponse()
    {
        using var scope = BuildProvider().CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var response = await mediator.Query<TestQuery, TestResponse>(new TestQuery("abc"), TestContext.Current.CancellationToken);

        response.Result.Should().Be("Query: abc");
    }

    [Fact]
    public async Task Query_ShouldAggregateFailures_WhenMultipleValidatorsFail()
    {
        using var scope = BuildProvider(withValidator: true, multipleValidators: true).CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var act = async () => await mediator.Query<TestQuery, TestResponse>(new TestQuery("forbidden")).ConfigureAwait(true);

        var ex = await act.Should().ThrowAsync<ValidationException>();
        ex.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("forbidden"));
    }

    [Fact]
    public async Task Query_ShouldPass_WhenMultipleValidatorsSucceed()
    {
        using var scope = BuildProvider(withValidator: true, multipleValidators: true).CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var response = await mediator.Query<TestQuery, TestResponse>(new TestQuery("123"), TestContext.Current.CancellationToken);

        response.Result.Should().Be("Query: 123");
    }

    [Fact]
    public async Task Query_ShouldThrowValidationException_WhenIdIsEmpty()
    {
        using var scope = BuildProvider(withValidator: true).CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var act = async () => await mediator.Query<TestQuery, TestResponse>(new TestQuery(string.Empty)).ConfigureAwait(true);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*Id is required*");
    }

    [Fact]
    public async Task Query_ShouldThrowArgumentNullException_WhenQueryIsNull()
    {
        using var scope = BuildProvider().CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var act = async () => await mediator.Query<TestQuery, TestResponse>(null!).ConfigureAwait(true);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Query_ShouldUsesCachedInvoker_OnSubsequentCalls()
    {
        var provider = BuildProvider();
        var query = new TestQuery("x");

        for (var i = 0; i < 5; i++)
        {
            using var scope = provider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var response = await mediator.Query<TestQuery, TestResponse>(query, TestContext.Current.CancellationToken);
            response.Result.Should().Be("Query: x");
        }
    }
}
