# Warum Startup Strategies? 🤔

## Die Demo-Story für BASTA!

### Act 1: Das Problem 😱

**Ausgangssituation**: Du hast verschiedene Error Handling Ansätze:
- Basic (V1) - Einfach, aber nicht production-ready
- Intermediate (V2) - Besser mit ProblemDetails
- Full-Blown - Production-ready mit allen Features

**Die naive Lösung in Program.cs:**

```csharp
var builder = WebApplication.CreateBuilder(args);

// Services konfigurieren
if (strategyType == StrategyType.Basic) {
    builder.Services.AddBasicErrorHandling();
} else if (strategyType == StrategyType.Intermediate) {
    builder.Services.AddIntermediateErrorHandling();
} else if (strategyType == StrategyType.FullBlown) {
    builder.Services.AddErrorHandling(builder.Configuration);
}

// Domain Services
builder.Services.AddApi(builder.Configuration);
builder.Services.AddRegisterEndpoints();
builder.Services.AddValidation();
builder.Services.AddJsonSerializeOptions();
builder.Services.AddAllowedQueryParameter();

var app = builder.Build();

// Pipeline konfigurieren
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
- ❌ **40+ Zeilen Code** nur für die Konfiguration
- ❌ **If-Else-Ketten** machen Code schwer wartbar
- ❌ **Duplizierter Code** zwischen den Branches
- ❌ **Schwer testbar** - Program.cs ist statisch
- ❌ **Neue Strategy?** Überall neue If-Bedingungen hinzufügen

### Act 2: Die Lösung ✨

**Strategy Pattern auf Startup-Ebene!**

#### Schritt 1: Interface definieren

```csharp
public interface IStartupStrategy
{
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    void ConfigurePipeline(WebApplication app);
}
```

#### Schritt 2: Strategies implementieren

Jede Strategy kennt **NUR** ihre eigene Konfiguration:

```csharp
public class BasicStartupStrategy : IStartupStrategy
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddApi(configuration);
        services.AddBasicErrorHandling();  // ← Strategy-spezifisch
        services.AddRegisterEndpoints();
        services.AddValidation();
        services.AddJsonSerializeOptions();
        services.AddAllowedQueryParameter();
    }

    public void ConfigurePipeline(WebApplication app)
    {
        app.UseHttpsRedirection();
        var apiBasePath = app.MapGroup("api/v1");
        app.AddBastaErrorHandlingMiddleware();  // ← Strategy-spezifisch
        app.UseAllowedQueryParameter();

        var registerEndpoints = app.Services.GetRequiredService<RegisterEndpoints>();
        registerEndpoints.MapEndpoints(apiBasePath);
    }
}
```

#### Schritt 3: Program.cs wird minimal

```csharp
var builder = WebApplication.CreateBuilder(args);

// STEP 1: Determine strategy from environment
var strategyType = builder.Environment.EnvironmentName.ToEnum<StrategyType>();

// STEP 2: Configure EVERYTHING via the selected strategy
builder.ConfigureForStrategy(strategyType);

// STEP 3: Build application
var webApplication = builder.Build();

// STEP 4: Configure pipeline via the selected strategy
webApplication.UseStrategyPipeline();

