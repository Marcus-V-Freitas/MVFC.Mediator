namespace MVFC.Mediator.Tests;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 5)]
public class RealWorldScenarioBenchmark
{
    private IServiceProvider _serviceProvider = null!;
    private TestCommand _command = null!;

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddValidatorsFromAssemblyContaining<TestCommandValidator>();

        _serviceProvider = services.BuildServiceProvider();
        _command = new TestCommand("test");
    }

    [Benchmark(Description = "Simula request HTTP individual")]
    public async Task<TestResponse> SingleRequest()
    {
        using var scope = _serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        return await mediator.Send<TestCommand, TestResponse>(_command);
    }

    [Benchmark(Description = "Simula múltiplos requests em paralelo")]
    public async Task<TestResponse[]> BatchRequests()
    {
        var tasks = new Task<TestResponse>[10];

        for (int i = 0; i < 10; i++)
        {
            var scope = _serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            tasks[i] = mediator.Send<TestCommand, TestResponse>(_command).AsTask();
        }

        return await Task.WhenAll(tasks);
    }
}