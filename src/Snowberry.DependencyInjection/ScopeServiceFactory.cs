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
    public ScopeServiceFactory(IScope scope!!, IServiceFactoryScoped serviceFactory!!)
    {
        Scope = scope;
        ServiceFactory = serviceFactory;
    }

    /// <inheritdoc/>
    public object CreateInstance(Type type)
    {
        return ServiceFactory.CreateInstance(type, Scope);
    }

    /// <inheritdoc/>
    public T CreateInstance<T>()
    {
        return ServiceFactory.CreateInstance<T>(Scope);
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

    /// <summary>
    /// The service factory that will be used to resolve all scope related requests.
    /// </summary>
    public IServiceFactoryScoped ServiceFactory { get; }

    /// <summary>
    /// The scope that owns this <see cref="IServiceFactory"/>.
    /// </summary>
    public IScope Scope { get; }
}
