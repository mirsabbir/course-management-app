# Course Management App
This project is a web-based Course Management System for a university. It allows users (staff and students) to manage and interact with courses, classes, and students in various ways. The system enables authenticated staff members to create, read, update, and delete courses, classes, and students. It also allows staff members to enroll students in courses and classes, as well as view student and class information. Students can view their enrolled courses and classes, along with other students in their classes.

## Table of Contents

2. [Architecture](#architecture)
   - 2.1 [Staff Features](#staff-features)
   - 2.2 [Student Features](#student-features)
3. [Design Goals](#design-goals)
4. [Requirements](#requirements)
   - 4.1 [Dockerized Environment](#dockerized-environment)
   - 4.2 [Tech Stack](#tech-stack)
5. [Installation and Usage](#installation-and-usage)
   - 5.1 [Prerequisites](#prerequisites)
   - 5.2 [Setup](#setup)
6. [API Documentation](#api-documentation)
   - 6.1 [Authentication](#authentication)
   - 6.2 [Staff Endpoint Example](#staff-endpoint-example)
7. [Database Schema](#database-schema)
8. [Testing](#testing)
9. [Submission Instructions](#submission-instructions)
10. [License](#license)

## Architecture
### Components & Their Responsibilities
#### Frontend (React UI)
Technologies: React, Axios (for HTTP requests), React Router (for routing).
Responsibilities:
Provide an intuitive interface for students and staff.
Allow staff to manage courses, classes, students, and enrollments.
Allow students to view their enrolled courses and classes and see other students in their classes.
Protect routes and functionality with JWT-based authentication (i.e., show "Login" button if not authenticated and "Welcome {name}, Logout" if authenticated).
#### Backend (Web API - .NET 9)
- Built with .NET 9, adhering to Clean Architecture with four distinct projects: API, Domain, Infrastructure, and Application.
- Fluent Validation is used for input validation across the system.
- Error handling is managed through exception handling middleware, providing structured error responses.
- Supports pagination for API endpoints.
- A custom attribute is implemented to enforce authentication and authorization checks (scopes and roles).
- Unit and integration tests are written for robust testing.
- Logs are structured using Serilog for clear and consistent monitoring.
- Open API Swagger is utilized for API documentation.
#### Authorization Server (Web API - .NET 9)
- Developed an OAuth server using Duende Identity Server and ASP.NET Identity for authentication.
- Followed Clean Architecture, organizing the solution into four projects: API, Domain, Infrastructure, and Application.
- Utilized RSA keys for signing JWTs, securely stored in the `.env` file.
- The User entity supports create, update, and delete operations with an invitation flow (valid for 1 day).
- APIs are protected with scope validation and role-based access control.
- Input validation is handled using Fluent Validation.
- Exception handling middleware ensures accurate status codes and error messages.
- Comprehensive unit and integration tests are in place.
- Implemented structured logging with Serilog for better observability.
- Open API Swagger is used for API documentation.
#### Database (PostgreSQL)
Technologies: PostgreSQL for storing data in relational tables.
Responsibilities:
Store data related to courses, classes, students, and enrollments.
Support the relationships between tables, such as many-to-many between courses and students, and one-to-many between courses and classes.
Provide referential integrity and ensure data consistency.
![microservice](https://github.com/user-attachments/assets/0c523b63-ac0f-48d1-a82f-24c914492b7b)
