# Conference Demo - Mit Snapshot Tests 🎬📸

## 🎯 Demo Flow (40 Minuten)

### Part 1: Intro (3 min)
> "Heute zeige ich euch die Evolution von Error Handling - von basic zu enterprise-grade. Und ich zeige euch dabei schöne Snapshot Tests!"

### Part 2: V1 Basic Error Handling (8 min)

#### Setup
```bash
# Terminal 1: Start API mit V1
cd D:\GitHub\basta-spring-2026\StrategyPattern.Evolution
dotnet run
```

#### Zeige Test-Struktur (2 min)
```
Status_422_UnprocessableContent/
├── Requests/
│   └── MultipleValidationErrors.json    ← Zeige diese
└── Responses/
    └── MultipleValidationErrors.json    ← Und diese
```

**Talk Track**:
> "Das ist unser Test-Setup. Snapshot Testing - Request und Expected Response als JSON Files."

#### Request Snapshot (30 Sekunden)
```json
{
  "firstName": "J",        // Too short!
  "lastName": "",          // Empty!
  "email": "bad-email",    // Invalid!
  "userType": 0
}
```

#### Expected Response mit V3 (30 Sekunden)
```json
{
  "status": 422,
  "errors": {
    "FirstName": ["must be at least 2 characters"],
    "LastName": ["is required"],
    "Email": ["must be a valid email address"]
  }
}
```

**Talk Track**:
> "Das wäre die perfekte Response - field-level errors. Schauen wir ob V1 das schafft..."

#### Live Demo mit V1 (3 min)
```bash
# Terminal 2: Send Request
curl -X POST http://localhost:5000/api/v1/users \
  -H "Content-Type: application/json" \
  -d @StrategyPattern.Evolution.Test/Api/Users/V1/Create/Status_422_UnprocessableContent/Requests/MultipleValidationErrors.json
```

**Response mit V1:**
```json
{
  "error": "An error occurred",
  "message": "User validation failed"
}
```

**Talk Track**:
> "Autsch! HTTP 500, keine Details, keine field-level errors. Das ist V1 - funktioniert, aber nicht hilfreich."

---

### Part 3: V2 Intermediate (8 min)

#### Setup
```bash
# Stop API (Ctrl+C)
# Ändere Program.cs zu V2 (siehe Program.cs.EXAMPLES.md)
dotnet run
```

#### Live Demo mit V2 (3 min)
```bash
# Same Request
curl -X POST http://localhost:5000/api/v1/users \
  -H "Content-Type: application/json" \
  -d @StrategyPattern.Evolution.Test/Api/Users/V1/Create/Status_422_UnprocessableContent/Requests/MultipleValidationErrors.json
```

**Response mit V2:**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "User validation failed",
  "instance": "/api/v1/users"
}
```

**Talk Track**:
> "Besser! HTTP 400 statt 500, RFC 7807 ProblemDetails Format. Aber immer noch keine field-level details."

#### Test Duplicate Email (2 min)
```bash
# Send twice
curl -X POST http://localhost:5000/api/v1/users \
  -H "Content-Type: application/json" \
  -d '{"firstName":"John","lastName":"Doe","email":"duplicate@example.com","userType":0}'
```

**Second Response:**
```json
{
  "status": 409,
  "title": "Conflict",
  "detail": "User with email 'duplicate@example.com' already exists"
}
```

**Talk Track**:
> "Perfekt! HTTP 409 Conflict. Richtige Semantik. Aber field-level errors fehlen noch..."

---

### Part 4: V3 Advanced - THE FINALE! (12 min)

#### Setup
```bash
# Stop API
# Ändere Program.cs zu V3
dotnet run
```

#### Live Demo: Multiple Validation Errors (3 min)
```bash
curl -X POST http://localhost:5000/api/v1/users \
  -H "Content-Type: application/json" \
  -d @StrategyPattern.Evolution.Test/Api/Users/V1/Create/Status_422_UnprocessableContent/Requests/MultipleValidationErrors.json
