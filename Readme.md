# Strategy Pattern Evolution - Error Handling Demo

## 📖 Overview
This project demonstrates the evolution of error handling strategies in ASP.NET Core, from basic to enterprise-grade implementations, combined with a **Facade Pattern** for ultra-clean startup code. Perfect for conference talks and workshops about **combining Design Patterns** for clean architecture.

## 🎯 What You'll Learn
- **Design Pattern Combinations**: Facade + Strategy + Factory Pattern
- **Evolution**: From 40+ lines of Program.cs to just **3 lines**
- **Error Handling Evolution**: Basic → ProblemDetails → Enterprise-grade
- **RFC 7807 ProblemDetails** standard
- **Field-level validation** error responses
- **Production-ready patterns** inspired by our Siemens SDK
- **Security considerations** in error responses

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
│   ├── V1_Basic/
│   │   └── BasicErrorHandlingStrategy.cs          # Basic try-catch
│   ├── V2_Intermediate/
│   │   └── IntermediateErrorHandlingStrategy.cs   # ProblemDetails + Status Codes
│   └── V3_Advanced/
│       └── AdvancedErrorHandlingStrategy.cs       # Full-featured enterprise
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

| Feature | V1 Basic | V2 Intermediate | V5 Basta | FullBlown |
|---------|----------|-----------------|----------|-----------|
| HTTP Status Codes | ❌ Always 500 | ✅ Proper | 🎨 Art | ✅ Proper |
| RFC 7807 Format | ❌ | ✅ | ❌ Demo | ✅ |
| Field-level Validation | ❌ | ❌ | ❌ | ✅ |
| JSON Parse Diagnostics | ❌ | ❌ | ❌ | ✅ |
| Trace IDs | ❌ | ❌ | ❌ | ✅ |
| Security (Hide 5xx) | ❌ | ❌ | ❌ | ✅ |
| Extensible Handlers | ❌ | ❌ | ❌ | ✅ |
| ASCII Art Response | ❌ | ❌ | ✅ 🎉 | ❌ |
| **Lines in Program.cs** | **3** | **3** | **3** | **3** |

**Key Insight**: Same clean Program.cs - different behavior! That's the power of the Strategy Pattern.

## 🎓 Using This for Your Conference Talk

1. **Read the Demo Guide**: See `DEMO_GUIDE.md` for a complete presentation script
2. **Follow the Evolution**: Start with V1, show problems, evolve to V2, then V3
3. **Use Provided Requests**: The `demo-requests.http` file has all scenarios ready
4. **Emphasize the Pattern**: Show how business logic never changed - only the strategy

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
webApi.StrategyType = StrategyType.Basic; // ← Change this!
webApi.Run();
```

**Available Strategies:**
- `StrategyType.Basic` - V1: Simple 500 errors
- `StrategyType.Intermediate` - V2: ProblemDetails with status codes
- `StrategyType.Basta` - V5: ASCII Art Demo (fun!)
- `StrategyType.FullBlown` - Production-ready (Default)

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

- [RFC 7807 - Problem Details for HTTP APIs](https://tools.ietf.org/html/rfc7807)
- [Strategy Pattern - Gang of Four](https://refactoring.guru/design-patterns/strategy)
- [ASP.NET Core Error Handling](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
- [Microsoft.AspNetCore.Mvc.ProblemDetails](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.problemdetails)

## 🤝 Contributing

This is a conference demo project. Feel free to:
- Fork and adapt for your own talks
- Submit issues or improvements
- Share your conference experiences

## 📜 License

This project is intended for educational purposes. Use it freely for your conference talks and workshops.

## 🎤 Presented At

- BASTA! Spring 2026 (.NET Developer Conference)

## 👨‍💻 Authors

- philip.pregler@siemens.com
- rene.peuser@hotmail.de

---

**Happy Coding! 🚀**
