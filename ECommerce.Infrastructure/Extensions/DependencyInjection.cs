using ECommerce.Application.Services.Implementations;
using ECommerce.Application.Services.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Caching;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Email;
using ECommerce.Infrastructure.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

namespace ECommerce.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {

            var provider = configuration["DatabaseProvider"] ?? "MySQL";

            if (string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                var conn = configuration.GetConnectionString("SqlServer");

                services.AddDbContext<AppDbContext, SqlServerDbContext>(options =>
                    options.UseSqlServer(conn));
            }
            else if (string.Equals(provider, "MySQL", StringComparison.OrdinalIgnoreCase))
            {
                var conn = configuration.GetConnectionString("MySQL");
                services.AddDbContext<AppDbContext, MySqlDbContext>(options =>
                    options.UseMySql(conn, ServerVersion.AutoDetect(conn)));
            }
            else if (string.Equals(provider, "PostgreSQL", StringComparison.OrdinalIgnoreCase))
            {
                var conn = configuration.GetConnectionString("PostgreSQL");
                services.AddDbContext<AppDbContext, PostgresDbContext>(options =>
                    options.UseNpgsql(conn));
            }
            else
            {
                throw new InvalidOperationException($"Unsupported provider: {provider}");
            }

            // ✅ Add Identity
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // ✅ JWT Authentication setup
            var jwtSettings = configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               .AddJwtBearer(options =>
               {
                   options.RequireHttpsMetadata = false;
                   options.SaveToken = true;
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = jwtSettings["Issuer"],
                       ValidAudience = jwtSettings["Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(key)
                   };
               });

            // ✅ Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
            });

            // ✅ Redis cache setup
            var redisConnection = configuration.GetConnectionString("Redis");
            if (!string.IsNullOrEmpty(redisConnection))
            {
                services.AddSingleton<IConnectionMultiplexer>(sp =>
                    ConnectionMultiplexer.Connect(redisConnection));

                services.AddSingleton<ICacheService, RedisCacheService>();
            }

            // RabbitMQ setup
            services.AddSingleton<IMessageQueueService, RabbitMQService>();
            services.AddScoped<IEmailSenderService, EmailSenderService>();
            services.AddScoped<IAuthService, AuthService>();


            return services;
        }
    }
}
