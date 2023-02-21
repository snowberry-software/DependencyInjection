using Snowberry.DependencyInjection.Exceptions;
using Snowberry.DependencyInjection.Helper;
using Snowberry.DependencyInjection.Interfaces;
using Snowberry.DependencyInjection.Lookup;

namespace Snowberry.DependencyInjection;

public class ServiceContainer : IServiceContainer
{
    private List<IServiceDescriptor> _serviceDescriptors = new();
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
                _disposables = new List<IDisposable>() { disposable };
                return;
            }

            _disposables.Add(disposable);
        }
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
    public IServiceDescriptor? GetOptionalServiceDescriptor(Type serviceType)
    {
        lock (_lock)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(ServiceContainer));

            for (int i = 0; i < _serviceDescriptors.Count; i++)
            {
                var serviceDescriptor = _serviceDescriptors[i];
                if (serviceDescriptor.ServiceType == serviceType)
                    return serviceDescriptor;
            }
        }

        return null;
    }

    /// <inheritdoc/>
    public IServiceDescriptor? GetOptionalServiceDescriptor<T>()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return GetOptionalServiceDescriptor(typeof(T));
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
    public IServiceDescriptor GetServiceDescriptor(Type serviceType)
    {
        _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        var descriptor = GetOptionalServiceDescriptor(serviceType);

        return descriptor ?? throw new ServiceTypeNotRegistered(serviceType);
    }

    /// <inheritdoc/>
    public IServiceDescriptor GetServiceDescriptor<T>()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return GetServiceDescriptor(typeof(T));
    }

    /// <inheritdoc/>
    public bool IsServiceRegistered(Type serviceType)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        lock (_lock)
        {
            for (int i = 0; i < _serviceDescriptors.Count; i++)
                if (_serviceDescriptors[i].ServiceType == serviceType)
                    return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public bool IsServiceRegistered<T>()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return IsServiceRegistered(typeof(T));
    }

    /// <inheritdoc/>
    public IServiceRegistry RegisterSingleton<T>()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return Register(typeof(T), typeof(T), ServiceLifetime.Singleton, null);
    }

    /// <inheritdoc/>
    public IServiceRegistry RegisterSingleton<T>(T instance)
    {
        _ = instance ?? throw new ArgumentNullException(nameof(instance));

        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return Register(typeof(T), typeof(T), ServiceLifetime.Singleton, instance);
    }

    /// <inheritdoc/>
    public IServiceRegistry RegisterSingleton<T, TImpl>() where TImpl : T
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return Register(typeof(T), typeof(TImpl), ServiceLifetime.Singleton, null);
    }

    /// <inheritdoc/>
    public IServiceRegistry RegisterSingleton<T, TImpl>(TImpl instance) where TImpl : T
    {
        _ = instance ?? throw new ArgumentNullException(nameof(instance));

        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return Register(typeof(T), typeof(TImpl), ServiceLifetime.Singleton, instance);
    }

    /// <inheritdoc/>
    public IServiceRegistry RegisterTransient<T>()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return Register(typeof(T), typeof(T), ServiceLifetime.Transient, null);
    }

    /// <inheritdoc/>
    public IServiceRegistry RegisterTransient<T, TImpl>() where TImpl : T
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return Register(typeof(T), typeof(TImpl), ServiceLifetime.Transient, null);
    }

    /// <inheritdoc/>
    public IServiceRegistry RegisterScoped<T>()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return Register(typeof(T), typeof(T), ServiceLifetime.Scoped, null);
    }

    /// <inheritdoc/>
    public IServiceRegistry RegisterScoped<T, TImpl>() where TImpl : T
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return Register(typeof(T), typeof(TImpl), ServiceLifetime.Scoped, null);
    }

    /// <inheritdoc/>
    public IServiceRegistry Register(Type serviceType, Type implementationType, ServiceLifetime lifetime, object? singletonInstance)
    {
        _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        _ = implementationType ?? throw new ArgumentNullException(nameof(implementationType));

        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        if (singletonInstance != null && lifetime != ServiceLifetime.Singleton)
            throw new ArgumentException("Singleton can't be used in non-singleton lifetime!", nameof(singletonInstance));

        lock (_lock)
        {
            int index = _serviceDescriptors.FindIndex(x => x.ServiceType == serviceType);

            var descriptor = lifetime switch
            {
                ServiceLifetime.Singleton => ServiceDescriptor.Singleton(serviceType, implementationType, singletonInstance),
                ServiceLifetime.Scoped => ServiceDescriptor.Scoped(serviceType, implementationType),
                ServiceLifetime.Transient => ServiceDescriptor.Transient(serviceType, implementationType),
                _ => ThrowHelper.ThrowServiceLifetimeNotImplemented(lifetime) as IServiceDescriptor
            };

            if (index == -1)
            {
                _serviceDescriptors.Add(descriptor!);
                return this;
            }

            if (AreRegisteredServicesReadOnly)
                throw new InvalidOperationException($"Service type '{serviceType.FullName}' is already registered!");

            _serviceDescriptors[index] = descriptor!;
            return this;
        }
    }

    /// <inheritdoc/>
    public IServiceRegistry UnregisterService<T>(out bool successful)
    {
        return UnregisterService(typeof(T), out successful);
    }

    /// <inheritdoc/>
    public IServiceRegistry UnregisterService(Type serviceType, out bool successful)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        lock (_lock)
        {
            for (int i = _serviceDescriptors.Count - 1; i >= 0; i--)
            {
                var serviceDescriptor = _serviceDescriptors[i];

                if (serviceDescriptor.ServiceType == serviceType)
                {
                    if (serviceDescriptor.Lifetime is ServiceLifetime.Singleton && serviceDescriptor.SingletonInstance is IDisposable disposableSingleton)
                    {
                        _disposables?.Remove(disposableSingleton);
                        disposableSingleton.Dispose();
                    }

                    _serviceDescriptors.RemoveAt(i);
                    successful = true;

                    return this;
                }
            }
        }

        successful = false;
        return this;
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
                return _serviceDescriptors.Count;
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
