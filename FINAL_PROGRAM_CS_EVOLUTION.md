# Die Evolution der Program.cs 🚀

## Von 40+ Zeilen zu 3 Zeilen - Die Demo-Story

### Act 1: Die naive Lösung (40+ Zeilen) 😱

```csharp
var builder = WebApplication.CreateBuilder(args);

// ❌ If-Else-Ketten für Service-Registrierung
if (strategyType == StrategyType.Basic) {
    builder.Services.AddBasicErrorHandling();
} else if (strategyType == StrategyType.Intermediate) {
    builder.Services.AddIntermediateErrorHandling();
} else if (strategyType == StrategyType.FullBlown) {
    builder.Services.AddErrorHandling(builder.Configuration);
}

// Common Services (müssen immer da sein)
builder.Services.AddApi(builder.Configuration);
builder.Services.AddRegisterEndpoints();
builder.Services.AddValidation();
builder.Services.AddJsonSerializeOptions();
builder.Services.AddAllowedQueryParameter();

var app = builder.Build();

// ❌ Wieder If-Else-Ketten für Pipeline
app.UseHttpsRedirection();
var apiBasePath = app.MapGroup("api/v1");

if (strategyType == StrategyType.Basic) {
    app.AddBastaErrorHandlingMiddleware();
} else if (strategyType == StrategyType.Intermediate) {
    app.AddBastaErrorHandlingMiddleware();
} else if (strategyType == StrategyType.FullBlown) {
    app.UseErrorHandling();
}

app.UseAllowedQueryParameter();

var registerEndpoints = app.Services.GetRequiredService<RegisterEndpoints>();
registerEndpoints.MapEndpoints(apiBasePath);

app.Run();
```

**Probleme:**
- ❌ 40+ Zeilen Code
- ❌ Duplizierte If-Else-Ketten
- ❌ Schwer wartbar
- ❌ Neue Strategy = Änderungen an mehreren Stellen

---

### Act 2: Strategy Pattern (5 Zeilen) ✨

```csharp
var builder = WebApplication.CreateBuilder(args);

var strategyType = builder.Environment.EnvironmentName.ToEnum<StrategyType>();
builder.ConfigureForStrategy(strategyType);

var webApplication = builder.Build();
webApplication.UseStrategyPipeline();

webApplication.Run();
```

**Besser, aber:**
- ✅ Nur 5 Zeilen - viel besser!
- ✅ Keine If-Else-Ketten mehr
- ⚠️ Aber immer noch WebApplicationBuilder Details sichtbar

---

### Act 3: Facade Pattern (3 Zeilen!) 🎯

```csharp
var webApi = new BastaStrategyWebApi(args);

webApi.Run();
```

**Perfect!**
- ✅ **Nur 3 Zeilen!**
- ✅ Keine technischen Details mehr sichtbar
- ✅ Inspiriert von Siemens ServerlessMinimalWebApi Pattern
- ✅ Facade versteckt die Komplexität
- ✅ Intern nutzt es das Strategy Pattern

---

## Die Architektur dahinter

### Layer 1: Program.cs (Facade)
```
Program.cs (3 Zeilen)
    ↓
BastaStrategyWebApi (Facade)
```

### Layer 2: Strategy Pattern
```
BastaStrategyWebApi
    ↓
StartupStrategyFactory
    ↓
IStartupStrategy
    ├─ BasicStartupStrategy
    ├─ IntermediateStartupStrategy
    ├─ BastaStartupStrategy
    └─ FullBlownStartupStrategy
```

### Layer 3: Konkrete Konfiguration
Jede Strategy konfiguriert:
- ✅ Services (Domain, Error Handling, Validation, etc.)
- ✅ Middleware Pipeline (HTTPS, Error Handling, Routing, etc.)
- ✅ API Endpoints (Route Groups, Mapping)

---

## Advanced Features der BastaStrategyWebApi

### 1. Environment-basierte Strategy (Default)

```csharp
// Strategy wird automatisch aus Environment bestimmt
var webApi = new BastaStrategyWebApi(args);
webApi.Run();

// ASPNETCORE_ENVIRONMENT=Basic → BasicStartupStrategy
// ASPNETCORE_ENVIRONMENT=Intermediate → IntermediateStartupStrategy
// etc.
```

### 2. Manuelles Override (für Tests)

```csharp
var webApi = new BastaStrategyWebApi(args);
webApi.StrategyType = StrategyType.Basic; // ← Override
webApi.Run();
```

### 3. Custom Service Registrations

