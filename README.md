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

## Usefull Commands

In the `src/Rise.Server` folder

`dotnet watch --non-interactive` 

The `dotnet watch` command is a file watcher. When it detects a change, it runs the `dotnet run` command or a specified `dotnet` command. If it runs `dotnet run`, and the change is supported for [hot reload](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-watch#hot-reload), it hot reloads the specified application. If the change isn't supported, it restarts the application. This process enables fast iterative development from the command line.

`dotnet run`

The `dotnet run` command provides a convenient option to run your application from the source code with one command. It's useful for fast iterative development from the command line. The command depends on the [`dotnet build`](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-build) command to build the code. Any requirements for the build apply to `dotnet run` as wel

`dotnet clean `- you won't need this often

The `dotnet clean` command cleans the output of the previous build. It's implemented as an [MSBuild target](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-targets), so the project is evaluated when the command is run. Only the outputs created during the build are cleaned. Both intermediate (*obj*) and final output (*bin*) folders are cleaned.

## Authentication

Authentication and authorization is present, you'll host and maintain the user accounts in your own database without any external identity provider. You can login with the following test users with the password `A1b2C3!`

### Users

- student1@student.hogent.be
- student2@student.hogent.be
- student3@student.hogent.be
- student4@student.hogent.be

### Roles

There are 3 built-in roles, but adjust as needed

- Student
- Teacher
- Administrator

### Use cases

- Register
- Login
- Logout
- GetInfo

## Solution Structure Overview

The template is designed as a boilerplate or template for .NET solutions, following best practices for structuring projects, separation of concerns, and maintainability. Here's a breakdown of the solution structure and its workings, explained:

When you open the solution, you’ll notice it’s organized into multiple projects, which is a common approach in large, enterprise-level applications. Each project within the solution has a specific responsibility. This approach is based on the **Clean Architecture**, **Domain-Driven Design (DDD)** and **Vertical Slicing**  principles. The goal is to keep different aspects of the application separated and independent, making it easier to scale, maintain, and test.

Here are the main projects in the solution:

1. **Domain**
2. **Services**
3. **Persistence**
4. **Server**
5. **Client**
6. **Shared**

Let’s look at each of these in more detail:

------

### 1. **Domain Project**

**Folder**: `Domain`

**Purpose**: The **Domain** project holds the core logic of the application. It defines the business rules, which are independent of the UI, database, or any external technology. The principle here is to keep the domain logic isolated, making sure it’s not affected by external frameworks or infrastructure.

**Typical Contents**:

- **Entities**: Classes that represent the core objects of the application, such as `Order`, `Customer`, or `Product`.

- **Value Objects**: Immutable objects that represent a concept (like `Money` or `Address`).

  > Currently not provided in the template, but you can read more here: [Domain Driven Design - Best Practises](https://hogent-web.github.io/csharp/chapters/03/slides/index.html#75)

**Why this separation?**: Keeping the domain logic separate ensures that the business rules remain consistent even if the application’s presentation or infrastructure changes. This allows for flexibility and ensures that changes to other parts of the system don't break the business logic.

------

### 2. **Services Project**

**Folder**: `Services`

**Purpose**: The **Services** project is responsible for the application-specific logic, such as orchestrating use cases, handling commands, and queries, and processing workflows. It acts as an intermediary between the **Domain** and the **Infrastructure** or **API** layers.

**Typical Contents**:

- **Use Cases**: These classes are responsible for specific actions in the system, like creating an order or processing a payment.

**Why this separation?**: This project enforces the **Separation of Concerns (SoC)**. It also makes testing easier, as this layer can be unit tested without worrying about external dependencies. 

> Note that we can swap out the API for something else, for example a console application and the business rules will still apply.
>
> We do not recommend abstracting your database as we see it as a migration to another database provider, not an abstraction. Read more about it [Should you Abstract the Database ](https://enterprisecraftsmanship.com/posts/should-you-abstract-database/). However you will have to switch to a real database provider not SQLite.

------

### 3. **Persistence Project**

**Folder**: `Persistence`

**Purpose**: The **Persistence** project deals with Database mappings and database migrations, that's it.

**Typical Contents**:

- **Configurations**: Entity configurations, for example how a product is mapped to a table in SQL, using Entity Framework Core.
- **Data Migrations**: Scripts or classes for evolving the database schema over time.
- **Triggers**: Stuff that needs to happen when something is saved or retrieved from the database. It's rather optional but these triggers are database agnostic (they will work for any provider e.g. MariaDb, Microsoft SQL Server,... )

**Why this separation?**: So it's easier to find the configurations and keep them out of the **Domain** logic, Domain classes should **not** know how they're stored.

------

### 4. **Server Project**

**Folder**: `Server`

**Purpose**: The **API** project is the entry point for the application, where the HTTP endpoints are defined. It handles requests from clients (via RESTful HTTP requests) and returns responses. It uses **FastEndspoints** to expose application functionality to the outside world.

**Typical Contents**:

- **Endpoints**: These handle HTTP requests and responses. They receive requests, pass them to the appropriate application service, and return the result.
- **Processors**: Custom components that handle cross-cutting concerns such as logging, or error handling.
- **Dependency Injection Configuration**: The **Server** project contains the setup for the dependency injection container, where the various services and other dependencies are registered.
- **Serving the Blazor Client** : If no endspoints are found, the **Server** returns the Blazor WebAssembly (WASM) **Client**, it's rather optional but it makes hosting a lot easier (No CORS issues etc.)

**Centralized Response Handling**:

You might notice something interesting about how endpoints send responses. In `Program.cs`, the FastEndpoints configuration includes `ep.DontAutoSendResponse()`. This setting disables the default behavior where an endpoint would immediately send back whatever it returns.

So how are responses sent? We use a custom **Post-Processor** called `GlobalResponseSender`. This processor runs after every endpoint and is responsible for creating the final HTTP response. It takes the object returned by your endpoint—typically an `Ardalis.Result`—and intelligently maps it to the correct HTTP status code.

For example:
- If your endpoint returns a successful `Result<ProductDto>`, the processor creates a `200 OK` response containing the product data.
- If it returns `Result.Invalid(errors)`, the processor creates a `400 Bad Request` response with the validation errors.
- If it returns `Result.NotFound()`, it becomes a `404 Not Found` response.

This pattern is powerful because it keeps your endpoint logic clean and focused on its core task, while ensuring all your API responses are consistent and handled in one central place.

**Why this separation?**: The **API** layer provides a clean separation between the user interface (UI) and the business logic. This project acts as the boundary between your back-end system and the outside world, and it enforces that external clients (e.g., mobile apps or front-end websites) communicate in a consistent and defined way.

------

### 5. **Client Project**

**Folder**: `Client` 

A Blazor Web Assembly Standalone client, just like React, Vue, Svelte, Angular,... but written in C#. 

---

### 6. **Shared Project**

The **Shared** project is the glue between the **Client** and the **Server**. It decouples the Domain from the the Client, therefore we can still adjust the database , Services and Domain layer without breaking any clients. If we don't remove properties from the Data Transfer Objects (**DTO**)

- **Service Interfaces**: The contract between the **Client** and **API**.
- **Data Transfer Objects**: Simple classes without any domain logic. They're used to transfer data from the **API** to the **Client**.

------

### 7. **Testing Projects**

**Folder**: `Client.Tests`, ``Services.Tests` and `Domain.Tests`

While not always included in the base template, most well-architected solutions should have dedicated testing projects, typically organized into **Unit Tests**, **Integration Tests**, and possibly **End-to-End Tests**.

- **Unit Tests**: Test individual components (usually found in the `Domain` or `Client` layers) in isolation from dependencies.

- **Integration Tests**: Ensure different parts of the system work together correctly (e.g., API and database).

  > We did not provide any integration tests, these are for you to figure out. But you can take a look [here](https://fast-endpoints.com/docs/integration-unit-testing#integration-testing) to get you in the right direction.

By separating the tests into their own projects, you ensure that they remain maintainable, modular, and focused on the specific functionality being tested.

---

### 8. **Cross-Cutting Concerns**

In some solutions, you may see additional projects or services to handle **cross-cutting concerns** like **logging**, **caching**, **authorization**, or **exception handling**. These concerns can be plugged into multiple layers of the solution but are typically handled in the **Persistence** / **Infrastructure** and **Server** projects.

------

### Key Concepts Explained

1. **Separation of Concerns (SoC)**: Each project in the solution has a single, well-defined responsibility. By separating concerns, changes in one part of the system (e.g., switching databases) do not ripple through the entire codebase.
2. **Dependency Injection (DI)**: This design pattern is used to inject dependencies into classes. The **API** project often configures DI, so classes get the services or repositories they need without creating them directly. This promotes loose coupling and makes the code easier to test.
3. **Domain-Driven Design (DDD)**: The structure of the **Domain** project follows DDD principles, where the business rules and logic are core to the application and should be isolated from infrastructure concerns. This keeps your business logic intact even as external technologies evolve.

------

### Conclusion

The `dotnet-template` solution is structured to encourage scalability, maintainability, and testability. Each project serves a distinct purpose:

- **Domain** defines core business logic.
- **Services** manages use cases and orchestrates the flow of information.
- **Persistence** handles the interaction with external systems and data storage.
- **API** exposes the functionality to the outside world via HTTP.
- **Client** a User Interface that could be swapped if need be.

## Course

There is .NET course from 1-2 years ago which is no longer maintained but still relevant. 

https://hogent-web.github.io/csharp/

