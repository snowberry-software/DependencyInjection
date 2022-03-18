namespace Snowberry.DependencyInjection.Interfaces;

/// <summary>
/// Provides a method to register disposable objects.
/// </summary>
public interface IDisposeableContainer
{
    /// <summary>
    /// Registers a disposable object.
    /// </summary>
    /// <param name="disposable">The disposeable object to register.</param>
    void RegisterDisposable(IDisposable disposable);

    /// <summary>
    /// Gets the amound of registered disposable objects.
    /// </summary>
    int DisposeableCount { get; }
}