```

**Response mit V3:**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.23",
  "title": "One or more validation errors occurred.",
  "status": 422,
  "errors": {
    "FirstName": [
      "First name must be at least 2 characters"
    ],
    "LastName": [
      "Last name is required"
    ],
    "Email": [
      "Email must be a valid email address"
    ]
  },
  "traceId": "0HN...",
  "timestamp": "2026-02-28T..."
}
```

**Talk Track**:
> "BOOM! 💥 DAS ist enterprise-grade! Field-level errors, HTTP 422, Trace-ID für Support, Timestamp!"

#### Zeige Snapshot Test (3 min)

**Test Code** (Open in IDE):
```csharp
[TestMethod]
[DynamicRequestLocator]
public Task Should_Return_422_When_Request_Contains(string useCase)
{
    return Client.AssertPostAsync<ValidationProblemDetails>(
        "api/v1/users",
        useCase,
        useCase
    );
}
```

**Talk Track**:
> "Und hier ist der Test. EINE Zeile! Der `DynamicRequestLocator` findet automatisch alle Request/Response Paare in den Ordnern."

#### Run Tests (2 min)
```bash
# Terminal 2
cd StrategyPattern.Evolution.Test
dotnet test --filter "TestCategory~422"
```

**Output:**
```
✅ Should_Return_422_When_Request_Contains("EmptyFirstName")
✅ Should_Return_422_When_Request_Contains("FirstNameTooShort")
✅ Should_Return_422_When_Request_Contains("InvalidEmail")
✅ Should_Return_422_When_Request_Contains("MultipleValidationErrors")

Passed! - 4 tests
```

**Talk Track**:
> "Grün! Alle 4 Tests laufen durch. Und ich kann beliebig viele hinzufügen - einfach neue JSON Files, kein Code ändern!"

#### Live Demo: JSON Parse Error (2 min)
```bash
curl -X POST http://localhost:5000/api/v1/users \
  -H "Content-Type: application/json" \
  -d @StrategyPattern.Evolution.Test/Api/Users/V1/Create/Status_400_BadRequest/Requests/MissingComma.json
```

**Response:**
```json
{
  "status": 400,
  "title": "Bad Request",
  "detail": "The JSON value could not be converted. Expected ',' after property value.",
  "extensions": {
    "lineNumber": 2,
    "bytePosition": 18,
    "path": "$.lastName"
  }
}
```

**Talk Track**:
> "Sogar JSON Parse Errors bekommen Details! Line number, position, was fehlt. Developer Experience at its finest!"

---

### Part 5: The Key Point (5 min)

#### Zeige Business Logic (2 min)
**Open**: `CreateUserEndpoint.cs` (Lines 68-78)

```csharp
static Task<CreateUserResponse> HandleAsync(
    CreateUserRequest createUserRequest,
    UserDemoRepository repository,
    HttpContext httpContext,
    CancellationToken cancellationToken = default)
{
    // Validate the user input
    UserValidator.Validate(createUserRequest);

    // Create the user
    var user = new User(...);

    // Add to repository
    repository.Add(user);

    return Task.FromResult(new CreateUserResponse(user));
}
```

**Talk Track**:
> "DER CODE HAT SICH NIE GEÄNDERT! 🎯 Das ist die Strategy Pattern Power. Error Handling ist eine Cross-Cutting Concern - komplett getrennt von Business Logic!"

#### Zeige Snapshot Benefits (2 min)

**Request Snapshot** vs **Traditional Test**:

```csharp
// ❌ Traditional Test - 20 Lines
Assert.AreEqual("First name is required", errors["FirstName"][0]);
Assert.AreEqual("Last name is required", errors["LastName"][0]);
Assert.AreEqual(422, response.Status);
// ... 15 more lines

// ✅ Snapshot Test - 1 Line + 2 JSON Files
return Client.AssertPostAsync<ValidationProblemDetails>(
    "api/v1/users",
    "MultipleValidationErrors",
    "MultipleValidationErrors"
);
```

