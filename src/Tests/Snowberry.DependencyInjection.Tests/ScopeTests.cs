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

            Assert.Equal(1, scope.DisposableCount);

            Assert.Equal(service, scope.ServiceFactory.GetService<ITestService>());
            Assert.Equal(service, scope.ServiceFactory.GetOptionalService<ITestService>());
        }

        Assert.Equal(0, serviceContainer.DisposableCount);
        Assert.True(service.IsDisposed);
    }

    [Fact]
    public void GlobalScope()
    {
        ITestService globalTestService;
        ITestService scopedTestService;

        IServiceContainer serviceContainer;
        IScope scope;
        using (serviceContainer = new ServiceContainer())
        {
            serviceContainer.RegisterScoped<ITestService, TestService>();

            globalTestService = serviceContainer.GetService<ITestService>();

            using (scope = serviceContainer.CreateScope())
            {
                scopedTestService = scope.ServiceFactory.GetService<ITestService>();

                Assert.Equal(1, scope.DisposableCount);
                Assert.Equal(scopedTestService, scope.ServiceFactory.GetService<ITestService>());
            }

            Assert.NotEqual(globalTestService, scopedTestService);
            Assert.True(scopedTestService.IsDisposed);

            Assert.Equal(1, scope.DisposableCount);
            Assert.Equal(1, serviceContainer.DisposableCount);
        }

        Assert.Equal(1, serviceContainer.DisposableCount);
        Assert.Equal(1, scope.DisposableCount);
        Assert.True(scope.IsDisposed);
        Assert.True(serviceContainer.IsDisposed);
    }

    [Fact]
    public void ScopeResolvingNotEqual()
    {

    }
}
