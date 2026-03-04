using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.AspNetCore.Mvc;
using StrategyPattern.Evolution.Api.User.V1.Create;

namespace StrategyPattern.Evolution.Test.Api.Users.V1.Create.BastaShowcase
{
    // Problem: First level support report a client is not able to create a
    //          User.
    //
    // Request:
    //          {
    //             "firstName": "Basta",
    //             "lastName": "2026",
    //             "email": "basta@2026.local",
    //             "userType": "Conference"
    //          }
    //
    // Response: ? Unknown ?
    [TestClass]
    [TestCategory("Basta")]
    public class Basta_Show_Case_Test : ApiTestBase
    {
        [TestMethod]
        [DynamicRequestLocator] // ITestDataSource
        public Task What_Is_The_Problem(string useCase)
        {
            return Client.AssertPostAsync<CreateUserResponse>("api/v1/users",
                                                              useCase,
                                                              useCase);
        }


        [TestMethod]
        [DynamicRequestLocator]
        public Task What_Is_The_Problem_As_Error(string useCase)
        {
            return Client.AssertPostAsErrorAsync<ProblemDetails>("api/v1/users",
                                                                 useCase,
                                                                 "Basta_User_Error.json");
        }
    }
}
