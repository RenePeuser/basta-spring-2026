# Strategy Pattern Evolution - Multiple Real-World Demos

## 📖 Overview
This project demonstrates the **Strategy Pattern** in ASP.NET Core through multiple real-world scenarios:
1. **Error Handling Strategies** - Evolution from basic to enterprise-grade implementations
2. **Canvas Visualization Strategies** - Converting graph data to Mermaid/Graphviz diagrams
3. **Endpoint Registration Strategies** - Flexible endpoint mapping patterns

All combined with a **Facade Pattern** for ultra-clean startup code.

## 🎯 What You'll Learn
- **Design Pattern Combinations**: Facade + Strategy + Factory Pattern
- **Strategy Pattern in Action**: Multiple concrete implementations with runtime selection
- **Evolution**: From 40+ lines of Program.cs to just **3 lines**
- **Error Handling Evolution**: Basic → ProblemDetails → Enterprise-grade
- **RFC 9457 ProblemDetails** standard (successor to RFC 7807)
- **Field-level validation** error responses
- **Graph Visualization**: JSON Canvas → Mermaid/Graphviz conversion strategies
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
- 🔀 **Strategy Pattern** - Flexible behavior selection
- 🏭 **Factory Pattern** - Strategy creation & selection

## 🎯 Three Strategy Pattern Applications

This project showcases the Strategy Pattern in **three different contexts**:

### 1️⃣ Error Handling Strategies
**7 different strategies** for handling HTTP errors:
- V1: None (worst case)
- V2: Basic (always 500)
- V3: Switch (proper status codes)
- V4: Switch with Context
- V5: Specific Exception Handlers
- V6: SOLID Strategy (production-ready)
- V7: Basta (ASCII art demo)

**Key Benefit**: Change error handling behavior without touching business logic

### 2️⃣ Canvas Visualization Strategies
Convert graph data (JSON) to different diagram formats:
- **Mermaid Strategy** → GitHub-friendly markdown diagrams
- **Graphviz Strategy** → Professional DOT format
- **Extensible** → Add PlantUML, D2, or custom formats

**Key Benefit**: One data model, multiple visualization outputs

### 3️⃣ Endpoint Registration Strategies
Flexible endpoint mapping with `IEndpointRegistration`:
- Each endpoint implements its own mapping strategy
- Automatic DI registration
- Isolated & testable

**Key Benefit**: Scalable endpoint architecture

## 🏗️ Project Structure

```
StrategyPattern.Evolution/
├── Api/
│   └── User/
│       └── V1/
│           └── Create/
│               ├── Endpoints/
│               │   └── CreateUserEndpoint.cs      # Simple User API
│               ├── Models/
│               │   └── User.cs                    # User domain model
│               ├── Requests/
│               │   └── CreateUserRequest.cs       # API request model
│               └── Responses/
│                   └── CreateUserResponse.cs      # API response model
│
├── Strategies/
│   ├── Endpoints/
│   │   └── RegisterEndpoints.cs                   # 🎯 Strategy: Endpoint registration
│   │
│   ├── Sample_Graph/
│   │   ├── Canvas_01_*.json                       # Sample Canvas graph data
│   │   └── Canvas_01_*.md                         # Rendered Mermaid output
│   │
│   ├── V1_None/
│   │   └── NoErrorMiddleware.cs                   # Worst case (educational)
│   ├── V2_Basic/
│   │   └── BasicErrorMiddleware.cs                # Basic try-catch (always 500)
│   ├── V3_Switch/
│   │   └── SwitchErrorMiddleware.cs               # Switch + ProblemDetails
│   ├── V4_SwitchContext/
│   │   └── SwitchContextErrorMiddleware.cs        # Context-aware switching
│   ├── V5_SpecificStrategyErrorHandling/
│   │   └── SpecificStrategyErrorHandling.cs       # 🎯 Extensible handlers
│   ├── V6_Solid_Strategy/
│   │   ├── ErrorHandlingStrategy.cs               # 🎯 SOLID principles
│   │   ├── ErrorResponseWriter.cs                 # Response formatting
│   │   └── Exceptions/                            # Specialized handlers
│   └── V7_Basta/
│       └── BastaErrorHandlingStrategy.cs          # ASCII Art demo
│
└── Startup/
    ├── BastaStrategyWebApi.cs                     # 🎭 Facade Pattern
    ├── IStartupStrategy.cs                        # Strategy interface
    ├── StartupStrategyFactory.cs                  # 🏭 Factory Pattern
    └── Specific/
        └── V*_*.cs                                # Concrete startup strategies
```

**Legend:**
- 🎯 = Strategy Pattern implementation
- 🎭 = Facade Pattern
- 🏭 = Factory Pattern

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

## 🎨 Strategy Pattern Showcase: Canvas Visualization

