namespace Snowberry.DependencyInjection.Tests.Services.Interfaces;

public interface ITestService : IDisposable
{
    string? Name { get; set; }

    bool IsDisposed { get; }
}
