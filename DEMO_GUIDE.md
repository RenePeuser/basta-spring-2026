# Error Handling Strategy Evolution - Conference Demo Guide

## 🎯 Demo Objective
Show how error handling can evolve from basic to enterprise-grade, using the Strategy Pattern to swap implementations without changing business logic.

## 📋 Prerequisites
- .NET 9 SDK installed
- REST client (Postman, curl, or VS Code REST Client)
- Project builds successfully

---

## 🚀 Demo Flow (30-40 minutes)

### Part 1: Introduction (5 minutes)
**Talk Track**:
> "Today we'll build a simple User API and evolve its error handling from basic to enterprise-grade. We'll see how the Strategy Pattern lets us swap error handling implementations without touching business logic."

**Show**:
- The simple User model and CreateUserEndpoint
- Point out: validation, duplicate check logic

---

### Part 2: V1 - Basic Error Handling (8 minutes)

#### Setup
In `Program.cs`, add BEFORE `var webApplication = builder.Build();`:

```csharp
// V1 - Basic Error Handling
builder.Services.AddBasicErrorHandling();
```

After `var apiBasePath = ...`, add:

```csharp
webApplication.UseMiddleware<BasicErrorHandlingMiddleware>();
```

#### Demo Scenario 1: Validation Error
**Request**:
```http
POST http://localhost:5000/api/v1/users
Content-Type: application/json

{
  "firstName": "",
  "lastName": "Doe",
  "email": "invalid-email",
  "userType": 0
}
```

**Expected Response** (V1 - Basic):
```json
{
  "error": "An error occurred",
  "message": "User validation failed"
}
```

**Talk Track**:
> "Notice: Always HTTP 500, no field-level details, just a generic message. Clients can't tell what's wrong."

#### Demo Scenario 2: Duplicate User
**Request** (send twice):
```http
POST http://localhost:5000/api/v1/users
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "userType": 0
}
```

**Talk Track**:
> "First request succeeds, second fails with 500. But this isn't a server error - it's a conflict! Wrong status code confuses clients and monitoring."

**Problems Summary**:
- ❌ All errors return HTTP 500
- ❌ No standard format (not RFC 7807)
- ❌ No field-level validation details
- ❌ Security risk: exposes stack traces
- ❌ Poor client experience

---

### Part 3: V2 - Intermediate Error Handling (10 minutes)

#### Setup
**Replace** in `Program.cs`:

```csharp
// V2 - Intermediate Error Handling
builder.Services.AddIntermediateErrorHandling();

// ...later...
webApplication.UseMiddleware<IntermediateErrorHandlingMiddleware>();
```

#### Demo Scenario 1: Validation Error (Repeat)
**Expected Response** (V2 - Intermediate):
```json
{
  "type": "https://httpstatuses.com/400",
  "title": "Bad Request",
  "status": 400,
  "detail": "User validation failed",
  "instance": "/api/v1/users"
}
```

**Talk Track**:
> "Now we get HTTP 400 - correct! And RFC 7807 ProblemDetails format. Industry standard. But... where are the field-level details?"

#### Demo Scenario 2: Duplicate User (Repeat)
**Expected Response** (V2 - Intermediate):
```json
{
  "type": "https://httpstatuses.com/409",
  "title": "Conflict",
  "status": 409,
  "detail": "User with email 'john@example.com' already exists",
  "instance": "/api/v1/users"
}
```

**Talk Track**:
> "HTTP 409 Conflict - perfect! Clients can now handle this properly. But we're still missing detailed validation."

#### Demo Scenario 3: JSON Parse Error
**Request** (malformed JSON):
```http
POST http://localhost:5000/api/v1/users
Content-Type: application/json

{
  "firstName": "John"
  "lastName": "Doe"
}
```

**Talk Track**:
> "Malformed JSON still crashes or gives generic errors. We need more sophistication."

**Improvements**:
- ✅ Proper HTTP status codes (400, 409, 404, etc.)
- ✅ RFC 7807 ProblemDetails format
- ✅ Meaningful error titles and details
- ⚠️  Still no field-level validation
- ⚠️  No JSON parsing error details

---

### Part 4: V3 - Advanced Error Handling (12 minutes)

#### Setup
**Replace** in `Program.cs`:

```csharp
// V3 - Advanced Error Handling
builder.Services.AddAdvancedErrorHandling(builder.Configuration);

// ...later...
webApplication.UseAdvancedErrorHandling();
```

#### Demo Scenario 1: Multiple Validation Errors
**Request**:
```http
POST http://localhost:5000/api/v1/users
Content-Type: application/json

{
  "firstName": "J",
  "lastName": "",
  "email": "not-an-email",
  "userType": 0
}
```

