﻿using Snowberry.DependencyInjection.Tests.Services;
using Snowberry.DependencyInjection.Tests.Services.AsyncDisposeTestServices;
using Snowberry.DependencyInjection.Tests.Services.Interfaces;
using Xunit;

namespace Snowberry.DependencyInjection.Tests;

#if NETCOREAPP
public class AsyncDisposeTests
{
    [Fact]
    public async Task SingleService()
    {
        ITestService testService;
        var serviceContainer = new ServiceContainer();
        {
            serviceContainer.RegisterScoped<ITestService, TestAsyncService>();

            testService = serviceContainer.GetService<ITestService>();
            testService.Name = "Test";
        }

        await serviceContainer.DisposeAsync();

        Assert.Equal("Test", testService.Name);
        Assert.True(testService.IsDisposed);
    }

    [Fact]
    public async Task SingleServiceWithScopeAndTransient()
    {
        ITestService testService;
        ITestService testServiceScoped;
        var serviceContainer = new ServiceContainer();
        {
            serviceContainer.RegisterScoped<ITestService, TestAsyncService>();
            serviceContainer.RegisterScoped<ITestAnotherService, TestAnotherServiceAsync>();
            serviceContainer.RegisterScoped<ITestService, TestAsyncService>("scopedKey");
            serviceContainer.RegisterScoped<ITestService, TestService>("defaultDispose");

            testService = serviceContainer.GetService<ITestService>();
            testService.Name = "Test";

            var scope = serviceContainer.CreateScope();
            testServiceScoped = scope.ServiceFactory.GetService<ITestService>();
            testServiceScoped.Name = "Test2";

            var testServiceScopedKey = scope.ServiceFactory.GetKeyedService<ITestService>("scopedKey");
            Assert.NotEqual(testServiceScoped, testServiceScopedKey);
            Assert.NotEqual(testServiceScoped.Name, testServiceScopedKey.Name);

            var defaultDisposeService = scope.ServiceFactory.GetKeyedService<ITestService>("defaultDispose");
            var anotherService = (TestAnotherServiceAsync)scope.ServiceFactory.GetService<ITestAnotherService>();

            Assert.Equal(4, scope.DisposableCount);
            await scope.DisposeAsync();

            Assert.True(testServiceScopedKey.IsDisposed);
            Assert.True(defaultDisposeService.IsDisposed);
            Assert.True(anotherService.IsDisposed);
        }

        Assert.Equal(1, serviceContainer.DisposableCount);
        await serviceContainer.DisposeAsync();

        Assert.Equal("Test", testService.Name);
        Assert.True(testService.IsDisposed);
        Assert.Equal(1, serviceContainer.DisposableCount);

        Assert.Equal("Test2", testServiceScoped.Name);
        Assert.True(testServiceScoped.IsDisposed);
    }
}
#endif
