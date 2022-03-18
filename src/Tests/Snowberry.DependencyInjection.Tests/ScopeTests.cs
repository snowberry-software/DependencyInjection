using Snowberry.DependencyInjection.Interfaces;
using Snowberry.DependencyInjection.Tests.Services;
using Xunit;

namespace Snowberry.DependencyInjection.Tests;

public class ScopeTests
{
    [Fact]
    public void ScopeDispose()
    {
        using var serviceContainer = new ServiceContainer();
        serviceContainer.RegisterScoped<ITestService, TestService>();

        ITestService service;
        using (var scope = serviceContainer.CreateScope())
        {
            service = scope.ServiceFactory.GetService<ITestService>();

            Assert.Equal(1, scope.DisposeableCount);

            Assert.Equal(service, scope.ServiceFactory.GetService<ITestService>());
            Assert.Equal(service, scope.ServiceFactory.GetOptionalService<ITestService>());
        }

        Assert.Equal(0, serviceContainer.DisposeableCount);
        Assert.True(service.IsDisposed);
    }

    [Fact]
    public void GlobalScope()
    {
        IServiceContainer serviceContainer;
        IScope scope;
        using (serviceContainer = new ServiceContainer())
        {
            serviceContainer.RegisterScoped<ITestService, TestService>();

            var globalService = serviceContainer.GetService<ITestService>();

            ITestService scopedService;
            using (scope = serviceContainer.CreateScope())
            {
                scopedService = scope.ServiceFactory.GetService<ITestService>();

                Assert.Equal(1, scope.DisposeableCount);
                Assert.Equal(scopedService, scope.ServiceFactory.GetService<ITestService>());
            }

            Assert.NotEqual(globalService, scopedService);

            Assert.Equal(1, serviceContainer.DisposeableCount);
            Assert.True(scopedService.IsDisposed);
        }

        Assert.Equal(0, serviceContainer.DisposeableCount);
        Assert.Equal(0, scope.DisposeableCount);
    }

    [Fact]
    public void ScopeResolvingNotEqual()
    {

    }
}
