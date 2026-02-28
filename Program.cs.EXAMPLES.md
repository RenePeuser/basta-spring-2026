# Program.cs Examples for Switching Strategies

## Current Program.cs Structure

Your `Program.cs` currently looks like this (simplified):

```csharp
using Asp.Versioning;
using StrategyPattern.Evolution.Api;
using StrategyPattern.Evolution.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApi(builder.Configuration);
builder.Services.AddRegisterEndpoints();

var webApplication = builder.Build();

webApplication.UseHttpsRedirection();

var apiVersionSet = webApplication.NewApiVersionSet()
                                  .HasApiVersion(new ApiVersion(1))
                                  .ReportApiVersions()
                                  .Build();

var apiBasePath = webApplication.MapGroup("api/v{apiVersion:apiVersion}")
                             .WithApiVersionSet(apiVersionSet);

webApplication.UseHttpsRedirection();

var registerEndpoints = webApplication.Services.GetRequiredService<RegisterEndpoints>();
registerEndpoints.MapEndpoints(apiBasePath);

webApplication.Run();
```

---

## How to Configure Each Strategy

### Option 1: V1 - Basic Error Handling

**Add after line 2 (imports):**
```csharp
using StrategyPattern.Evolution.Strategies.V1_Basic;
```

**Add after line 7 (builder.Services.AddApi...):**
```csharp
// V1 - Basic Error Handling Strategy
builder.Services.AddBasicErrorHandling();
```

**Add after line 11 (before webApplication.Run()):**
```csharp
// Use V1 Basic Error Handling Middleware
webApplication.UseMiddleware<BasicErrorHandlingMiddleware>();
```

**Full V1 Example:**
```csharp
using Asp.Versioning;
using StrategyPattern.Evolution.Api;
using StrategyPattern.Evolution.Endpoints;
using StrategyPattern.Evolution.Strategies.V1_Basic;  // <-- ADD THIS

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApi(builder.Configuration);
builder.Services.AddRegisterEndpoints();

// V1 - Basic Error Handling Strategy
builder.Services.AddBasicErrorHandling();  // <-- ADD THIS

var webApplication = builder.Build();

webApplication.UseHttpsRedirection();

// Use V1 Basic Error Handling Middleware
webApplication.UseMiddleware<BasicErrorHandlingMiddleware>();  // <-- ADD THIS

var apiVersionSet = webApplication.NewApiVersionSet()
                                  .HasApiVersion(new ApiVersion(1))
                                  .ReportApiVersions()
                                  .Build();

var apiBasePath = webApplication.MapGroup("api/v{apiVersion:apiVersion}")
                             .WithApiVersionSet(apiVersionSet);

webApplication.UseHttpsRedirection();

var registerEndpoints = webApplication.Services.GetRequiredService<RegisterEndpoints>();
registerEndpoints.MapEndpoints(apiBasePath);

webApplication.Run();
```

---

### Option 2: V2 - Intermediate Error Handling

**Add after line 2 (imports):**
```csharp
using StrategyPattern.Evolution.Strategies.V2_Intermediate;
```

**Add after line 7:**
```csharp
// V2 - Intermediate Error Handling Strategy
builder.Services.AddIntermediateErrorHandling();
```

**Add after line 11:**
```csharp
// Use V2 Intermediate Error Handling Middleware
webApplication.UseMiddleware<IntermediateErrorHandlingMiddleware>();
```

**Full V2 Example:**
```csharp
using Asp.Versioning;
using StrategyPattern.Evolution.Api;
using StrategyPattern.Evolution.Endpoints;
using StrategyPattern.Evolution.Strategies.V2_Intermediate;  // <-- ADD THIS

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApi(builder.Configuration);
builder.Services.AddRegisterEndpoints();

// V2 - Intermediate Error Handling Strategy
builder.Services.AddIntermediateErrorHandling();  // <-- ADD THIS

var webApplication = builder.Build();

webApplication.UseHttpsRedirection();

// Use V2 Intermediate Error Handling Middleware
webApplication.UseMiddleware<IntermediateErrorHandlingMiddleware>();  // <-- ADD THIS

var apiVersionSet = webApplication.NewApiVersionSet()
                                  .HasApiVersion(new ApiVersion(1))
                                  .ReportApiVersions()
                                  .Build();

var apiBasePath = webApplication.MapGroup("api/v{apiVersion:apiVersion}")
                             .WithApiVersionSet(apiVersionSet);

webApplication.UseHttpsRedirection();

var registerEndpoints = webApplication.Services.GetRequiredService<RegisterEndpoints>();
registerEndpoints.MapEndpoints(apiBasePath);

webApplication.Run();
```

