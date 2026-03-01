# Error Handling Strategy Evolution

This folder demonstrates the evolution of error handling strategies from basic to enterprise-grade implementations.

## Strategy Versions (V1 → V7)

### V1 - None (Worst Case)
**Location**: `V1_None/NoneErrorHandlingStrategy.cs`
**StrategyType**: `StrategyType.None`

**What it does**:
- Returns empty JSON `{}`
- Always returns HTTP 400 BadRequest
- No exception details whatsoever

**Use Case**: Educational - demonstrates what NOT to do

**Characteristics**:
- ❌ No useful error information
- ❌ Wrong HTTP status code
- ❌ Terrible developer experience
- ✅ Shows the worst possible approach

---

### V2 - Basic Error Handling
**Location**: `V2_Basic/BasicErrorHandlingStrategy.cs`
**StrategyType**: `StrategyType.Basic`

**What it does**:
- Simple try-catch in middleware
- Returns HTTP 500 for all errors
- Basic JSON error response with exception message

**Use Case**: Quick prototypes where error handling is not critical

**Characteristics**:
- ✅ Easy to understand and implement
- ❌ Always returns 500 (incorrect for client errors)
- ❌ No distinction between error types
- ❌ Security risk (exposes exception details)
- ❌ Not RFC 7807 compliant

---

### V3 - Switch Pattern (ProblemDetails)
**Location**: `V3_Switch/SwitchErrorHandlingStrategy.cs`
**StrategyType**: `StrategyType.Switch`

**What it does**:
- Uses C# switch expression for exception type mapping
- Proper HTTP status codes (400, 403, 404, 409, 500)
- RFC 7807 ProblemDetails format
- Request path included in response
- Fun type link to http.cat

**Use Case**: Production APIs with standard error handling needs

**Characteristics**:
- ✅ Industry standard format (RFC 7807)
- ✅ Correct HTTP semantics
- ✅ Better client experience
- ⚠️  Still manual exception mapping in switch
- ❌ No extensibility for custom handlers
- ❌ Inner exceptions not handled

---

### V4 - Specific Exception Handlers
**Location**: `V4_SpecificExceptionHandlers/SpecificExceptionHandlersStrategy.cs`
**StrategyType**: `StrategyType.SpecificExceptionHandlers`

**What it does**:
- Introduces `ISpecificExceptionHandler` pattern
- Handles inner exceptions properly
- Each exception type gets its own handler
- Example: `BadHttpRequestExceptionHandler` with safety checks

**Use Case**: APIs that need extensible error handling

**Characteristics**:
- ✅ Extensible handler pattern
- ✅ Inner exception handling
- ✅ Safety checks in handlers
- ✅ Single responsibility per handler
- ⚠️  Still writes responses in handlers (code duplication)
- ❌ No centralized response writing

---

### V5 - Specific Handlers with Full Context
**Location**: `V5_SpecificHandlersAndFullContext/NextLevelErrorHandlingStrategy.cs`
**StrategyType**: `StrategyType.SpecificHandlersAndFullContext`

**What it does**:
- Introduces `HttpCallInfos` for rich context
- Centralized `IErrorResponseWriter`
- Handlers return `ProblemDetails`, don't write responses
- Default fallback handler for unhandled exceptions
- Separation: handlers provide info, writer writes response

**Use Case**: Complex APIs with rich error context needs

**Characteristics**:
- ✅ No code duplication (centralized writing)
- ✅ Rich context (HttpCallInfos)
- ✅ Fallback safety with DefaultExceptionHandler
- ✅ Clean separation of concerns
- ✅ Generic base class `SpecificErrorHandler<TException>`
- ⚠️  Still custom implementation

---

