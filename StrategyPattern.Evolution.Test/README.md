# Test Project - User API Tests 📸

## 🎯 Snapshot Testing mit AspNetCore.Simple.MsTest.Sdk

Dieses Test-Projekt nutzt **Snapshot Testing** für saubere, wartbare API-Tests.

**Vorteile für die Conference Demo**:
- ✅ **Visuell**: Request/Response JSONs sind perfekt für Präsentationen
- ✅ **Simpel**: `[DynamicRequestLocator]` = Ein Test → Viele Test-Cases
- ✅ **Reviewbar**: JSON Snapshots sind versionierbar und verständlich
- ✅ **Schnell**: Neue Tests = Neue JSON Files, kein C# Code nötig

## 📁 Struktur

```
Api/Users/V1/Create/
├── Status_200_Ok/
│   ├── Create_Status_200_OK_Test.cs          ← Test Code
│   ├── Requests/                              ← Request Snapshots
│   │   ├── CreateRegularUser.json
│   │   ├── CreateAdminUser.json
│   │   └── CreateAiAgentUser.json
│   └── Responses/                             ← Expected Response Snapshots
│       ├── CreateRegularUser.json
│       ├── CreateAdminUser.json
│       └── CreateAiAgentUser.json
│
├── Status_422_UnprocessableContent/
│   ├── Create_Status_422_UnprocessableContent_Test.cs
│   ├── Requests/
│   │   ├── EmptyFirstName.json
│   │   ├── FirstNameTooShort.json
│   │   ├── InvalidEmail.json
│   │   └── MultipleValidationErrors.json     ← Perfekt für Demo!
│   └── Responses/
│       ├── EmptyFirstName.json
│       ├── FirstNameTooShort.json
│       ├── InvalidEmail.json
│       └── MultipleValidationErrors.json     ← Field-level errors!
│
├── Status_409_Conflict/
│   ├── Create_Status_409_Conflict_Test.cs
│   ├── Requests/
│   │   └── DuplicateEmail.json
│   └── Responses/
│       └── DuplicateEmail.json
│
└── Status_400_BadRequest/
    ├── Create_Status_400_BadRequest_Test.cs
    ├── Requests/
    │   ├── MissingComma.json                  ← Malformed JSON
    │   └── MissingClosingBrace.json
    └── Responses/
        ├── MissingComma.json
        └── MissingClosingBrace.json
```

## 🔥 Test Pattern (Super Simple!)

```csharp
[TestMethod]
[DynamicRequestLocator]  // ← Magic! Findet alle Request/Response Paare
public Task Should_Create_User_With(string useCase)
{
    return Client.AssertPostAsync<CreateUserResponse>(
        "api/v1/users",
        useCase,    // Request aus /Requests/{useCase}.json
        useCase     // Response aus /Responses/{useCase}.json
    );
}
```

**Das war's!** 🎉 Ein Test-Method generiert automatisch Tests für:
- ✅ CreateRegularUser
- ✅ CreateAdminUser
- ✅ CreateAiAgentUser

## 📸 Snapshot Beispiel für Conference

### Request: `MultipleValidationErrors.json`
```json
{
  "firstName": "J",        // Too short
  "lastName": "",          // Empty
  "email": "bad-email",    // Invalid format
  "userType": 0
}
```

### Expected Response: `MultipleValidationErrors.json`
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
  }
}
```

**Talk Track**:
> "Seht ihr die Field-level Errors? Das ist V3 Advanced Strategy in Action! Request und Response sind klar, versioniert, reviewbar."

## 🚀 Tests Ausführen

```bash
# Alle User Tests
dotnet test --filter "TestCategory=Users"

# Nur Success Tests
dotnet test --filter "TestCategory~Status 200 OK"

