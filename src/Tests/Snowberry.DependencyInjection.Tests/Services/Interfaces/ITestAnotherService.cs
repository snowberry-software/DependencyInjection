namespace Snowberry.DependencyInjection.Tests.Services.Interfaces;

public interface ITestAnotherService
{
    int Number { get; }

    ITestService TestService { get; set; }

    ITestService TestServiceSame { get; }
}
