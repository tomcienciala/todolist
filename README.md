# ToDoList Demo

This is a small demo application to manage tasks. It demonstrates the use of the Unit of Work and Repository design patterns in
a basic CRUD application, though these patterns may not be necessary for such a simple application. It also demonstrates the use of
unit tests with help from Moq and FluentAssertion Nuget packages.

## Features

- Create, read, update, and delete tasks
- Uses PostgreSQL as the database backend
- Demonstrates simple implementation of Unit of Work and Repository patterns

## Prerequisites

Before you begin, ensure you have met the following requirements:

- [.NET SDK](https://dotnet.microsoft.com/download) installed
- PostgreSQL installed and running locally (or use a hosted PostgreSQL instance)
- User secrets configured for your application

## Setup Instructions

### 1. Create a PostgreSQL Database

Create a new PostgreSQL database that will store your to-do list tasks:

```sql
CREATE DATABASE ToDoListDb;
```

### 2. Initialize User Secrets

Store your connection string in the user secrets store:

```shell
dotnet user-secrets init
```

### 3. Set the Database Connection String

Configure your connection string to point to your PostgreSQL instance. Replace `yourUsername` and `yourPassword` with your PostgreSQL credentials:

```shell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=ToDoListDb;Username=yourUsername;Password=yourPassword"
```

### 4. Update Database Schema

Apply the database schema changes to your PostgreSQL database. This ensures your database is up to date with the latest migrations:

```shell
dotnet ef database update
```

### 5. Run the Application

Now that the database is set up, you can run the application locally:

```shell
dotnet run
```

## File Structure

Here’s an overview of the project structure:

- **ToDoList.Api**: Contains the API controllers for handling task-related requests.
- **ToDoList.Shared**: Contains the data models for tasks for transferring between server and client. They can be shared with MAUI or Blazor apps.
- **ToDoList.Application**: Application layer that contains task service, that manipulates with tasks.
- **ToDoList.Data**: Contains the database context, migration files simple Unit of Work and generic repository implementations.

