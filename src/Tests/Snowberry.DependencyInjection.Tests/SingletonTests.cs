using Snowberry.DependencyInjection.Tests.Services;
using Snowberry.DependencyInjection.Tests.Services.Interfaces;
using Xunit;

namespace Snowberry.DependencyInjection.Tests;

public class SingletonTests
{
    [Fact]
    public void RegisterSingletonWithSameType()
    {
        using var serviceContainer = new ServiceContainer();
        serviceContainer.RegisterSingleton<TestService>();

        Assert.Equal(1, serviceContainer.Count);

        var service = serviceContainer.GetService<TestService>();
        Assert.NotNull(service);
    }

    [Fact]
    public void RegisterSingletonWithProvidedInstance()
    {
        using var serviceContainer = new ServiceContainer();
        serviceContainer.RegisterSingleton<ITestService>(new TestService()
        {
            Name = "1"
        });

        Assert.Equal(1, serviceContainer.Count);

        var service = serviceContainer.GetService<ITestService>();
        Assert.NotNull(service);
        Assert.Equal("1", service.Name);
    }

    [Fact]
    public void RegisterSingletonWithoutProvidedInstance()
    {
        using var serviceContainer = new ServiceContainer();
        serviceContainer.RegisterSingleton<ITestService, TestService>();

        Assert.Equal(1, serviceContainer.Count);

        var service = serviceContainer.GetService<ITestService>();
        Assert.NotNull(service);
    }
}
