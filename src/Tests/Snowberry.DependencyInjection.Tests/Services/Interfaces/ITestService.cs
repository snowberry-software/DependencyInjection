namespace Snowberry.DependencyInjection.Tests.Services.Interfaces;

public interface ITestService : IDisposable
{
    public string? Name { get; set; }

    bool IsDisposed { get; }
}
