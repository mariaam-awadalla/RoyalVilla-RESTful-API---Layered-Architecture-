# RoyalVilla RESTful API

A RESTful Web API built using ASP.NET Core and Entity Framework Core for managing Villas, Villa Amenities, and User Authentication using JWT.

The project demonstrates real-world backend development concepts including Authentication, Authorization, DTOs, AutoMapper, Entity Relationships, Middleware Configuration, and Standardized API Responses.

---

# Project Structure

RoyalVilla_API/
├── Controllers/                 # API endpoints
│   ├── AuthController.cs
│   ├── VillaController.cs
│   └── VillaAmenityController.cs
│
├── Data/                        # Database layer
│   └── ApplicationDbContext.cs
│
├── Models/                      # Domain entities
│   ├── User.cs
│   ├── Villa.cs
│   ├── VillaAmenity.cs
│   │
│   └── DTO/                     # Data Transfer Objects
│       ├── APIResponse.cs
│       ├── LoginRequestDTO.cs
│       ├── LoginResponseDTO.cs
│       ├── RegistrationRequestDTO.cs
│       ├── UserDTO.cs
│       ├── VillaDTO.cs
│       ├── VillaCreateDTO.cs
│       ├── VillaUpdateDTO.cs
│       ├── VillaAmenityDTO.cs
│       ├── VillaAmenityCreateDTO.cs
│       └── VillaAmenityUpdateDTO.cs
│
├── Services/                    # Business logic layer
│   ├── IAuthService.cs
│   └── AuthService.cs
│
├── Migrations/                  # EF Core migrations
│
├── Program.cs                   # Application bootstrap & middleware
├── appsettings.json             # Configuration settings
└── RoyalVilla_API.http          # HTTP requests for testing

---

# Features

## Authentication & Authorization

- User Registration
- User Login
- JWT Token Generation
- Role-Based Authorization
- Protected Endpoints using `[Authorize]`
- Role Restrictions using `[Authorize(Roles = "...")]`

---

## Villas Management

- Create Villa
- Get All Villas
- Get Villa By Id
- Update Villa
- Delete Villa

---

## Villa Amenities Management

- Create Villa Amenities
- Update Amenities
- Get Amenities
- Entity Relationships with Villas

---

# Core Concepts Implemented

## RESTful API Design

The API follows REST conventions:

- GET
- POST
- PUT
- DELETE

with proper status codes:

- 200 OK
- 201 Created
- 400 Bad Request
- 401 Unauthorized
- 404 Not Found
- 409 Conflict
- 500 Internal Server Error

---

# JWT Authentication

The project uses JWT Bearer Authentication.

Generated JWT tokens include claims such as:

- User ID
- Email
- User Name
- Role
- Expiration Date

JWT tokens are signed using a Secret Key configured inside:

```json
"JWTSettings": {
  "Secret": "YOUR_SECRET_KEY"
}