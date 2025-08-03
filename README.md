# TU Routing API – Backend Service

This repository provides backend services for the TU Berlin routing system, developed as part of the PP3S project. The service exposes RESTful endpoints for route calculations (walking, scooter, hybrid), delivers location data, and supports database integration, containerization via Docker, and modular service layers.

## Tech Stack

- **Language**: C# (.NET 6/7)
- **Framework**: ASP.NET Core Web API
- **Database**: PostgreSQL (optional, used for route and location caching)
- **Containerization**: Docker, Docker Compose
- **Project Format**: Solution-based (`api.sln`)

## Project Structure

```
.
├── Contracts/         # Data transfer objects (DTOs)
├── Controllers/       # API route handlers
├── Migrations/        # EF Core migrations
├── Models/            # Domain models / entities
├── Properties/        # Project metadata
├── Services/          # Core application logic
├── Program.cs         # Application entry point
├── Dockerfile         # Docker build instructions
├── docker-compose.yml # Service orchestration with DB
├── appsettings.json   # Main configuration file
├── appsettings.Development.json # Local dev config
├── api.sln            # Solution file
├── api.csproj         # Project definition

```

## Setup & Installation

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker](https://www.docker.com/)
- PostgreSQL (handled automatically via Docker Compose)

### Local Development (optional)

```bash
dotnet build
dotnet run
````

### Run with Docker Compose (recommended)

```bash
sudo docker compose up -d --build
```

> Backend will be available at: [http://localhost:5000](http://localhost:5000)
> Swagger UI: [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html)

## Configuration

Configuration is handled via `appsettings.json` or environment variables.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=tu_routing;Username=postgres;Password=your_password"
  }
}
```

For dev settings, edit `appsettings.Development.json`.

If using `.env`, provide:

```env
DB_HOST=localhost
DB_PORT=5432
DB_NAME=tu_routing
DB_USER=postgres
DB_PASSWORD=your_password
```

## API Endpoints

| Method | Route                     | Description                                    |
|--------|---------------------------|------------------------------------------------|
| GET    | `/all-pointers`           | Returns all campus points of interest (POIs)  |
| POST   | `/hybrid-several-points`  | Calculates a combined **walking + public transport** route across multiple waypoints |
| POST   | `/scooter-route`          | Calculates a route optimized for scooter usage |
| POST   | `/walking`                | Calculates a walking-only route between two points |

> `hybrid-several-points` combines walking and public transport (BVG/VBB) for optimal routing across multiple destinations.

Interactive API documentation is available via Swagger:  
[http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html)

## Contributors

This backend was developed by Group D as part of the **TU Berlin PP3S project** in Summer Semester 2025.
