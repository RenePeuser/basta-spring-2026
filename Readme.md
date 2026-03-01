# Strategy Pattern Evolution - Error Handling Demo

## 📖 Overview
This project demonstrates the evolution of error handling strategies in ASP.NET Core, from basic to enterprise-grade implementations, combined with a **Facade Pattern** for ultra-clean startup code.

## 🎯 What You'll Learn
- **Design Pattern Combinations**: Facade + Strategy + Factory Pattern
- **Evolution**: From 40+ lines of Program.cs to just **3 lines**
- **Error Handling Evolution**: Basic → ProblemDetails → Enterprise-grade
- **RFC 9457 ProblemDetails** standard (successor to RFC 7807)
- **Field-level validation** error responses
- **Production-ready patterns** inspired by real-world SDKs
- **Security considerations** in error responses

## 📜 Standards: RFC 7807 → RFC 9457

This project follows **RFC 9457** (Problem Details for HTTP APIs), which supersedes RFC 7807.

**What changed from RFC 7807 to RFC 9457?**
- Updated reference from RFC 7231 to RFC 9110 (HTTP Semantics)
- Clarified that the `instance` field is **optional** (not required)
- Better guidance on extension members
- Improved security considerations
- Same core format and principles

**Both versions define the same standard structure:**
```json
{
  "type": "https://example.com/problems/validation-error",
  "title": "Validation Error",
  "status": 400,
  "detail": "The request contains invalid data",
  "instance": "/api/v1/users"  // ← Optional in RFC 9457
}
```

This project uses RFC 9457 as the current standard while acknowledging that many APIs still reference RFC 7807. Both are compatible.

## ✨ The Ultra-Clean Program.cs

```csharp
var webApi = new BastaStrategyWebApi(args);

webApi.Run();
```

**That's it! Only 2 lines!** 🎉

Behind the scenes:
- 🎭 **Facade Pattern** - Hides complexity
- 🔀 **Strategy Pattern** - Flexible error handling
- 🏭 **Factory Pattern** - Strategy selection

## 🏗️ Project Structure

```
StrategyPattern.Evolution/
├── Api/
│   └── User/
│       └── V1/
│           └── Create/
│               ├── Endpoints/
│               │   └── CreateUserEndpoint.cs      # Simple User API
│               ├── UserDemoData.cs                # In-memory repository
│               └── ValidationException.cs         # Custom validation
│
├── Strategies/
│   ├── V1_None/
│   │   └── NoneErrorHandlingStrategy.cs           # Worst case (educational)
│   ├── V2_Basic/
│   │   └── BasicErrorHandlingStrategy.cs          # Basic try-catch (always 500)
│   ├── V3_Switch/
│   │   └── SwitchErrorHandlingStrategy.cs         # Switch + ProblemDetails
│   ├── V4_SpecificExceptionHandlers/
│   │   └── SpecificExceptionHandlersStrategy.cs   # Extensible handlers
│   ├── V5_SpecificHandlersAndFullContext/
│   │   └── NextLevelErrorHandlingStrategy.cs      # Full context + centralized
│   ├── V6_NextLevel/
│   │   └── (Enterprise NuGet Package)             # Production-ready
│   └── V7_Basta/
│       └── BastaErrorHandlingStrategy.cs          # ASCII Art demo
│
├── ErrorResponseHandling/                         # Full error handling system
│   ├── Handlers/                                  # Specialized exception handlers
│   ├── ResponseWriters/                           # Response formatting
│   └── Settings/                                  # Configuration
│
└── Endpoints/
    └── RegisterEndpoints.cs                       # Endpoint registration
```

## 🚀 Quick Start

### Prerequisites
- .NET 9 SDK
- Visual Studio 2022 / VS Code / Rider
- REST Client (Postman, curl, or VS Code REST Client extension)

### Run the Demo

1. **Clone and Build**
```bash
cd D:\GitHub\basta-spring-2026
dotnet restore
dotnet build
```

2. **Run the Application**
```bash
cd StrategyPattern.Evolution
dotnet run
```

3. **Test with REST Client**
   - Open `demo-requests.http` in VS Code (with REST Client extension)
   - Or use Postman/curl with the provided requests

## 📊 Strategy Comparison

