## 🔐 Authentication & Authorization

This project implements a secure **JWT (JSON Web Token)** authentication system using **ASP.NET Core Identity**.

### Key Features

**IdentityCore Integration**  
A lightweight membership system used for managing users, passwords, and roles without unnecessary UI components.

**JWT Bearer Tokens**  
Implements secure and stateless authentication where the server issues a signed token to the client after successful authentication.

**Custom Claims**  
Tokens include essential user data such as **User ID**, **Email**, and **JTI (Token ID)**.  
This reduces additional database queries and improves performance when identifying users.

**Entity Framework Core Integration**  
Uses `IdentityDbContext` to handle user persistence and manage authentication-related data within **SQL Server**.

---

### How It Works

1. **Registration / Login**  
   The user sends their credentials to the `AuthController`.

2. **User Validation**  
   The system validates the credentials using `UserManager`.

3. **Token Generation**  
   A `SecurityTokenDescriptor` defines the user's identity through **claims** and signs the token using a secure private key.

4. **Authorized Requests**  
   The client includes the JWT token in the `Authorization` header for all protected API requests.
