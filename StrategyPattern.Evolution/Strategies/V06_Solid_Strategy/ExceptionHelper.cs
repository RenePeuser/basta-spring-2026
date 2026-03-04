namespace StrategyPattern.Evolution.V06_Solid_Strategy
{
    public class ExceptionHelper
    {
        public IEnumerable<Exception> FindAllInnerExceptions(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            if (exception.InnerException != null)
            {
                var innerExceptions = FindAllInnerExceptions(exception.InnerException);

                foreach (var innerException in innerExceptions)
                {
                    yield return innerException;
                }
            }

            yield return exception;
        }
    }
}
