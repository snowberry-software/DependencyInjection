namespace Snowberry.DependencyInjection.Tests.Services.Interfaces;

public interface IOpenGenericService<T>
{
    /// <summary>
    /// Gets the full name of type <typeparamref name="T"/>.
    /// </summary>
    string ToStringT();

    /// <summary>
    /// The test property.
    /// </summary>
    IOpenGenericService2<float> TestProperty { get; set; }

    /// <summary>
    /// The keyed test property.
    /// </summary>
    IOpenGenericService2<float> TestPropertyKeyed { get; set; }
}
