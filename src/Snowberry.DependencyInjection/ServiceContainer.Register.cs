using Snowberry.DependencyInjection.Helper;
using Snowberry.DependencyInjection.Implementation;
using Snowberry.DependencyInjection.Interfaces;

namespace Snowberry.DependencyInjection;

public partial class ServiceContainer
{
    /// <inheritdoc/>
    public bool IsServiceRegistered(Type serviceType, object? serviceKey)
    {
        _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        lock (_lock)
        {
            var serviceIdentifier = new ServiceIdentifier(serviceType, serviceKey);
            return _serviceDescriptorMapping.ContainsKey(serviceIdentifier);
        }
    }

    /// <inheritdoc/>
    public bool IsServiceRegistered<T>(object? serviceKey)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return IsServiceRegistered(typeof(T), serviceKey);
    }

    /// <inheritdoc/>
    public IServiceRegistry RegisterSingleton<T>(object? serviceKey = null)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return Register(
            typeof(T),
            typeof(T),
            serviceKey,
            ServiceLifetime.Singleton,
            null);
    }

    /// <inheritdoc/>
    public IServiceRegistry RegisterSingleton<T>(T instance, object? serviceKey = null)
    {
        _ = instance ?? throw new ArgumentNullException(nameof(instance));

        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return Register(
            typeof(T),
            typeof(T),
            serviceKey,
            ServiceLifetime.Singleton,
            instance);
    }

    /// <inheritdoc/>
    public IServiceRegistry RegisterSingleton<T, TImpl>(object? serviceKey = null) where TImpl : T
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return Register(
            typeof(T),
            typeof(TImpl),
            serviceKey,
            ServiceLifetime.Singleton,
            null);
    }

    /// <inheritdoc/>
    public IServiceRegistry RegisterSingleton<T, TImpl>(TImpl instance, object? serviceKey = null) where TImpl : T
    {
        _ = instance ?? throw new ArgumentNullException(nameof(instance));

        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return Register(
            typeof(T),
            typeof(TImpl),
            serviceKey,
            ServiceLifetime.Singleton,
            instance);
    }

    /// <inheritdoc/>
    public IServiceRegistry RegisterTransient<T>(object? serviceKey = null)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return Register(
            typeof(T),
            typeof(T),
            serviceKey,
            ServiceLifetime.Transient,
            null);
    }

    /// <inheritdoc/>
    public IServiceRegistry RegisterTransient<T, TImpl>(object? serviceKey = null) where TImpl : T
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return Register(
            typeof(T),
            typeof(TImpl),
            serviceKey,
            ServiceLifetime.Transient,
            null);
    }

    /// <inheritdoc/>
    public IServiceRegistry RegisterScoped<T>(object? serviceKey = null)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return Register(
            typeof(T),
            typeof(T),
            serviceKey,
            ServiceLifetime.Scoped,
            null);
    }

    /// <inheritdoc/>
    public IServiceRegistry RegisterScoped<T, TImpl>(object? serviceKey = null) where TImpl : T
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        return Register(
            typeof(T),
            typeof(TImpl),
            serviceKey,
            ServiceLifetime.Scoped,
            null);
    }

    /// <inheritdoc/>
    public IServiceRegistry Register(
        Type serviceType,
        Type implementationType,
        object? serviceKey,
        ServiceLifetime lifetime,
        object? singletonInstance)
    {
        _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        _ = implementationType ?? throw new ArgumentNullException(nameof(implementationType));

        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        if (singletonInstance != null && lifetime != ServiceLifetime.Singleton)
            throw new ArgumentException("Singleton can't be used in non-singleton lifetime!", nameof(singletonInstance));

        lock (_lock)
        {
            var serviceIdentifier = new ServiceIdentifier(serviceType, serviceKey);

            bool foundExistingServiceDescriptor = false;
            if (_serviceDescriptorMapping.ContainsKey(serviceIdentifier))
                foundExistingServiceDescriptor = true;

            var newDescriptor = lifetime switch
            {
                ServiceLifetime.Singleton => ServiceDescriptor.Singleton(serviceType, implementationType, singletonInstance),
                ServiceLifetime.Scoped => ServiceDescriptor.Scoped(serviceType, implementationType),
                ServiceLifetime.Transient => ServiceDescriptor.Transient(serviceType, implementationType),
                _ => ThrowHelper.ThrowServiceLifetimeNotImplemented(lifetime) as IServiceDescriptor
            };

            if (!foundExistingServiceDescriptor)
            {
                _serviceDescriptorMapping.TryAdd(serviceIdentifier, newDescriptor!);
                return this;
            }

            if (AreRegisteredServicesReadOnly)
                throw new InvalidOperationException($"Service type '{serviceType.FullName}' is already registered!");

            UnregisterService(serviceType, serviceKey, out _);
            _serviceDescriptorMapping.AddOrUpdate(serviceIdentifier, newDescriptor!, (_, _) => newDescriptor!);
            return this;
        }
    }

    /// <inheritdoc/>
    public IServiceRegistry UnregisterService<T>(object? serviceKey, out bool successful)
    {
        return UnregisterService(typeof(T), serviceKey, out successful);
    }

    /// <inheritdoc/>
    public IServiceRegistry UnregisterService(Type serviceType, object? serviceKey, out bool successful)
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(ServiceContainer));

        lock (_lock)
        {
            var serviceIdentifier = new ServiceIdentifier(serviceType, serviceKey);

            if (_serviceDescriptorMapping.TryRemove(serviceIdentifier, out var serviceDescriptor))
            {
                if (serviceDescriptor.Lifetime is ServiceLifetime.Singleton && serviceDescriptor.SingletonInstance is IDisposable disposableSingleton)
                {
                    _disposableContainer.Remove(disposableSingleton);
                    disposableSingleton.Dispose();
                }

                successful = true;
                return this;
            }
        }

        successful = false;
        return this;
    }
}
