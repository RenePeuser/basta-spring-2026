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

        /// <summary>
        ///     Initializes the test assembly by setting up the API test environment.
        ///     Import this happens one time before all tests are running. This is
        ///     like your prod case. Because your API is running continuously.
        /// </summary>
        /// <param name="_">The test context.</param>
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext _)
        {
            // 1. Super simple just use the provided API test base class and you are ready to go
            _apiTestBase = new ApiTestBase<Program>("Development", // The environment name
                                                    (_, _) =>
                                                    {
                                                    }); // Configure environment variables

            Client = _apiTestBase.CreateClient();

            AssertObjectExtensions.WriteResponse = false;
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

        protected static string GetUniqueRunnerName()
        {
            // Hint: In real world app use caller file path and create a unique name
            // based on that. This way you can run tests in parallel without conflicts.
            return Environment.MachineName;
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            _apiTestBase?.Dispose();
            Client?.Dispose();
        }
    }
}
