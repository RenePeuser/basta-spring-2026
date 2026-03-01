# Startup Strategy Pattern

This implementation demonstrates the **Strategy Pattern at the Startup level**.
Each `StrategyType` has its own startup configuration that registers only the necessary services and middleware.

## 🎯 Concept

Instead of having a complex `Program.cs` with many conditionals, we use a **Facade + Strategy Pattern combination**:

```csharp
// This is ALL that's in Program.cs (only 3 lines!):
var webApi = new BastaStrategyWebApi(args);

webApi.Run();
```

**The ENTIRE startup logic is hidden behind a Facade that internally uses the Strategy Pattern!**

The `BastaStrategyWebApi` Facade:
- Automatically determines the strategy (default: FullBlown, or via environment/manual override)
- Internally calls the Strategy Pattern
- Configures services and pipeline
- Inspired by real-world SDK patterns

Each strategy is responsible for:
- ✅ **All service registrations** (Domain, Error Handling, Validation, etc.)
- ✅ **Complete middleware pipeline** (HTTPS, Error Handling, Routing, etc.)
- ✅ **API configuration** (Route Groups, Endpoint Mapping)

## 💡 Why Startup Strategies?

### Problem without Strategy Pattern:

```csharp
// Program.cs quickly becomes complex and cluttered
if (environment == "Basic") {
    services.AddBasicErrorHandling();
} else if (environment == "Intermediate") {
    services.AddIntermediateErrorHandling();
} else if (environment == "FullBlown") {
    services.AddErrorHandling(configuration);
}

services.AddApi(configuration);
services.AddValidation();
// ... 20+ more registrations

// And then again for the pipeline...
if (environment == "Basic") {
    app.AddBastaErrorHandlingMiddleware();
} else if (environment == "Intermediate") {
    app.AddBastaErrorHandlingMiddleware();
} else if (environment == "FullBlown") {
    app.UseErrorHandling();
}

app.UseHttpsRedirection();
// ... 10+ more middleware
```

**This quickly becomes messy!** 😱

### Solution with Facade + Strategy Pattern:

```csharp
// Program.cs is minimal - only 3 lines!
var webApi = new BastaStrategyWebApi(args);

webApi.Run();
```

**Ultra clean, testable, extensible!** ✨

The Facade hides complexity and internally uses the Strategy Pattern.

### Why is this important?

1. **Testing**: Different environments with different configurations
2. **Development**: Quickly switch between configurations
3. **Production**: Different setups for different deployment scenarios
4. **Maintenance**: New configuration? Simply add a new Strategy class!

## 📦 Available Startup Strategies

### V1_None
- **StrategyType**: `None`
- **Services**: `AddNoneErrorHandling()`
- **Pipeline**: `AddBastaErrorHandlingMiddleware()`
- **Use Case**: Educational - worst case scenario

### V2_Basic
- **StrategyType**: `Basic`
- **Services**: `AddBasicErrorHandling()`
- **Pipeline**: `AddBastaErrorHandlingMiddleware()`
- **Use Case**: Simplest implementation - 500 errors for everything

### V3_SwitchStrategy
- **StrategyType**: `Switch`
- **Services**: `AddSwitchErrorHandlingStrategy()`
- **Pipeline**: `AddBastaErrorHandlingMiddleware()`
- **Use Case**: ProblemDetails with proper HTTP status codes

### V4_SpecificExceptionHandlers
- **StrategyType**: `SpecificExceptionHandlers`
- **Services**: `AddSpecificExceptionHandlers()`
- **Pipeline**: `AddBastaErrorHandlingMiddleware()`
- **Use Case**: Extensible handler pattern with inner exception support

### V5_SpecificExceptionHandlersAndFullContext
- **StrategyType**: `SpecificHandlersAndFullContext`
- **Services**: `AddAdvancedErrorHandling()`
- **Pipeline**: `AddBastaErrorHandlingMiddleware()`
- **Use Case**: Full context + centralized response writing

### V6_NextLevel
- **StrategyType**: `NextLevel`
- **Services**: Enterprise error handling (NuGet package)
- **Pipeline**: `UseErrorHandling()` (Enterprise middleware)
- **Use Case**: Production-ready with all features (Default)

### V7_Basta
- **StrategyType**: `Basta`
- **Services**: `AddBastaAdvancedErrorHandlingStrategy()`
- **Pipeline**: `AddBastaErrorHandlingMiddleware()`
- **Use Case**: Special demo with ASCII Art response 🎉

## 🚀 Usage

### Option 1: Default (NextLevel)

```csharp
var webApi = new BastaStrategyWebApi(args);
webApi.Run();
// → Uses NextLevel Strategy (V6 - Enterprise NuGet)
```

### Option 2: Manual Override

```csharp
var webApi = new BastaStrategyWebApi(args);
webApi.StrategyType = StrategyType.Switch; // Change to any V1-V7
webApi.Run();
```

### Option 3: Custom Services (optional)

```csharp
var webApi = new BastaStrategyWebApi(args);

webApi.RegisterServices = (services, config) =>
{
    services.AddSingleton<IMyService, MyService>();
};

webApi.SetupApplication = app =>
{
    app.UseMyCustomMiddleware();
};

webApi.Run();
```

## 🎨 Architecture Flow

```
┌──────────────────┐
│  Program.cs      │  ← Only 3 lines!
│  (Facade Call)   │
└────────┬─────────┘
         │
         v
┌───────────────────────┐
│ BastaStrategyWebApi   │  ← Facade Pattern
│ (hides complexity)    │
└────────┬──────────────┘
         │
         v
┌───────────────────────┐
│ StartupStrategy       │  ← Factory Pattern
│ Factory               │
└────────┬──────────────┘
         │
         v
┌───────────────────────┐
│ IStartupStrategy      │  ← Strategy Pattern
│ - ConfigureServices   │
│ - ConfigurePipeline   │
└────────┬──────────────┘
         │
         ├─────► BasicStartupStrategy
         ├─────► IntermediateStartupStrategy
         ├─────► BastaStartupStrategy
         └─────► FullBlownStartupStrategy
```

**Three Design Patterns Combined:**
1. **Facade Pattern** - BastaStrategyWebApi hides complexity
2. **Factory Pattern** - StartupStrategyFactory creates strategies
3. **Strategy Pattern** - Interchangeable startup configurations

## 🎓 Key Benefits

1. **Single Responsibility**: Each startup strategy handles only its own configuration
2. **Open/Closed Principle**: Add new strategies without changing existing ones
3. **Clean Program.cs**: Entry point remains minimal and clear
4. **Testable**: Each strategy can be tested in isolation
5. **Environment-based**: Perfect for integration tests with different environments

## 📊 Comparison: Before vs. After

### Before (complex Program.cs)
```csharp
if (strategyType == StrategyType.Basic) {
    builder.Services.AddBasicErrorHandling();
} else if (strategyType == StrategyType.Intermediate) {
    builder.Services.AddIntermediateErrorHandling();
} else if (strategyType == StrategyType.FullBlown) {
    builder.Services.AddErrorHandling(configuration);
}
// ... and then again for the pipeline
```

### After (Clean with Facade + Strategy Pattern)
```csharp
var webApi = new BastaStrategyWebApi(args);
webApi.Run();
// Done! 🎉
```

**From many lines to just 3 lines!**

## 🔧 Extension

Adding a new strategy is simple:

1. Add new `StrategyType` to the enum
2. Create new `XyzStartupStrategy : IStartupStrategy`
3. Register in the factory
4. Done! 🚀
