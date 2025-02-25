# Authorization Server

This is the Authorization Server for the Course Management application, built using **Duende Identity Server** with **ASP.NET Identity** and **.NET 9**. It serves as the OAuth 2.0 and OpenID Connect provider, issuing JWT access tokens for secure authentication and authorization.

## Features
- OAuth 2.0 and OpenID Connect authentication.
- User management with **ASP.NET Identity**.
- JWT-based access tokens.
- Supports **Authorization Code Flow with PKCE**.
- Integration with **PostgreSQL** for identity data storage.
- Structured logging.
- Unit tests to ensure system reliability.
- **Invitation flow and email sending implementation**.
- **Request validation using FluentValidation**.

## Technologies Used
- **.NET 9**
- **Duende Identity Server**
- **Entity Framework Core**
- **PostgreSQL**
- **Docker & Docker Compose**
- **Serilog** (for structured logging)
- **FluentValidation** (for request validation)
- **SMTP or Email Service** (for sending invitations)

## Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Docker & Docker Compose](https://www.docker.com/)
- [PostgreSQL](https://www.postgresql.org/)

### Default Endpoints
- **Identity Server Discovery Document:** `http://localhost:5161/.well-known/openid-configuration`
- **Authorize Endpoint:** `http://localhost:5161/connect/authorize`
- **Token Endpoint:** `http://localhost:5161/connect/token`
- **User Info Endpoint:** `http://localhost:5161/connect/userinfo`

## Security Best Practices
- **Use HTTPS in production.**
- **Secure database credentials.**
- **Rotate signing keys periodically.**
- **Restrict CORS and allowed redirect URIs.**

## Logs & Monitoring
Structured logs are captured using **Serilog**, and important security events are logged. Ensure proper monitoring is set up in production.