The **Canvas Visualization** demonstrates the Strategy Pattern for converting graph data structures to different output formats:

### Real-World Scenario
Transform complex **Canvas JSON** (nodes + edges) into visual diagrams using different rendering strategies:
- **Mermaid Strategy** - GitHub-friendly markdown diagrams
- **Graphviz Strategy** - Professional graph visualization (DOT format)
- **Future Strategy** - PlantUML, D2, or custom formats

### Sample Data Location
```
Strategies/Sample_Graph/
├── Canvas_01_Delete_google-search-agent.json  # Complex AWS architecture graph
└── Canvas_01_Delete_google-search-agent.md    # Rendered Mermaid output
```

### Strategy Benefits
✅ **Runtime Selection** - Choose output format at runtime
✅ **Easy Extension** - Add new formats without changing existing code
✅ **Testability** - Each strategy can be tested independently
✅ **Separation of Concerns** - Graph logic separated from rendering

### Visual Example
The sample shows an AWS microservices architecture with:
- **21 nodes** (Fargate, S3, Aurora, Bedrock, etc.)
- **15 edges** (relationships between services)
- **Color coding** (marked for deletion, added, changed)
- **Statistics & Impact Analysis**

## 🔌 Strategy Pattern Showcase: Endpoint Registration

The **Endpoint Registration** system demonstrates another Strategy Pattern application:

### Interface: `IEndpointRegistration`
```csharp
internal interface IEndpointRegistration
{
    void Map(IEndpointRouteBuilder versionBasePath);
}
```

### Benefits
✅ **Flexible Endpoint Behavior** - Each endpoint defines its own mapping strategy
✅ **Dependency Injection** - All endpoint strategies registered automatically
✅ **Scalability** - Add new endpoints without modifying existing code
✅ **Testability** - Each endpoint can be tested in isolation

**Location**: `Strategies/Endpoints/RegisterEndpoints.cs`

## 📊 Error Handling Strategy Comparison

| Feature | V1 None | V2 Basic | V3 Switch | V4 Context | V5 Handlers | V6 SOLID | V7 Basta |
|---------|---------|----------|-----------|------------|-------------|----------|----------|
| HTTP Status Codes | ❌ Wrong | ❌ Always 500 | ✅ Proper | ✅ Proper | ✅ Proper | ✅ Proper | 🎨 Art |
| RFC 9457 Format | ❌ | ❌ | ✅ | ✅ | ✅ | ✅ | ❌ Demo |
| Inner Exceptions | ❌ | ❌ | ❌ | ❌ | ✅ | ✅ | ❌ |
| Extensible Handlers | ❌ | ❌ | ❌ | ❌ | ✅ | ✅ | ❌ |
| Centralized Writing | ❌ | ❌ | ❌ | ❌ | ⚠️ | ✅ | ❌ |
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
- `StrategyType.V1_None` - V1: Worst case (empty JSON, always 400) - Educational only
- `StrategyType.V2_Basic` - V2: Simple 500 errors - Basic try-catch
- `StrategyType.V3_Switch` - V3: ProblemDetails with status codes
- `StrategyType.V4_SwitchContext` - V4: Context-aware switching
- `StrategyType.V5_SpecificStrategyErrorHandling` - V5: Extensible handler pattern
- `StrategyType.V6_Solid_Strategy` - V6: SOLID principles + full features (Recommended)
- `StrategyType.V7_Basta` - V7: ASCII Art Demo (fun!) 🎉

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

1. **Strategy Pattern Everywhere**:
   - 🔧 Error Handling Strategies (7 implementations)
   - 🎨 Canvas Visualization Strategies (Mermaid, Graphviz)
   - 🔌 Endpoint Registration Strategies (flexible mapping)

2. **Design Patterns in Combination**: Facade + Strategy + Factory = Clean Code

3. **From 40+ Lines to 3 Lines**: Strategic refactoring reduces complexity

4. **Evolution Philosophy**: Start Simple, Evolve Gradually
   - V1 (None) → V2 (Basic) → V3 (Switch) → V4 (Context) → V5 (Handlers) → V6 (SOLID)

5. **Strategy Pattern Benefits**:
   - ✅ Decouple algorithms from business logic
   - ✅ Runtime selection of behavior
   - ✅ Easy to extend without modifying existing code (Open/Closed Principle)
   - ✅ Each strategy is independently testable

6. **Facade Pattern**: Hide complexity behind a simple API (3 lines in Program.cs)

7. **Production-Ready Patterns**: Inspired by real-world SDK (Siemens)

8. **Security First**: Hide 5xx details in production (V6 strategy)

9. **Developer Experience**: Clean startup code = maintainable codebase

10. **Real-World Applications**: All patterns demonstrated with production-ready examples

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