// STEP 5: Run!
webApplication.Run();
```

**Vorteile:**
- ✅ **Nur 5 Zeilen Code** in Program.cs
- ✅ **Keine If-Else-Ketten** mehr
- ✅ **Single Responsibility** - jede Strategy kennt nur ihre Konfiguration
- ✅ **Open/Closed Principle** - neue Strategy? Einfach neue Klasse hinzufügen
- ✅ **Testbar** - jede Strategy ist isoliert testbar
- ✅ **Environment-driven** - perfekt für Tests und verschiedene Umgebungen

### Act 3: Die Demo 🎤

#### 1. Zeige die komplexe Program.cs (Before)

> "Schaut mal, so würde das ohne Strategy Pattern aussehen... 40+ Zeilen,
> if-else-Ketten, schwer wartbar. Und bei jeder neuen Strategy müssen wir
> an mehreren Stellen ändern."

#### 2. Zeige die Strategy-Lösung (After)

> "Und jetzt mit Strategy Pattern... nur 5 Zeilen! Die gesamte Konfiguration
> ist in den Strategy-Klassen gekapselt."

```csharp
var strategyType = builder.Environment.EnvironmentName.ToEnum<StrategyType>();
builder.ConfigureForStrategy(strategyType);
var webApplication = builder.Build();
webApplication.UseStrategyPipeline();
webApplication.Run();
```

#### 3. Live-Demo: Strategy wechseln

```bash
# Basic Strategy
set ASPNETCORE_ENVIRONMENT=Basic
dotnet run
# POST an API → 500 Error

# Intermediate Strategy
set ASPNETCORE_ENVIRONMENT=Intermediate
dotnet run
# POST an API → ProblemDetails mit 400

# BASTA! Strategy
set ASPNETCORE_ENVIRONMENT=Basta
dotnet run
# POST an API → ASCII Art! 🎨
```

#### 4. Zeige eine Strategy-Implementierung

> "Schaut, jede Strategy ist komplett selbständig. Die BasicStartupStrategy
> kennt nur ihre eigene Konfiguration. Keine Abhängigkeiten zu anderen Strategies."

#### 5. Neue Strategy hinzufügen

> "Neue Strategy hinzufügen? Kein Problem:"

```csharp
public class CustomStartupStrategy : IStartupStrategy
{
    public void ConfigureServices(...) { /* Your config */ }
    public void ConfigurePipeline(...) { /* Your pipeline */ }
}
```

> "Fertig! Program.cs bleibt unverändert."

## 🎯 Key Takeaways für die Audience

1. **Strategy Pattern ist nicht nur für Business Logic**
   - Auch perfekt für Infrastruktur-Konfiguration
   - Macht Startup-Code clean und wartbar

2. **Environment-driven Configuration**
   - Verschiedene Environments = verschiedene Strategies
   - Perfekt für Testing, Demo, Production

3. **SOLID Principles in Action**
   - Single Responsibility: Jede Strategy kennt nur ihre Konfiguration
   - Open/Closed: Neue Strategy ohne Änderung bestehenden Codes

4. **Real-World Problem gelöst**
   - Nicht nur theoretisch - echtes Problem aus der Praxis
   - Jeder kennt komplexe Program.cs mit vielen If-Bedingungen

## 📊 Vorher/Nachher Vergleich

| Aspekt | Ohne Strategy | Mit Strategy |
|--------|---------------|--------------|
| **Zeilen in Program.cs** | 40+ | 5 |
| **If-Else-Ketten** | Ja, mehrfach | Nein |
| **Neue Strategy hinzufügen** | Änderungen an mehreren Stellen | Nur neue Klasse |
| **Testbarkeit** | Schwierig | Einfach |
| **Wartbarkeit** | Schwer | Einfach |
| **Übersichtlichkeit** | Komplex | Crystal Clear ✨ |

## 🚀 Zusammenfassung

**Die Frage**: "Warum sollte man Startup Strategies nutzen?"

**Die Antwort**:
1. **Program.cs bleibt minimal** (5 Zeilen statt 40+)
2. **Keine komplexen If-Else-Ketten** mehr
3. **Jede Strategy ist isoliert** und selbständig
4. **Neue Strategies** einfach hinzufügen (Open/Closed)
5. **Perfekt für environment-spezifische Konfigurationen**
6. **Testbar** und **wartbar**

**Das Ergebnis**: Clean Code, der das Strategy Pattern nicht nur in der Business Logic,
sondern auch auf Infrastructure-Ebene nutzt. 🎉

---

*"Das Strategy Pattern ist nicht nur ein Design Pattern -
es ist eine Denkweise für sauberen, erweiterbaren Code!"* 🎯
