using System.Security.Claims;
using System.Text;
using CapstoneProjectRegistration.API.Infrastructure;
using CapstoneProjectRegistration.API.Security;
using CapstoneProjectRegistration.Repositories;
using CapstoneProjectRegistration.Repositories.Data;
using CapstoneProjectRegistration.Repositories.Entities;
using CapstoneProjectRegistration.Repositories.Implements;
using CapstoneProjectRegistration.Repositories.Interfaces;
using CapstoneProjectRegistration.Services.Implements;
using CapstoneProjectRegistration.Services.Interface;
using CapstoneProjectRegistration.Services.MyMapper;
using CapstoneProjectRegistration.Services.Service.RegistrationPeriods;
using CapstoneProjectRegistration.Services.Service.Topicss;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CapstoneProjectRegistration.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration.Get<AppSetting>();

        builder.Services.AddDbContext<CapstoneDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<int>>()
            .AddEntityFrameworkStores<CapstoneDbContext>();

        builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

        var jwtKey = builder.Configuration["Jwt:SecretKey"] ?? string.Empty;
        var issuer = builder.Configuration["Jwt:Issuer"] ?? "CapstoneProjectRegistration";
        var audience = builder.Configuration["Jwt:Audience"] ?? "CapstoneProjectRegistration.Clients";

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
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthPolicies.RegistrationPeriodsManage,
                p => p.RequireRole(AppRoles.Admin));

            options.AddPolicy(AuthPolicies.TopicsCreate,
                p => p.RequireRole(AppRoles.Lecturer, AppRoles.Admin));

            options.AddPolicy(AuthPolicies.TopicsUpdate,
                p => p.RequireRole(AppRoles.Lecturer, AppRoles.Admin));

            options.AddPolicy(AuthPolicies.TopicsDelete,
                p => p.RequireRole(AppRoles.Admin));

            options.AddPolicy(AuthPolicies.TopicsAssignReviewers,
                p => p.RequireRole(AppRoles.Admin));

            options.AddPolicy(AuthPolicies.TopicsSubmitReview,
                p => p.RequireRole(AppRoles.Lecturer, AppRoles.Admin));

            options.AddPolicy(AuthPolicies.TopicsReadReviewSummary,
                p => p.RequireRole(AppRoles.Lecturer, AppRoles.Admin));

            options.AddPolicy(AuthPolicies.TopicsPublish,
                p => p.RequireRole(AppRoles.Admin));

            options.AddPolicy(AuthPolicies.TopicsImport,
                p => p.RequireRole(AppRoles.Lecturer, AppRoles.Admin));

            options.AddPolicy(AuthPolicies.TopicsSimilarityCheck,
                p => p.RequireRole(AppRoles.Lecturer, AppRoles.Admin));
        });

        builder.Services.AddHttpContextAccessor();
        var autoMapperLicense = builder.Configuration["AutoMapper:LicenseKey"] ?? string.Empty;
        builder.Services.AddAutoMapper(
            cfg => { cfg.LicenseKey = autoMapperLicense; },
            typeof(MapperConfigurationsProfile));
        builder.Services.AddSingleton(configuration!);
        builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        builder.Services.AddScoped<ITopicService, TopicService>();
        builder.Services.AddScoped<IRegistrationPeriodService, RegistrationPeriodService>();
        builder.Services.AddScoped<ITopicImportService, TopicImportService>();
        builder.Services.AddScoped<ITopicSimilarityService, TopicSimilarityService>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Capstone Project Registration API",
                Version = "v1",
                Description =
                    "REST API v1. **Dev accounts** (password `DevPass@1` for all): " +
                    "Admin `admin@gmail.com`, Lecturer `lecturer1@gmail.com`, Student `thinhpdqse171589@fpt.edu.vn`. " +
                    "Obtain a JWT via `POST /api/v1/authentication/token`, then authorize with Bearer."
            });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT from POST /api/v1/authentication/token"
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

            options.TagActionsBy(api =>
            {
                var tags = api.ActionDescriptor?.EndpointMetadata?
                    .OfType<TagsAttribute>()
                    .SelectMany(t => t.Tags)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();
                if (tags is { Count: > 0 })
                {
                    return tags;
                }

                var controller = api.ActionDescriptor?.RouteValues?["controller"] ?? "Other";
                return new[] { controller };
            });

            options.OrderActionsBy(apiDesc =>
            {
                var tag = apiDesc.ActionDescriptor?.EndpointMetadata?.OfType<TagsAttribute>().FirstOrDefault();
                var t = tag?.Tags?.FirstOrDefault() ?? apiDesc.ActionDescriptor?.RouteValues?["controller"] ?? "";
                return $"{t}_{apiDesc.RelativePath}";
            });
        });

        var app = builder.Build();

        var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
        var seedLogger = loggerFactory.CreateLogger("IdentityDataSeeder");
        await IdentityDataSeeder.SeedAsync(app.Services, seedLogger);

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}
