# Agents Guide

This repository publishes a Docker Image that can be used as a replacement for OAuth 2.0 Identity Providers such as Auth0, Okta, Keycloak and FusionAuth etc. when testing components that use such providers. Use in production should be avoided.

The following guidelines are for agents (automated or human) contributing to the development and maintenance of this project.

## Agent Responsibilities

1. **Development**
    - You are an expert C#/.NET developer as [described here](.github/agents/CSharpExpert.agent.md).
    - Implement new features, security updates, housekeeping and bug fixes.
    - All new features and bug fixes should be accompanied by appropriate tests.
    - Follow existing code style and contribution guidelines.
    - Keep dependencies up to date as necessary, especially security-related updates.
    - Ensure that all tests pass before submitting changes.

2. **Testing**
    - Add or update tests relevant to your changes.
    - Run the test suite locally to verify all tests pass prior to opening a pull request.

3. **Review**
    - Submit changes via Pull Request for peer review.
    - Automated checks (e.g. build and test runs) must be green before merging.

4. **Publication**
    - Publication of the Docker image is automated via GitHub Actions but triggerred manually.
    - To trigger a new image release:
        1. Merge all changes to the `main` branch.
        2. Tag the repository with a version following [Semantic Versioning](https://semver.org/). To build a pre-release image, add a suffix `-preview{NUMBER}` where `{NUMBER}` is 3 numeric digits.
        3. The publication workflow will use the tagged commit to generate and publish the Docker image to GitHub Container Registry.

## Technology Stack

- **Language**: C# with .NET 10.0
- **Web Framework**: ASP.NET Core Minimal APIs
- **Testing Framework**: NUnit with Shouldly for assertions
- **Containerization**: Docker with docker-compose for orchestration
- **Code Analysis**: StyleCop.Analyzers for code quality enforcement

## Code Style and Conventions

The project uses StyleCop analyzers and EditorConfig to enforce consistent code style.

### General Formatting

- **Indentation**: 3 spaces for C# files, 2 spaces for other files
- **Line Endings**: CRLF for most files, LF for shell scripts and .dockerignore
- **Max Line Length**: 160 characters
- **Trailing Commas**: Required in multiline lists (warning level)

### Naming Conventions

- **Private Instance Fields**: Use `_camelCase` prefix (e.g., `_httpClient`)
- **Private Static Fields**: Use `PascalCase` (e.g., `DefaultTimeout`)
- **Test Methods**: Use `snake_case` for test method names (e.g., `should_return_token_when_valid_code_provided`)
- **Test Classes in Scenarios**: Use `snake_case` for scenario test class names

### Code Organization

- **Endpoints**: Organize OAuth2 endpoints in the `Endpoints` folder using extension methods on `WebApplication`
- **Grant Type Handlers**: Implement `IGrantTypeHandler` interface for each OAuth2 grant type
- **Services**: Place reusable services in the `Services` folder
- **Models**: Domain models in the `Model` folder with authorization models in `Model/Authorization`

## Building and Testing

### Build Commands

To build the Docker image:
```bash
docker compose -f docker-compose.app.yaml build
```

### Running Tests

The project has two test suites:

1. **Unit/Integration Tests** (in `app/Stackage.OAuth2.Fake.Tests`):
   ```bash
   dotnet test app/Stackage.OAuth2.Fake.Tests/Stackage.OAuth2.Fake.Tests.csproj
   ```

2. **Outside-In Tests** (in `outside-in.tests/Stackage.OAuth2.Fake.OutsideIn.Tests`):
   - These tests require the Docker container to be running
   - Run using `build.ps1` (PowerShell) or via the CI workflow
   ```bash
   # Start the application
   docker compose -f docker-compose.app.yaml up --detach --renew-anon-volumes app

   # Set environment variables
   export STACKAGEOAUTH2FAKETESTS_APP_URL="http://localhost:32111/"
   export STACKAGEOAUTH2FAKETESTS_ISSUER_URL="http://localhost:32111"

   # Run tests
   dotnet test outside-in.tests/Stackage.OAuth2.Fake.OutsideIn.Tests/Stackage.OAuth2.Fake.OutsideIn.Tests.csproj
   ```

### Verify Formatting

Before submitting changes, verify code formatting:
```bash
dotnet format --verify-no-changes
```

## Testing Conventions

### Test Value Naming

The project uses specific conventions for test values to indicate their relationship and purpose:

#### Correlated Values

When a test value must correlate with another value in the test (i.e., they must match or have a specific relationship):

- **Strings**: Prefix with the word `Arbitrary` (e.g., `"ArbitraryClientId"`, `"ArbitrarySubject"`)
- **Non-strings**: Include the word `arbitrary` in the variable name (e.g., `var arbitraryExpirySeconds = 3600`)

**Example**:
```csharp
var clientId = "ArbitraryClientId";
var authorization = UserAuthorization.Create(clientId: clientId, scope: Scope.Empty);
// Later in test, use the same clientId to demonstrate correlation
var httpRequest = CreateRequest(new Dictionary<string, StringValues>
{
   ["client_id"] = clientId, // Must match the clientId used in authorization
});
```

When a test uses a method in another class to generate a test-double, the correlating value should be passed to that method to make it clear it correlates with the test.

#### Independent Values

When a test value doesn't need to correlate with other values (i.e., any valid value will do):

- **Strings**: Use inline values or prefix with `Valid` or any descriptive inline string (e.g., `"valid-subject"`, `"ValidNickname"`)
- **Non-strings**: Use inline values directly (e.g., `1200`, `600`, `true`)

**Example**:
```csharp
var httpRequest = CreateRequest(new Dictionary<string, StringValues>
{
   ["scope"] = "openid profile", // Inline value - doesn't correlate with anything else
   ["expires_in"] = 3600, // Inline value - any valid number works
});
```

### Test Structure

- **Unit/Integration Tests**: Use NUnit's `[Test]` attribute with descriptive `snake_case` method names
- **Outside-In Tests**: Use `[OneTimeSetUp]` for arranging shared test state, with individual `[Test]` methods for assertions
- **Assertions**: Prefer NUnit's `Assert.That` with `Is` constraints. Use Shouldly for collection comparisons (`ShouldBeEquivalentTo`)

## OAuth2 API Guidelines

The application implements OAuth 2.0 and OpenID Connect specifications. Follow these RFCs when implementing or modifying OAuth2 functionality:

### Core OAuth2 Specifications

- **[RFC 6749](https://tools.ietf.org/html/rfc6749)**: The OAuth 2.0 Authorization Framework
  - Section 3.1: Authorization Endpoint
  - Section 3.2: Token Endpoint
  - Section 4.1: Authorization Code Grant
  - Section 6: Refreshing Access Tokens

- **[RFC 8628](https://tools.ietf.org/html/rfc8628)**: OAuth 2.0 Device Authorization Grant
  - Section 3.1: Device Authorization Request
  - Section 3.2: Device Authorization Response
  - Section 3.4: Device Access Token Request

### OpenID Connect

- **[OpenID Connect Core 1.0](https://openid.net/specs/openid-connect-core-1_0.html)**
  - Section 3: Authentication using the Authorization Code Flow
  - Section 5: Claims and Scopes

- **[OpenID Connect Discovery 1.0](https://openid.net/specs/openid-connect-discovery-1_0.html)**
  - Section 4: Obtaining OpenID Provider Configuration Information (`.well-known/openid-configuration`)

### Token Specifications

- **[RFC 7519](https://tools.ietf.org/html/rfc7519)**: JSON Web Token (JWT)
  - Defines the structure and claims of JWTs
  - Specifies how to create and validate JWT tokens
- **[RFC 7517](https://tools.ietf.org/html/rfc7517)**: JSON Web Key (JWK)
  - Defines a JSON representation of cryptographic keys
  - Used for publishing public keys in the JWKS endpoint
- **[RFC 7523](https://tools.ietf.org/html/rfc7523)**: JSON Web Token (JWT) Profile for OAuth 2.0 Client Authentication and Authorization Grants
  - Defines how to use JWTs as authorization grants (using `urn:ietf:params:oauth:grant-type:jwt-bearer` grant type)
  - Defines how to use JWTs for client authentication (using `urn:ietf:params:oauth:client-assertion-type:jwt-bearer` assertion type)

### Error Handling

When implementing OAuth2 error responses, follow the error codes defined in:
- **RFC 6749 Section 4.1.2.1**: Authorization Endpoint Error Responses
- **RFC 6749 Section 5.2**: Token Endpoint Error Responses
- **RFC 8628 Section 3.5**: Device Authorization Error Responses

Common error codes include:
- `invalid_request`: Missing or invalid parameters
- `unauthorized_client`: Client not authorized for the grant type
- `unsupported_grant_type`: Grant type not supported
- `invalid_grant`: Invalid or expired authorization code
- `unsupported_response_type`: Response type not supported

## Internal API

The application provides an internal API endpoint `.internal/create-token` for testing purposes. This endpoint allows direct token creation without following the full OAuth2 flow. It should not be exposed in production environments.
