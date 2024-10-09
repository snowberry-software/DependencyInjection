using Snowberry.DependencyInjection.Tests.Services.Interfaces;

namespace Snowberry.DependencyInjection.Tests.Services;

public class OpenGenericService2<T> : IOpenGenericService2<T>
{
    /// <inheritdoc/>
    public string ToStringT()
    {
        return typeof(T).FullName ?? string.Empty;
    }
}
