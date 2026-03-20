namespace MVFC.Mediator.Tests;

public sealed class NotificationTests
{
    private static ServiceProvider BuildProvider(int handlerCount = 1)
    {
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddTransient<INotificationHandler<TestNotification>, TestNotificationHandler>();

        if (handlerCount > 1)
            services.AddTransient<INotificationHandler<TestNotification>, TestNotificationHandlerB>();
        
        if (handlerCount > 2)
            services.AddTransient<INotificationHandler<TestNotification>, TestNotificationHandlerC>();

        return services.BuildServiceProvider();
    }


    [Fact]
    public async Task Publish_ShouldInvokeSingleHandler()
    {
        TestNotificationHandler.CallCount = 0;

        using var scope = BuildProvider(handlerCount: 1).CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Publish(new TestNotification("hello"), TestContext.Current.CancellationToken);

        TestNotificationHandler.CallCount.Should().Be(1);
    }

    [Fact]
    public async Task Publish_ShouldInvokeAllHandlers_WhenThreeOrMoreRegistered()
    {
        TestNotificationHandler.CallCount = 0;
        TestNotificationHandlerB.CallCount = 0;
        TestNotificationHandlerC.CallCount = 0;

        using var scope = BuildProvider(handlerCount: 3).CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Publish(new TestNotification("multi"), TestContext.Current.CancellationToken);

        TestNotificationHandler.CallCount.Should().Be(1);
        TestNotificationHandlerB.CallCount.Should().Be(1);
        TestNotificationHandlerC.CallCount.Should().Be(1);
    }

    [Fact]
    public async Task Publish_ShouldInvokeAllHandlers_WhenMultipleRegistered()
    {
        TestNotificationHandler.CallCount = 0;
        TestNotificationHandlerB.CallCount = 0;

        using var scope = BuildProvider(handlerCount: 2).CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Publish(new TestNotification("fan-out"), TestContext.Current.CancellationToken);

        TestNotificationHandler.CallCount.Should().Be(1);
        TestNotificationHandlerB.CallCount.Should().Be(1);
    }

    [Fact]
    public async Task Publish_ShouldNotThrow_WhenNoHandlersRegistered()
    {
        var services = new ServiceCollection();
        services.AddMediator();
        using var scope = services.BuildServiceProvider().CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var act = async () => await mediator.Publish(new TestNotification("orphan")).ConfigureAwait(true);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Publish_ShouldThrowArgumentNullException_WhenNotificationIsNull()
    {
        using var scope = BuildProvider().CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var act = async () => await mediator.Publish<TestNotification>(null!).ConfigureAwait(true);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
