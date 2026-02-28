using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.AspNetCore.Mvc;

namespace StrategyPattern.Evolution.Test.Api.Users.V1.Create.Status_422_UnprocessableContent
{
    /// <summary>
    /// For request bodies that are syntactically correct yet semantically invalid (e.g., in POST, PUT, PATCH),
    /// a 422 Unprocessable Content status is recommended
    /// (<see href="https://www.rfc-editor.org/rfc/rfc9110#status.422">RFC 9110</see>).
    /// </summary>
    [TestClass]
    [TestCategory("Users")]
    [TestCategory("Users V1 Create Status 422 UnprocessableContent")]
    public class Create_Status_422_UnprocessableContent_Test : ApiTestBase
    {
        [TestMethod]
        [DynamicRequestLocator]
        public Task Should_Return_422_When_Request_Contains(string useCase)
        {
            return Client.AssertPostAsErrorAsync<ValidationProblemDetails>("api/v1/users",
                                                                           useCase,
                                                                           useCase);
        }
    }
}
