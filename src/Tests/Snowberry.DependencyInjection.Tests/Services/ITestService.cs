namespace Snowberry.DependencyInjection.Tests.Services;

public interface ITestService : IDisposable
{
    public string? Name { get; set; }

    bool IsDisposed { get; }
}
