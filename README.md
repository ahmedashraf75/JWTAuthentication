🔐 Authentication & Authorization
This project implements a secure JWT (JSON Web Token) authentication system using ASP.NET Core Identity.

Key Features
IdentityCore Integration: A lightweight membership system for managing users, passwords, and roles without unnecessary UI overhead.

JWT Bearer Tokens: Secure, stateless authentication where the server issues a signed token to the client.

Custom Claims: Tokens include specific user data (ID, Email, JTI) to reduce database lookups and improve performance.

Entity Framework Core: Uses IdentityDbContext to manage user data persistence in SQL Server.

How it Works
Registration/Login: The user sends credentials to the AuthController.

Validation: The system validates the user via UserManager.

Token Generation: A SecurityTokenDescriptor defines the user's identity (claims) and signs it with a private key.

Authorization: The client sends the token in the Authorization header for all subsequent API requests.

Project Structure
DTOs/Requests: Contains data transfer objects for incoming data like UserRegistrationDto.

DTOs/Responses: Contains outgoing data structures like AuthResult.

Data: Contains the AppDbContext which handles the database connection.
