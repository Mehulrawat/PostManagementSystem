# Post Management System (Beginner Project)

This is a beginner-friendly implementation of the Post Management System according to the BRD (see `/mnt/data/BRD(PostManagement).pdf`).

## Backend

Projects:
- PostManagement.Api (ASP.NET Core Web API, JWT auth)
- PostManagement.Application (DTOs, services, interfaces)
- PostManagement.Domain (entities, enums)
- PostManagement.Infrastructure (EF Core, SQL Server, repositories, security)

### How to run

1. Make sure you have **.NET 8 SDK** and **SQL Server** installed.
2. Update the connection string in `PostManagement.Api/appsettings.json` (`DefaultConnection`).
3. In a terminal:

```bash
cd PostManagement.Api
dotnet restore
dotnet ef migrations add InitialCreate -p ../PostManagement.Infrastructure -s .
dotnet ef database update -p ../PostManagement.Infrastructure -s .
dotnet run
```

4. Open Swagger UI at https://localhost:5001/swagger (or the URL printed in the console).

## Frontend (Angular)

Create Angular app using CLI:

```bash
npm install -g @angular/cli
ng new post-management-ui --routing --style=scss
```

Then create components/services for:
- Login / Register
- Post list, My posts, Post detail
- Comments
- Admin dashboards

Call the backend endpoints from Angular using `HttpClient`.

This solution already includes:
- Auto deactivation of user accounts after 1 year of inactivity (checked at login).
- JWT authentication.
- Basic post and comment management.

## Defaults and roles
- A single superadmin is seeded: `superadmin@system.local` / `superadmin` with password `ChangeMyPa$$word1`.
- Basic Auth is supported for `/auth/basic-login` (use HTTP Basic with any active account) and returns a JWT.
- Admin/SuperAdmin endpoints live under `/api/**` (JWT required). Only SuperAdmin can promote/demote admins; Admins can suspend/delete regular users.
- Password reset endpoint resets to the default `ChangeMyPa$$word1`.
- All `/api/**` endpoints require JWT auth; use `/auth/login` or Basic login to obtain a token.
