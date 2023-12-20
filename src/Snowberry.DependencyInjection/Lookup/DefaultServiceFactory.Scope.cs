using Snowberry.DependencyInjection.Exceptions;
using Snowberry.DependencyInjection.Helper;
using Snowberry.DependencyInjection.Interfaces;

namespace Snowberry.DependencyInjection.Lookup;

public partial class DefaultServiceFactory
{
    /// <inheritdoc/>
    public object GetService(Type serviceType, IScope? scope)
    {
        _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        object? service = GetOptionalService(serviceType, scope);

        return service ?? throw new ServiceTypeNotRegistered(serviceType);
    }

    /// <inheritdoc/>
    public object? GetOptionalService(Type serviceType, IScope? scope)
    {
        _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        var descriptor = ServiceDescriptorReceiver.GetOptionalServiceDescriptor(serviceType, null);

        if (descriptor == null)
            return null;

        return GetInstanceFromDescriptor(descriptor, scope, null);
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

    /// <inheritdoc/>
    public T? GetOptionalService<T>(IScope? scope)
    {
        object? service = GetOptionalService(typeof(T), scope)!;

        if (service is T type)
            return type;

        return default;
    }

    /// <inheritdoc/>
    public object GetKeyedService(Type serviceType, object? serviceKey, IScope? scope)
    {
        _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        object? service = GetOptionalKeyedService(serviceType, serviceKey, scope);

        return service ?? throw new ServiceTypeNotRegistered(serviceType);
    }

    /// <inheritdoc/>
    public object? GetOptionalKeyedService(Type serviceType, object? serviceKey, IScope? scope)
    {
        _ = serviceType ?? throw new ArgumentNullException(nameof(serviceType));

        var descriptor = ServiceDescriptorReceiver.GetOptionalServiceDescriptor(serviceType, serviceKey);

        if (descriptor == null)
            return null;

        return GetInstanceFromDescriptor(descriptor, scope, serviceKey);
    }

    /// <inheritdoc/>
    public T GetKeyedService<T>(object? serviceKey, IScope? scope)
    {
        object service = GetKeyedService(typeof(T), serviceKey, scope)!;

        if (service is T type)
            return type;

        ThrowHelper.ThrowInvalidServiceImplementationCast(typeof(T), service.GetType());
        return default!;
    }

    /// <inheritdoc/>
    public T? GetOptionalKeyedService<T>(object? serviceKey, IScope? scope)
    {
        object? service = GetOptionalKeyedService(typeof(T), serviceKey, scope)!;

        if (service is T type)
            return type;

        return default;
    }
}
