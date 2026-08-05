# Verification

CI proves .NET restore, formatting, Release compilation, four deterministic tests, NuGet audit, Compose model validity, and a clean web build. The project named `Integration.Tests` currently contains only an environment-safety scaffold; it does not exercise SQL Server, Redis, RabbitMQ, BFF authentication, or cross-tenant requests. Do not present those integrations as verified until live tests exist.
