using Snowberry.DependencyInjection.Tests.Services;
using Snowberry.DependencyInjection.Tests.Services.Interfaces;
using Xunit;

namespace Snowberry.DependencyInjection.Tests;

public class TransientTests
{
    [Fact]
    public void RegisterTransientWithSameType()
    {
        using var serviceContainer = new ServiceContainer();
        serviceContainer.RegisterTransient<TestService>();

        Assert.Equal(1, serviceContainer.Count);

        TestTransient<TestService>(serviceContainer);
    }

    [Fact]
    public void RegisterTransient()
    {
        using var serviceContainer = new ServiceContainer();
        serviceContainer.RegisterTransient<ITestService, TestService>();

        Assert.Equal(1, serviceContainer.Count);

        TestTransient<ITestService>(serviceContainer);
    }

    [Fact]
    public void RegisterTransientWithFactory()
    {
        using var serviceContainer = new ServiceContainer();
        serviceContainer.RegisterTransient<ITestService, TestService>(instanceFactory: (sp, serviceKey) =>
        {
            return new()
            {
                Name = "Factory1337"
            };
        });

        Assert.Equal(1, serviceContainer.Count);

        var serviceB = TestTransient<ITestService>(serviceContainer);
        Assert.Equal("Factory1337", serviceB.Name);
    }

    public static T TestTransient<T>(ServiceContainer serviceContainer) where T : ITestService
    {
        _ = serviceContainer ?? throw new ArgumentNullException(nameof(serviceContainer));

        Assert.Equal(1, serviceContainer.Count);

        var serviceA = serviceContainer.GetService<T>();
        Assert.NotNull(serviceA);

        var serviceB = serviceContainer.GetService<T>();
        Assert.NotNull(serviceB);

        Assert.NotEqual(serviceA, serviceB);

        serviceA.Name = "x";

        Assert.NotSame(serviceA.Name, serviceB.Name);
        return serviceB;
    }
}