```csharp
var webApi = new BastaStrategyWebApi(args);

// Zusätzliche Services NACH der Strategy-Konfiguration
webApi.RegisterServices = (services, config) =>
{
    services.AddSingleton<IMyCustomService, MyCustomService>();
};

webApi.Run();
```

### 4. Custom Application Setup

```csharp
var webApi = new BastaStrategyWebApi(args);

// Zusätzliche Middleware NACH der Strategy-Pipeline
webApi.SetupApplication = app =>
{
    app.UseMyCustomMiddleware();
};

webApi.Run();
```

---

## Demo-Flow für BASTA! 🎤

### 1. Start: Zeige die naive Lösung
> "So würden wir das normalerweise machen... 40+ Zeilen, if-else überall.
> Das wird schnell unübersichtlich und schwer zu warten."

### 2. Einführung Strategy Pattern
> "Wir kapseln die Konfiguration in Strategies. Schon viel besser - 5 Zeilen!
> Aber wir sehen immer noch technische Details wie WebApplicationBuilder."

### 3. Die finale Lösung - Facade Pattern
> "Und jetzt die finale Lösung - inspiriert vom Siemens SDK:
> Nur noch 3 Zeilen! Wir verstecken alle Details hinter einer Facade."

```csharp
var webApi = new BastaStrategyWebApi(args);
webApi.Run();
```

> "Schaut wie clean das ist! Intern nutzt es das Strategy Pattern,
> aber von außen ist es extrem einfach zu nutzen."

### 4. Live Demo - Strategy wechseln

```bash
# Terminal 1: Basic
set ASPNETCORE_ENVIRONMENT=Basic
dotnet run
# → POST an API → 500 Error

# Terminal 2: Intermediate
set ASPNETCORE_ENVIRONMENT=Intermediate
dotnet run
# → POST an API → ProblemDetails

# Terminal 3: BASTA!
set ASPNETCORE_ENVIRONMENT=Basta
dotnet run
# → POST an API → ASCII Art! 🎨
```

### 5. Code Dive - Zeige die Facade

> "Die Facade ist super einfach - sie nimmt nur die args und ruft intern
> das Strategy Pattern auf. Schaut mal hier..."

```csharp
public class BastaStrategyWebApi(string[] args)
{
    public StrategyType? StrategyType { get; set; }

    public void Run()
    {
        // Determine strategy from environment
        var strategyType = StrategyType ??
            builder.Environment.EnvironmentName.ToEnum<StrategyType>();

        // Get and execute strategy
        var strategy = StartupStrategyFactory.GetStartupStrategy(strategyType);
        strategy.ConfigureServices(...);
        strategy.ConfigurePipeline(...);
    }
}
```

---

## Vergleich: Vorher vs. Nachher

| Aspekt | Naive Lösung | Strategy Pattern | Facade Pattern |
|--------|--------------|------------------|----------------|
| **Zeilen Code** | 40+ | 5 | **3** ✨ |
| **If-Else-Ketten** | Ja, mehrfach | Nein | Nein |
| **Technische Details** | Alle sichtbar | Teilweise | **Versteckt** ✨ |
| **Wartbarkeit** | Schwer | Gut | **Excellent** ✨ |
| **Erweiterbar** | Schwer | Einfach | **Einfach** ✨ |
| **Testbarkeit** | Schwer | Gut | **Excellent** ✨ |

---

## Key Takeaways 🎯

1. **Von 40+ Zeilen zu 3 Zeilen**
   - Strategy Pattern reduziert Komplexität
   - Facade Pattern versteckt technische Details

2. **Design Patterns in Kombination**
   - Strategy Pattern für Austauschbarkeit
   - Facade Pattern für einfache API
   - Factory Pattern für Strategy-Auswahl

3. **Inspiriert von Production Code**
   - Siemens ServerlessMinimalWebApi Pattern
   - Bewährtes Pattern aus echten Projekten

4. **Clean Code Principles**
   - Single Responsibility
   - Open/Closed Principle
   - Separation of Concerns

---

## Die perfekte Demo-Aussage

> "Wir haben eine komplexe Program.cs mit 40+ Zeilen und vielen If-Else-Ketten
> genommen und sie auf **3 Zeilen** reduziert - ohne Funktionalität zu verlieren!
>
> Das ist die Kraft von Design Patterns: Strategy Pattern für Austauschbarkeit,
> Facade Pattern für Einfachheit, und Factory Pattern für Flexibilität.
>
> Und das Beste: Diese Lösung ist inspiriert von echtem Production Code aus
> dem Siemens SDK. Das ist kein theoretisches Beispiel - das nutzen wir wirklich!" 🎉

---

*Von 40+ Zeilen zu 3 Zeilen - Design Patterns in Action!* 🚀
