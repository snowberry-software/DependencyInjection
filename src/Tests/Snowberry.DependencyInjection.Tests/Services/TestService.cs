using Snowberry.DependencyInjection.Tests.Services.Interfaces;

namespace Snowberry.DependencyInjection.Tests.Services;

internal class TestService : ITestService, IDisposable
{
    public string? Name { get; set; }

    /// <inheritdoc/>
    public void Dispose()
    {
        IsDisposed = true;
    }

    public bool IsDisposed { get; private set; }
}