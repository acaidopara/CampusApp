# Rise - Gent7

## Team Members

- Maxim Bauwelinck - maxim.bauwelinck@student.hogent.be - MaximBauwelinck1
- Yenthly Devolder - yenthly.devolder@student.hogent.be - Yenthly-Devolder
- Milan Dhondt - milan.dhondt@student.hogent.be - milandhondt
- Britt Emanuel - britt.emanuel@student.hogent.be - BrittEmanuel2001
- Eray Köksoy - eray.koksoy@student.hogent.be - ErayKoksoy
- Tristan Van Speybroeck - tristan.vanspeybroeck@student.hogent.be - Tristanvanspeybroeck

## Technologies & Packages Used

- [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) - Frontend.
- [ASP.NET 9](https://dotnet.microsoft.com/en-us/apps/aspnet) - Backend.
- [Entity Framework 9](https://learn.microsoft.com/en-us/ef/) - Database Access with Unit Of Work and Repository patterns.
- [EntityFrameworkCore Triggered](https://github.com/koenbeuk/EntityFrameworkCore.Triggered) - Database Triggers which are agnostic to the database provider.
- [User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets) - Securely store secrets in DEV.
- [GuardClauses](https://github.com/ardalis/GuardClauses) - Validation Helper.
- [Ardalis.Result](https://github.com/ardalis/Result) - A result abstraction that can be mapped to HTTP response codes if needed.
- [FastEndpoints](https://fast-endpoints.com/) - is a developer friendly alternative to Minimal APIs & MVC.
- [Serilog](https://serilog.net/) - Framework for structured tracable logging to Console and Files.
- [FluentValidation](https://docs.fluentvalidation.net/en/latest/) - is a .NET library for building strongly-typed validation rules.
- [Blazored.FluentValidation](https://docs.fluentvalidation.net/en/latest/) - Blazor + Fluentvalidation.
- [bUnit](https://bunit.dev) - Blazor Component Testing.
- [xUnit](https://xunit.net) - (Unit) Testing.
- [nSubstitute](https://nsubstitute.github.io) - Mocking for testing.
- [Shouldly](https://docs.shouldly.org) - Helper for testing.
- [Destructurama.Attributed](https://github.com/destructurama/attributed) - Masking for sensitive datatypes.

## Software 
1. Install [Rider](https://www.jetbrains.com/rider/) or [Visual Studio](https://visualstudio.microsoft.com/)
2. Make sure you have [ASP.NET 9](https://dotnet.microsoft.com/en-us/download) installed (comes with Rider and Visual Studio) 

## Installation Instructions

1. Clone the repository.

2. Open the `Rise.sln` file in [Rider](https://www.jetbrains.com/rider/), [Visual Studio](https://visualstudio.microsoft.com/) or  [Visual Studio Code](https://code.visualstudio.com/). (we prefer Rider, but you're free to choose.)

3. Make an .env file in root of `Rise.server` with the following contents.

`` DB_CONNECTION=Server={hostName/IP};Database={DBName};User={user};Password={pwd};``

4. Run the project using the `Rise.Server` project as the startup project.

5. The project should open in your default browser on port 5001.

6. The database MySql is used in this project, if you don't have this on your device please follow the instructions below ("Start up with docker").

## Start up with docker

1. Navigate to the docker file ``` cd .\docker\```.

2. Create .env file in `Rise.Server` with the following code:

```DB_CONNECTION="Server=localhost;Port=13306;Database=RiseDb;User=root;Password=root;"```

3. Run de command ```docker compose up -d```.

4. Now run `Rise.Server`.

## Creation of the database

Is done by the app itself using migrations. To add and remove migrations, install the dotnet ef tool globally by running the following command in your terminal (only do this once)

```
dotnet tool install --global dotnet-ef
```

## Migrations

Adapting the database schema can be done using migrations. To create a new migration, run the following command in the `src` folder

```
dotnet ef migrations add YourMigrationName --startup-project Rise.Server --project Rise.Persistence
```

And then update the database using the following command, or run the `Rise.Server`

```
dotnet ef database update --startup-project Rise.Server --project Rise.Persistence
```

## Authentication

Authentication and authorization is present, you'll host and maintain the user accounts in your own database without any external identity provider. You can login with the following test users with the password `A1b2C3!`

### Users

- student1@student.hogent.be
- student2@student.hogent.be
- student3@student.hogent.be
- student4@student.hogent.be

### Roles

- Student
- Teacher
- Administrator
