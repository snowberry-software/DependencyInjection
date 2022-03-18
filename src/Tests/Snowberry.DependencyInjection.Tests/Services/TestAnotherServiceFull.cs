using Snowberry.DependencyInjection.Attributes;

namespace Snowberry.DependencyInjection.Tests.Services;

/// <summary>
/// Used to test constructor and property injection.
/// </summary>
internal class TestAnotherServiceFull : ITestAnotherService
{
    public TestAnotherServiceFull(ITestService testService)
    {
        TestServiceSame = testService;
    }

#nullable disable
    [Inject]
    public ITestService TestService { get; set; }

    public ITestService TestServiceSame { get; }
#nullable enable

    public int Number { get; }
}
