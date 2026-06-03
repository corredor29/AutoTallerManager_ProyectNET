using Application.Mapping;
using Api.Filters;
using Api.Middleware;
using Api.RateLimiting;
using Api.Responses;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Services;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

MapsterConfig.Register(TypeAdapterConfig.GlobalSettings);
builder.Services.AddScoped<ApiResponseFilter>();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseFilter>();
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                x => x.Key,
                x => x.Value!.Errors.Select(error =>
                        string.IsNullOrWhiteSpace(error.ErrorMessage) ? "The input is invalid." : error.ErrorMessage)
                    .ToArray());

        var response = new ApiErrorResponse
        {
            Success    = false,
            StatusCode = StatusCodes.Status400BadRequest,
            Title      = "Validation failed",
            Detail     = "One or more validation errors occurred.",
            TraceId    = context.HttpContext.TraceIdentifier,
            Errors     = errors
        };

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "AutoTallerManager API",
        Version     = "v1",
        Description = "Sistema de Gestión de Taller Automotriz"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Enter a valid JWT token (without 'Bearer' prefix)."
    };

    options.AddSecurityDefinition("Bearer", securityScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<RouteRateLimitOptions>(builder.Configuration.GetSection(RouteRateLimitOptions.SectionName));

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration is missing.");
var rateLimitOptions = builder.Configuration.GetSection(RouteRateLimitOptions.SectionName).Get<RouteRateLimitOptions>()
    ?? new RouteRateLimitOptions();

if (string.IsNullOrWhiteSpace(jwtOptions.Key))
    throw new InvalidOperationException("JWT signing key is missing.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime         = true,
            ValidIssuer              = jwtOptions.Issuer,
            ValidAudience            = jwtOptions.Audience,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ClockSkew                = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        context.HttpContext.Response.Headers["Retry-After"] = "60";

        var response = new ApiErrorResponse
        {
            Success    = false,
            StatusCode = StatusCodes.Status429TooManyRequests,
            Title      = "Too many requests",
            Detail     = "The rate limit for this endpoint has been exceeded. Please try again later.",
            TraceId    = context.HttpContext.TraceIdentifier
        };

        await context.HttpContext.Response.WriteAsync(
            JsonSerializer.Serialize(response),
            cancellationToken);
    };

    // ── Auth ───────────────────────────────────────
    options.AddPolicy("auth", _ =>
        RateLimitPartition.GetFixedWindowLimiter(
            "auth",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit          = 30,
                Window               = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit           = 0,
                AutoReplenishment    = true
            }));

    // ── Service Orders ─────────────────────────────
    options.AddPolicy("service-orders", _ =>
        RateLimitPartition.GetFixedWindowLimiter(
            "service-orders",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit          = rateLimitOptions.ServiceOrders.PermitLimit,
                Window               = TimeSpan.FromMinutes(rateLimitOptions.ServiceOrders.WindowMinutes),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit           = rateLimitOptions.ServiceOrders.QueueLimit,
                AutoReplenishment    = true
            }));

    // ── Parts ──────────────────────────────────────
    options.AddPolicy("parts", _ =>
        RateLimitPartition.GetFixedWindowLimiter(
            "parts",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit          = rateLimitOptions.Parts.PermitLimit,
                Window               = TimeSpan.FromMinutes(rateLimitOptions.Parts.WindowMinutes),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit           = rateLimitOptions.Parts.QueueLimit,
                AutoReplenishment    = true
            }));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFront", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5500",
                "http://127.0.0.1:5500",
                "http://localhost:3000",
                "http://127.0.0.1:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithExposedHeaders("Retry-After"); 
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await databaseInitializer.InitializeAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AutoTallerManager v1");
        options.DisplayRequestDuration();
        options.EnablePersistAuthorization();
    });
}

app.UseMiddleware<ApiExceptionMiddleware>();
app.UseCors("AllowFront");
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }