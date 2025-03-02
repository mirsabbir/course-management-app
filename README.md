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
Technologies: React, Axios (for HTTP requests), Redux (optional, for state management), React Router (for routing).
Responsibilities:
Provide an intuitive interface for students and staff.
Allow staff to manage courses, classes, students, and enrollments.
Allow students to view their enrolled courses and classes and see other students in their classes.
Protect routes and functionality with JWT-based authentication (i.e., show "Login" button if not authenticated and "Welcome {name}, Logout" if authenticated).
#### Backend (Web API - .NET 9)
Technologies: .NET 9, Entity Framework (ORM), ASP.NET Core Web API, JWT Authentication, Structured Logging (using Serilog).
Responsibilities:
Handle API requests (CRUD operations for courses, classes, students, and enrollments).
Secure endpoints using JWT Authentication.
Communicate with the PostgreSQL database to store and retrieve data.
Implement business logic for operations like enrolling a student in a course (which automatically enrolls them in all related classes).
Log structured events to provide traceability for API operations.
#### Database (PostgreSQL)
Technologies: PostgreSQL for storing data in relational tables.
Responsibilities:
Store data related to courses, classes, students, and enrollments.
Support the relationships between tables, such as many-to-many between courses and students, and one-to-many between courses and classes.
Provide referential integrity and ensure data consistency.
![microservice](https://github.com/user-attachments/assets/0c523b63-ac0f-48d1-a82f-24c914492b7b)
