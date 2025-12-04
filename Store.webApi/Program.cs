using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Store.DataAccess.Context;
using Store.DataAccess.Entities;
using Store.DataAccess.Repositories;
using Store.DataAccess.Repositories.Interfaces;
using Store.DataAccess.UnitOfWork;
using Store.WebApi.Mapping;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load appsettings
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

Console.WriteLine($"=== APPLICATION STARTING ===");
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"Content Root: {builder.Environment.ContentRootPath}");
Console.WriteLine($"Application Name: {builder.Environment.ApplicationName}");

// ------------------------------------------------------------
// 1. التحقق من الإعدادات
// ------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"ConnectionString Loaded: {!string.IsNullOrEmpty(connectionString)}");

if (string.IsNullOrEmpty(connectionString))
{
    connectionString = "Server=DESKTOP-4OQALO4;Database=StoreDb;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=true";
    Console.WriteLine($"⚠️ Using fallback ConnectionString");
}

// ------------------------------------------------------------
// 2. Add Services
// ------------------------------------------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// 2.1 Database
builder.Services.AddDbContext<StoreContext>(options =>
{
    Console.WriteLine($"Configuring DbContext with: {connectionString}");
    options.UseSqlServer(connectionString);
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    options.EnableDetailedErrors(builder.Environment.IsDevelopment());
});

// 2.2 Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<StoreContext>()
.AddDefaultTokenProviders();

// 2.3 JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong123!";
var issuer = jwtSettings["Issuer"] ?? "StoreWebApi";
var audience = jwtSettings["Audience"] ?? "StoreWebApi";
var expiryInMinutes = jwtSettings.GetValue("ExpiryInMinutes", 59);

// Validate secret key length
if (secretKey.Length < 32)
{
    Console.WriteLine($"⚠️ WARNING: Secret key is too short ({secretKey.Length} chars). Using a longer one...");
    secretKey = "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong123!";
}

Console.WriteLine($"=== JWT CONFIGURATION ===");
Console.WriteLine($"Issuer: {issuer}");
Console.WriteLine($"Audience: {audience}");
Console.WriteLine($"Expiry: {expiryInMinutes} minutes");
Console.WriteLine($"Secret Key Length: {secretKey.Length}");
Console.WriteLine($"Secret Key Starts With: {secretKey.Substring(0, Math.Min(10, secretKey.Length))}...");
Console.WriteLine("==========================");

