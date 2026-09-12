# JobTrack

JobTrack is a web application for tracking job applications and preparing for technical interviews.

The application allows users to organize their job search, track interview processes, practice technical topics through tests, and monitor their preparation progress.

## Features

### User

- Registration and login
- JWT-based authentication
- Job application management
- Application statuses:
  - Saved
  - Applied
  - HR Interview
  - Technical Interview
  - Offer
  - Rejected
  - No Response
- Search and filtering of job applications
- Technologies associated with job applications
- Interview tracking
- Technical interview preparation materials
- Tests grouped by technologies and topics
- Automatic calculation of test results
- Preparation progress by technology
- Job readiness based on technologies required by job applications
- Dashboard with statistics and recent activity
- Real-time notifications when a new test is published

### Admin

- Technology management
- Topic and study material management
- Question and answer management
- Test creation and editing
- Publishing and unpublishing tests
- Real-time synchronization of published tests with connected users

## Technologies

### Frontend

- Angular 20
- TypeScript
- SCSS
- Angular Forms
- SignalR client
- Marked for rendering Markdown study materials

### Backend

- ASP.NET Core Web API
- .NET 9
- Entity Framework Core
- PostgreSQL
- JWT authentication
- SignalR

### Testing

- xUnit
- Entity Framework Core InMemory
- Selenium WebDriver
- Page Object Model

## Architecture

JobTrack uses a client-server architecture.

```text
Angular Client
      |
      | HTTP / REST
      v
ASP.NET Core Web API
      |
      | Entity Framework Core
      v
PostgreSQL Database

ASP.NET Core
      |
      | SignalR
      v
Angular Client
```

The Angular application communicates with the ASP.NET Core backend through REST API endpoints.

Entity Framework Core is used for database access and migrations.

SignalR is used for real-time communication. When an administrator publishes or unpublishes a test, connected users receive the change without refreshing the page.

## Project Structure

```text
JobTrack/
│
├── client/
│   └── Angular frontend
│
├── server/
│   ├── JobTrack.Api/
│   │   └── ASP.NET Core Web API
│   │
│   ├── JobTrack.Tests/
│   │   └── Backend tests
│   │
│   ├── JobTrack.UiTests/
│   │   └── Selenium end-to-end tests
│   │
│   └── JobTrack.sln
│
└── README.md
```

## Database

The application uses PostgreSQL.

The database contains entities for:

- Users
- Job applications
- Technologies
- Interviews
- Topics
- Questions
- Answer options
- Tests
- Test attempts
- Test answers

Entity Framework Core migrations are included in the project.

## Running the Project Locally

### Prerequisites

Install:

- .NET 9 SDK
- Node.js and npm
- Angular CLI
- PostgreSQL

### Backend configuration

The backend uses .NET User Secrets for sensitive local configuration.

Navigate to:

```bash
cd server/JobTrack.Api
```

Initialize User Secrets:

```bash
dotnet user-secrets init
```

Set the PostgreSQL connection string:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5433;Database=jobtrack_db;Username=postgres;Password=YOUR_PASSWORD"
```

Set the JWT key:

```bash
dotnet user-secrets set "Jwt:Key" "YOUR_SECRET_JWT_KEY"
```

Apply database migrations:

```bash
dotnet ef database update
```

Run the backend:

```bash
dotnet run
```

By default, the API runs at:

```text
http://localhost:5254
```

Swagger is available at:

```text
http://localhost:5254/swagger
```

### Frontend

Navigate to:

```bash
cd client
```

Install dependencies:

```bash
npm install
```

Run the Angular application:

```bash
ng serve
```

The frontend is available at:

```text
http://localhost:4200
```

## Testing

### Backend Tests

Backend and business logic are tested using xUnit and an Entity Framework Core InMemory database.

Run:

```bash
cd server
dotnet test ./JobTrack.Tests/JobTrack.Tests.csproj
```

The current backend test suite contains 7 tests.

The tests cover scenarios such as:

- Creating job applications
- Invalid technologies
- User ownership of job applications
- Starting published tests
- Preventing unpublished tests from being started
- Preventing completed test attempts from being submitted again

### Selenium Tests

Basic end-to-end user flows are tested using Selenium WebDriver and the Page Object Model.

Before running the Selenium tests, both the backend and frontend must be running.

Set test user credentials in PowerShell:

```powershell
$env:JOBTRACK_TEST_EMAIL="your-test-user@example.com"
$env:JOBTRACK_TEST_PASSWORD="your-test-password"
```

Then run:

```bash
dotnet test ./JobTrack.UiTests/JobTrack.UiTests.csproj
```

The Selenium tests cover:

- Invalid login and error display
- Authenticated interaction with the job applications page

## Security

Passwords are stored as hashes.

Authentication is implemented using JWT tokens.

Sensitive development configuration such as database passwords and JWT keys is not stored in the repository. Local development secrets are managed using .NET User Secrets.

Authorization is role-based and separates User and Admin functionality.

Users can only access and modify their own job applications, interviews, and test results.

## Real-Time Notifications

SignalR is used to provide real-time communication between the backend and connected clients.

When an administrator publishes a test:

1. The backend saves the new publication state.
2. A SignalR event is sent to connected users.
3. The user receives a notification immediately.
4. If the user is already on the Preparation page, the available tests are updated without refreshing the page.

The same mechanism is used when a test is unpublished.

## Deployment

Production deployment will include:

- Angular frontend hosting
- ASP.NET Core API hosting
- PostgreSQL database hosting
- Environment-based configuration for production secrets and URLs

Production links will be added after deployment.

## Author

Jovana Šajkić 95/2022
Prirodno-matematički fakultet, Univerzitet u Kragujevcu  
Institut za matematiku i informatiku
