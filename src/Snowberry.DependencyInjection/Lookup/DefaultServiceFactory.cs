using Snowberry.DependencyInjection.Helper;
using Snowberry.DependencyInjection.Implementation;
using Snowberry.DependencyInjection.Interfaces;
using Snowberry.DependencyInjection.Lookup.Cache;

namespace Snowberry.DependencyInjection.Lookup;

public partial class DefaultServiceFactory : IScopedServiceFactory
{
    private object _lock = new();
    private List<IScopeServiceCacheEntry> _scopeCache = [];

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
    /// <param name="serviceDescriptor">The service descriptor.</param>
    /// <param name="serviceKey">The service key.</param>
    /// <returns>Either the existing instance for the <paramref name="serviceType"/> or a newly created one using the <paramref name="implementationType"/>.</returns>
    private object LocateOrCreateScopeInstance(IScope? scope, IServiceDescriptor serviceDescriptor, object? serviceKey)
    {
        var serviceIdentifier = new ServiceIdentifier(serviceDescriptor.ServiceType, serviceKey);

        lock (_lock)
        {
            if (scope != null && scope.IsDisposed)
                throw new ObjectDisposedException(nameof(IScope));

            for (int i = _scopeCache.Count - 1; i >= 0; i--)
            {
                var entry = _scopeCache[i];

                if (entry.Scope != scope)
                    continue;

                if (!entry.ServiceIdentifier.Equals(serviceIdentifier))
                    continue;

                return entry.Instance;
            }
        }

        object? scopedInstance = CreateInstance(serviceDescriptor.ImplementationType, scope);

        if (scopedInstance is IDisposable scopedDisposable)
            if (scope != null)
                scope.RegisterDisposable(scopedDisposable);
            else
                ServiceDescriptorReceiver.RegisterDisposable(scopedDisposable);

        lock (_lock)
        {
            _scopeCache.Add(new ScopeServiceCacheEntry(
                scope,
                serviceIdentifier,
                scopedInstance));
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

    protected object GetInstanceFromServiceType(Type serviceType, IScope? scope, object? serviceKey)
    {
        _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        var typeCode = Type.GetTypeCode(serviceType);
        if (serviceType.IsValueType && typeCode is not TypeCode.Empty and not TypeCode.Object)
            return CreateBuiltInType(serviceType);

        var descriptor = ServiceDescriptorReceiver.GetServiceDescriptor(serviceType, serviceKey);
        return GetInstanceFromDescriptor(descriptor, scope, serviceKey);
    }

    protected object CreateBuiltInType(Type type)
    {
        var typeCode = Type.GetTypeCode(type);
        return typeCode switch
        {
            TypeCode.Empty => throw new NotImplementedException(),
            TypeCode.Object => throw new NotImplementedException(),
            TypeCode.Boolean => false,
            TypeCode.Byte => (byte)0,
            TypeCode.Char => (char)0,
            TypeCode.DateTime => DateTime.Now,
            TypeCode.DBNull => DBNull.Value,
            TypeCode.Decimal => (decimal)0,
            TypeCode.Double => 0D,
            TypeCode.Int16 => (short)0,
            TypeCode.Int32 => 0,
            TypeCode.Int64 => (long)0,
            TypeCode.SByte => (sbyte)0,
            TypeCode.Single => 0F,
            TypeCode.String => "",
            TypeCode.UInt16 => (ushort)0,
            TypeCode.UInt32 => (uint)0,
            TypeCode.UInt64 => (ulong)0,
            _ => Activator.CreateInstance(type)
        };
    }

    protected object GetInstanceFromDescriptor(IServiceDescriptor serviceDescriptor, IScope? scope, object? serviceKey)
    {
        _ = serviceDescriptor ?? throw new ArgumentNullException(nameof(serviceDescriptor));

        switch (serviceDescriptor.Lifetime)
        {
            case ServiceLifetime.Singleton:

                // NOTE(VNC): Only register the disposable of the singleton if no explicit instance has been set before.
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
                    return LocateOrCreateScopeInstance(scope, serviceDescriptor, serviceKey);
                }

            default:
                return ThrowHelper.ThrowServiceLifetimeNotImplemented(serviceDescriptor.Lifetime);
        }
    }

    /// <inheritdoc/>
    public T? GetOptionalService<T>()
    {
        return GetOptionalService<T>(null);
    }

    /// <inheritdoc/>
    public T GetService<T>()
    {
        return GetService<T>(null);
    }

    /// <inheritdoc/>
    public object? GetService(Type serviceType)
    {
        _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        return GetService(serviceType, null);
    }

    /// <inheritdoc/>
    public object? GetOptionalService(Type serviceType)
    {
        _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        return GetOptionalService(serviceType, null);
    }

    /// <inheritdoc/>
    public object GetKeyedService(Type serviceType, object? serviceKey)
    {
        _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        return GetKeyedService(serviceType, serviceKey, null);
    }

    /// <inheritdoc/>
    public object? GetOptionalKeyedService(Type serviceType, object? serviceKey)
    {
        _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        return GetOptionalKeyedService(serviceType, serviceKey, null);
    }

    /// <inheritdoc/>
    public T GetKeyedService<T>(object? serviceKey)
    {
        return GetKeyedService<T>(serviceKey, null);
    }

    /// <inheritdoc/>
    public T? GetOptionalKeyedService<T>(object? serviceKey)
    {
        return GetOptionalKeyedService<T>(serviceKey, null);
    }

    /// <summary>
    /// The receiver that will be used to resolve a <see cref="IServiceDescriptor"/>.
    /// </summary>
    public IServiceDescriptorReceiver ServiceDescriptorReceiver { get; }
}
