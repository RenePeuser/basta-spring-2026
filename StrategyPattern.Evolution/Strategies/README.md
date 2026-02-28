# Error Handling Strategy Evolution Demo

This folder demonstrates the evolution of error handling strategies from basic to advanced implementations.

## Strategy Versions

### V1 - Basic Error Handling
**Location**: `V1_Basic/BasicErrorHandlingStrategy.cs`

**Features**:
- Simple try-catch in middleware
- Returns HTTP 500 for all errors
- Basic JSON error response with message

**Use Case**: Quick prototypes, simple APIs

**Demo Points**:
- ✅ Easy to understand and implement
- ❌ No distinction between error types
- ❌ Security risk (exposes exception details)
- ❌ Not RFC 7807 compliant

---

### V2 - Intermediate Error Handling
**Location**: `V2_Intermediate/IntermediateErrorHandlingStrategy.cs`

**Features**:
- Exception type differentiation
- Proper HTTP status codes (400, 403, 404, 409, 500)
- RFC 7807 ProblemDetails format
- Request path in error response

**Use Case**: Production APIs with standard error handling needs

**Demo Points**:
- ✅ Industry standard format (RFC 7807)
- ✅ Correct HTTP semantics
- ✅ Better client experience
- ⚠️  Still manual exception mapping
- ❌ No validation error details
- ❌ No JSON parsing error handling

---

### V3 - Advanced Error Handling
**Location**: `V3_Advanced/AdvancedErrorHandlingStrategy.cs`

**Features**:
- Multiple specialized exception handlers
- JSON parsing and validation errors with detailed diagnostics
- Request body buffering for error context
- Security-aware responses (hides details in production)
- Validation error extensions with field-level details
- Automatic error code range handling
- Extensive metadata and tracing support

**Use Case**: Enterprise-grade APIs, complex validation scenarios

**Demo Points**:
- ✅ Comprehensive error coverage
- ✅ Developer-friendly detailed errors (dev mode)
- ✅ Security-first (production mode)
- ✅ Excellent debugging experience
- ✅ Field-level validation errors
- ⚠️  Higher complexity
- ⚠️  Requires configuration

---

## Demo Flow

1. **Start with V1**: Show how quick it is to set up, then trigger an error
2. **Problem**: All errors look the same, no proper status codes
3. **Move to V2**: Show ProblemDetails, proper status codes
4. **Problem**: What about JSON parsing errors? Validation errors with multiple fields?
5. **Move to V3**: Show the full power with detailed validation errors and field-level diagnostics

## Architecture

Die aktuelle Implementierung kombiniert **drei Design Patterns**:

### 1. Facade Pattern - BastaStrategyWebApi

Die `Program.cs` ist ultra-minimal:
```csharp
var webApi = new BastaStrategyWebApi(args);
webApi.Run();
```

Die Facade versteckt alle Komplexität und macht die Nutzung trivial.

### 2. Strategy Pattern - Error Handling Strategies

Jede Error Handling Strategy implementiert `IBastaErrorHandler`:
- `BasicErrorHandlingStrategy` (V1) - Simple 500 errors
- `IntermediateErrorHandlingStrategy` (V2) - ProblemDetails
- `BastaAdvancedErrorHandlingStrategy` (V5) - ASCII Art Demo
- Plus: Siemens Error Handling System für FullBlown

### 3. Startup Strategy Pattern

Jede `IStartupStrategy` konfiguriert:
- ✅ Alle Services (Domain, Error Handling, Validation, etc.)
- ✅ Komplette Pipeline (HTTPS, Middleware, Routing, etc.)
- ✅ Endpoint Mapping

**Zentrale Komponenten:**
1. **`IBastaErrorHandler`** - Interface für Error Handling Strategies
2. **`BastaErrorHandlingMiddleware`** - Zentrale Middleware
3. **`IStartupStrategy`** - Interface für Startup-Konfigurationen
4. **`BastaStrategyWebApi`** - Facade die alles zusammenbringt

### Vorteile

- ✅ **Nur 3 Zeilen** in Program.cs
- ✅ **Facade Pattern** versteckt Komplexität
- ✅ **Strategy Pattern** für Flexibilität
- ✅ **Factory Pattern** für Strategy-Auswahl
- ✅ **Inspiriert von Production Code** (Siemens SDK)

## How to Switch Between Strategies

### In Program.cs (empfohlen für Demo):

```csharp
var webApi = new BastaStrategyWebApi(args);
webApi.StrategyType = StrategyType.Basic; // ← Hier ändern!
webApi.Run();
```

**Verfügbare StrategyTypes:**
- `StrategyType.Basic` - V1 Error Handling
- `StrategyType.Intermediate` - V2 ProblemDetails
- `StrategyType.Basta` - V5 ASCII Art
- `StrategyType.FullBlown` - Production-ready (Default)
- `StrategyType.Advanced` - Alias für FullBlown

**Pro-Tip für die Demo**: Ändere einfach die eine Zeile in Program.cs und starte neu!

## Test Scenarios for Demo

1. **Null argument**: Throw `ArgumentNullException`
2. **Invalid operation**: Throw `InvalidOperationException`
3. **Not found**: Throw `KeyNotFoundException`
4. **JSON parsing error**: Send malformed JSON
5. **Validation error**: Send invalid user data (empty email, etc.)
