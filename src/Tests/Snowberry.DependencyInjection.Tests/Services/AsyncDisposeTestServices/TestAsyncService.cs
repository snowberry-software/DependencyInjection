using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snowberry.DependencyInjection.Tests.Services.AsyncDisposeTestServices;

internal class TestAsyncService : ITestService
#if NETCOREAPP
    , IAsyncDisposable
#endif
{
    private bool _disposed;

    /// <inheritdoc/>
    public void Dispose()
    {
        throw new InvalidOperationException();
    }

#if NETCOREAPP
    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await Task.Delay(Random.Shared.Next(250, 1000));

        _disposed = true;
    }
#endif

    public string? Name { get; set; }

    public bool IsDisposed => _disposed;

}
