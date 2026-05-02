# stackage-oauth2-fake-docker

> ⚠️ **WARNING: This is a DEV/TEST tool only. It is NOT suitable for use in production environments.** It is intentionally insecure and designed only to simplify testing of applications that integrate with OAuth 2.0 / OpenID Connect identity providers.

## Overview

This repository publishes a Docker Image (`ghcr.io/concilify/stackage-oauth2-fake`) that can be used as a replacement for OAuth 2.0 Identity Providers such as Auth0, Okta, Keycloak, and FusionAuth when testing components that depend on such providers. It has currently only been tested as a replacement for Auth0.

The image implements a minimal but functional OAuth 2.0 and OpenID Connect server, supporting parts of the following specifications:

- **[RFC 6749](https://tools.ietf.org/html/rfc6749)** – The OAuth 2.0 Authorization Framework (Authorization Code Grant, Token Endpoint, Refresh Tokens)
- **[RFC 8628](https://tools.ietf.org/html/rfc8628)** – OAuth 2.0 Device Authorization Grant
- **[RFC 7519](https://tools.ietf.org/html/rfc7519)** – JSON Web Token (JWT)
- **[RFC 7517](https://tools.ietf.org/html/rfc7517)** – JSON Web Key (JWK)
- **[RFC 7523](https://tools.ietf.org/html/rfc7523)** – JWT Profile for OAuth 2.0 Client Authentication and Authorization Grants
- **[OpenID Connect Core 1.0](https://openid.net/specs/openid-connect-core-1_0.html)** – Authentication using the Authorization Code Flow, Claims and Scopes
- **[OpenID Connect Discovery 1.0](https://openid.net/specs/openid-connect-discovery-1_0.html)** – OpenID Provider Configuration (`/.well-known/openid-configuration`)

---

## Consumers

### Prerequisites

- [Docker](https://www.docker.com/) with Docker Compose

### Pulling the Docker Image

The Docker image is published to GitHub Container Registry (GHCR):

```bash
docker pull ghcr.io/concilify/stackage-oauth2-fake:{version}
```

Replace `{version}` with the desired release version (e.g. `1.0`).

### Example Docker Compose File

```yaml
services:
  init-volume:
    # users.json is copied into a named volume rather than bound directly so that the
    # application can update the file at runtime without affecting the file in the repository.
    image: alpine:latest
    command: ["/bin/sh", "-c", "cp /source/users.json /target/users.json"]
    volumes:
      - ./users.json:/source/users.json:ro
      - users-config:/target
    restart: no

  oauth2-fake:
    image: ghcr.io/concilify/stackage-oauth2-fake:{version}
    ports:
      - "32111:32111"
    environment:
      - ASPNETCORE_HTTP_PORTS=32111
      - ISSUER_URL=http://localhost:32111
      - TOKEN_PATH=/oauth2/token
      - AUTHORIZATION_PATH=/oauth2/authorize
      - DEVICE_AUTHORIZATION_PATH=/oauth2/device/authorize
      - DEVICE_VERIFICATION_PATH=/oauth2/device/verify
      - LOGOUT_PATH=/logout
      - DEFAULT_SUBJECT=default-subject
      - DEFAULT_TOKEN_EXPIRY_SECONDS=1200
    volumes:
      - users-config:/app
    depends_on:
      - init-volume

volumes:
  users-config:
```

The `users.json` file defines the set of users and their claims. See [users.json](app/Stackage.OAuth2.Fake/users.json) for an example.

### Endpoints

#### OAuth 2.0 / OpenID Connect Endpoints

| Method | Path (default) | Specification | Description |
|--------|----------------|---------------|-------------|
| `GET` | `/.well-known/openid-configuration` | [OpenID Connect Discovery 1.0 §4](https://openid.net/specs/openid-connect-discovery-1_0.html#ProviderConfig) | Returns OpenID Provider Configuration metadata |
| `GET` | `/.well-known/jwks.json` | [RFC 7517](https://tools.ietf.org/html/rfc7517) | Returns the JSON Web Key Set used to verify token signatures |
| `GET` | `/oauth2/authorize` | [RFC 6749 §4.1](https://tools.ietf.org/html/rfc6749#section-4.1) | Authorization endpoint; returns an authorization code immediately without requiring user login |
| `POST` | `/oauth2/device/authorize` | [RFC 8628 §3.1](https://tools.ietf.org/html/rfc8628#section-3.1) | Device authorization endpoint; returns a device code and user code |
| `POST` | `/oauth2/token` | [RFC 6749 §3.2](https://tools.ietf.org/html/rfc6749#section-3.2) | Token endpoint; exchanges authorization codes, device codes, or refresh tokens for access tokens |
| `GET` | `/logout` | – | Logout endpoint; redirects to the `returnTo` query parameter URL |

#### Internal API Endpoints

The following internal endpoints are provided to assist with test setup and verification. They should not be exposed outside of test environments.

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/.internal/health` | Health check endpoint |
| `POST` | `/.internal/create-token` | Creates a JWT access token directly, bypassing the OAuth 2.0 flow |
| `POST` | `/.internal/user-authorization` | Seeds a user authorization code that can be consumed by the token endpoint |
| `GET` | `/.internal/user-authorization` | Retrieves a seeded user authorization by code |
| `POST` | `/.internal/device-authorization` | Seeds a device authorization that can be consumed by the token endpoint |
| `GET` | `/.internal/device-authorization` | Retrieves a seeded device authorization by device code |
| `POST` | `/.internal/refresh-token` | Seeds a refresh token that can be consumed by the token endpoint |
| `GET` | `/.internal/refresh-token` | Retrieves a seeded refresh token |
| `POST` | `/.internal/users` | Adds a user with claims to the user store |
| `GET` | `/.internal/users` | Retrieves all users, or a single user by subject |
| `GET` | `/.internal/history/requests` | Returns a history of all HTTP requests received by the server |
| `DELETE` | `/.internal/history` | Clears the request history |

### Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `ISSUER_URL` | _(required)_ | The URL of the issuer, included in issued tokens and discovery metadata (e.g. `http://localhost:32111`) |
| `TOKEN_PATH` | `/oauth2/token` | Path of the token endpoint |
| `AUTHORIZATION_PATH` | `/oauth2/authorize` | Path of the authorization endpoint |
| `DEVICE_AUTHORIZATION_PATH` | `/oauth2/device/authorize` | Path of the device authorization endpoint |
| `DEVICE_VERIFICATION_PATH` | `/oauth2/device/verify` | Path of the device verification page (used in device flow response URLs) |
| `LOGOUT_PATH` | `/logout` | Path of the logout endpoint |
| `DEFAULT_SUBJECT` | _(none)_ | The subject used when authenticating via the authorization or device authorization endpoints when no subject has been explicitly specified |
| `DEFAULT_TOKEN_EXPIRY_SECONDS` | `1200` | The default token expiry time in seconds (20 minutes) |

---

## Developers

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/) with Docker Compose

### Building

To build the Docker image locally:

```powershell
docker compose -f docker-compose.app.yaml build
```

### Running Tests

The project has two test suites:

**1. Unit/Integration Tests**

```powershell
dotnet test app/Stackage.OAuth2.Fake.Tests/Stackage.OAuth2.Fake.Tests.csproj
```

**2. Outside-In Tests**

These tests run against the Docker container and require it to be running first.

The simplest way to run everything is with the included `build.ps1` script, which builds the image, starts the container, and runs the outside-in tests:

```powershell
./build.ps1
```

Alternatively, run the steps manually:

```powershell
# Start the application container
docker compose -f docker-compose.app.yaml up --detach --renew-anon-volumes app

# Set required environment variables
$env:STACKAGEOAUTH2FAKETESTS_APP_URL = "http://localhost:32111/"
$env:STACKAGEOAUTH2FAKETESTS_ISSUER_URL = "http://localhost:32111"

# Run the outside-in tests
dotnet test outside-in.tests/Stackage.OAuth2.Fake.OutsideIn.Tests/Stackage.OAuth2.Fake.OutsideIn.Tests.csproj
```

### Verifying Code Formatting

Before submitting a pull request, verify that the code is correctly formatted. `dotnet format` is built into the .NET SDK and requires no additional installation:

```powershell
dotnet format --verify-no-changes
```

### Commit Message Convention

Use the prefix `BREAKING` in the commit message title when a change introduces a breaking API or behaviour change that requires a major version increment. For example:

```
BREAKING: Remove support for legacy grant type
```

### Publishing the Docker Image

Publication is automated via GitHub Actions and is triggered by pushing a version tag to the `main` branch.

**Release build** (triggers [`release.yml`](.github/workflows/release.yml)):

```powershell
git tag v{major}.{minor}
git push origin v{major}.{minor}
```

For example: `git tag v1.2; git push origin v1.2`

This workflow verifies that the tagged commit exists on `main`, runs the outside-in tests, and pushes the image to GHCR as `ghcr.io/concilify/stackage-oauth2-fake:{major}.{minor}`.

**Preview build** (triggers [`release-preview.yml`](.github/workflows/release-preview.yml)):

```powershell
git tag v{major}.{minor}-preview{NNN}
git push origin v{major}.{minor}-preview{NNN}
```

For example: `git tag v1.2-preview001; git push origin v1.2-preview001`

This workflow builds and pushes the image without running tests, useful for publishing a candidate image for early testing.

---

## Agents

This repository includes guidance files for both human contributors and AI coding agents:

| File | Description |
|------|-------------|
| [`AGENTS.md`](AGENTS.md) | Comprehensive guide covering agent responsibilities, technology stack, code style conventions, build/test instructions, testing conventions, and OAuth2 API guidelines. Start here. |
| [`.github/copilot-instructions.md`](.github/copilot-instructions.md) | GitHub Copilot-specific instructions that are automatically included when using Copilot in this repository. |
| [`.github/agents/CSharpExpert.agent.md`](.github/agents/CSharpExpert.agent.md) | A custom agent definition that provides a specialised C#/.NET expert persona, used to assist with development tasks in this repository. |
