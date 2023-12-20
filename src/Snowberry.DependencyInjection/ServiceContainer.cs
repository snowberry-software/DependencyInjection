using System.Collections.Concurrent;
using System.Reflection;
using Snowberry.DependencyInjection.Exceptions;
using Snowberry.DependencyInjection.Implementation;
using Snowberry.DependencyInjection.Interfaces;
using Snowberry.DependencyInjection.Lookup;

namespace Snowberry.DependencyInjection;

public partial class ServiceContainer : IServiceContainer
{
    private ConcurrentDictionary<IServiceIdentifier, IServiceDescriptor> _serviceDescriptorMapping = [];
    private List<IDisposable>? _disposables;
    private readonly object _lock = new();
    private bool _isDisposed;

    /// <summary>
    /// Creates a container with the default options.
    /// </summary>
    public ServiceContainer() : this(null!, ServiceContainerOptions.Default)
    {
    }

    /// <summary>
    /// Creates a new container using the given <paramref name="options"/>.
    /// </summary>
    /// <param name="options">The options for the container.</param>
    public ServiceContainer(ServiceContainerOptions options) : this(null!, options)
    {
    }

    /// <summary>
    /// Creates a new container using the given <paramref name="serviceFactory"/> and <paramref name="options"/>.
    /// </summary>
    /// <param name="serviceFactory">The service factory that will be used.</param>
    /// <param name="options">The options for the container.</param>
    public ServiceContainer(IScopedServiceFactory serviceFactory, ServiceContainerOptions options)
    {
        ServiceFactory = serviceFactory ?? new DefaultServiceFactory(this);
        Options = options;
    }

    /// <summary>
    /// Adds default services to the container.
    /// </summary>
    protected virtual void AddDefaultServices()
    {

    }

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (_lock)
        {
            if (IsDisposed)
                return;

            _isDisposed = true;
            ServiceFactory.NotifyScopeDisposed(null);

            if (_disposables == null)
                return;

            for (int i = _disposables.Count - 1; i >= 0; i--)
            {
                var disposable = _disposables[i];
                disposable.Dispose();
            }

            _disposables.Clear();
        }
    }

    /// <inheritdoc/>
    public void RegisterDisposable(IDisposable disposable)
    {
        _ = disposable ?? throw new ArgumentNullException(nameof(disposable));

        lock (_lock)
        {
            if (_disposables == null)
            {
                _disposables = [disposable];
                return;
            }

            _disposables.Add(disposable);
        }
    }

    /// <inheritdoc/>
    public IServiceDescriptor? GetOptionalServiceDescriptor(Type serviceType, object? serviceKey)
    {
        lock (_lock)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(ServiceContainer));

            var serviceIdentifier = new ServiceIdentifier(serviceType, serviceKey);

            if (_serviceDescriptorMapping.TryGetValue(serviceIdentifier, out var serviceDescriptor))
                return serviceDescriptor;
        }

        return null;
    }

    /// <inheritdoc/>
    public IServiceDescriptor GetServiceDescriptor(Type serviceType, object? serviceKey)
    {
        _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        var descriptor = GetOptionalServiceDescriptor(serviceType, serviceKey);

        return descriptor ?? throw new ServiceTypeNotRegistered(serviceType);
    }

    /// <inheritdoc/>
    public IServiceDescriptor GetServiceDescriptor<T>(object? serviceKey)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return GetServiceDescriptor(typeof(T), serviceKey);
    }

    /// <inheritdoc/>
    public IServiceDescriptor? GetOptionalServiceDescriptor<T>(object? serviceKey)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return GetOptionalServiceDescriptor(typeof(T), serviceKey);
    }

    /// <inheritdoc/>
    public object CreateInstance(Type type)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return ServiceFactory.CreateInstance(type);
    }

    /// <inheritdoc/>
    public T CreateInstance<T>()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return ServiceFactory.CreateInstance<T>();
    }

    /// <inheritdoc/>
    public IScope CreateScope()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        var scope = new Scope();
        scope.SetServiceFactory(new ScopeServiceFactory(scope, ServiceFactory));
        ServiceFactory.NotifyScopeCreated(scope);
        return scope;
    }

    /// <inheritdoc/>
    public object? GetOptionalService(Type serviceType)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return ServiceFactory.GetOptionalService(serviceType);
    }

    /// <inheritdoc/>
    public T? GetOptionalService<T>()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return ServiceFactory.GetOptionalService<T>();
    }

    /// <inheritdoc/>
    public T GetService<T>()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return ServiceFactory.GetService<T>();
    }

    /// <inheritdoc/>
    public object? GetService(Type serviceType)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return ServiceFactory.GetService(serviceType);
    }

    /// <inheritdoc/>
    public object GetKeyedService(Type serviceType, object? serviceKey)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return ServiceFactory.GetKeyedService(serviceType, serviceKey);
    }

    /// <inheritdoc/>
    public object? GetOptionalKeyedService(Type serviceType, object? serviceKey)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return ServiceFactory.GetOptionalKeyedService(serviceType, serviceKey);
    }

    /// <inheritdoc/>
    public T GetKeyedService<T>(object? serviceKey)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return ServiceFactory.GetKeyedService<T>(serviceKey);
    }

    /// <inheritdoc/>
    public T? GetOptionalKeyedService<T>(object? serviceKey)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return ServiceFactory.GetOptionalKeyedService<T>(serviceKey);
    }

    /// <inheritdoc/>
    public ConstructorInfo? GetConstructor(Type instanceType)
    {
        return ((IServiceFactory)ServiceFactory).GetConstructor(instanceType);
    }

    /// <summary>
    /// The service factory that will be used.
    /// </summary>
    public IScopedServiceFactory ServiceFactory { get; }

    /// <inheritdoc/>
    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _serviceDescriptorMapping.Count;
            }
        }
    }

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

    /// <summary>
    /// Gets the options that were used for the container.
    /// </summary>
    public ServiceContainerOptions Options { get; }

    /// <summary>
    /// Returns whether the registered services are read-only and can't be overwritten.
    /// </summary>
    public bool AreRegisteredServicesReadOnly => (Options & ServiceContainerOptions.ReadOnly) == ServiceContainerOptions.ReadOnly;

    /// <inheritdoc/>
    public bool IsDisposed => _isDisposed;
}
