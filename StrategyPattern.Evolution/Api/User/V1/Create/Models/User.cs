namespace StrategyPattern.Evolution.Api.User.V1.Create
{
    public enum UserType
    {
        Developer,
        Admin,
        AiAgent
    }

    public record User(Guid Id,
                       string FirstName,
                       string LastName,
                       string Email,
                       UserType UserType);
}
