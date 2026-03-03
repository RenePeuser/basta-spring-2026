//using Extensions.Pack;
//using Siemens.AspNet.ErrorHandling.Contracts;

//namespace StrategyPattern.Evolution.Api.User.V1.Create
//{
//    internal static class AddCreateUserRequestSampleValueProviderExtension
//    {
//        internal static void AddCreateUserRequestSampleValueProvider(this IServiceCollection services)
//        {
//            services.AddSingleton<ISpecificTypeSampleValueProvider, CreateUserRequestSampleValueProvider>();
//        }
//    }

//    public class CreateUserRequestSampleValueProvider : SpecificTypeSampleValueProviderBase<CreateUserRequest>
//    {
//        protected override IEnumerable<CreateUserRequest> GetSampleValues()
//        {
//            var requestSample = new CreateUserRequest()
//            {
//                Email = "basta2026@outlook.de",
//                FirstName = "Basta",
//                LastName = "2026",
//                UserType = UserType.AiAgent
//            };

//            yield return requestSample;
//        }
//    }
//}
