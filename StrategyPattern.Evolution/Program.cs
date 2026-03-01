// Hint all test cases are based on Next Level strategy.
using StrategyPattern.Evolution;

var webApi = new BastaStrategyWebApi(args);

webApi.StrategyType = StrategyType.V7_Basta;

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
