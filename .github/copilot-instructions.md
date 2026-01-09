# GitHub Copilot Instructions

This repository implements a fake OAuth 2.0 Identity Provider for testing purposes using C# and .NET.

## Getting Started

Please read the [AGENTS.md](../AGENTS.md) file in the repository root for comprehensive guidelines on:

- Development responsibilities and workflow
- Technology stack and code style conventions
- Building and testing instructions
- Testing conventions and value naming
- OAuth2 API specifications and guidelines

## Quick Reference

### Technology Stack
- **Language**: C# with .NET 10.0
- **Framework**: ASP.NET Core Minimal APIs
- **Testing**: NUnit with Shouldly
- **Containerization**: Docker

### Build & Test
```bash
# Build Docker image
docker compose -f docker-compose.app.yaml build

# Run unit/integration tests
dotnet test app/Stackage.OAuth2.Fake.Tests/Stackage.OAuth2.Fake.Tests.csproj

# Verify formatting
dotnet format --verify-no-changes
```

### Code Style Highlights
- **Indentation**: 3 spaces for C# files
- **Private fields**: `_camelCase` prefix
- **Test methods**: `snake_case` naming
- **Max line length**: 160 characters

### Key Conventions
- Follow StyleCop analyzers and EditorConfig rules
- Use `Arbitrary` prefix for correlated test values (e.g., `"ArbitraryClientId"`)
- Implement `IGrantTypeHandler` interface for OAuth2 grant types
- Organize endpoints in the `Endpoints` folder
- Place models in `Model` folder with authorization models in `Model/Authorization`

### Custom Agents
This repository includes a custom C# Expert agent at `.github/agents/CSharpExpert.agent.md`. Prefer using this agent for C#/.NET specific tasks as it has specialized knowledge of .NET conventions and best practices.

## Important Notes
- This is a **test/development tool only** - not for production use
- Implements OAuth 2.0 (RFC 6749, RFC 8628) and OpenID Connect specifications
- All changes should include appropriate tests
- Ensure all tests pass before submitting changes
