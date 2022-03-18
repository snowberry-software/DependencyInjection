using System.Reflection;
using Snowberry.DependencyInjection.Attributes;
using Snowberry.DependencyInjection.Exceptions;
using Snowberry.DependencyInjection.Helper;
using Snowberry.DependencyInjection.Interfaces;
using Snowberry.DependencyInjection.Lookup.Cache;

namespace Snowberry.DependencyInjection.Lookup;

public class DefaultServiceFactory : IScopedServiceFactory
{
    private object _lock = new();
    private List<IScopeServiceCacheEntry> _scopeCache = new();

    public DefaultServiceFactory(IServiceDescriptorReceiver serviceDescriptorReceiver)
    {
        ServiceDescriptorReceiver = serviceDescriptorReceiver;
    }

    /// <inheritdoc/>
    public void NotifyScopeCreated(IScope scope)
    {
        scope.OnDispose += Scope_OnDispose;
    }

    /// <inheritdoc/>
    public void NotifyScopeDisposed(IScope? scope)
    {
        lock (_lock)
        {
            // Remove all scope dependent service instances.
            for (int i = _scopeCache.Count - 1; i >= 0; i--)
            {
                var entry = _scopeCache[i];

                if (entry.Scope != scope)
                    continue;

                _scopeCache.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Tries to resolve the <paramref name="serviceType"/> instance for the given <paramref name="scope"/> and it will create and register a new one if it does not exist.
    /// </summary>
    /// <param name="scope">The current scope.</param>
    /// <param name="serviceType">The service type.</param>
    /// <param name="implementationType">The implementation of the service type.</param>
    /// <returns>Either the existing instance for the <paramref name="serviceType"/> or a newly created one using the <paramref name="implementationType"/>.</returns>
    private object LocateOrCreateScopeInstance(IScope? scope, Type serviceType!!, Type implementationType!!)
    {
        lock (_lock)
        {
            if (scope != null && scope.IsDisposed)
                throw new ObjectDisposedException(nameof(IScope));

            for (int i = _scopeCache.Count - 1; i >= 0; i--)
            {
                var entry = _scopeCache[i];

                if (entry.Scope != scope)
                    continue;

                if (entry.ServiceType != serviceType)
                    continue;

                return entry.Instance;
            }
        }

        object? scopedInstance = CreateInstance(implementationType, scope);

        if (scopedInstance is IDisposable scopedDisposable)
            if (scope != null)
                scope.RegisterDisposable(scopedDisposable);
            else
                ServiceDescriptorReceiver.RegisterDisposable(scopedDisposable);

        lock (_lock)
        {
            _scopeCache.Add(new ScopeServiceCacheEntry(scope, serviceType, scopedInstance));
        }

        return scopedInstance;
    }

    private void Scope_OnDispose(object? sender, EventArgs e)
    {
        if (sender is not IScope scope)
            return;

        NotifyScopeDisposed(scope);

        scope.OnDispose -= Scope_OnDispose;
    }

    /// <inheritdoc/>
    public object CreateInstance(Type type!!)
    {
        return CreateInstance(type, null);
    }

    /// <inheritdoc/>
    public T CreateInstance<T>()
    {
        return CreateInstance<T>(null);
    }

    /// <inheritdoc/>
    public object CreateInstance(Type type!!, IScope? scope)
    {
        if (type.IsInterface || type.IsAbstract)
            throw new InvalidServiceImplementationType(type, $"Cannot instantiate abstract classes or interfaces! ({type.FullName})!");

        // Get the constructor with the largest number of parameters.
        var constructor = type.GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (constructor == null)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type)!;

        }
        else
        {
            object[] args = constructor.GetParameters()
                .Select(param => GetInstanceFromServiceType(param.ParameterType, scope))
                .ToArray();

            object? instance = constructor.Invoke(args);

            var properties = type.GetProperties()
                .Where(x => x.SetMethod != null);

            foreach (var property in properties)
            {
                var injectAttribute = property.GetCustomAttribute<InjectAttribute>();

                if (injectAttribute == null)
                    continue;

                object? propertyValue = GetOptionalService(property.PropertyType, scope);

                if (injectAttribute.IsRequired && propertyValue is null)
                    throw new ServiceTypeNotRegistered(property.PropertyType, $"The required service for the property `{property.Name}` is not registered!");

                property.SetValue(instance, propertyValue);
            }

            return instance;
        }

        ThrowHelper.ThrowInvalidConstructor(type);
        return null!;

    }

    protected object GetInstanceFromServiceType(Type serviceType!!, IScope? scope)
    {
        var descriptor = ServiceDescriptorReceiver.GetServiceDescriptor(serviceType);
        return GetInstanceFromDescriptor(descriptor, scope);
    }

    protected object GetInstanceFromDescriptor(IServiceDescriptor serviceDescriptor!!, IScope? scope)
    {
        switch (serviceDescriptor.Lifetime)
        {
            case ServiceLifetime.Singleton:

                // Only dispose the singleton if no explicit instance has been registered before.
                if (serviceDescriptor.SingletonInstance == null)
                {
                    serviceDescriptor.SingletonInstance = CreateInstance(serviceDescriptor.ImplementationType, scope);

                    if (serviceDescriptor.SingletonInstance is IDisposable singletonDisposable)
                        ServiceDescriptorReceiver.RegisterDisposable(singletonDisposable);
                }

                return serviceDescriptor.SingletonInstance;

            case ServiceLifetime.Transient:

                object? transientInstance = CreateInstance(serviceDescriptor.ImplementationType, scope);

                if (transientInstance is IDisposable transientDisposable)
                {
                    if (scope != null)
                        scope.RegisterDisposable(transientDisposable);
                    else
                        ServiceDescriptorReceiver.RegisterDisposable(transientDisposable);
                }

                return transientInstance;

            case ServiceLifetime.Scoped:
                {
                    return LocateOrCreateScopeInstance(scope, serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType);
                }

            default:
                return ThrowHelper.ThrowServiceLifetimeNotImplemented(serviceDescriptor.Lifetime);
        }
    }

    /// <inheritdoc/>
    public T CreateInstance<T>(IScope? scope)
    {
        object service = CreateInstance(typeof(T), scope)!;

        if (service is T type)
            return type;

        ThrowHelper.ThrowInvalidServiceImplementationCast(typeof(T), service.GetType());
        return default!;
    }

    /// <inheritdoc/>
    public object? GetOptionalService(Type serviceType!!)
    {
        return GetOptionalService(serviceType, null);
    }

    /// <inheritdoc/>
    public T? GetOptionalService<T>()
    {
        return GetOptionalService<T>(null);
    }

    /// <inheritdoc/>
    public object? GetOptionalService(Type serviceType!!, IScope? scope)
    {
        var descriptor = ServiceDescriptorReceiver.GetOptionalServiceDescriptor(serviceType);

        if (descriptor == null)
            return null;

        return GetInstanceFromDescriptor(descriptor, scope);
    }

    /// <inheritdoc/>
    public T? GetOptionalService<T>(IScope? scope)
    {
        object? service = GetOptionalService(typeof(T), scope)!;

        if (service is T type)
            return type;

        return default;
    }

    /// <inheritdoc/>
    public T GetService<T>()
    {
        return GetService<T>(null);
    }

    /// <inheritdoc/>
    public object? GetService(Type serviceType!!)
    {
        return GetService(serviceType, null);
    }

    /// <inheritdoc/>
    public object GetService(Type serviceType!!, IScope? scope)
    {
        object? service = GetOptionalService(serviceType, scope);

        if (service == null)
            throw new ServiceTypeNotRegistered(serviceType);

        return service;
    }

    /// <inheritdoc/>
    public T GetService<T>(IScope? scope)
    {
        object service = GetService(typeof(T), scope)!;

        if (service is T type)
            return type;

        ThrowHelper.ThrowInvalidServiceImplementationCast(typeof(T), service.GetType());
        return default!;
    }

    /// <summary>
    /// The receiver that will be used to resolve a <see cref="IServiceDescriptor"/>.
    /// </summary>
    public IServiceDescriptorReceiver ServiceDescriptorReceiver { get; }
}
