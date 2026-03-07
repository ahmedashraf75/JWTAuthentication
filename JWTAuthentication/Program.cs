using JWTAuthentication.Configuration.Models;
using JWTAuthentication.Data;
using JWTAuthentication.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


/*********************************************************************************************************************************/
// this say to the application that we want to use the JwtConfig class to bind the configuration section
// named "JwtConfig" from the appsettings.json file. This allows us to easily access the JWT configuration settings
// throughout our application by injecting the IOptions<JwtConfig> interface where needed.
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

// this code is configuring the authentication services for the application to use JWT (JSON Web Token) bearer authentication.
// like checking the validity of the token, the issuer, the audience, and the signing key. The signing key is created using a secret key
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Secret"]))
        };
    }
    );

// this code is adding the default identity services to the application, which includes user management and authentication features.
// It is configuring the identity system to require confirmed accounts for signing in and specifying
// that the user data will be stored in an Entity Framework Core database using the AppDbContext class.
//IdenityUser is a class provided by ASP.NET Core Identity that represents a user in the identity system.
//By using AddDefaultIdentity<IdentityUser>, we are telling the application to use the default implementation of the identity system
//with the IdentityUser class as the user entity. This allows us to manage user accounts, roles, and authentication features
//without having to create our own custom user class.
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
        options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDbContext>();


// this code is adding the AppDbContext to the application's service container and configuring it to use SQL Server as the database provider.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

/*********************************************************************************************************************************/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
