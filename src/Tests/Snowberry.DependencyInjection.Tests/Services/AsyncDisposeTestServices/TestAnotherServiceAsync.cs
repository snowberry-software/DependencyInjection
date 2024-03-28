namespace Snowberry.DependencyInjection.Tests.Services.AsyncDisposeTestServices;

public class TestAnotherServiceAsync : ITestAnotherService
#if NETCOREAPP
    , IAsyncDisposable
#endif
{
#if NETCOREAPP
    public async ValueTask DisposeAsync()
    {
        await Task.Delay(200);
        IsDisposed = true;
    }
#endif

    public int Number => 0;

    public ITestService TestService { get; set; } = null!;

    public ITestService TestServiceSame { get; } = null!;

    public bool IsDisposed { get; private set; }
}
