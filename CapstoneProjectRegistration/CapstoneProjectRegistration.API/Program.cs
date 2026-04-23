using System.Text;
using CapstoneProjectRegistration.Repositories;
using CapstoneProjectRegistration.Repositories.Data;
using CapstoneProjectRegistration.Repositories.Implements;
using CapstoneProjectRegistration.Repositories.Interfaces;
using CapstoneProjectRegistration.Services.Interface;
using CapstoneProjectRegistration.Services.MyMapper;
using CapstoneProjectRegistration.Services.Implements;
using CapstoneProjectRegistration.Services.Service.RegistrationPeriods;
using CapstoneProjectRegistration.Services.Service.Topicss;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CapstoneProjectRegistration.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration.Get<AppSetting>();

            builder.Services.AddDbContext<CapstoneDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            var jwtKey = builder.Configuration["Jwt:SecretKey"] ?? string.Empty;
            var issuer = builder.Configuration["Jwt:Issuer"] ?? "CapstoneProjectRegistration";
            var audience = builder.Configuration["Jwt:Audience"] ?? "CapstoneProjectRegistration.Clients";

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
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
                    Version = "v1"
                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT from POST /api/auth/login (demo: admin@fpt.edu.vn / Admin@123)"
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
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
