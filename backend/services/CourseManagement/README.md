# Course Management API

This is the **Course Management API**, a microservice responsible for handling course-related operations in the application. It is built using **.NET 9** and follows the **Clean Architecture** principles to ensure maintainability and scalability.

## Features
- CRUD operations for courses.
- JWT authentication and authorization via **Duende Identity Server**.
- Integration with **PostgreSQL** for data storage.
- Structured logging with **Serilog**.
- Unit tests to ensure system reliability.
- Request validation using **FluentValidation**.

## Technologies Used
- **.NET 9**
- **Entity Framework Core**
- **PostgreSQL**
- **Duende Identity Server** (for authentication & authorization)
- **Docker & Docker Compose**
- **Serilog** (for logging)
- **FluentValidation** (for request validation)

## Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Docker & Docker Compose](https://www.docker.com/)
- [PostgreSQL](https://www.postgresql.org/)

## API Endpoints
### Course Endpoints
- **Get all courses**: `GET /api/courses`
- **Get course by ID**: `GET /api/courses/{id}`
- **Create a course**: `POST /api/courses`
- **Update a course**: `PUT /api/courses/{id}`
- **Delete a course**: `DELETE /api/courses/{id}`

### Authentication
This API requires authentication via JWT tokens issued by the Authorization Server.
- **Authorization Header Format:**
  ```sh
  Authorization: Bearer {token}
  ```

## Security Best Practices
- **Use HTTPS in production.**
- **Secure database credentials.**
- **Restrict CORS and allowed origins.**

## Logs & Monitoring
Structured logs are captured using **Serilog**, and key events are logged for monitoring and debugging.

