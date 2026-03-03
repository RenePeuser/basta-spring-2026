namespace StrategyPattern.Evolution.V1_None
{
    public class NoErrorMiddleware() : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception)
            {
                // Ooops - an exception occurred, we are afraid to handle it.
                // Keep eyes closed send {} and hope the best -> And say The
                // User is guilty - As well it was Friday, and we await the WE
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new object());
            }
        }
    }
}
