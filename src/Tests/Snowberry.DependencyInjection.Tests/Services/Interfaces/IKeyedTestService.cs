namespace Snowberry.DependencyInjection.Tests.Services.Interfaces;

public interface IKeyedTestService
{
    ITestService? KeyedConstructorTestService { get; }

    ITestService? KeyedPropertyInjectTestService { get; set; }
}
