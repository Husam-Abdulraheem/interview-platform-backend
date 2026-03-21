# Interview Platform API 🚀

A robust, enterprise-grade backend service built with **.NET 8 Web API** and **Clean Architecture** principles. The platform empowers users to administer courses, conduct interviews, evaluate answers seamlessly, and utilize AI-driven assistance via the Gemini API.

## ✨ Features

- **Clean Architecture:** Ensures separation of concerns across Domain, Application, Infrastructure, and Presentation layers.
- **RESTful API:** Developed with ASP.NET Core 8 Web API.
- **Entity Framework Core 8:** Type-safe database access with Pomelo MySQL.
- **JWT Authentication & Authorization:** Secure endpoints with industry-standard JSON Web Tokens.
- **Data Validation & Mapping:** Streamlined validation using **FluentValidation** and object mapping with **Mapster**.
- **Global Error Handling:** Consistent and structured error responses using ASP.NET Core Problem Details.
- **AI Evaluation Integration:** Built-in support for evaluating interview responses via the **Google Gemini API**.
- **Comprehensive Testing:** Robust unit testing suite using **xUnit**, **Moq**, and **FluentAssertions**.
- **Interactive API Docs:** Automatic Swagger/OpenAPI generation for seamless modern API discovery and testing.

---

## 🛠️ Technology Stack

| Category | Technologies |
|---|---|
| **Framework** | .NET 8, ASP.NET Core Web API |
| **Language** | C# 12 |
| **Database** | MySQL (Pomelo.EntityFrameworkCore.MySql) |
| **ORM** | Entity Framework Core 8 |
| **Authentication** | JWT (JSON Web Tokens) |
| **Architecture** | Clean Architecture, Repository Pattern, Unit of Work |
| **Utilities** | Mapster, FluentValidation, Swashbuckle (Swagger) |
| **AI Integration** | Google Gemini API (`IAiEvaluationService`) |
| **Testing** | xUnit, Moq, FluentAssertions, Coverlet |

---

## 📂 Project Structure

```bash
InterviewPlatform/
├── src/
│   └── InterviewPlatform/
│       ├── Controllers/      # API Controllers and Presentation logic
│       ├── Application/      # Business logic, Services, Interfaces, DTOs, Mapping
│       ├── Core/             # Domain entities and core abstractions
│       ├── Infrastructure/   # Data Access, DbContext, Services, EF Core Migrations
│       ├── Program.cs        # Application entry point & Dependency Injection configuration
│       └── Dockerfile        # Containerization configuration
└── tests/
    └── InterviewPlatform.Tests/ # xUnit test projects for all application layers
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- MySQL Server instance
- A Google Gemini API Key (for interview evaluations)

### 1. Clone the repository

```bash
git clone <your-repository-url>
cd InterviewPlatform
```

### 2. Configure the Application

Update the connection string and JWT settings in `src/InterviewPlatform/appsettings.json` or `appsettings.Development.json` with your MySQL server credentials and token structure:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=InterviewPlatformDb;User=root;Password=your_password;"
},
"Jwt": {
  "Key": "your_secure_super_secret_jwt_key_here",
  "Issuer": "InterviewPlatform",
  "Audience": "InterviewPlatformUsers"
}
```

### 3. Apply Migrations

Navigate to the source directory and apply the EF Core migrations to create the database schema:

```bash
cd src/InterviewPlatform
dotnet ef database update
```

### 4. Running the Application

You can launch the web API using the .NET CLI:

```bash
cd src/InterviewPlatform
dotnet run
```

Navigate to `https://localhost:<port>/swagger` in your browser or an API Client to access the Swagger UI and test the API endpoints interactively.

### 5. Running Tests

To run the full test suite and ensure all layers are functioning correctly:

```bash
cd tests/InterviewPlatform.Tests
dotnet test
```

---

## 🔒 Authentication

The platform secures endpoints using **JWT Bearer Authentication**. To access protected routes:
1. Retrieve a token via the authentication endpoints (e.g., login or register).
2. In the Swagger UI, click **Authorize** in the top right.
3. Enter `Bearer <your_token>` and click **Authorize**.

---

## 🤝 Contributing

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

*Built with ❤️ for better and smarter interviewing experiences.*
