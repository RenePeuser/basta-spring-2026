# V6 - NextLevel (Enterprise NuGet Package)

**Package**: [Siemens.AspNet.ErrorHandling](https://www.nuget.org/packages/Siemens.AspNet.ErrorHandling)

---

## ✅ Capabilities (Pros)

### Core Features
- **Battle-tested**: Used in production environments across 50+ APIs
- **Zero maintenance**: No custom code to maintain, updates handled by package
- **All V5 features plus enterprise additions**
- **RFC 9457 compliant**: Full ProblemDetails standard support

### Specialized Exception Handlers
Built-in handlers for common scenarios (priority-based, order matters):
1. **BadHttpRequestBindParameterException**: Query/route parameter binding errors
2. **BadHttpRequestBindBodyException**: Request body binding errors
3. **JsonReaderException**: Malformed JSON with exact position
4. **JsonSerializationException**: JSON serialization/deserialization errors
5. **ValidationDetailsException**: Field-level validation errors (Status 422)
6. **ProblemDetailsException**: RFC 9457 ProblemDetails exceptions
7. **ValidationDetailsExtendedException**: Extended validation with custom rules
8. **JsonException**: Generic JSON processing errors
9. **DefaultExceptionHandler**: Fallback for unhandled exceptions

### Security Features
- **HideErrorResponseHandler**: Automatically hides sensitive details based on status code ranges
- **Configurable visibility**: `ShowExceptionDetailsOnErrorCodeRanges` controls what to show/hide
- **Default secure**: 5xx errors hide stack traces in production by default
- **4xx errors show details**: Client errors provide debugging information

### Advanced Capabilities
- **Inner exception handling**: Automatically finds root cause with `ExceptionHelper`
- **Rich context**: `HttpCallInfos` provides method, path, body, headers, traceId
- **Priority system**: First matching handler wins (with special BadHttpRequest priority)
- **Immutable collections**: Thread-safe handler resolution
- **Distinct handlers**: Prevents duplicate handler execution
- **Centralized writing**: Single `IErrorResponseWriter` for consistent formatting
- **Safety fallback**: Multiple fallback layers ensure no exceptions slip through
- **Environment-aware**: Different behavior for dev/staging/production via configuration
- **TraceId support**: Full correlation ID support for distributed tracing
- **🔌 Fully extensible**: Add custom `ISpecificErrorHandler` for ANY exception (third-party, domain, library)
  - Handle Stripe, AWS, Azure SDK exceptions
  - Handle Entity Framework, Dapper exceptions
  - Handle FluentValidation, AutoMapper exceptions
  - Handle your own domain exceptions
  - Simply inherit from `SpecificErrorHandler<TException>` and register

### Developer Experience
- **Simple registration**: `services.AddErrorHandling(configuration)` and `app.UseErrorHandling()`
- **Convention-based**: Works out of the box with sensible defaults
- **Configuration-driven**: Change behavior without code changes
- **Well-documented**: Inline XML comments for IntelliSense

---

## ❌ Problems (Cons)

### Dependency Management
- **External dependency**: Requires NuGet package in your project
- **Package updates**: Need to test updates before deploying
- **Version conflicts**: Potential conflicts with other packages
- **Breaking changes**: Major version updates might require code changes

### Complexity
- **Learning curve**: Team needs to understand package configuration and conventions
- **Configuration required**: Not zero-config, needs `ErrorHandlerSettings` setup
- **Less control**: Can't easily modify internal handler behavior
- **Black box**: Internal implementation details are hidden

### Architectural
- **Overkill for simple APIs**: Too much for APIs that only return 200/500
- **Handler priority**: Need to understand handler registration order
- **Extension point**: Custom handlers require implementing `ISpecificErrorHandler` interface
- **Testing complexity**: Need to understand how to mock/stub package components

### Operational
- **Debugging**: Harder to debug issues inside the package
- **Configuration errors**: Misconfiguration can lead to unexpected behavior
- **Documentation dependency**: Need to refer to package documentation
- **Update cycle**: Can't immediately fix bugs, need to wait for package update

---

## 🎯 When to Use V6

**✅ Use V6 when:**
- Building enterprise/production APIs
- Need comprehensive error handling without building it yourself
- Want security-first error responses
- Have multiple APIs that need consistent error handling
- Need field-level validation, JSON diagnostics, tracing
- Want to focus on business logic, not error handling infrastructure
- Using third-party libraries (Stripe, AWS, EF Core) and want custom error handling
- Need extensibility: add handlers for specific exceptions without forking the package

**❌ Don't use V6 when:**
- Building a simple prototype or POC
- Only need basic 200/500 responses
- Want full control over error handling internals
- Can't add external dependencies
- Team doesn't want to learn package configuration

---

## 📦 Usage

### Installation
```bash
dotnet add package Siemens.AspNet.ErrorHandling
```

### Registration
```csharp
// Program.cs
builder.Services.AddErrorHandling(builder.Configuration);
app.UseErrorHandling();
```

### Configuration
```json
// appsettings.Production.json
{
  "ErrorHandlerSettings": {
    "ShowExceptionDetailsOnErrorCodeRanges": [
      { "Start": 0, "End": 499 }  // Show details only for 4xx
    ]
  }
}

// appsettings.Development.json
{
  "ErrorHandlerSettings": {
    "ShowExceptionDetailsOnErrorCodeRanges": [
      { "Start": 0, "End": 599 }  // Show all details in dev
    ]
  }
}
```

---

## 🔍 Implementation Highlights

From the actual `ErrorHandlingStrategy` implementation:

```csharp
// 1. Find ALL inner exceptions (root cause)
var exceptions = exceptionHelper.FindAllInnerExceptions(exception);

// 2. Rich context collection
var httpCallInfos = await httpContextToEndpointInfoConverter.ConvertAsync(httpContext);

// 3. Priority-based handler resolution (BadHttpRequest gets priority)
var exceptionHandlers = specificErrorHandlers
    .Where(handler => exceptions.Any(e => handler.CanHandle(e)))
    .Distinct()
    .ToImmutableList();

var badRequestHandler = exceptionHandlers
    .FirstOrDefault(handler => handler.GetExceptionType() == typeof(BadHttpRequestException));

// 4. Fallback to default if no handler found
var problemDetails = exceptionHandler?.HandleAsync(httpCallInfos, exception)
    ?? defaultExceptionHandler.HandleAsync(httpCallInfos, exception);

// 5. Security: Hide response for configured status code ranges
if (errorHandlerSettings.ShowExceptionDetailsOnErrorCodeRanges
    .Any(range => !range.IsInRange(currentStatusCode)))
{
    await hideErrorResponseHandler.HandleAsync(httpContext, exception, problemDetails);
    return;
}

// 6. Centralized writing
await errorResponseWriter.WriteResponseAsync(httpContext, problemDetails);
```

**Key Design Decisions:**
- **Immutable collections** for thread safety
- **Distinct()** prevents duplicate handler execution
- **Priority system** with BadHttpRequest special handling
- **Multiple fallbacks** ensure robustness
- **Security-first** with configurable hiding

---

## 🔌 Extensibility: Custom Exception Handlers

One of V6's most powerful features: **You can add your own exception handlers for third-party exceptions!**

### How to Add a Custom Handler

**1. Create your handler by inheriting from `SpecificErrorHandler<TException>`:**

```csharp
using Siemens.AspNet.ErrorHandling.Contracts;
using Microsoft.AspNetCore.Mvc;

// Example: Handle Stripe API exceptions
internal sealed class StripeExceptionHandler()
    : SpecificErrorHandler<Stripe.StripeException>
{
    protected override Task<ProblemDetails> HandleExceptionAsync(
        HttpCallInfos httpCallInfos,
        Stripe.StripeException exception)
    {
        // Custom logic for Stripe exceptions
        var problemDetails = new ProblemDetails
        {
            Title = "Payment processing error",
            Detail = exception.StripeError?.Message ?? exception.Message,
            Status = exception.HttpStatusCode,
            Type = "https://stripe.com/docs/error-codes",
            Instance = httpCallInfos.Path
        };

        // Add Stripe-specific metadata
        problemDetails.Extensions["stripeErrorCode"] = exception.StripeError?.Code;
        problemDetails.Extensions["stripeRequestId"] = exception.StripeError?.RequestId;

        return Task.FromResult(problemDetails);
    }
}
```

**2. Create an extension method for registration:**

```csharp
internal static class AddStripeExceptionHandlerExtension
{
    public static void AddStripeExceptionHandler(this IServiceCollection services)
    {
        // Register as ISpecificErrorHandler - that's it!
        services.AddSingletonIfNotExists<ISpecificErrorHandler, StripeExceptionHandler>();
    }
}
```

**3. Register your handler BEFORE `AddErrorHandling()`:**

```csharp
// Program.cs
builder.Services.AddStripeExceptionHandler();  // ← Your custom handler
builder.Services.AddErrorHandling(builder.Configuration);

app.UseErrorHandling();
```

**That's it!** Your handler is now part of the error handling pipeline. 🎉

### More Examples

**Example: Entity Framework DbUpdateException**

```csharp
internal sealed class DbUpdateExceptionHandler()
    : SpecificErrorHandler<DbUpdateException>
{
    protected override Task<ProblemDetails> HandleExceptionAsync(
        HttpCallInfos httpCallInfos,
        DbUpdateException exception)
    {
        // Check for unique constraint violation
        if (exception.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
        {
            return Task.FromResult<ProblemDetails>(new ProblemDetails
            {
                Title = "Duplicate entry",
                Detail = "A record with this key already exists.",
                Status = StatusCodes.Status409Conflict,
                Type = "https://example.com/errors/duplicate"
            });
        }

        // Default 500 for other DB errors
        return Task.FromResult<ProblemDetails>(new ProblemDetails
        {
            Title = "Database error",
            Detail = "An error occurred while saving to the database.",
            Status = StatusCodes.Status500InternalServerError
        });
    }
}
```

**Example: Third-party API Client Exception (e.g., Refit)**

```csharp
internal sealed class RefitExceptionHandler()
    : SpecificErrorHandler<Refit.ApiException>
{
    protected override Task<ProblemDetails> HandleExceptionAsync(
        HttpCallInfos httpCallInfos,
        Refit.ApiException exception)
    {
        return Task.FromResult<ProblemDetails>(new ProblemDetails
        {
            Title = "External API error",
            Detail = $"External service returned: {exception.StatusCode}",
            Status = exception.StatusCode == System.Net.HttpStatusCode.NotFound
                ? StatusCodes.Status404NotFound
                : StatusCodes.Status502BadGateway,
            Type = "https://example.com/errors/external-api"
        });
    }
}
```

### Advanced: Conditional Handling

You can override `CanHandle()` for more control:

```csharp
internal sealed class CustomValidationExceptionHandler()
    : SpecificErrorHandler<ValidationException>
{
    // Only handle ValidationException if it has a specific property
    protected override bool CanHandle(ValidationException exception)
    {
        return exception.Errors?.Any() == true;
    }

    protected override Task<ProblemDetails> HandleExceptionAsync(
        HttpCallInfos httpCallInfos,
        ValidationException exception)
    {
        // ... custom handling
    }
}
```

### Key Points

✅ **Inherit from `SpecificErrorHandler<TException>`**

✅ **Register as `ISpecificErrorHandler` singleton**

✅ **Register BEFORE `AddErrorHandling()`**

✅ **Order matters**: First registered handler for an exception type wins

✅ **Access to `HttpCallInfos`**: Full context (path, body, headers, traceId)

✅ **Return `ProblemDetails`**: Any ProblemDetails type (base, Validation, Extended)

✅ **Thread-safe**: All handlers are singletons, must be thread-safe


### What You Can Handle

- **Third-party exceptions**: Stripe, Twilio, SendGrid, AWS SDK, Azure SDK
- **ORM exceptions**: Entity Framework, Dapper, NHibernate
- **HTTP client exceptions**: HttpClient, Refit, RestSharp
- **Your domain exceptions**: Any custom exception from your business logic
- **Library exceptions**: FluentValidation, AutoMapper, MediatR

**The package doesn't limit you!** As long as it's an exception, you can handle it. 🚀
