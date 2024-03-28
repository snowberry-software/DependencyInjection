using Snowberry.DependencyInjection.Tests.Services;
using Snowberry.DependencyInjection.Tests.Services.KeyedTestServices;
using Xunit;

namespace Snowberry.DependencyInjection.Tests;

public class KeyedTests
{
    [Fact]
    public void Keyed_Simple_Singleton()
    {
        using var serviceContainer = new ServiceContainer();
        serviceContainer.RegisterSingleton<ITestService>(new TestService()
        {
            Name = "1"
        });
        serviceContainer.RegisterSingleton<ITestService>(new TestService()
        {
            Name = "2"
        }, "_KEY_");

        Assert.Equal("2", serviceContainer.GetKeyedService<ITestService>("_KEY_").Name);
        Assert.Equal(2, serviceContainer.Count);
        Assert.Equal(2, serviceContainer.GetServiceDescriptors().Length);
    }

    [Fact]
    public void Keyed_Simple_Transient()
    {
        using var serviceContainer = new ServiceContainer();
        serviceContainer.RegisterTransient<ITestService, TestServiceKeyedA>("_KEY0_");
        serviceContainer.RegisterTransient<ITestService, TestServiceKeyedB>("_KEY1_");
        serviceContainer.RegisterTransient<IKeyedTestService, TestServiceKeyedC>("_KEY2_");

        var a = serviceContainer.GetKeyedService<ITestService>("_KEY0_");
        var b = serviceContainer.GetKeyedService<ITestService>("_KEY1_");
        var c = serviceContainer.GetKeyedService<IKeyedTestService>("_KEY2_");

        Assert.IsType<TestServiceKeyedA>(a);
        Assert.IsType<TestServiceKeyedB>(b);
        Assert.IsType<TestServiceKeyedC>(c);

        Assert.NotNull(c.KeyedConstructorTestService);
        Assert.NotNull(c.KeyedPropertyInjectTestService);
        Assert.IsType<TestServiceKeyedB>(c.KeyedConstructorTestService);
        Assert.IsType<TestServiceKeyedB>(c.KeyedPropertyInjectTestService);
    }

    [Fact]
    public void Keyed_Simple_Scoped()
    {
        using var serviceContainer = new ServiceContainer();
        serviceContainer.RegisterScoped<ITestService, TestServiceKeyedA>("_KEY0_");
        serviceContainer.RegisterScoped<ITestService, TestServiceKeyedB>("_KEY1_");
        serviceContainer.RegisterScoped<IKeyedTestService, TestServiceKeyedC>("_KEY2_");

        ITestService scopedService;
        using (var scope = serviceContainer.CreateScope())
        {
            var a = scopedService = scope.ServiceFactory.GetKeyedService<ITestService>("_KEY0_");
            var b = scope.ServiceFactory.GetKeyedService<ITestService>("_KEY1_");
            var c = scope.ServiceFactory.GetKeyedService<IKeyedTestService>("_KEY2_");

            Assert.IsType<TestServiceKeyedA>(a);
            Assert.IsType<TestServiceKeyedB>(b);
            Assert.IsType<TestServiceKeyedC>(c);

            Assert.NotNull(c.KeyedConstructorTestService);
            Assert.NotNull(c.KeyedPropertyInjectTestService);
            Assert.IsType<TestServiceKeyedB>(c.KeyedConstructorTestService);
            Assert.IsType<TestServiceKeyedB>(c.KeyedPropertyInjectTestService);

            Assert.StrictEqual(
                scope.ServiceFactory.GetKeyedService<ITestService>("_KEY1_"),
                scope.ServiceFactory.GetKeyedService<ITestService>("_KEY1_"));

            Assert.True(ReferenceEquals(
                scope.ServiceFactory.GetKeyedService<ITestService>("_KEY1_"),
                scope.ServiceFactory.GetKeyedService<ITestService>("_KEY1_")));
        }

        Assert.True(scopedService.IsDisposed);
        Assert.NotEqual(serviceContainer.GetKeyedService<ITestService>("_KEY0_"), scopedService);
    }
}
