namespace Snowberry.DependencyInjection.Tests.Services;

public interface ITestAnotherService
{
    int Number { get; }

    ITestService TestService { get; set; }

    ITestService TestServiceSame { get; }
}
