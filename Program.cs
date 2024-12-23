using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CubeEnergy.Data;
using CubeEnergy.Repositories;
using CubeEnergy.Services;
using CubeEnergy.Utilities;
using CloudinaryDotNet;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))
    )
);

// Register repositories
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IKycRepository, KycRepository>();
builder.Services.AddScoped<INewsletterRepository, NewsletterRepository>(); // Register NewsletterRepository
builder.Services.AddScoped<IAdminRepository, AdminRepository>(); // Register the Admin repository

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<KycService>();
builder.Services.AddScoped<IInverterService, InverterService>();
builder.Services.AddScoped<IAdminService, AdminService>(); // Register the Admin service
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<INewsletterService, NewsletterService>(); // Register NewsletterService
builder.Services.AddScoped<AuthService>();

// Register utilities and other services
builder.Services.AddSingleton<EmailService>();

builder.Services.AddSingleton<JwtService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var jwtSettings = configuration.GetSection("JwtSettings");

    return new JwtService(
        jwtSettings["Secret"],
        jwtSettings["Issuer"],
        jwtSettings["Audience"],
        jwtSettings["RefreshTokenSecret"]
    );
});

builder.Services.AddSingleton<BcryptService>();

builder.Services.AddSingleton(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var cloudName = configuration["Cloudinary:CloudName"];
    var apiKey = configuration["Cloudinary:ApiKey"];
    var apiSecret = configuration["Cloudinary:ApiSecret"];

    var account = new Account(cloudName, apiKey, apiSecret);
    return new Cloudinary(account);
});

// Register FormFileOperationFilter if used in Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MyCubeEnergy API",
        Version = "v1",
        Description = "API for CubeEnergy including KYC, Authentication, and more. || C# and MySQL",
        Contact = new OpenApiContact
        {
            Name = "MyCubeEnergy Support",
            Email = "support@mycubeenergy.com"
        }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and the JWT token. Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Register the FormFileOperationFilter if used
    options.OperationFilter<FormFileOperationFilter>();
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
    };
});

var app = builder.Build();

// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyCubeEnergy API v1");
});

// Use CORS
app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
