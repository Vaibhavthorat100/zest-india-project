# 🎓 Student Management System - ASP.NET Core Web API & React UI

[![ASP.NET Core 8.0](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=for-the-badge&logo=.net)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/Language-C%23%2012-239120?style=for-the-badge&logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Entity Framework Core](https://img.shields.io/badge/EF%20Core-8.0.2-68217A?style=for-the-badge)](https://docs.microsoft.com/en-us/ef/core/)
[![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-CC292B?style=for-the-badge&logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)
[![JWT Authentication](https://img.shields.io/badge/Security-JWT%20Bearer-000000?style=for-the-badge&logo=json-web-tokens)](https://jwt.io/)
[![Serilog](https://img.shields.io/badge/Logging-Serilog-008080?style=for-the-badge)](https://serilog.net/)
[![Swagger](https://img.shields.io/badge/API%20Docs-Swagger-85EA2D?style=for-the-badge&logo=swagger)](https://swagger.io/)
[![React UI](https://img.shields.io/badge/Frontend-React%2018%20%2B%20Vite-61DAFB?style=for-the-badge&logo=react)](https://react.dev/)
[![Docker](https://img.shields.io/badge/Container-Docker-2496ED?style=for-the-badge&logo=docker)](https://www.docker.com/)

---

## 📌 Project Overview

The **Student Management System** is a full-stack, enterprise-grade application built with **ASP.NET Core 8.0 Web API** and a modern **React 18 UI Dashboard**. 

The solution follows a strict **Clean Layered Architecture** (`Controllers` ➔ `Services` ➔ `UnitOfWork` ➔ `Repositories` ➔ `DbContext`), implementing **JWT Bearer Token Security**, **PBKDF2 Cryptographic Salted Password Hashing**, **Entity Framework Core SQL Server Data Access**, **Global Exception Middleware**, **Serilog Logging**, **xUnit Unit Testing**, and **Docker Containerization**.

---

## 🏗️ Architecture & Directory Structure

```
zest-india-project/
├── StudentManagementAPI/               # Main ASP.NET Core 8.0 Web API Project
│   ├── Controllers/                    # REST API Controllers
│   │   ├── AuthController.cs            # JWT Authentication (Register & Login)
│   │   └── StudentsController.cs        # Protected Student CRUD Endpoints
│   ├── Services/                       # Business Logic Layer
│   │   ├── IStudentService.cs & StudentService.cs
│   │   └── IAuthService.cs & AuthService.cs
│   ├── Repositories/                   # Data Access Layer & Unit of Work Pattern
│   │   ├── IStudentRepository.cs & StudentRepository.cs
│   │   └── IUnitOfWork.cs & UnitOfWork.cs
│   ├── Data/                           # EF Core DbContext & Fluent API Configurations
│   │   ├── ApplicationDbContext.cs
│   │   ├── DbInitializer.cs            # Automatic Seed Data
│   │   └── Configurations/
│   │       └── EntityConfigurations.cs
│   ├── DTOs/                           # Data Transfer Objects & Validation
│   │   ├── StudentDtos.cs
│   │   ├── AuthDtos.cs
│   │   └── ApiResponse.cs              # Standardized API Response Envelope
│   ├── Models/                         # Database Entities
│   │   ├── Student.cs
│   │   └── ApplicationUser.cs
│   ├── Middleware/                     # Centralized Error & Exception Handling
│   │   └── GlobalExceptionMiddleware.cs
│   ├── Common/                         # Security & Domain Exception Utilities
│   │   ├── Security/PasswordHasher.cs   # PBKDF2 Salted Hashing
│   │   └── Exceptions/CustomExceptions.cs
│   ├── Program.cs                      # Application Entrypoint & DI Setup
│   ├── appsettings.json                # Connection Strings & JWT Settings
│   └── Dockerfile                      # Multi-Stage Build File
├── StudentManagementAPI.Tests/         # xUnit Unit Testing Suite
│   └── StudentServiceTests.cs          # Mocked Unit Tests (Moq)
├── frontend/                           # React 18 + Vite Interactive UI
│   ├── src/
│   │   ├── App.jsx                     # Dashboard UI with Dual Demo/Live Mode
│   │   ├── index.css                   # Glassmorphism Design System
│   │   └── main.jsx
│   └── package.json
├── docker-compose.yml                  # Orchestrates SQL Server 2022 + Web API
└── README.md                           # Documentation & Deployment Guide
```

---

## 🌟 Technical Features & Implementation Highlights

1. **Clean Layered Architecture**:
   - Clear separation of concerns with Interfaces for Dependency Injection.
   - **Unit of Work Pattern** (`IUnitOfWork`) for atomic database transactions.

2. **Security & Authentication**:
   - **JWT Bearer Token** authorization (`[Authorize]` guard on API controllers).
   - OWASP Recommended **PBKDF2 Password Hashing** (HMACSHA512 + 128-bit Salt, 100,000 Iterations).

3. **Database Management**:
   - Entity Framework Core Code-First mapped to **Microsoft SQL Server**.
   - `Student` Entity Schema: `Id`, `Name`, `Email` (Unique), `Age`, `Course`, `CreatedDate`.
   - Automatic Database Initialization & Data Seeding on startup.

4. **Cross-Cutting Concerns**:
   - **Global Exception Middleware**: Intercepts unhandled exceptions (`NotFoundException`, `ConflictException`, `UnauthorizedException`) and formats them into clean HTTP JSON responses.
   - **Serilog Logging**: Console output & daily rolling file sink (`Logs/app_.log`).
   - **Swagger OpenAPI**: Interactive API docs with `Authorize` Bearer token scheme.
   - **Health Checks**: `/health` endpoint for DB & API monitoring.

5. **Bonus Offerings**:
   - **Unit Tests**: Full xUnit test suite using `Moq` mocking framework.
   - **Docker Support**: Containerized ASP.NET Core API + SQL Server 2022 setup.
   - **React UI Dashboard**: Full CRUD interface with search, live filters, modal forms, and live API mode.

---

## 🛠️ Step-by-Step How-To-Run Guide

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or [Visual Studio 2022](https://visualstudio.microsoft.com/)
- [Microsoft SQL Server](https://www.microsoft.com/sql-server) (LocalDB, SQL Express, or Docker SQL Server)
- [Node.js v18+](https://nodejs.org/) (optional, for React UI)
- [Docker Desktop](https://www.docker.com/) (optional)

---

### 🚀 Option 1: Run Backend Web API (`dotnet CLI`)

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/Vaibhavthorat100/zest-india-project.git
   cd zest-india-project
   ```

2. **Configure Connection String (Optional)**:
   In `StudentManagementAPI/appsettings.json`, set your SQL Server connection string if needed:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ZestStudentDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
   }
   ```

3. **Run Database Migrations / Launch API**:
   ```bash
   cd StudentManagementAPI
   dotnet run
   ```

4. **Access Swagger Documentation**:
   Open your browser at:
   👉 **`http://localhost:5000`** or **`https://localhost:5001`**

---

### 💻 Option 2: Run React Frontend UI Dashboard

1. **Open a Terminal & Navigate to Frontend**:
   ```bash
   cd frontend
   npm install
   npm run dev
   ```

2. **Open Dashboard in Browser**:
   👉 **`http://localhost:3000`**

---

### 🐳 Option 3: Run with 1-Click Docker Compose

To start both **SQL Server 2022 Container** and the **ASP.NET Core Web API Container** together:

```bash
docker-compose up --build
```

Access Swagger UI at `http://localhost:5000`.

---

### 🧪 Option 4: Run Automated Unit Tests

To run the xUnit test suite:

```bash
dotnet test StudentManagementAPI.Tests/StudentManagementAPI.Tests.csproj
```

---

## 🔑 Default Seeded Credentials

The application automatically seeds an initial Administrator account upon first run:

| Credential Field | Value |
|---|---|
| **Username** | `admin` |
| **Email** | `admin@zestindia.com` |
| **Password** | `Admin@123` |

### How to Test JWT Auth in Swagger:
1. Go to `http://localhost:5000` (Swagger UI).
2. Expand `POST /api/auth/login` and click **Try it out**.
3. Submit the JSON payload with `UsernameOrEmail: "admin"` and `Password: "Admin@123"`.
4. Copy the returned `token` string.
5. Click the green **Authorize 🔓** button at the top right of Swagger.
6. Enter `Bearer YOUR_TOKEN_HERE` and click **Authorize**.
7. Now test any protected `/api/students` endpoint!

---

## 📡 REST API Endpoint Documentation

### Authentication Endpoints (`/api/auth`)

| Method | Endpoint | Description | Auth Required |
|---|---|---|---|
| `POST` | `/api/auth/register` | Register a new user/admin account | ❌ No |
| `POST` | `/api/auth/login` | Authenticate credentials & return JWT | ❌ No |

### Student Management Endpoints (`/api/students`)

| Method | Endpoint | Description | Auth Required |
|---|---|---|---|
| `GET` | `/api/students` | Get list of all students | ✅ Yes (JWT) |
| `GET` | `/api/students/{id}` | Get student details by ID | ✅ Yes (JWT) |
| `POST` | `/api/students` | Add a new student record | ✅ Yes (JWT) |
| `PUT` | `/api/students/{id}` | Update student details by ID | ✅ Yes (JWT) |
| `DELETE` | `/api/students/{id}` | Delete student record by ID | ✅ Yes (JWT) |

### Sample Request Payload for `POST /api/students`:
```json
{
  "name": "Rahul Sharma",
  "email": "rahul.sharma@example.com",
  "age": 22,
  "course": "Computer Science"
}
```

---

## 📄 License & Contact

This project is submitted for technical evaluation.
- **Language & Framework**: C#, .NET 8.0, React 18
- **Database**: Microsoft SQL Server & EF Core
- **Repository**: [GitHub Repository Link](https://github.com/Vaibhavthorat100/zest-india-project)
