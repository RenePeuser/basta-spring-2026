
using Extensions.Pack;

namespace StrategyPattern.Evolution
{
    public enum StrategyType
    {
        V1_None,
        V2_Basic,
        V3_Switch,
        V4_SpecificExceptionHandlers,
        V5_SpecificHandlersAndFullContext,
        V6_NextLevel,
        V7_Basta
    }


    internal static class AddBastaErrorHandlingMiddlewareExtension
    {
        internal static void AddBastaErrorHandlingMiddleware(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<BastaErrorHandlingMiddleware>();
        }

        internal static void UseBastaErrorHandlingMiddleware(this WebApplication webApplication)
        {
            webApplication.UseMiddleware<BastaErrorHandlingMiddleware>();
        }
    }

    public interface IBastaErrorHandler
    {
        Task HandleAsync(HttpContext httpContext, Exception exception);
    }

    public class BastaErrorHandlingMiddleware(IBastaErrorHandler errorHandleStrategy) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                await errorHandleStrategy.HandleAsync(context, exception);
            }
        }
    }
}
