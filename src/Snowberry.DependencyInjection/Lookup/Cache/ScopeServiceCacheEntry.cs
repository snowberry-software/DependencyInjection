using Snowberry.DependencyInjection.Interfaces;

namespace Snowberry.DependencyInjection.Lookup.Cache;

public struct ScopeServiceCacheEntry : IScopeServiceCacheEntry
{
    public ScopeServiceCacheEntry(IScope? scope, Type serviceType!!, object instance!!)
    {
        Scope = scope;
        ServiceType = serviceType;
        Instance = instance;
    }

    /// <inheritdoc/>
    public IScope? Scope { get; }

    /// <inheritdoc/>
    public Type ServiceType { get; }

    /// <inheritdoc/>
    public object Instance { get; }
}
