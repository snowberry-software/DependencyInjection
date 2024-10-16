﻿using System.Reflection;
using Snowberry.DependencyInjection.Interfaces;

namespace Snowberry.DependencyInjection;

/// <summary>
/// <see cref="IServiceFactory"/> for a per-scope use.
/// </summary>
public sealed class ScopeServiceFactory : IServiceFactory
{
    /// <summary>
    /// Creates a new service factory for the given <paramref name="scope"/>.
    /// </summary>
    /// <param name="scope">The scope.</param>
    /// <param name="serviceFactory">The service factory that will be used to resolve all scope related requests.</param>
    public ScopeServiceFactory(IScope scope, IServiceFactoryScoped serviceFactory)
    {
        Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        ServiceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
    }

    /// <inheritdoc/>
    public object CreateInstance(Type type, Type[]? genericTypeParameters = null)
    {
        return ServiceFactory.CreateInstance(type, Scope, genericTypeParameters);
    }

    /// <inheritdoc/>
    public T CreateInstance<T>(Type[]? genericTypeParameters = null)
    {
        return ServiceFactory.CreateInstance<T>(Scope, genericTypeParameters);
    }

    /// <inheritdoc/>
    public object GetService(Type serviceType)
    {
        return ServiceFactory.GetService(serviceType, Scope);
    }

    /// <inheritdoc/>
    public T GetService<T>()
    {
        return ServiceFactory.GetService<T>(Scope);
    }

    /// <inheritdoc/>
    public object? GetOptionalService(Type serviceType)
    {
        return ServiceFactory.GetOptionalService(serviceType, Scope);
    }

    /// <inheritdoc/>
    public T? GetOptionalService<T>()
    {
        return ServiceFactory.GetOptionalService<T>(Scope);
    }

    /// <inheritdoc/>
    public ConstructorInfo? GetConstructor(Type instanceType)
    {
        return ServiceFactory.GetConstructor(instanceType);
    }

    /// <inheritdoc/>
    public object GetKeyedService(Type serviceType, object? serviceKey)
    {
        return ServiceFactory.GetKeyedService(serviceType, serviceKey, Scope);
    }

    /// <inheritdoc/>
    public object? GetOptionalKeyedService(Type serviceType, object? serviceKey)
    {
        return ServiceFactory.GetOptionalKeyedService(serviceType, serviceKey, Scope);
    }

    /// <inheritdoc/>
    public T GetKeyedService<T>(object? serviceKey)
    {
        return ServiceFactory.GetKeyedService<T>(serviceKey, Scope);
    }

    /// <inheritdoc/>
    public T? GetOptionalKeyedService<T>(object? serviceKey)
    {
        return ServiceFactory.GetOptionalKeyedService<T>(serviceKey, Scope);
    }

    /// <summary>
    /// The service factory that will be used to resolve all scope related requests.
    /// </summary>
    public IServiceFactoryScoped ServiceFactory { get; }

    /// <summary>
    /// The scope that owns this <see cref="IServiceFactory"/>.
    /// </summary>
    public IScope Scope { get; }
}
