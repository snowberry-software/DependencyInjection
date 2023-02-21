using Snowberry.DependencyInjection.Interfaces;

namespace Snowberry.DependencyInjection.Lookup.Cache;

public readonly struct ScopeServiceCacheEntry : IScopeServiceCacheEntry
{
    public ScopeServiceCacheEntry(IScope? scope, Type serviceType, object instance)
    {
        Scope = scope;
        ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        Instance = instance ?? throw new ArgumentNullException(nameof(instance));
    }

    /// <inheritdoc/>
    public IScope? Scope { get; }

    /// <inheritdoc/>
    public Type ServiceType { get; }

    /// <inheritdoc/>
    public object Instance { get; }
}
