using AspNetCore.Simple.MsTest.Sdk;
using StrategyPattern.Evolution.Api.User.V1.Create;

namespace StrategyPattern.Evolution.Test.Api.Users.V1.Create.Status_200_Ok
{
    [TestClass]
    [TestCategory("Users")]
    [TestCategory("Users V1 Create Status 200 OK")]
    public class Create_Status_200_OK_Test : ApiTestBase
    {
        [TestMethod]
        [DynamicRequestLocator]
        public Task Should_Create_User_Of_Type(string useCase)
        {
            return Client.AssertPostAsync<CreateUserResponse>("api/v1/users",
                                                              useCase,
                                                              useCase);
        }
    }
}
