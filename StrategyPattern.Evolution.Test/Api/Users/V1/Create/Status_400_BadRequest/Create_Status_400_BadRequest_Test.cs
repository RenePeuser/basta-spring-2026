using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.AspNetCore.Mvc;

namespace StrategyPattern.Evolution.Test.Api.Users.V1.Create.Status_400_BadRequest
{
    [TestClass]
    [TestCategory("Users")]
    [TestCategory("Users V1 Create Status 400 BadRequest")]
    public class Create_Status_400_BadRequest_Test : ApiTestBase
    {
        [TestMethod]
        [DynamicRequestLocator]
        public Task Should_Return_400_When_Json_Is(string useCase)
        {
            return Client.AssertPostAsErrorAsync<ProblemDetails>("api/v1/users",
                                                                 useCase,
                                                                 useCase);
        }
    }
}
