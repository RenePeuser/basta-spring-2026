# Startup Strategy Demo Guide

## 🎯 Übersicht

Die Anwendung nutzt ein **doppeltes Strategy Pattern**:
1. **Error Handling Strategies** - Verschiedene Fehlerbehandlungs-Ansätze
2. **Startup Strategies** - Verschiedene Startup-Konfigurationen

## 🚀 Quick Start

### Die finale Program.cs (ULTRA CLEAN!)

```csharp
var webApi = new BastaStrategyWebApi(args);

webApi.Run();
```

**Das war's! Nur 3 Zeilen!** 🎉

### Was ist das Besondere?

Die `BastaStrategyWebApi` ist eine **Facade**, die:
- ✅ Alle Komplexität versteckt
- ✅ Intern das Strategy Pattern nutzt
- ✅ Inspiriert vom Siemens `ServerlessMinimalWebApi` Pattern
- ✅ Default Strategy ist `FullBlown` (Production-ready)
- ✅ Kann manuell überschrieben werden via `webApi.StrategyType = ...`

**ALLE Registrierungen** sind in den Startup-Strategies gekapselt:
- ✅ Domain Services (`AddApi`)
- ✅ Error Handling (strategy-spezifisch)
- ✅ Validation, JSON Serialization
- ✅ Query Parameter Security
- ✅ HTTPS Redirection
- ✅ API Routing, Middleware Pipeline
- ✅ Endpoint Registration

**Die Program.cs kennt nur noch 1 Ding:**
1. Erstelle und starte die Web API mit der Facade!

## 🎮 Strategies testen

### Option 1: Manual Override (empfohlen für Demo)

Ändere `Program.cs`:
```csharp
var webApi = new BastaStrategyWebApi(args);
webApi.StrategyType = StrategyType.Basic; // ← Hier ändern!
webApi.Run();
```

### Option 2: Via Environment Variable

```bash
# Dann in Program.cs ohne StrategyType Override
var webApi = new BastaStrategyWebApi(args);
webApi.Run(); // → Nutzt Environment für Strategy-Auswahl

# Basic Strategy (V1)
set ASPNETCORE_ENVIRONMENT=Basic
dotnet run

# Intermediate Strategy (V2)
set ASPNETCORE_ENVIRONMENT=Intermediate
dotnet run

# BASTA! Special (V5)
set ASPNETCORE_ENVIRONMENT=Basta
dotnet run

# Full-Blown Production (Default)
set ASPNETCORE_ENVIRONMENT=FullBlown
dotnet run
```

### Via launchSettings.json

```json
{
  "profiles": {
    "Basic": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Basic"
      }
    },
    "Intermediate": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Intermediate"
      }
    },
    "Basta": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Basta"
      }
    },
    "FullBlown": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "FullBlown"
      }
    }
  }
}
```

## 📋 Demo-Script für BASTA!

### 1. Start: Zeige die clean Program.cs

> "Schaut mal, wie einfach unsere Program.cs ist! Nur 2 Zeilen für die komplette Strategy-Konfiguration."

```csharp
builder.ConfigureForStrategy(strategyType);
webApplication.UseStrategyPipeline();
```

### 2. Zeige die Console-Output beim Start

Wenn die App startet, siehst du:

```
╔══════════════════════════════════════════════════════════════════╗
║  Strategy: Basic                                                 ║
║  V1 - Basic Error Handling (Simple 500 responses)                ║
╚══════════════════════════════════════════════════════════════════╝
```

### 3. Basic Strategy (V1) Demo

**Starte mit**: `ASPNETCORE_ENVIRONMENT=Basic`

**Trigger Error**: POST zu `/api/v1/users` mit fehlendem Field

**Response**:
```json
{
  "error": "An error occurred",
  "message": "The FirstName field is required."
}
```

**Talking Point**:
- "Einfach, aber nicht ideal"
- "Immer Status 500"
- "Keine Unterscheidung zwischen Error-Typen"

### 4. Intermediate Strategy (V2) Demo

