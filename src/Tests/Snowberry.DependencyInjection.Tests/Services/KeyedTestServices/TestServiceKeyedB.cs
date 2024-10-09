using Snowberry.DependencyInjection.Tests.Services.Interfaces;

namespace Snowberry.DependencyInjection.Tests.Services.KeyedTestServices;

public class TestServiceKeyedB : TestServiceKeyedA, ITestService
{
    public TestServiceKeyedB() : base()
    {
    }
}