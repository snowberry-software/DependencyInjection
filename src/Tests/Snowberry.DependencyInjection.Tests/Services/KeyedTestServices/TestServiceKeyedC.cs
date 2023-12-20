using Snowberry.DependencyInjection.Attributes;

namespace Snowberry.DependencyInjection.Tests.Services.KeyedTestServices;

public class TestServiceKeyedC : IKeyedTestService
{
    [PreferredConstructor]
    public TestServiceKeyedC([KeyedService("_KEY1_")] ITestService testService)
    {
        KeyedConstructorTestService = testService;
    }

    public ITestService? KeyedConstructorTestService { get; }

    [Inject]
    [KeyedService("_KEY1_")]
    public ITestService? KeyedPropertyInjectTestService { get; set; }
}