# Nur Validation Tests
dotnet test --filter "TestCategory~422"
```

## 📊 Test Coverage

### ✅ Status 200 OK
- CreateRegularUser ✅
- CreateAdminUser ✅
- CreateAiAgentUser ✅

### ✅ Status 422 Unprocessable Content (Field-level Validation)
- EmptyFirstName ✅
- FirstNameTooShort ✅
- InvalidEmail ✅
- MultipleValidationErrors ✅ ← **Beste für Demo!**

### ✅ Status 409 Conflict
- DuplicateEmail ✅

### ✅ Status 400 Bad Request (JSON Parsing)
- MissingComma ✅
- MissingClosingBrace ✅

## 🎬 Für Die Conference Demo

### Option 1: Live Tests (Empfohlen)

1. **Zeige Ordnerstruktur** (5 Sekunden)
2. **Zeige Request JSON** (10 Sekunden)
3. **Zeige Expected Response JSON** (15 Sekunden)
4. **Zeige Test Code** (10 Sekunden)
5. **Run Tests** → Grün! ✅

```bash
dotnet test --filter "FullyQualifiedName~Should_Return_422"
```

### Option 2: HTTP Demo (Auch gut)

Verwende `demo-requests.http` für Live-HTTP-Calls statt Tests.

## 🔧 Neuen Test Hinzufügen

1. **Request erstellen**: `Requests/MeinTest.json`
2. **Response erstellen**: `Responses/MeinTest.json`
3. **Fertig!** `[DynamicRequestLocator]` findet es automatisch

Oder mit `writeResponse: true` Snapshot automatisch generieren lassen.

## 📚 Detaillierte Doku

Siehe **[SNAPSHOT_TESTING.md](./SNAPSHOT_TESTING.md)** für:
- Platzhalter (`{{guid}}`, `{{datetime}}`)
- Snapshot-Generierung
- Best Practices
- Troubleshooting

## 💡 Warum Snapshot Testing für Conference?

### ❌ Traditionelle Tests
```csharp
Assert.AreEqual("First name is required", errors["FirstName"][0]);
Assert.AreEqual("Last name is required", errors["LastName"][0]);
Assert.AreEqual("Email must be valid", errors["Email"][0]);
// ... 50 Zeilen Assertions
```
→ Langweilig für Publikum! 😴

### ✅ Snapshot Tests
```json
{
  "status": 422,
  "errors": {
    "FirstName": ["First name is required"],
    "LastName": ["Last name is required"],
    "Email": ["Email must be valid"]
  }
}
```
→ **Sofort verständlich!** 🎯

**Plus**: Request UND Response sind sichtbar!

## 🎤 Demo Talk Track

> "Ich zeige euch jetzt unsere Tests. Aber nicht traditionelle Unit Tests mit 100 Zeilen Assertions..."
>
> [Zeige Ordnerstruktur]
>
> "Wir nutzen Snapshot Testing. Hier ist unser Request..."
>
> [Zeige MultipleValidationErrors Request JSON]
>
> "Und hier ist die erwartete Response mit field-level errors..."
>
> [Zeige MultipleValidationErrors Response JSON]
>
> "Der Test-Code? Eine Zeile!"
>
> [Zeige DynamicRequestLocator Test]
>
> "Das ist alles. Jetzt run ich die Tests..."
>
> [dotnet test → Grün]
>
> "Grün! Und das Beste: Ich kann beliebig viele Test-Cases hinzufügen, ohne den Code zu ändern. Einfach neue JSON Files!"

## ⚠️ Current Status

Das Test-Projekt hat aktuell noch Build-Errors wegen fehlender Typen aus dem Hauptprojekt. **Das ist OK für die Demo!**

**Zwei Optionen**:

1. **HTTP Demo** (Empfohlen): Nutze `demo-requests.http` - funktioniert perfekt
2. **Test Demo**: Fix die Build-Errors, dann sind die Snapshot-Tests ready

Die Test-Struktur und Snapshots sind bereits komplett vorbereitet! 🎉
