using System;
using System.Net;
using System.Text.Json;
using Extensions.Pack;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;
using Siemens.AspNet.ErrorHandling;
using Siemens.AspNet.ErrorHandling.Contracts;

namespace StrategyPattern.Evolution
{
    public static class AddSpecificExceptionHandlerWithContextStrategyExtensions
    {
        /// <summary>
        /// Registers the Next Level error handling strategy (V5).
        /// Introduces centralized response writing and rich context (HttpCallInfos).
        /// </summary>
        public static void AddSpecificExceptionHandlerWithContextStrategy(this IServiceCollection services)
        {
            services.AddDefaultExceptionHandler();
            services.AddJsonExceptionHandler();

            services.AddSingleton<IBastaErrorHandler, SpecificExceptionHandlerWithContextStrategy>();
        }
    }

    /// <summary>
    /// V5 - Specific Handlers with Full Context
    ///
    /// Separation of concerns: handlers provide ProblemDetails, writer writes the response.
    ///
    /// ✅ Capabilities:
    /// - No code duplication: centralized response writing via IErrorResponseWriter
    /// - Rich context via HttpCallInfos (method, path, body, headers, traceId)
    /// - Fallback safety: DefaultExceptionHandler catches unhandled exceptions
    /// - Clean separation of concerns: handlers create, writer writes
    /// - Generic base class SpecificErrorHandler<TException> for easy handler creation
    /// - Single point of control for response format
    /// - Easy to add logging, metrics, or custom headers in ONE place
    /// - Consistent response format guaranteed across all handlers
    ///
    /// ❌ Problems:
    /// - No built-in field-level validation error support
    /// - No JSON parsing error diagnostics
    /// - Missing advanced features: error code ranges, security filtering
    /// - No configuration-based behavior changes
    /// </summary>
    internal class SpecificExceptionHandlerWithContextStrategy(ExceptionHelper exceptionHelper,
                                                               IErrorResponseWriter errorResponseWriter,
                                                               DefaultExceptionHandler defaultExceptionHandler,
                                                               IHttpContextToEndpointInfoConverter httpContextToEndpointInfoConverter,
                                                               IEnumerable<INextLevelExceptionHandler> specificExceptionHandlers) : IBastaErrorHandler
    {
        public async Task HandleAsync(HttpContext httpContext,
                                      Exception exception)
        {
            // Get most suitable exceptions
            var exceptionToHandle = exceptionHelper.FindAllInnerExceptions(exception).First();

            // We collect all infos already formatted to avoid "n" data preparation and to avoid
            // lots of code in the base classes
            var httpCallInfos = await httpContextToEndpointInfoConverter.ConvertAsync(httpContext).ConfigureAwait(false);

            // First is just for demo purposes - we will see in the next strategy
            // In real world use Where with full blown infos which strategy exist, why, and what !
            var matchingStrategy = specificExceptionHandlers.First(handler => handler.CanHandle(exceptionToHandle));

            // Getting all the error infos from the specific error handles
            var problemDetails = await matchingStrategy.HandleAsync(httpCallInfos, exceptionToHandle);

            // Safety fallback -> if anything goes wrong in the specific error handlers or if they are not able
            if (problemDetails.IsNull())
            {
                problemDetails = await defaultExceptionHandler.HandleAsync(httpCallInfos, exceptionToHandle).ConfigureAwait(false);
            }

            // Not the error handlers are writing ! they only provide best possible error infos -
            // we are writing the response here - to avoid code duplication and to have a single
            // point of writing the response
            await errorResponseWriter.WriteResponseAsync(httpContext, problemDetails).ConfigureAwait(false);
        }
    }

    internal interface INextLevelExceptionHandler
    {
        bool CanHandle(Exception exception);

        Task<ProblemDetails?> HandleAsync(HttpCallInfos httpCallInfos,
                                          Exception exception);
    }

    internal static class AddJsonExceptionHandlerExtension
    {
        internal static void AddJsonExceptionHandler(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<INextLevelExceptionHandler, JsonExceptionHandler>();
        }
    }

    internal class JsonExceptionHandler : INextLevelExceptionHandler
    {
        public bool CanHandle(Exception exception)
        {
            var canHandle = exception.GetType() == typeof(JsonException); // System.Text.Json.JsonException
            return canHandle;
        }

        public Task<ProblemDetails?> HandleAsync(HttpCallInfos httpCallInfos, Exception exception)
        {
            // Safety first !! Do not trust your caller - or your own code ;)
            if (CanHandle(exception).IsFalse())
            {
                // Critical throw an error during error exception handling !
                throw new InternalServerErrorDetailsException("Your specific exception handler is not able to handle",
                                                              $"Your specific exceptions handler: {nameof(JsonExceptionHandler)} ca not handle: {exception.GetType()}");
            }

            var status400BadRequest = StatusCodes.Status400BadRequest;

            var problemDetails = new ProblemDetails
            {
                Status = status400BadRequest, // Json exeption > Created by client
                Title = "Your json is not OK",
                Detail = exception.Message,
                Instance = httpCallInfos.Url.AbsolutePath,
                Type = $"https://http.cat/status/{status400BadRequest}"
            };

            return Task.FromResult<ProblemDetails?>(problemDetails);
        }
    }


    public abstract class SpecificErrorHandler<TException> : INextLevelExceptionHandler where TException : Exception
    {
        public Type GetExceptionType()
        {
            return typeof(TException);
        }

        public bool CanHandle(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            var baseCheck = typeof(TException).IsAssignableFrom(exception.GetType());
            if (!baseCheck)
            {
                return baseCheck;
            }

            var specificCheck = CanHandle((TException)exception);
            return specificCheck;
        }

        protected virtual bool CanHandle(TException exception)
        {
            return true;
        }

        public async Task<ProblemDetails?> HandleAsync(HttpCallInfos httpCallInfos,
                                                       Exception exception)
        {
            // Safety first
            if (CanHandle(exception))
            {
                return await HandleExceptionAsync(httpCallInfos, (TException)exception).ConfigureAwait(false);
            }

            // We cant/should not throw exceptions again so we return null
            return null;
        }

        protected abstract Task<ProblemDetails> HandleExceptionAsync(HttpCallInfos httpCallInfos,
                                                                     TException exception);
    }


    internal static class AddDefaultExceptionHandlerExtension
    {
        internal static void AddDefaultExceptionHandler(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<DefaultExceptionHandler>();
        }
    }

    internal sealed class DefaultExceptionHandler()
    {
        internal Task<ProblemDetails> HandleAsync(HttpCallInfos httpCallInfos,
                                                  Exception exception)
        {
            // Just a DEMO keep it simple - in real world we would have much more complex logic
            // here to get the best possible error infos
            var problemDetails = new ProblemDetails
            {
                Title = exception.Message,
                Status = StatusCodes.Status500InternalServerError, // This means something went horrible wrong !!
                Type = nameof(ProblemDetails),
                Detail = "Something went unexpected wrong -> Please check you logs"
            };

            return Task.FromResult(problemDetails);
        }
    }
}
