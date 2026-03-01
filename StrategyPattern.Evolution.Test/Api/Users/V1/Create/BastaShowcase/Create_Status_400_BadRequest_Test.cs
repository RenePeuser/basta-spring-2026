using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.AspNetCore.Mvc;

namespace StrategyPattern.Evolution.Test.Api.Users.V1.Create.BastaShowcase
{
    [TestClass]
    [TestCategory("Basta")]
    public class Basta_Show_Case_Test : ApiTestBase
    {
        [TestMethod]
        [DynamicRequestLocator]
        public Task What_Is_The_Problem(string useCase)
        {
            return Client.AssertPostAsErrorAsync<ProblemDetails>("api/v1/users",
                                                                 useCase,
                                                                 useCase);
        }
    }
}
