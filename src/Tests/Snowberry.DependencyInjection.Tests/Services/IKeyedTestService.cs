namespace Snowberry.DependencyInjection.Tests.Services;

public interface IKeyedTestService
{
    ITestService? KeyedConstructorTestService { get; }

    ITestService? KeyedPropertyInjectTestService { get; set; }
}
