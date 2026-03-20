namespace MVFC.Mediator.Tests.Fixture;

public sealed class AppHostFixture : IAsyncLifetime
{
    private ProjectAppHost _appHost = default!;
    private HttpClient _appHttpClient = default!;

    internal IPlaygroundApiClient PlaygroundApi { get; private set; } = default!;

    public async ValueTask InitializeAsync()
    {
        _appHost = new ProjectAppHost();

        await _appHost.StartAsync().ConfigureAwait(false);

        _appHttpClient = _appHost.CreateHttpClient("api");

        PlaygroundApi = RestService.For<IPlaygroundApiClient>(_appHttpClient);
    }

    public async ValueTask DisposeAsync()
    {
        _appHttpClient?.Dispose();
        await _appHost.DisposeAsync().ConfigureAwait(false);
    }
}
