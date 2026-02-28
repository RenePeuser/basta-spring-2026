using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.AspNetCore.Mvc;

namespace StrategyPattern.Evolution.Test.Api.Users.V1.Create.Status_403_Forbidden
{
    [TestClass]
    [TestCategory("Users")]
    [TestCategory("Users V1 Create Status 403 Forbidden")]
    public class Create_Status_403_Forbidden_Test : ApiTestBase
    {
        [TestMethod]
        [DataRow("devil=TRUNCATE YourTable", "SqlInjection_01.json")]
        [DataRow("devil=TRUNCATE YourTable&god=RESTORE YourTable", "SqlInjection_02.json")]
        public Task Should_Not_Be_Able_To_Create_A_User_If_Query_Parameters_Are_Used_Which_Are_Not_Declared(string queryParams,
                                                                                                            string response)
        {
            return Client.AssertPostAsErrorAsync<ProblemDetails>($"api/v1/users?{queryParams}",
                                                                 "AiAgent.json",
                                                                 response);
        }
    }
}
