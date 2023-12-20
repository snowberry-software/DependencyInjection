using Snowberry.DependencyInjection.Attributes;

namespace Snowberry.DependencyInjection.Tests.Services.PropertyTestServices;

/// <summary>
/// Used to test property injection only.
/// </summary>
public class TestAnotherServicePropertyNotRequired : ITestAnotherService
{
    public int Number => 0;

#nullable disable
    [Inject(false)]
    public ITestService TestService { get; set; }

    public ITestService TestServiceSame => TestService;
#nullable enable
}
