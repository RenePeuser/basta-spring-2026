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
        public Task Should_Return_A_Beautiful_Response_That_We_Know_What_Was_Wrong(string useCase)
        {
            return Client.AssertPostAsErrorAsync<ProblemDetails>("api/v1/users",
                                                                 useCase,
                                                                 useCase);
        }
    }
}