**Talk Track**:
> "Und Tests? Request + Expected Response als JSON. Reviewbar, versionierbar, verständlich. Kein 50-Zeilen Assertion-Code!"

---

### Part 6: Q&A (4 min)

**Häufige Fragen**:

Q: "Wie oft müssen Snapshots aktualisiert werden?"
A: "Bei API-Änderungen. Dann `writeResponse: true` und neue Snapshots generieren."

Q: "Was ist mit dynamischen Werten wie GUIDs?"
A: "Platzhalter: `{{guid}}`, `{{datetime}}`, `{{any}}`"

Q: "Performance von Snapshot Testing?"
A: "Gleich wie normale Tests. SDK macht JSON-Vergleich sehr effizient."

Q: "Muss ich zwischen Strategien wechseln?"
A: "Nein! Produktiv: V3. V1/V2 zeige ich nur für Demo. Start simple, evolve as needed."

---

## 🎯 Quick Commands Cheat Sheet

### API Starten
```bash
cd D:\GitHub\basta-spring-2026\StrategyPattern.Evolution
dotnet run
```

### Tests Ausführen
```bash
cd D:\GitHub\basta-spring-2026\StrategyPattern.Evolution.Test
dotnet test --filter "TestCategory=Users"
```

### Requests Senden
```bash
# Validation Error
curl -X POST http://localhost:5000/api/v1/users \
  -H "Content-Type: application/json" \
  -d '{"firstName":"J","lastName":"","email":"bad","userType":0}'

# Success
curl -X POST http://localhost:5000/api/v1/users \
  -H "Content-Type: application/json" \
  -d '{"firstName":"Jane","lastName":"Doe","email":"jane@example.com","userType":0}'

# Duplicate
curl -X POST http://localhost:5000/api/v1/users \
  -H "Content-Type: application/json" \
  -d '{"firstName":"John","lastName":"Doe","email":"dup@example.com","userType":0}'
# Send twice!
```

---

## 📸 Screenshot Opportunities

1. **Ordnerstruktur** - Zeige clean Request/Response Separation
2. **MultipleValidationErrors.json Request** - Alle Fields falsch
3. **MultipleValidationErrors.json Response** - Field-level errors
4. **Test Code** - Eine Zeile!
5. **Test Run** - Grün mit 4 Tests aus einer Method
6. **Business Logic** - Unverändert

---

## 💡 Backup Slides (Falls Zeit übrig)

- RFC 7807 Standard Erklärung
- Strategy Pattern Diagramm
- Konfiguration (appsettings.json)
- Security: Error Detail Hiding in Production
- Extension Points im V3 System

---

## 🎤 Closing Statement

> "Zusammenfassung: Start simple mit V1. Wenn ihr mehr braucht: V2 mit ProblemDetails. Enterprise? V3 gibt euch alles - field-level errors, trace IDs, security. Und mit Snapshot Testing habt ihr saubere, wartbare Tests. Request und Response als JSON - reviewbar, versionierbar, verständlich. Das wars von mir - Fragen?"

**[Applaus] 👏**

---

## ✅ Pre-Demo Checklist

- [ ] API kompiliert (`dotnet build StrategyPattern.Evolution/`)
- [ ] Tests kompilieren (`dotnet build StrategyPattern.Evolution.Test/`)
- [ ] Program.cs auf V3 konfiguriert (für Finale)
- [ ] `demo-requests.http` geöffnet (Backup)
- [ ] Terminal 1: API Ready
- [ ] Terminal 2: Curl/Test Commands Ready
- [ ] IDE: CreateUserEndpoint.cs geöffnet (Business Logic)
- [ ] IDE: Test mit DynamicRequestLocator geöffnet
- [ ] Explorer: Snapshot Ordner geöffnet
- [ ] Beamer/Screen-Share tested

**You're Ready! Go Rock That Conference! 🚀🎉**
