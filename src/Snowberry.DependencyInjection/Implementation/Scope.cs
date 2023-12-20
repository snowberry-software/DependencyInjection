using Snowberry.DependencyInjection.Interfaces;

namespace Snowberry.DependencyInjection.Implementation;

public class Scope : IScope
{
    /// <inheritdoc/>
    public event EventHandler? OnDispose;

    private bool _isDisposed;
    private readonly object _lock = new();
    private List<IDisposable>? _disposables;

    /// <summary>
    /// Creates a new scope.
    /// </summary>
    /// <remarks>The <see cref="SetServiceFactory(IServiceFactory)"/> must be called before using the <see cref="ServiceFactory"/> property.</remarks>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Scope()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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
    public void RegisterDisposable(IDisposable disposable)
    {
        _ = disposable ?? throw new ArgumentNullException(nameof(disposable));

        lock (_lock)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(Scope));

            if (_disposables == null)
            {
                _disposables = [disposable];
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
                return _disposables?.Count ?? 0;
        }
    }
}
