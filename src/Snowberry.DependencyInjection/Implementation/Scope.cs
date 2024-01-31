﻿using Snowberry.DependencyInjection.Interfaces;

namespace Snowberry.DependencyInjection.Implementation;

public class Scope : IScope
{
    /// <inheritdoc/>
    public event EventHandler? OnDispose;

    private bool _isDisposed;
    private readonly object _lock = new();
    private DisposableContainer _disposableContainer = new();

    /// <summary>
    /// Creates a new scope.
    /// </summary>
    /// <remarks>The <see cref="SetServiceFactory(IServiceFactory)"/> must be called before using the <see cref="ServiceFactory"/> property.</remarks>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Scope()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// Disposes the core.
    /// </summary>
    /// <returns>Whether the core is already disposed.</returns>
    private bool DisposeCore()
    {
        lock (_lock)
        {
            if (IsDisposed)
                return true;

            _isDisposed = true;
            return false;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        lock (_lock)
        {
            if (DisposeCore())
                return;

            try
            {
                _disposableContainer.Dispose();
            }
            finally
            {
                OnDispose?.Invoke(this, EventArgs.Empty);
            }
        }
    }

#if NETCOREAPP
    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if (DisposeCore())
            return;

        try
        {
            await _disposableContainer.DisposeAsync();
        }
        finally
        {
            OnDispose?.Invoke(this, EventArgs.Empty);
        }
    }
#endif

    /// <inheritdoc/>
    public void RegisterDisposable(IDisposable disposable)
    {
        RegisterDisposable((object)disposable);
    }

#if NETCOREAPP
    /// <inheritdoc/>
    public void RegisterDisposable(IAsyncDisposable disposable)
    {
        RegisterDisposable((object)disposable);
    }
#endif

    /// <inheritdoc/>
    public void RegisterDisposable(object disposable)
    {
        _ = disposable ?? throw new ArgumentNullException(nameof(disposable));

        lock (_lock)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(Scope));

            _disposableContainer.RegisterDisposable(disposable);
        }
    }

    /// <inheritdoc/>
    public void SetServiceFactory(IServiceFactory serviceFactory)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(Scope));

        if (ServiceFactory != null)
            throw new InvalidOperationException("The service factory is already set for the scope!");

        ServiceFactory = serviceFactory;
    }

    /// <inheritdoc/>
    public IServiceFactory ServiceFactory { get; private set; }

    /// <inheritdoc/>
    public bool IsDisposed => _isDisposed;

    /// <inheritdoc/>
    public int DisposableCount
    {
        get
        {
            lock (_lock)
                return _disposableContainer.DisposableCount;
        }
    }
}
