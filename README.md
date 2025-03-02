# Course Management App
This project is a web-based Course Management System for a university. It allows users (staff and students) to manage and interact with courses, classes, and students in various ways. The system enables authenticated staff members to create, read, update, and delete courses, classes, and students. It also allows staff members to enroll students in courses and classes, as well as view student and class information. Students can view their enrolled courses and classes, along with other students in their classes.

## Table of Contents

1. [Architecture](#architecture)
   - 1.1 [Components & Their Responsibilities](#components--their-responsibilities)
   - 1.2 [Architecture Diagram](#architecture-diagram)
2. [API Documentation](#api-documentation)
   - 2.1 [Base URL](#base-url)
   - 2.2 [Authentication](#authentication)
   - 2.3 [Classes APIs](#classes-endpoints)
   - 2.3 [Courses APIs](#courses-endpoints)
   - 2.3 [Students APIs](#students-endpoints)
3. [Installation and Usage](#installation-and-usage)
   - 3.1 [Prerequisites](#prerequisites)
   - 3.2 [Setup](#setup)
4. [How to use the application](#how-to-use-the-application)
   - 4.1 [Prerequisites](#prerequisites-1)
   - 4.2 [Steps to Use the Application](#steps-to-use-the-application)
6. [Database Schema](#database-schema)
   - 5.1 [Course Management Service Schema](#course-management-service-schema)
   - 5.2 [Authorization Service Schema](#authorization-service-schema)
7. [Feature And Requirement Fulfillment](#feature-and-requirement-fulfillment)

## Architecture
### Components & Their Responsibilities
#### Authorization Server (Web API - .NET 9)
- Developed an OAuth 2.0 server using Duende Identity Server and ASP.NET Identity for authentication.
- Followed Clean Architecture, organizing the solution into four projects: API, Domain, Infrastructure, and Application.
- Utilized RSA keys for signing JWTs, securely stored in the `.env` file.
- The User entity supports create, update, and delete operations with an invitation flow (valid for 1 day).
- APIs are protected with scope validation and role-based access control.
- Input validation is handled using Fluent Validation.
- Exception handling middleware ensures accurate status codes and error messages.
- Comprehensive unit and integration tests are in place.
- Implemented structured logging with Serilog for better observability.
- Open API Swagger is used for API documentation.
- [Readme file for Authorization Server](https://github.com/mirsabbir/course-management-app/blob/main/backend/services/Authorization/README.md)
#### Backend (Web API - .NET 9)
- Built with .NET 9, adhering to Clean Architecture with four distinct projects: API, Domain, Infrastructure, and Application.
- Fluent Validation is used for input validation across the system.
- Error handling is managed through exception handling middleware, providing structured error responses.
- Supports pagination for API endpoints.
- A custom attribute is implemented to enforce authentication and authorization checks (scopes and roles).
- Unit and integration tests are written for robust testing.
- Logs are structured using Serilog for clear and consistent monitoring.
- Open API Swagger is utilized for API documentation.
- [Readme file for Course Management API](https://github.com/mirsabbir/course-management-app/blob/main/backend/services/CourseManagement/README.md)
#### Frontend (React UI)
- Built with React, using Axios for HTTP requests and React Router for navigation.
- Implemented Authorization Code Flow to get access token from authorization server.
- Provides an intuitive interface for students and staff.
- Staff can manage courses, classes, students, and enrollments.
- Students can view enrolled courses, classes, and classmates.
#### Database (PostgreSQL)
- Uses PostgreSQL for relational data storage.
- Separate schemas are used for different services.
- Stores data for courses, classes, students, and enrollments.
- Database is not exposed to outside world
### Architecture Diagram
![microservice](https://github.com/user-attachments/assets/0c523b63-ac0f-48d1-a82f-24c914492b7b)
Fig: Microservice Overview ([Draw.io](https://github.com/mirsabbir/course-management-app/blob/main/docs/authorization.drawio))
![authorization](https://github.com/user-attachments/assets/bcb1f345-b2bf-4b7a-9436-6d0cab2c6b06)
Fig: Authorization Code Flow ([Draw.io](https://github.com/mirsabbir/course-management-app/blob/main/docs/microservice.drawio))

## API Documentation

### Base URL
```
http://localhost:5181/
```

### Authentication
This API uses JWT-based authentication. Include the access token in the `Authorization` header:
```
Authorization: Bearer YOUR_ACCESS_TOKEN
```

---

### Classes Endpoints
`GET /api/Classes` - Retrieve all classes (paginated)  
`POST /api/Classes` - Create a new class  
`GET /api/Classes/{classId}` - Retrieve details of a specific class  
`PUT /api/Classes/{classId}` - Update an existing class  
`DELETE /api/Classes/{classId}` - Delete a class  
`GET /api/Classes/{classId}/courses` - Retrieve courses associated with a class  
`GET /api/Classes/{classId}/students` - Retrieve students enrolled in a class  
`POST /api/Classes/{classId}/students` - Enroll a student in a class  
`DELETE /api/Classes/{classId}/students/{studentId}` - Remove a student from a class  
`GET /api/Classes/search` - Search for classes  

---

### Courses Endpoints
`GET /api/Courses` - Retrieve all courses (paginated)  
`POST /api/Courses` - Create a new course  
`GET /api/Courses/{courseId}` - Retrieve details of a specific course  
`PUT /api/Courses/{courseId}` - Update an existing course  
`DELETE /api/Courses/{courseId}` - Delete a course  
`GET /api/Courses/{courseId}/classes` - Retrieve classes associated with a course  
`POST /api/Courses/{courseId}/classes` - Add a class to a course  
`GET /api/Courses/{courseId}/students` - Retrieve students enrolled in a course  
`POST /api/Courses/{courseId}/students` - Enroll a student in a course  
`DELETE /api/Courses/{courseId}/students/{studentId}` - Remove a student from a course  
`GET /api/Courses/search` - Search for courses  
`DELETE /api/Courses/{courseId}/classes/{classId}` - Remove a class from a course  

---

### Students Endpoints
`GET /api/Students` - Retrieve all students (paginated)  
`POST /api/Students` - Create a new student  
`GET /api/Students/{studentId}` - Retrieve details of a specific student  
`PUT /api/Students/{studentId}` - Update an existing student  
`DELETE /api/Students/{studentId}` - Delete a student  
`GET /api/Students/search` - Search for students  
`GET /api/Students/{studentId}/courses` - Retrieve courses a student is enrolled in  
`GET /api/Students/{studentId}/classes` - Retrieve classes a student is enrolled in  
`GET /api/Students/me` - Retrieve details of the currently authenticated student  
`GET /api/Students/{studentId}/classes/{classId}/classmates` - Retrieve classmates for a student in a specific class  

---
## Installation and Usage
### Prerequisites
- Docker installed on your machine
- Docker Compose installed

### Setup
1. Clone the repository:
   ```sh
   git clone https://github.com/mirsabbir/course-management-app.git
   cd course-management-app/docker
   ```
2. Download the `.env` file from email and paste it to this location.
3. Run the application using Docker Compose:
   ```sh
   docker-compose up --build
   ```
4. The API should now be accessible at `http://localhost:3000`

## How to Use the Web Application

### Prerequisites
- Ensure the application is running on your local environment.
- You have received login credentials via email.

### Steps to Use the Application

1. Open your browser and navigate to **[http://localhost:3000](http://localhost:3000)**.
2. On the homepage, you will see a banner and a **Login** button (since you are not logged in).
3. Click the **Login** button. You will be redirected to the **Authorization microservice**.
   - Enter the credentials you received via email.
   - This will log you in as a **Staff** user.
   - After a successful login, you will be redirected back to the **Course Management UI**.
4. In the top-left corner, you will see a **hamburger menu (drawer)**. Click on it to access the following sections:
   - **Courses**
   - **Classes**
   - **Students**
5. To create a new student:
   - Navigate to the **Students** section.
   - Click the **Create Student** button.
6. Fill in the student details. **Note:** The student must be at least **6 years old**.
7. After creation, the student will receive an **email invitation**.
   - Open the email in an **incognito browser** and follow the instructions to set a password.
   - Once the password is set, the student can log in to the application.

---

### Additional Notes
- Ensure that email notifications are received for student account activation.
- If the login fails, double-check your credentials or reset your password using the provided link in the login page.
- For any issues, refer to the application logs or contact support.

## Database Schema
### Course Management Service Schema 
![Untitled](https://github.com/user-attachments/assets/a1caf7e4-c57f-4d70-8f86-1a73f4d32362)
[High Resolution ERD project](https://github.com/mirsabbir/course-management-app/blob/main/docs/course-management-erd.pgerd)
### Authorization Service Schema
![authorization-erd pgerd](https://github.com/user-attachments/assets/ab70e47d-9b14-4100-a169-c5c1a46d7bc1)
[High Resolution ERD project](https://github.com/mirsabbir/course-management-app/blob/main/docs/authorization-erd.pgerd)

## Feature And Requirement Fulfillment
The objective of this web application has been successfully met with the following features implemented:

### Authentication & Authorization
- Users can **log in** to the system and receive an **authenticated token**.
- All subsequent actions require authentication using the provided token.
- ![login](https://github.com/user-attachments/assets/22e9fdd5-dfb6-4d3a-8c75-c613b0bb1626)


### Staff Functionalities
- **Create:** Staff members can create **courses, classes, and students**.
- **Read:** Staff members can view **courses, classes, and students**.
- **Update:** Staff members can update **courses, classes, and students**.
- **Delete:** Staff members can delete **courses, classes, and students**.
- **Enrollment Management:**
  - Staff members can **enroll students in classes**.
  - Staff members can **enroll students in a course**, which automatically enrolls them in all associated classes.
- **View & Tracking:**
  - Staff members can view **all students enrolled in a given course**.
  - Staff members can view **all students in a given class**.
  - Staff members can view **all classes under a specific course**.
  - Staff members can view **all courses associated with a class**.
  - Staff members can track **who assigned a student to a class and when**.
  - ![ManageCourses](https://github.com/user-attachments/assets/95d2a58d-10f5-46be-871c-1d66647feaf3)
  - ![ManageClasses](https://github.com/user-attachments/assets/0ccb77a3-1d86-4e38-91f6-d91d960d0fa9)
  - ![manage students](https://github.com/user-attachments/assets/6fa03e87-1805-44ca-9384-19e5cb3f6f2b)

### Student Functionalities
- **View Enrollments:** Students can view their **enrolled classes and courses**.
- **Classmate Visibility:** Students can see the names of **other students in their classes**.
- ![viewcourses](https://github.com/user-attachments/assets/0d71c5b6-0ade-4f56-b5e7-b1e9887911e2)
- ![classes](https://github.com/user-attachments/assets/67eeb7f7-f5d9-4a16-b7f3-2ad598040d38)

### Conclusion
All the specified requirements have been implemented successfully, ensuring a fully functional and robust course management system.