| Feature | V1 None | V2 Basic | V3 Switch | V4 Handlers | V5 Context | V6 NextLevel | V7 Basta |
|---------|---------|----------|-----------|-------------|------------|--------------|----------|
| HTTP Status Codes | ❌ Wrong | ❌ Always 500 | ✅ Proper | ✅ Proper | ✅ Proper | ✅ Proper | 🎨 Art |
| RFC 9457 Format | ❌ | ❌ | ✅ | ✅ | ✅ | ✅ | ❌ Demo |
| Inner Exceptions | ❌ | ❌ | ❌ | ✅ | ✅ | ✅ | ❌ |
| Extensible Handlers | ❌ | ❌ | ❌ | ✅ | ✅ | ✅ | ❌ |
| Centralized Writing | ❌ | ❌ | ❌ | ❌ | ✅ | ✅ | ❌ |
| Field-level Validation | ❌ | ❌ | ❌ | ❌ | ⚠️ | ✅ | ❌ |
| Security (Hide 5xx) | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ | ❌ |
| Production-Ready | ❌ | ❌ | ⚠️ | ⚠️ | ⚠️ | ✅ | ❌ |
| ASCII Art Response | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ 🎉 |
| **Lines in Program.cs** | **3** | **3** | **3** | **3** | **3** | **3** | **3** |

**Key Insight**: Same clean Program.cs - completely different behavior! That's the power of the Strategy Pattern.

## 🧪 Test Scenarios

### Success Case
```bash
curl -X POST http://localhost:5000/api/v1/users \
  -H "Content-Type: application/json" \
  -d '{"firstName":"Jane","lastName":"Doe","email":"jane@example.com","userType":0}'
```

### Validation Error (Multiple Fields)
```bash
curl -X POST http://localhost:5000/api/v1/users \
  -H "Content-Type: application/json" \
  -d '{"firstName":"J","lastName":"","email":"bad-email","userType":0}'
```

### Duplicate Conflict (Run Twice)
```bash
curl -X POST http://localhost:5000/api/v1/users \
  -H "Content-Type: application/json" \
  -d '{"firstName":"John","lastName":"Doe","email":"duplicate@example.com","userType":0}'
```

## 🔧 Switching Between Strategies

In `Program.cs`, just change one line:

```csharp
var webApi = new BastaStrategyWebApi(args);
webApi.StrategyType = StrategyType.Switch; // ← Change this!
webApi.Run();
```

**Available Strategies:**
- `StrategyType.None` - V1: Worst case (empty JSON, always 400)
- `StrategyType.Basic` - V2: Simple 500 errors
- `StrategyType.Switch` - V3: ProblemDetails with status codes
- `StrategyType.SpecificExceptionHandlers` - V4: Extensible handler pattern
- `StrategyType.SpecificHandlersAndFullContext` - V5: Full context + centralized writing
- `StrategyType.NextLevel` - V6: Enterprise NuGet package (Default)
- `StrategyType.Basta` - V7: ASCII Art Demo (fun!) 🎉

## 📝 Configuration

### Production Settings (`appsettings.json`)
```json
{
  "ErrorHandlerSettings": {
    "ShowExceptionDetailsOnErrorCodeRanges": [
      {
        "Start": 0,
        "End": 499
      }
    ]
  }
}
```
**Effect**: Hides details for 5xx errors (security)

### Development Settings (`appsettings.Development.json`)
```json
{
  "ErrorHandlerSettings": {
    "ShowExceptionDetailsOnErrorCodeRanges": [
      {
        "Start": 0,
        "End": 599
      }
    ]
  }
}
```
**Effect**: Shows all error details (debugging)

## 🌟 Key Takeaways

1. **Design Patterns in Combination**: Facade + Strategy + Factory = Clean Code
2. **From 40+ Lines to 3 Lines**: Strategic refactoring reduces complexity
3. **Start Simple, Evolve**: V1 → V2 → FullBlown
4. **Strategy Pattern**: Decouple error handling from business logic
5. **Facade Pattern**: Hide complexity behind a simple API
6. **Production-Ready Patterns**: Inspired by real-world SDK (Siemens)
7. **Security First**: Hide 5xx details in production (FullBlown strategy)
8. **Developer Experience**: Clean startup code = maintainable codebase

## 📚 Resources

- [RFC 9457 - Problem Details for HTTP APIs](https://www.rfc-editor.org/rfc/rfc9457.html) (successor to RFC 7807)
- [Strategy Pattern - Gang of Four](https://refactoring.guru/design-patterns/strategy)
- [ASP.NET Core Error Handling](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
- [Microsoft.AspNetCore.Mvc.ProblemDetails](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.problemdetails)

## 🤝 Contributing

Feel free to:
- Fork and adapt this project
- Submit issues or improvements
- Use it for educational purposes

## 📜 License

This project is intended for educational purposes.

## 👨‍💻 Authors

- philip.pregler@siemens.com
- rene.peuser@hotmail.de

---

**Happy Coding! 🚀**
