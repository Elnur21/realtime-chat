# MediatR Implementation in RealTimeChat

## Overview

This document describes the implementation of MediatR in the RealTimeChat project. MediatR is a library that implements the mediator pattern, which helps decouple application components and makes the code more maintainable and testable.

## What is MediatR?

MediatR is a simple mediator pattern implementation in .NET. It allows you to:
- Decouple your application components
- Implement CQRS (Command Query Responsibility Segregation)
- Make your code more testable
- Reduce dependencies between classes

## Implementation Details

### 1. Package Installation
```xml
<PackageReference Include="MediatR" Version="12.2.0" />
```

### 2. Configuration in Program.cs
```csharp
// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
```

### 3. Project Structure

```
Handlers/
├── Commands/
│   ├── LoginCommand.cs
│   ├── LoginCommandHandler.cs
│   ├── CreateUserCommand.cs
│   ├── CreateUserCommandHandler.cs
│   ├── UpdateUserCommand.cs
│   ├── UpdateUserCommandHandler.cs
│   ├── DeleteUserCommand.cs
│   └── DeleteUserCommandHandler.cs
└── Queries/
    ├── GetAllUsersQuery.cs
    ├── GetAllUsersQueryHandler.cs
    ├── GetUserByIdQuery.cs
    └── GetUserByIdQueryHandler.cs
```

### 4. Commands (Write Operations)

#### Login Command
- **Command**: `LoginCommand`
- **Handler**: `LoginCommandHandler`
- **Purpose**: Handle user authentication and JWT token generation

#### Create User Command
- **Command**: `CreateUserCommand`
- **Handler**: `CreateUserCommandHandler`
- **Purpose**: Create new users in the database

#### Update User Command
- **Command**: `UpdateUserCommand`
- **Handler**: `UpdateUserCommandHandler`
- **Purpose**: Update existing user information

#### Delete User Command
- **Command**: `DeleteUserCommand`
- **Handler**: `DeleteUserCommandHandler`
- **Purpose**: Delete users from the database

### 5. Queries (Read Operations)

#### Get All Users Query
- **Query**: `GetAllUsersQuery`
- **Handler**: `GetAllUsersQueryHandler`
- **Purpose**: Retrieve all users from the database

#### Get User By ID Query
- **Query**: `GetUserByIdQuery`
- **Handler**: `GetUserByIdQueryHandler`
- **Purpose**: Retrieve a specific user by ID

### 6. Controller Updates

#### AuthController
```csharp
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand
        {
            Email = request.Email ?? string.Empty,
            Password = request.Password ?? string.Empty
        };

        var response = await _mediator.Send(command);
        
        if (response.Success)
        {
            return Ok(new { 
                token = response.Token,
                message = response.Message,
                user = response.User
            });
        }

        return Unauthorized(new { message = response.Message });
    }
}
```

#### UsersController
```csharp
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllUsersQuery();
        var response = await _mediator.Send(query);
        
        if (response.Success)
        {
            return Ok(response.Users);
        }
        
        return BadRequest(response.Message);
    }
}
```

## Benefits of This Implementation

### 1. **Separation of Concerns**
- Controllers only handle HTTP requests/responses
- Business logic is moved to handlers
- Data access is isolated in handlers

### 2. **Testability**
- Each handler can be tested independently
- Controllers can be tested with mocked mediators
- Business logic is easier to unit test

### 3. **Maintainability**
- Clear separation between commands and queries
- Easy to add new features without modifying existing code
- Consistent error handling across the application

### 4. **Scalability**
- Easy to add validation, logging, or other cross-cutting concerns
- Can implement CQRS pattern easily
- Supports async/await throughout the pipeline

## Usage Examples

### Login
```bash
POST /auth/login
{
  "email": "admin@test.com",
  "password": "123456"
}
```

### Get All Users
```bash
GET /users
Authorization: Bearer {token}
```

### Create User
```bash
POST /users
Authorization: Bearer {token}
{
  "name": "John Doe",
  "email": "john@example.com"
}
```

### Update User
```bash
PUT /users/{id}
Authorization: Bearer {token}
{
  "name": "John Updated",
  "email": "john.updated@example.com"
}
```

### Delete User
```bash
DELETE /users/{id}
Authorization: Bearer {token}
```

## Future Enhancements

1. **Validation**: Add FluentValidation to commands and queries
2. **Logging**: Implement structured logging in handlers
3. **Caching**: Add caching for frequently accessed data
4. **Pagination**: Implement pagination for large datasets
5. **Notifications**: Add domain events for side effects

## Conclusion

The MediatR implementation provides a clean, maintainable, and testable architecture for the RealTimeChat project. It separates concerns effectively and makes the codebase more organized and scalable.
