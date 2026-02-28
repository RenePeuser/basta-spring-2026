using AspNetCore.Simple.MsTest.Sdk;

namespace StrategyPattern.Evolution.Test.Api.Users.V1.Create.Status_401_Unauthorized
{
    [Ignore("We do not have IDP active for demo !")]
    [TestClass]
    [TestCategory("Users")]
    [TestCategory("Users V1 Create Status 401 Unauthorized")]
    public class Create_Status_401_Unauthorized_Test : ApiTestBase
    {
        [TestMethod]
        public Task Should_Not_Be_Able_To_Call_If_Caller_Is_Not_Authorized()
        {
            return Client.AssertPostAsUnauthorizedAsync("api/v1/users");
        }
    }
}