### V6 - NextLevel (NuGet Package)
**Location**: `V6_NextLevel/` (no code files)
**StrategyType**: `StrategyType.NextLevel`
**Package**: [Siemens.AspNet.ErrorHandling](https://www.nuget.org/packages/Siemens.AspNet.ErrorHandling)

**What it does**:
- 100% uses the enterprise NuGet package
- No custom implementation needed
- Production-ready error handling system
- Complete with security, validation, tracing

**Use Case**: Production-grade enterprise APIs

**Characteristics**:
- ✅ Battle-tested in production
- ✅ All features from V5 plus more
- ✅ Maintained and updated
- ✅ No custom code to maintain
- ✅ Security-aware (hides 5xx details in production)
- ✅ Field-level validation errors
- ✅ JSON parsing diagnostics

---

### V7 - Basta (ASCII Art Demo)
**Location**: `V7_Basta/BastaErrorHandlingStrategy.cs`
**StrategyType**: `StrategyType.Basta`

**What it does**:
- Returns beautiful ASCII art
- Shows BASTA! branding
- Demonstrates custom creative responses
- Just for fun and demo purposes! 🎉

**Use Case**: Conference demonstrations and fun

**Characteristics**:
- ✅ Eye-catching ASCII art
- ✅ Shows flexibility of strategy pattern
- ✅ Great for live demos
- ❌ Not for production use! 😄

---

## Evolution Flow Summary

```
V1: None          → Empty JSON, always 400 (worst case)
                    ↓ Problem: No error information at all

V2: Basic         → 500 + exception message
                    ↓ Problem: Always 500, security risk

V3: Switch        → ProblemDetails + proper status codes
                    ↓ Problem: Manual mapping, inner exceptions

V4: Handlers      → Extensible handler pattern + inner exceptions
                    ↓ Problem: Response writing in handlers

V5: Full Context  → HttpCallInfos + centralized writing
                    ↓ Problem: Custom implementation to maintain

V6: NextLevel     → Enterprise NuGet package (production-ready)
                    ↓ Special: Demo version

V7: Basta         → ASCII Art for conference fun! 🎉
```

## Architecture

The current implementation combines **three Design Patterns**:

### 1. Facade Pattern - BastaStrategyWebApi

The `Program.cs` is ultra-minimal:
```csharp
var webApi = new BastaStrategyWebApi(args);
webApi.Run();
```

The Facade hides all complexity and makes usage trivial.

### 2. Strategy Pattern - Error Handling Strategies

Each error handling strategy implements `IBastaErrorHandler`:
- `BasicErrorHandlingStrategy` (V1) - Simple 500 errors
- `IntermediateErrorHandlingStrategy` (V2) - ProblemDetails
- `BastaAdvancedErrorHandlingStrategy` (V5) - ASCII Art demo
- Plus: Enterprise error handling system for FullBlown

### 3. Startup Strategy Pattern

Each `IStartupStrategy` configures:
- ✅ All Services (Domain, Error Handling, Validation, etc.)
- ✅ Complete Pipeline (HTTPS, Middleware, Routing, etc.)
- ✅ Endpoint Mapping

**Core Components:**
1. **`IBastaErrorHandler`** - Interface for error handling strategies
2. **`BastaErrorHandlingMiddleware`** - Central middleware
3. **`IStartupStrategy`** - Interface for startup configurations
4. **`BastaStrategyWebApi`** - Facade that brings it all together

### Benefits

- ✅ **Only 3 lines** in Program.cs
- ✅ **Facade Pattern** hides complexity
- ✅ **Strategy Pattern** for flexibility
- ✅ **Factory Pattern** for strategy selection
- ✅ **Inspired by real-world production code**

## How to Switch Between Strategies

### In Program.cs:

```csharp
var webApi = new BastaStrategyWebApi(args);
webApi.StrategyType = StrategyType.Switch; // ← Change this!
webApi.Run();
```

**Available StrategyTypes:**
- `StrategyType.None` - V1 None (worst case)
- `StrategyType.Basic` - V2 Basic (always 500)
- `StrategyType.Switch` - V3 Switch (ProblemDetails)
- `StrategyType.SpecificExceptionHandlers` - V4 Handlers
- `StrategyType.SpecificHandlersAndFullContext` - V5 Full Context
- `StrategyType.NextLevel` - V6 Enterprise NuGet (Default)
- `StrategyType.Basta` - V7 ASCII Art 🎉

## Test Scenarios

1. **Null argument**: Throws `ArgumentNullException`
2. **Invalid operation**: Throws `InvalidOperationException`
3. **Not found**: Throws `KeyNotFoundException`
4. **JSON parsing error**: Send malformed JSON
5. **Validation error**: Send invalid user data (empty email, etc.)
