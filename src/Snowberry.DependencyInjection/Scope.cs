using Snowberry.DependencyInjection.Interfaces;

namespace Snowberry.DependencyInjection;

public class Scope : IScope
{
    /// <inheritdoc/>
    public event EventHandler? OnDispose;

    private bool _isDisposed;
    private readonly object _lock = new();
    private List<IDisposable>? _disposables;

#nullable disable
    public Scope()
    {
    }
#nullable enable

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (_lock)
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            if (_disposables == null)
                return;

            for (int i = _disposables.Count - 1; i >= 0; i--)
            {
                var disposable = _disposables[i];
                disposable.Dispose();
            }

            _disposables.Clear();

            OnDispose?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <inheritdoc/>
    public void RegisterDisposable(IDisposable disposable!!)
    {
        lock (_lock)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(Scope));

            if (_disposables == null)
            {
                _disposables = new List<IDisposable>() { disposable };
                return;
            }

            _disposables.Add(disposable);
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
    public int DisposeableCount
    {
        get
        {
            lock (_lock)
            {
                return _disposables?.Count ?? 0;
            }
        }
    }
}
