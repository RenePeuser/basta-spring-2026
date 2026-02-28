using Siemens.AspNet.ErrorHandling.Contracts;
using Siemens.AspNet.MinimalApi.Sdk.Contracts;

namespace StrategyPattern.Evolution.Api.User.V1.Create
{
    public record CreateUserRequest
    {
        [IsNotNull]
        [IsNotWhitespace]
        [NoLeadingWhitespaces]
        [NoTrailingWhitespaces]
        [SampleValue("Basta")]
        public required string FirstName { get; init; }

        [IsNotNull]
        [IsNotWhitespace]
        [NoLeadingWhitespaces]
        [NoTrailingWhitespaces]
        [SampleValue("2026")]
        public required string LastName { get; init; }

        [Email]
        [IsNotNull]
        [IsNotWhitespace]
        [NoLeadingWhitespaces]
        [NoTrailingWhitespaces]
        [SampleValue("basta2026@outlook.de")]
        public required string Email { get; init; }

        [IsNotNull]
        [EnumIsDefined]
        public required UserType UserType { get; init; }
    }
}
