# Startup Strategy Pattern

Diese Implementierung demonstriert das **Strategy Pattern auf Startup-Ebene**.
Jeder `StrategyType` hat seine eigene Startup-Konfiguration, die nur die notwendigen Services und Middleware registriert.

## 🎯 Konzept

Anstatt eine komplexe `Program.cs` mit vielen Bedingungen zu haben, nutzen wir eine **Facade + Strategy Pattern Kombination**:

```csharp
// Das ist ALLES was in Program.cs steht (nur 3 Zeilen!):
var webApi = new BastaStrategyWebApi(args);

webApi.Run();
```

**Die GESAMTE Startup-Logik ist hinter einer Facade versteckt, die intern das Strategy Pattern nutzt!**

Die `BastaStrategyWebApi` Facade:
- Bestimmt automatisch die Strategy (default: FullBlown, oder via Environment/Manual Override)
- Ruft intern das Strategy Pattern auf
- Konfiguriert Services und Pipeline
- Inspiriert vom Siemens `ServerlessMinimalWebApi` Pattern

Jede Strategy ist verantwortlich für:
- ✅ **Alle Service-Registrierungen** (Domain, Error Handling, Validation, etc.)
- ✅ **Komplette Middleware-Pipeline** (HTTPS, Error Handling, Routing, etc.)
- ✅ **API-Konfiguration** (Route Groups, Endpoint Mapping)

## 🎤 Demo Story: "Warum Startup Strategies?"

### Problem ohne Strategy Pattern:

```csharp
// Program.cs wird schnell komplex und unübersichtlich
if (environment == "Basic") {
    services.AddBasicErrorHandling();
} else if (environment == "Intermediate") {
    services.AddIntermediateErrorHandling();
} else if (environment == "FullBlown") {
    services.AddErrorHandling(configuration);
}

services.AddApi(configuration);
services.AddValidation();
// ... 20+ weitere Registrierungen

// Und dann wieder für die Pipeline...
if (environment == "Basic") {
    app.AddBastaErrorHandlingMiddleware();
} else if (environment == "Intermediate") {
    app.AddBastaErrorHandlingMiddleware();
} else if (environment == "FullBlown") {
    app.UseErrorHandling();
}

app.UseHttpsRedirection();
// ... 10+ weitere Middleware
```

**Das wird schnell unübersichtlich!** 😱

### Lösung mit Facade + Strategy Pattern:

```csharp
// Program.cs ist minimal - nur 3 Zeilen!
var webApi = new BastaStrategyWebApi(args);

webApi.Run();
```

**Ultra clean, testbar, erweiterbar!** ✨

Die Facade versteckt die Komplexität und nutzt intern das Strategy Pattern.

### Warum ist das wichtig?

1. **Testing**: Verschiedene Environments mit unterschiedlichen Konfigurationen
2. **Demo/Development**: Schnell zwischen Konfigurationen wechseln
3. **Production**: Unterschiedliche Setups für verschiedene Deployment-Szenarien
4. **Maintenance**: Neue Konfiguration? Einfach neue Strategy-Klasse hinzufügen!

## 📦 Verfügbare Startup Strategies

### 1. BasicStartupStrategy (V1)
- **StrategyType**: `Basic`
- **Services**: Nur `AddBasicErrorHandling()`
- **Pipeline**: Nur `AddBastaErrorHandlingMiddleware()`
- **Use Case**: Einfachste Demo - 500er Fehler für alles

### 2. IntermediateStartupStrategy (V2)
- **StrategyType**: `Intermediate`
- **Services**: Nur `AddIntermediateErrorHandling()`
- **Pipeline**: Nur `AddBastaErrorHandlingMiddleware()`
- **Use Case**: ProblemDetails mit HTTP Status Codes

### 3. BastaStartupStrategy (V5)
- **StrategyType**: `Basta`
- **Services**: Nur `AddBastaAdvancedErrorHandlingStrategy()`
- **Pipeline**: Nur `AddBastaErrorHandlingMiddleware()`
- **Use Case**: Special BASTA! Demo mit ASCII Art Response

### 4. FullBlownStartupStrategy
- **StrategyType**: `FullBlown` oder `Advanced`
- **Services**: Vollständiges Siemens Error Handling System
- **Pipeline**: `UseErrorHandling()` (Siemens Middleware)
- **Use Case**: Production-ready mit allen Features

## 🚀 Usage

### Option 1: Default (FullBlown)

```csharp
var webApi = new BastaStrategyWebApi(args);
webApi.Run();
// → Nutzt FullBlown Strategy
```

### Option 2: Manual Override

```csharp
var webApi = new BastaStrategyWebApi(args);
webApi.StrategyType = StrategyType.Basic; // oder Intermediate, Basta, FullBlown
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

## 🎨 Demo-Flow

```
┌──────────────────┐
│  Program.cs      │  ← Nur 3 Zeilen!
│  (Facade Call)   │
└────────┬─────────┘
         │
         v
┌───────────────────────┐
│ BastaStrategyWebApi   │  ← Facade Pattern
│ (versteckt Details)   │
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

**Drei Design Patterns in Kombination:**
1. **Facade Pattern** - BastaStrategyWebApi versteckt Komplexität
2. **Factory Pattern** - StartupStrategyFactory erstellt Strategy
3. **Strategy Pattern** - Austauschbare Startup-Konfigurationen

## 🎓 Warum ist das cool für die Demo?

1. **Single Responsibility**: Jede Startup-Strategy kümmert sich nur um ihre eigene Konfiguration
2. **Open/Closed Principle**: Neue Strategies hinzufügen ohne bestehende zu ändern
3. **Clean Program.cs**: Die Haupt-Entry-Point bleibt minimal und übersichtlich
4. **Testbar**: Jede Strategy kann isoliert getestet werden
5. **Environment-basiert**: Perfekt für Integration-Tests mit verschiedenen Umgebungen

## 📊 Vergleich: Vorher vs. Nachher

### Vorher (komplexe Program.cs)
```csharp
if (strategyType == StrategyType.Basic) {
    builder.Services.AddBasicErrorHandling();
} else if (strategyType == StrategyType.Intermediate) {
    builder.Services.AddIntermediateErrorHandling();
} else if (strategyType == StrategyType.FullBlown) {
    builder.Services.AddErrorHandling(configuration);
}
// ... und dann nochmal für die Pipeline
```

### Nachher (Clean mit Facade + Strategy Pattern)
```csharp
var webApi = new BastaStrategyWebApi(args);
webApi.Run();
// Done! 🎉
```

**Von vielen Zeilen zu nur 3 Zeilen!**

## 🔧 Erweiterung

Neue Strategy hinzufügen? Einfach:

1. Neuen `StrategyType` zum Enum hinzufügen
2. Neue `XyzStartupStrategy : IStartupStrategy` erstellen
3. In der Factory registrieren
4. Fertig! 🚀

## 🎤 BASTA! Talking Points

- "Die Program.cs hat nur 3 Zeilen - inspiriert vom Siemens SDK!"
- "Wir kombinieren Facade Pattern (für Einfachheit) mit Strategy Pattern (für Flexibilität)"
- "Alle Komplexität ist versteckt, aber die Power ist noch da"
- "Jede Strategy konfiguriert ALLES - Services, Pipeline, Endpoints"
- "Neue Konfigurationen? Einfach neue Strategy-Klasse hinzufügen!"