**Expected Response** (V3 - Advanced):
```json
{
  "type": "https://httpstatuses.com/422",
  "title": "Validation Error",
  "status": 422,
  "detail": "One or more validation errors occurred",
  "instance": "/api/v1/users",
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
  "traceId": "0HN7...",
  "timestamp": "2026-02-28T10:30:00Z"
}
```

**Talk Track**:
> "Now THIS is developer-friendly! Field-level errors, trace IDs for support, timestamps. Clients can show specific field errors in their UI."

#### Demo Scenario 2: JSON Parse Error with Details
**Request** (malformed JSON):
```http
POST http://localhost:5000/api/v1/users
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com"
  "userType": 0
}
```

**Expected Response** (V3 - Advanced):
```json
{
  "type": "https://httpstatuses.com/400",
  "title": "JSON Parse Error",
  "status": 400,
  "detail": "The JSON value could not be converted to...",
  "instance": "/api/v1/users",
  "extensions": {
    "lineNumber": 5,
    "bytePosition": 87,
    "path": "$.userType",
    "expectedToken": ","
  },
  "traceId": "0HN7..."
}
```

**Talk Track**:
> "JSON parse errors now show EXACTLY where the problem is - line number, position, what's expected. Saves developers hours!"

#### Demo Scenario 3: Configuration-Based Security
**Show `appsettings.json`**:

```json
"ErrorHandlerSettings": {
  "ShowExceptionDetailsOnErrorCodeRanges": [
    {
      "Start": 0,
      "End": 499
    }
  ]
}
```

**Talk Track**:
> "In production, set End to 499. 5xx errors (server failures) get sanitized responses - no stack traces leaked. Security first!"

**Production Response** (5xx error):
```json
{
  "type": "https://httpstatuses.com/500",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "An unexpected error occurred. Please contact support with the trace ID.",
  "traceId": "0HN7..."
}
```

**Benefits Summary**:
- ✅ Field-level validation errors
- ✅ JSON parsing diagnostics
- ✅ Trace IDs for support
- ✅ Security-aware (hides 5xx details in prod)
- ✅ Timestamps and metadata
- ✅ Extensible handler system
- ✅ Zero business logic changes

---

### Part 5: The Strategy Pattern Power (5 minutes)

**Show the Code**:

```csharp
// Business logic NEVER changed!
// We just swapped strategies:

// V1
builder.Services.AddBasicErrorHandling();

// V2
builder.Services.AddIntermediateErrorHandling();

// V3
builder.Services.AddAdvancedErrorHandling(builder.Configuration);
```

**Talk Track**:
> "The CreateUserEndpoint NEVER changed. That's the Strategy Pattern in action. Error handling is a cross-cutting concern - separate it from business logic. Start simple, evolve as needed."

---

## 📝 Test Requests Quick Reference

### Success Case
```http
POST http://localhost:5000/api/v1/users
Content-Type: application/json

{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane@example.com",
  "userType": 0
}
```

### Validation Error
```http
POST http://localhost:5000/api/v1/users
Content-Type: application/json

{
  "firstName": "",
  "lastName": "D",
  "email": "bad-email",
  "userType": 0
}
```

### Duplicate Conflict
```http
# Send twice to same email
POST http://localhost:5000/api/v1/users
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "duplicate@example.com",
  "userType": 0
}
```

### JSON Parse Error
```http
POST http://localhost:5000/api/v1/users
Content-Type: application/json

{
  "firstName": "John"
  "lastName": "Doe"
}
```

---

## 🎤 Key Takeaways for Audience

1. **Start Simple**: V1 is fine for prototypes
2. **Evolve Gradually**: Don't over-engineer from day one
3. **Use Standards**: RFC 7807 ProblemDetails is your friend
4. **Strategy Pattern**: Separate concerns, swap implementations
5. **Security Matters**: Hide 5xx details in production
6. **Developer Experience**: Field-level errors save time
7. **Configuration Over Code**: Make it configurable

---

## 🐛 Troubleshooting

### Port Already in Use
```bash
# Change port in launchSettings.json or run:
dotnet run --urls "http://localhost:5001"
```

### Build Errors
```bash
dotnet clean
dotnet restore
dotnet build
```

### Clear User Repository
```csharp
// In CreateUserEndpoint, inject and call:
repository.Clear();
```

---

## 📚 Further Reading
- RFC 7807: Problem Details for HTTP APIs
- Strategy Pattern (Gang of Four)
- ASP.NET Core Exception Handling Best Practices
