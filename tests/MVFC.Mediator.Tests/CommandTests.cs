namespace MVFC.Mediator.Tests;

public sealed class CommandTests
{
    private static ServiceProvider BuildProvider(bool withValidator = false, bool multipleValidators = false)
    {
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddTransient<ICommandHandler<TestVoidCommand>, TestVoidCommandHandler>();
        services.AddTransient<ICommandHandler<TestCommand, TestResponse>, TestCommandHandler>();

        if (withValidator)
        {
            services.AddSingleton<IValidator<TestCommand>, TestCommandValidator>();
            services.AddSingleton<IValidator<TestVoidCommand>, TestVoidCommandValidator>();
            if (multipleValidators)
            {
                services.AddSingleton<IValidator<TestCommand>, TestCommandValidatorB>();
                services.AddSingleton<IValidator<TestVoidCommand>, TestVoidCommandValidatorB>();
            }
        }

        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Send_VoidCommand_ShouldInvokeHandler()
    {
        TestVoidCommandHandler.CallCount = 0;
        using var scope = BuildProvider().CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(new TestVoidCommand("void"), TestContext.Current.CancellationToken);

        TestVoidCommandHandler.CallCount.Should().Be(1);
    }

    [Fact]
    public async Task Send_CommandWithResponse_ShouldReturnResponse()
    {
        using var scope = BuildProvider().CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var response = await mediator.Send<TestCommand, TestResponse>(new TestCommand("resp"), TestContext.Current.CancellationToken);

        response.Result.Should().Be("Processed: resp");
    }

    [Fact]
    public async Task Send_ShouldThrowArgumentNullException_WhenCommandIsNull()
    {
        using var scope = BuildProvider().CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var actVoid = async () => await mediator.Send<TestVoidCommand>(null!).ConfigureAwait(true);
        var actResp = async () => await mediator.Send<TestCommand, TestResponse>(null!).ConfigureAwait(true);

        await actVoid.Should().ThrowAsync<ArgumentNullException>();
        await actResp.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Send_ShouldThrowValidationException_WhenValidationFails()
    {
        using var scope = BuildProvider(withValidator: true).CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var act = async () => await mediator.Send<TestCommand, TestResponse>(new TestCommand("x")).ConfigureAwait(false); // too short

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Send_ShouldAggregateFailures_WhenMultipleValidatorsFail()
    {
        using var scope = BuildProvider(withValidator: true, multipleValidators: true).CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var act = async () => await mediator.Send<TestCommand, TestResponse>(new TestCommand("forbidden")).ConfigureAwait(true);

        var ex = await act.Should().ThrowAsync<ValidationException>();
        ex.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("forbidden"));
    }

    [Fact]
    public async Task Send_ShouldUseCachedInvoker_OnSubsequentCalls()
    {
        TestVoidCommandHandler.CallCount = 0;
        var provider = BuildProvider();
        var cmd = new TestVoidCommand("cached");

        for (var i = 0; i < 3; i++)
        {
            using var scope = provider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(cmd, TestContext.Current.CancellationToken);
        }

        TestVoidCommandHandler.CallCount.Should().Be(3);
    }

    [Fact]
    public async Task Send_ResponseCommand_ShouldUseCachedInvoker_OnSubsequentCalls()
    {
        var provider = BuildProvider();
        var cmd = new TestCommand("cached-resp");

        for (var i = 0; i < 3; i++)
        {
            using var scope = provider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var resp = await mediator.Send<TestCommand, TestResponse>(cmd, TestContext.Current.CancellationToken);
            resp.Result.Should().Be("Processed: cached-resp");
        }
    }

    [Fact]
    public async Task Send_ShouldPass_WhenMultipleValidatorsSucceed()
    {
        using var scope = BuildProvider(withValidator: true, multipleValidators: true).CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var response = await mediator.Send<TestCommand, TestResponse>(new TestCommand("valid"), TestContext.Current.CancellationToken);

        response.Result.Should().Be("Processed: valid");
    }

    [Fact]
    public async Task Send_VoidCommand_ShouldPass_WhenMultipleValidatorsSucceed()
    {
        TestVoidCommandHandler.CallCount = 0;
        using var scope = BuildProvider(withValidator: true, multipleValidators: true).CreateScope(); // VoidCommand handles ICommand, but I haven't added validators for it in BuildProvider
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(new TestVoidCommand("valid-void"), TestContext.Current.CancellationToken);

        TestVoidCommandHandler.CallCount.Should().Be(1);
    }

    [Fact]
    public async Task Send_VoidCommand_ShouldThrowValidationException_WhenValidationFails()
    {
        using var scope = BuildProvider(withValidator: true).CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var act = async () => await mediator.Send(new TestVoidCommand("x")).ConfigureAwait(false); // too short

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Send_VoidCommand_ShouldAggregateFailures_WhenMultipleValidatorsFail()
    {
        using var scope = BuildProvider(withValidator: true, multipleValidators: true).CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var act = async () => await mediator.Send(new TestVoidCommand("forbidden")).ConfigureAwait(true);

        var ex = await act.Should().ThrowAsync<ValidationException>();
        ex.Which.Errors.Should().Contain(e => e.ErrorMessage.Contains("forbidden"));
    }
}
