namespace Snowberry.DependencyInjection.Tests.Services.Interfaces;

public interface IOpenGenericService2<T>
{
    /// <summary>
    /// Gets the full name of type <typeparamref name="T"/>.
    /// </summary>
    string ToStringT();
}
