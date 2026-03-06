using StrategyPattern.Evolution;


// Real use case if you have different kind of web api hosts
// - Serverless (Azure Functions, AWS Lambda, Google Cloud Functions)
// - EC2, AppService
var webApi = new BastaStrategyWebApi(args);

webApi.StrategyType = StrategyType.V8_Enterprise;

webApi.Run();

// This is important that you are able to use
// API test via WebApplicationFactory<Program>
// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0
namespace StrategyPattern.Evolution
{
#pragma warning disable CA1515 // !! Needed for system test to access the entry point !!
    public partial class Program;
#pragma warning restore CA1515 // !! Needed for system test to access the entry point !!
}
