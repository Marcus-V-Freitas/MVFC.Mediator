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
        TestVoidCommandHandler.ResetCount();
        using var scope = BuildProvider().CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(new TestVoidCommand("void"), TestContext.Current.CancellationToken);

        TestVoidCommandHandler.GetCount().Should().Be(1);
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
        TestVoidCommandHandler.ResetCount();
        var provider = BuildProvider();
        var cmd = new TestVoidCommand("cached");

        for (var i = 0; i < 3; i++)
        {
            using var scope = provider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            await mediator.Send(cmd, TestContext.Current.CancellationToken);
        }

        TestVoidCommandHandler.GetCount().Should().Be(3);
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
        TestVoidCommandHandler.ResetCount();
        using var scope = BuildProvider(withValidator: true, multipleValidators: true).CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(new TestVoidCommand("valid-void"), TestContext.Current.CancellationToken);

        TestVoidCommandHandler.GetCount().Should().Be(1);
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

    [Fact]
    public void AddMediator_WithFilter_ShouldUseCallingAssembly_WhenNoAssemblyProvided()
    {
        var services = new ServiceCollection();
        services.AddMediator(t => t.Name.Contains("Void", StringComparison.Ordinal));

        var provider = services.BuildServiceProvider();
        var voidHandler = provider.GetService<ICommandHandler<TestVoidCommand>>();
        var respHandler = provider.GetService<ICommandHandler<TestCommand, TestResponse>>();

        voidHandler.Should().NotBeNull();
        respHandler.Should().BeNull();
    }

    [Fact]
    public void AddMediator_WithoutFilter_ShouldUseCallingAssembly_WhenNoAssemblyProvided()
    {
        var services = new ServiceCollection();
        services.AddMediator();

        var provider = services.BuildServiceProvider();
        var voidHandler = provider.GetService<ICommandHandler<TestVoidCommand>>();
        var respHandler = provider.GetService<ICommandHandler<TestCommand, TestResponse>>();

        voidHandler.Should().NotBeNull();
        respHandler.Should().NotBeNull();
    }

    [Fact]
    public void AddMediator_WithFilter_ShouldOnlyRegisterMatchingHandlers()
    {
        var services = new ServiceCollection();
        var assembly = typeof(TestVoidCommandHandler).Assembly;
        services.AddMediator(t => t.Name.Contains("Void", StringComparison.Ordinal), assembly);

        var provider = services.BuildServiceProvider();
        var voidHandler = provider.GetService<ICommandHandler<TestVoidCommand>>();
        var respHandler = provider.GetService<ICommandHandler<TestCommand, TestResponse>>();

        voidHandler.Should().NotBeNull();
        respHandler.Should().BeNull();
    }

    [Fact]
    public void AddMediator_WithFilter_ShouldRegisterNothing_WhenFilterFailsAll()
    {
        var services = new ServiceCollection();
        var assembly = typeof(TestVoidCommandHandler).Assembly;
        services.AddMediator(t => t.Name == "NonExistentHandler", assembly);

        var provider = services.BuildServiceProvider();
        var voidHandler = provider.GetService<ICommandHandler<TestVoidCommand>>();
        var respHandler = provider.GetService<ICommandHandler<TestCommand, TestResponse>>();

        voidHandler.Should().BeNull();
        respHandler.Should().BeNull();
    }

    [Fact]
    public void AddMediatorSpecificHandlers_ShouldOnlyRegisterProvidedTypes()
    {
        var services = new ServiceCollection();
        services.AddMediatorSpecificHandlers(typeof(TestCommandHandler));
        var provider = services.BuildServiceProvider();
        var respHandler = provider.GetService<ICommandHandler<TestCommand, TestResponse>>();
        var voidHandler = provider.GetService<ICommandHandler<TestVoidCommand>>();
        respHandler.Should().NotBeNull();
        voidHandler.Should().BeNull();
    }

    [Fact]
    public void AddMediatorSpecificHandlers_ShouldRegisterMultipleTypesSuccessfully()
    {
        var services = new ServiceCollection();
        services.AddMediatorSpecificHandlers(
            typeof(TestCommandHandler),
            typeof(TestVoidCommandHandler));

        var provider = services.BuildServiceProvider();
        var respHandler = provider.GetService<ICommandHandler<TestCommand, TestResponse>>();
        var voidHandler = provider.GetService<ICommandHandler<TestVoidCommand>>();

        respHandler.Should().NotBeNull();
        voidHandler.Should().NotBeNull();
    }

    [Fact]
    public void AddMediator_WithFilter_ShouldThrowArgumentNullException_WhenAssembliesAreNull()
    {
        var services = new ServiceCollection();

        var act = () => services.AddMediator(t => true, null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddMediatorSpecificHandlers_ShouldThrowArgumentNullException_WhenTypesAreNull()
    {
        var services = new ServiceCollection();

        var act = () => services.AddMediatorSpecificHandlers(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
