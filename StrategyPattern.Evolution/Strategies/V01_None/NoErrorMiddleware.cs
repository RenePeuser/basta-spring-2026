namespace StrategyPattern.Evolution.V01_None
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
                // ToDo: It is Friday 6pm we will fix it on monday morning - Weekend !
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new object());
            }
        }
    }
}
