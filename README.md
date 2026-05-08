# RentalApp

The purpose of this app is to create a rental app where users can list items (tools, camping gear, board games) for rent, discover items near their location using spatial search, request rentals from item owners, and provide feedback through reviews.
Implemented features are:
- User registration and login
- Local authentication
- Role-based security
- Item listing and browsing
- Rental request management
- Review system
- PostgreSQL database integration
- Entity Framework Core migrations
- Repository and service-based architecture
- Unit testing
- GitHub Actions build and test workflow


This version of the app uses PostgreSQL for data storage and Entity Framework Core for object-relational mapping
and migrations.

To fully understand how it works, you should follow an appropriate set of tutorials such as 
[this one](https://edinburgh-napier.github.io/SET09102/tutorials/csharp/) which covers all of the main
concepts and techniques used here. However, if you want to jump straight in and work out any problems
as you go along, that will also work.
You can use any development environment with this project including

* [Rider](https://www.jetbrains.com/rider/)
* [Visual Studio](https://visualstudio.microsoft.com/)
* [Visual Studio Code](https://code.visualstudio.com/)

The instructions assume you will be using VSCode since that is a lowest-common-denominator choice.

## Compatibility

This app is built using the following tool versions.

| Name | Version |
|---|---|
| .NET | 10.0 Preview / 8.0 or later depending on environment |
| PostgreSQL Docker image | 16 |
| Entity Framework Core | Project dependency version |
| .NET MAUI | Included with .NET workload |


## Getting started

### Prerequisites

Before using this app, ensure you have:

- .NET SDK
- .NET MAUI workload
- Docker Desktop
- PostgreSQL container
- Visual Studio Code, Visual Studio, or Rider
- Git

### Configuration

1. Copy `StarterApp.Database/appsettings.json.template` to `StarterApp.Database/appsettings.json`
2. Update the connection string with your PostgreSQL credentials:
   ```json
   {
     "ConnectionStrings": {
       "DevelopmentConnection": "Host=localhost;Username=student_user;Password=password123;Database=starterapp"
     }
   }
   ```

### Initial Setup

1. Navigate to the Migrations project and create the initial migration:
   ```bash
   cd StarterApp.Migrations
   dotnet ef migrations add InitialCreate
   ```

2. Apply the migration to create the database:
   ```bash
   dotnet ef database update
   ```

3. Build and run the application:
   ```bash
   cd ../StarterApp
   dotnet build
   dotnet run
   ```
### Tests

To run tests naviagate to the terminal then run

```bash
dotnet test StarterApp.Tests/StarterApp.Tests.csproj
```
   

### Tutorial

For a comprehensive guide on using this app and understanding its architecture, see the
[MAUI + MVVM + Database Tutorial](https://edinburgh-napier.github.io/SET09102/tutorials/csharp/maui-mvvm-database/).