---

### Option 3: V3 - Advanced Error Handling (Default/Recommended for Demo)

**Add after line 2 (imports):**
```csharp
using StrategyPattern.Evolution.Strategies.V3_Advanced;
```

**Add after line 7:**
```csharp
// V3 - Advanced Error Handling Strategy
builder.Services.AddAdvancedErrorHandling(builder.Configuration);
```

**Add after line 11:**
```csharp
// Use V3 Advanced Error Handling (includes buffering + full error handling)
webApplication.UseAdvancedErrorHandling();
```

**Full V3 Example (RECOMMENDED):**
```csharp
using Asp.Versioning;
using StrategyPattern.Evolution.Api;
using StrategyPattern.Evolution.Endpoints;
using StrategyPattern.Evolution.Strategies.V3_Advanced;  // <-- ADD THIS

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApi(builder.Configuration);
builder.Services.AddRegisterEndpoints();

// V3 - Advanced Error Handling Strategy
builder.Services.AddAdvancedErrorHandling(builder.Configuration);  // <-- ADD THIS

var webApplication = builder.Build();

webApplication.UseHttpsRedirection();

// Use V3 Advanced Error Handling (includes buffering + full error handling)
webApplication.UseAdvancedErrorHandling();  // <-- ADD THIS

var apiVersionSet = webApplication.NewApiVersionSet()
                                  .HasApiVersion(new ApiVersion(1))
                                  .ReportApiVersions()
                                  .Build();

var apiBasePath = webApplication.MapGroup("api/v{apiVersion:apiVersion}")
                             .WithApiVersionSet(apiVersionSet);

webApplication.UseHttpsRedirection();

var registerEndpoints = webApplication.Services.GetRequiredService<RegisterEndpoints>();
registerEndpoints.MapEndpoints(apiBasePath);

webApplication.Run();
```

---

## 🎬 Demo Presentation Flow

### Step 1: Start with V1 (5 minutes)
1. Configure Program.cs for V1
2. Run the application
3. Test validation error → Show 500 response
4. Test duplicate email → Show 500 response
5. **Talk point**: "See? Everything is 500. Not helpful for clients!"

### Step 2: Evolve to V2 (7 minutes)
1. Stop application
2. Replace V1 code with V2 code in Program.cs
3. Run the application
4. Test validation error → Show 400 response with ProblemDetails
5. Test duplicate email → Show 409 response
6. **Talk point**: "Much better! Proper status codes, RFC 7807 format. But no field-level details..."

### Step 3: Show the Power of V3 (10 minutes)
1. Stop application
2. Replace V2 code with V3 code in Program.cs
3. Run the application
4. Test multiple validation errors → Show 422 with field-level errors
5. Test JSON parse error → Show detailed diagnostics
6. **Talk point**: "Enterprise-grade! Field-level errors, trace IDs, security-aware. Production-ready!"

### Step 4: Emphasize the Key Point (3 minutes)
1. Open CreateUserEndpoint.cs
2. Show lines 68-78 (business logic)
3. **Talk point**: "THIS CODE NEVER CHANGED! That's the Strategy Pattern. We swapped error handling implementations without touching business logic."

---

## 📌 Important Notes

- **Only use ONE strategy at a time** - don't mix them
- **Restart the application** after changing strategies
- **Clear any cached data** (the UserRepository is in-memory, so restart clears it)
- **Test port conflicts**: If port 5000 is busy, change it in launchSettings.json

---

## 🔧 Troubleshooting

### "Port already in use"
Change the port in `Properties/launchSettings.json` or run:
```bash
dotnet run --urls "http://localhost:5001"
```

### "Middleware not found"
Make sure you added the correct `using` statement at the top of Program.cs

### "Service not registered"
Make sure you called the `Add*ErrorHandling()` method before `builder.Build()`

---

**Ready to present! 🎉**
