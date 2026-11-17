# StationeryHub — Web Technologies Deliverable 2

A **D2-only** ASP.NET Core MVC (.NET 10) conversion of the original D1 HTML + Bootstrap 5 pages.

## D2 requirements

- MVC architecture: controllers, models/ViewModels, Razor views, shared layout and Bootstrap 5.
- Identity login, registration and logout using scaffolded Razor Pages (`Areas/Identity/Pages/Account`).
- **Dynamic pages**: MVC controllers pass objects to Razor Views; product search/category filtering, Admin dashboard metrics/tables, and Customer dashboard are rendered using Razor model data and loops.
- Role protection: Customer dashboard requires a Customer/Admin account. Admin dashboard requires the Admin role.
- Original D1 design and page content are preserved in the converted pages as closely as possible.

## What's deliberately not in D2

Product/order/cart SQL tables, shop EF Core migrations, database-backed product CRUD, stock editing, persistent cart and checkout. These belong to **Deliverable 3**.

The D2 product catalog and order tables are **sample in-memory ViewModel data**, clearly marked as demo. Product search and category filtering are functional but do not alter a database. Login/Register **are real** and store Identity users with hashed passwords in SQL Server LocalDB.

## Setup (Windows)

Prerequisites:
- .NET 10 SDK (used by the original MVC project)
- SQL Server Express LocalDB (`(localdb)\MSSQLLocalDB`) installed, usually through Visual Studio

Open a terminal in this folder (the folder containing `Project.slnx`) and run:

```powershell
 dotnet restore .\Project\Project.csproj
 dotnet build .\Project\Project.csproj
 dotnet run --project .\Project\Project.csproj
```

Then open the `localhost` URL printed by `dotnet run` (e.g. `http://localhost:5111`).

**Identity database:** This D2 copy uses a separate LocalDB database called `StationeryHubD2Db`. On first start, `EnsureCreatedAsync()` creates the required **Identity** tables without an EF migration. This deliberately avoids modifying the original Codex/D3 database `StationeryHubDb`. Do not point the D2 copy to a D3 database. `EnsureCreated` is intended only for this disposable D2 database; when starting D3, use a fresh migrations-based workflow.

No admin credentials are committed to Git. To create an admin account, set environment variables *in the same PowerShell session before first launch*:

```powershell
$env:SeedAdmin__Email = 'admin@stationeryhub.local'
$env:SeedAdmin__Password = 'UseYourOwnStrongPassword1!'
dotnet run --project .\Project\Project.csproj
```

After the administrator is created, stop the app, clear credentials from the shell and restart if needed:

```powershell
Remove-Item Env:SeedAdmin__Email
Remove-Item Env:SeedAdmin__Password
```

A regular user can register at `/Identity/Account/Register`; all public registrations are assigned the Customer role.

## Paths and demonstration

- `/` or `/Home/Index` — dynamic D1 home page, product search/filter via MVC
- `/Home/Privacy` — privacy page
- `/Home/Login` — redirects to scaffolded Identity login
- `/Home/Signup` — redirects to scaffolded Identity register
- `/Identity/Account/Login` — working login, Razor Pages
- `/Identity/Account/Register` — working registration, Razor Pages
- `/Home/Customer` — role-protected personalized welcome and **sample** order history
- `/Admin` — Admin-role-only **sample** dashboard

## File map

- `Project/Controllers/` — MVC endpoints
- `Project/Models/ShopModels.cs` — Identity user and view models
- `Project/Services/DemoStore.cs` — in-memory demo data for dynamic MVC rendering
- `Project/Views/` — MVC Razor views based on D1 pages
- `Project/Areas/Identity/Pages/Account/` — Identity Login/Register/Logout Razor pages
- `Project/Data/ApplicationDbContext.cs` — Identity-only EF Core context
- `Project/Data/IdentityDatabaseSetup.cs` — Identity tables/roles setup, **no migrations**
- `Project/wwwroot/` — Bootstrap / CSS / JavaScript assets

## Notes on the original project

The original user-provided `Project.zip` is **not altered**. This is a separate D2 snapshot to copy into the Git repository's `Project/` folder. Previous D1 commits remain in Git history after replacing the working files with D2 content.

## Verification checklist

1. Restore/build with no errors.
2. Home and Privacy load, search and category filtering update products.
3. Registration creates a Customer account; login/logout work.
4. Non-logged-in users are redirected to login for `/Home/Customer` and `/Admin`.
5. Logged-in Customers cannot open Admin dashboard (Access Denied).
6. Optional Admin account can view both Admin and Customer dashboards.

Build/runtime verification must be performed on a Windows machine with the .NET 10 SDK and SQL Server LocalDB; this ZIP is a source-only deliverable.
