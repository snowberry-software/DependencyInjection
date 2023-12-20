namespace Snowberry.DependencyInjection.Attributes;

/// <summary>
/// Specifies which key should be used to receive the service.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public sealed class KeyedServiceAttribute : Attribute
{
    public KeyedServiceAttribute(object? serviceKey)
    {
        ServiceKey = serviceKey;
    }

    /// <summary>
    /// The optional service key.
    /// </summary>
    public object? ServiceKey { get; }
}
