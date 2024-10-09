using Snowberry.DependencyInjection.Tests.Services.Interfaces;

namespace Snowberry.DependencyInjection.Tests.Services.KeyedTestServices;

public class TestServiceKeyedA : ITestService
{
    public TestServiceKeyedA()
    {
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        IsDisposed = true;
    }

    /// <inheritdoc/>
    public string? Name { get; set; }

    public bool IsDisposed { get; set; }
}
