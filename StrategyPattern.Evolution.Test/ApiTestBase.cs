using System.Collections.Immutable;
using AspNetCore.Simple.MsTest.Sdk;
using StrategyPattern.Evolution.Api.User.V1.Create;

[assembly: DoNotParallelize]

namespace StrategyPattern.Evolution.Test
{
    /// <summary>
    ///     Your base class for all API tests
    /// </summary>
    [TestClass]
    public abstract class ApiTestBase()
    {
        private static ApiTestBase<Program> _apiTestBase = null!;

        protected static HttpClient Client { get; private set; } = null!;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext _)
        {
            // Super simple just use the provided API test base class,
            // and you are ready to go
            _apiTestBase = new ApiTestBase<Program>("Development", 
                                                    (_, _) =>
                                                    {
                                                    }); // Configure environment variables

            // Create the client that will be used for all API calls in the tests
            Client = _apiTestBase.CreateClient();

            // Special case for snapshot testing
            AssertObjectExtensions.WriteResponse = true;
            AssertObjectExtensions.DifferenceFunc = DifferenceFunc;
        }

        private static IEnumerable<Difference> DifferenceFunc(ImmutableList<Difference> differences)
        {
            foreach (var difference in differences)
            {
                if (difference.MemberPath.Contains(nameof(User.Id), StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                yield return difference;
            }
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            _apiTestBase?.Dispose();
            Client?.Dispose();
        }
    }
}
