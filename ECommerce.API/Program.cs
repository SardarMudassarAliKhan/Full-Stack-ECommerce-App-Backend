
using ECommerce.Application.Services.Implementations;
using ECommerce.Application.Services.Interfaces;
using ECommerce.Domain.Repositories;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Extensions;
using ECommerce.Infrastructure.Identity;
using ECommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ECommerce.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());

                // ------------------------------------------------------
                // Add Controllers
                // ------------------------------------------------------
                builder.Services.AddControllers();

                // add infrastructure (DbContext + provider selection)
                builder.Services.AddInfrastructure(builder.Configuration);

                builder.Services.AddHostedService<EmailNotificationConsumer>();

                // ------------------------------------------------------
                //  Repository Registrations
                // ------------------------------------------------------
                builder.Services.AddScoped<IProductRepository, ProductRepository>();
                builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
                builder.Services.AddScoped<IOrderRepository, OrderRepository>();
                builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();


                // ------------------------------------------------------
                //  Service Registrations (Application Layer)
                // ------------------------------------------------------
                builder.Services.AddScoped<IProductService, ProductService>();
                builder.Services.AddScoped<ICustomerService, CustomerService>();
                builder.Services.AddScoped<IOrderService, OrderService>();
                builder.Services.AddScoped<IOrderItemService, OrderItemService>();


                // ------------------------------------------------------
                //  Swagger
                // ------------------------------------------------------
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new() { Title = "ECommerce API", Version = "v1" });

                    // Add JWT Authentication to Swagger
                    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                        Description = "Enter JWT token: Bearer {your token}"
                    });

                    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
           }
            });
                });

                // ------------------------------------------------------
                // CORS
                // ------------------------------------------------------
                var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowFrontend", policy =>
                    {
                        policy.WithOrigins(allowedOrigins!)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
                });

                // Add services to the container.

                builder.Services.AddControllers();
                // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
                builder.Services.AddOpenApi();
            

            var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment() || app.Environment.IsProduction() || app.Environment.IsStaging())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseMiddleware<ECommerce.API.Middleware.ExceptionHandlingMiddleware>();

                app.UseHttpsRedirection();

                app.UseCors("AllowFrontend");

                // Add this before mapping controllers
                app.UseAuthentication();   // must come first
                app.UseAuthorization();

                app.MapControllers();


                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    // Apply pending migrations on startup
                    try
                    {
                        var dbContext = services.GetRequiredService<AppDbContext>();
                        await dbContext.Database.MigrateAsync();
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred while migrating the database.");
                        throw;
                    }

                    await IdentitySeeder.SeedRolesAndAdminAsync(services);
                }

                app.Run();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