// 2.4 Authentication - FIXED VERSION
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.IncludeErrorDetails = true;

    // FIXED: Correct token validation parameters
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,

        ValidateAudience = true,
        ValidAudience = audience,

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

        // IMPORTANT: These must match what's in your AuthController
        NameClaimType = ClaimTypes.NameIdentifier, // This should be "sub" or ClaimTypes.NameIdentifier
        RoleClaimType = ClaimTypes.Role,

        // Try to validate all possible claim types
        ValidateActor = false,
        ValidateTokenReplay = false
    };

    // FIXED: Improved event handling
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var path = context.Request.Path;
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            Console.WriteLine($"\n[OnMessageReceived] Path: {path}");

            if (!string.IsNullOrEmpty(authHeader))
            {
                Console.WriteLine($"Auth Header Found: Yes");

                // Clean up the token
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = authHeader.Substring("Bearer ".Length).Trim();
                }
                else
                {
                    context.Token = authHeader.Trim();
                }

                Console.WriteLine($"Token Length: {context.Token?.Length ?? 0}");
                if (!string.IsNullOrEmpty(context.Token))
                {
                    Console.WriteLine($"Token Preview: {context.Token.Substring(0, Math.Min(30, context.Token.Length))}...");

                    // Debug: Check token format
                    var parts = context.Token.Split('.');
                    Console.WriteLine($"Token Parts: {parts.Length}");
                    if (parts.Length == 3)
                    {
                        Console.WriteLine("✓ Token has correct JWT format (3 parts)");
                    }
                    else
                    {
                        Console.WriteLine($"✗ Token has incorrect format: {parts.Length} parts instead of 3");
                    }
                }
            }
            else
            {
                Console.WriteLine("No Authorization header found");
            }

            return Task.CompletedTask;
        },

        OnTokenValidated = context =>
        {
            Console.WriteLine($"\n[OnTokenValidated] SUCCESS!");
            Console.WriteLine($"Path: {context.Request.Path}");
            Console.WriteLine($"User ID: {context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier)}");
            Console.WriteLine($"Email: {context.Principal?.FindFirstValue(ClaimTypes.Email)}");

            // Debug all claims
            if (context.Principal?.Claims != null)
            {
                Console.WriteLine("All Claims:");
                foreach (var claim in context.Principal.Claims)
                {
                    Console.WriteLine($"  {claim.Type} = {claim.Value}");
                }
            }

            return Task.CompletedTask;
        },

        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"\n[OnAuthenticationFailed] ERROR!");
            Console.WriteLine($"Path: {context.Request.Path}");
            Console.WriteLine($"Exception Type: {context.Exception.GetType().Name}");
            Console.WriteLine($"Message: {context.Exception.Message}");

            if (context.Exception.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {context.Exception.InnerException.Message}");
            }

            // Log the actual token for debugging
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader))
            {
                Console.WriteLine($"Auth Header: {authHeader.Substring(0, Math.Min(100, authHeader.Length))}...");
            }

            return Task.CompletedTask;
        },

        OnChallenge = context =>
        {
            Console.WriteLine($"\n[OnChallenge]");
            Console.WriteLine($"Path: {context.Request.Path}");
            Console.WriteLine($"Error: {context.Error}");
            Console.WriteLine($"Description: {context.ErrorDescription}");

            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader))
            {
                Console.WriteLine($"Auth Header Present: Yes");
                Console.WriteLine($"Auth Header: {authHeader.Substring(0, Math.Min(100, authHeader.Length))}...");
            }
            else
            {
                Console.WriteLine("No Auth Header");
            }

            return Task.CompletedTask;
        }
    };
});

// 2.5 Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

// 2.6 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Store API",
        Version = "v1",
        Description = "Store Management API",
        Contact = new OpenApiContact
        {
            Name = "Store Team",
            Email = "support@store.com"
        }
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

// 2.7 AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// 2.8 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 2.9 HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// ------------------------------------------------------------
// 3. Build App
// ------------------------------------------------------------
var app = builder.Build();

// ------------------------------------------------------------
// 4. Middleware Pipeline
// ------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Request logging middleware
app.Use(async (context, next) =>
{
    Console.WriteLine($"\n[{DateTime.Now:HH:mm:ss}] {context.Request.Method} {context.Request.Path}");

    // Check auth header
    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
    if (!string.IsNullOrEmpty(authHeader))
    {
        Console.WriteLine($"Auth Header Present: Yes");

        // Check for common issues
        if (authHeader.StartsWith("Bearer Bearer "))
        {
            Console.WriteLine("⚠️ WARNING: Double 'Bearer' detected!");
            var fixedHeader = "Bearer " + authHeader.Substring("Bearer Bearer ".Length);
            context.Request.Headers["Authorization"] = fixedHeader;
            Console.WriteLine($"Fixed header to: {fixedHeader.Substring(0, Math.Min(60, fixedHeader.Length))}...");
        }
        else if (!authHeader.StartsWith("Bearer "))
        {
            Console.WriteLine("⚠️ WARNING: Missing 'Bearer' prefix. Adding it...");
            context.Request.Headers["Authorization"] = "Bearer " + authHeader;
        }
        else
        {
            Console.WriteLine($"Auth Header: {authHeader.Substring(0, Math.Min(60, authHeader.Length))}...");
        }
    }
    else
    {
        Console.WriteLine($"Auth Header Present: No");
    }

    await next();

    Console.WriteLine($"Response Status: {context.Response.StatusCode}");
});

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Store API V1");
    c.RoutePrefix = "swagger";
    c.DisplayRequestDuration();
    c.EnableDeepLinking();
    c.EnableFilter();

    // Add instructions
    c.DisplayOperationId();
    c.EnableValidator();
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// CRITICAL: Must be in this order
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ------------------------------------------------------------
// 5. Database Seeding
// ------------------------------------------------------------


    // Create admin user


    // ------------------------------------------------------------
    

app.Run();