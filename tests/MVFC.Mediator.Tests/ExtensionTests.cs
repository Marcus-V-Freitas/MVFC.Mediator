namespace MVFC.Mediator.Tests;

public sealed class ExtensionTests
{
    [Fact]
    public void AddMediator_ShouldRegisterMediator()
    {
        var services = new ServiceCollection();
        services.AddMediator();

        var sp = services.BuildServiceProvider();
        sp.GetService<IMediator>().Should().NotBeNull();
        sp.GetService<IMediator>().Should().BeOfType<Mediator>();
    }

    [Fact]
    public void AddMediator_ShouldThrowArgumentNullException_WhenServicesIsNull()
    {
        IServiceCollection services = null!;
        var act = () => services.AddMediator();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AddMediator_ShouldScanProvidedAssemblies()
    {
        var services = new ServiceCollection();
        // Scanning the unit test assembly where TestVoidCommandHandler is defined
        services.AddMediator(typeof(ExtensionTests).Assembly);

        var sp = services.BuildServiceProvider();
        
        // Check if handlers are registered
        sp.GetService<ICommandHandler<TestVoidCommand>>().Should().NotBeNull();
        sp.GetService<ICommandHandler<TestCommand, TestResponse>>().Should().NotBeNull();
    }
}
