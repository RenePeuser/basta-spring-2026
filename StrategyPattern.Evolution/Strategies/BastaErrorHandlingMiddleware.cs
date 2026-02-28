
namespace StrategyPattern.Evolution
{
    public enum StrategyType
    {
        None, // Default MS
        Basic,
        Intermediate,
        Advanced,
        Basta,
        FullBlown
    }


    internal static class AddBastaErrorHandlingMiddlewareExtension
    {
        internal static void AddBastaErrorHandlingMiddleware(this WebApplication webApplication)
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