**Starte mit**: `ASPNETCORE_ENVIRONMENT=Intermediate`

**Trigger Error**: Gleicher Request

**Response** (ProblemDetails):
```json
{
  "type": "https://httpstatuses.com/400",
  "title": "Bad Request",
  "status": 400,
  "detail": "The FirstName field is required.",
  "instance": "/api/v1/users"
}
```

**Talking Point**:
- "RFC 7807 konform"
- "Richtiger Status Code (400 statt 500)"
- "Strukturierte Error-Information"

### 5. BASTA! Special (V5) Demo 🎨

**Starte mit**: `ASPNETCORE_ENVIRONMENT=Basta`

**Trigger Error**: Gleicher Request

**Response** (ASCII Art):
```
██████╗  █████╗ ███████╗████████╗ █████╗
██╔══██╗██╔══██╗██╔════╝╚══██╔══╝██╔══██╗
██████╔╝███████║███████╗   ██║   ███████║
██╔══██╗██╔══██║╚════██║   ██║   ██╔══██║
██████╔╝██║  ██║███████║   ██║   ██║  ██║
╚═════╝ ╚═╝  ╚═╝╚══════╝   ╚═╝   ╚═╝  ╚═╝

╔═══════════════════════════════════════╗
║  EXCEPTION DETECTED                   ║
║  → Strategy Resolver Activated        ║
║  → Clean JSON Contract Generated      ║
╚═══════════════════════════════════════╝
```

**Talking Point**:
- "Vollständig austauschbare Strategy!"
- "Gleiches Interface, komplett anderes Verhalten"
- "Perfekt für die BASTA! Demo"

### 6. Full-Blown Production (FullBlown) Demo

**Starte mit**: `ASPNETCORE_ENVIRONMENT=FullBlown`

**Trigger Validation Error**: POST mit mehreren invaliden Fields

**Response** (Extended ProblemDetails):
```json
{
  "type": "https://httpstatuses.com/422",
  "title": "Validation Failed",
  "status": 422,
  "errors": {
    "FirstName": ["The FirstName field is required."],
    "Email": ["The Email field must be a valid email address."]
  },
  "traceId": "00-abc123...",
  "instance": "/api/v1/users"
}
```

**Talking Point**:
- "Production-ready mit allen Features"
- "Field-level validation errors"
- "Trace IDs für Debugging"
- "Siemens Error Handling System"

## 🎓 Architecture Highlights

### Was ist besonders?

1. **Strategy Pattern in 2 Ebenen**:
   - Error Handling Strategies (was passiert bei Errors)
   - Startup Strategies (wie wird die App konfiguriert)

2. **Environment-driven Configuration**:
   - Keine Code-Änderungen nötig
   - Nur Environment Variable ändern

3. **Open/Closed Principle**:
   - Neue Strategy? Einfach neue Klasse hinzufügen
   - Keine Änderung an bestehenden Strategies

4. **Single Responsibility**:
   - Jede Strategy kümmert sich nur um ihre Konfiguration
   - Program.cs bleibt minimal

## 🎤 Demo-Reihenfolge (empfohlen)

1. **Start mit Program.cs** - Zeige wie clean sie ist
2. **Basic Demo** - Zeige das Problem
3. **Intermediate Demo** - Zeige die Verbesserung
4. **BASTA! Special** - Zeige die Flexibilität (🎉 Highlight!)
5. **Full-Blown** - Zeige Production-Ready
6. **Code Dive** - Zeige die Startup Strategies
7. **Fazit** - Strategy Pattern macht Konfiguration austauschbar

## 💡 Key Takeaways

- ✅ **Strategy Pattern** macht Code flexibel und testbar
- ✅ **Clean Architecture** - Separation of Concerns
- ✅ **Environment-driven** - Perfect für Testing
- ✅ **Open/Closed** - Easy zu erweitern
- ✅ **Production-ready** - Von Basic bis Full-Blown

## 🔗 Weitere Infos

- `Startup/README.md` - Details zu Startup Strategies
- `Strategies/README.md` - Details zu Error Handling Strategies
